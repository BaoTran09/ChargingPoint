using ChargingPoint.DB;
using ChargingPoint.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ChargingPoint.Services
{
    public interface ICloudinaryImageService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string entityType, long entityId, bool isPrimary = false);
        Task<bool> DeleteImageAsync(long imageId);
        Task<bool> SetPrimaryImageAsync(long imageId, string entityType, long entityId);
        Task<List<Image>> GetEntityImagesAsync(string entityType, long entityId);
        Task<string?> GetPrimaryImageUrlAsync(string entityType, long entityId);
    }

    public class ImageUploadResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Image? Image { get; set; }
    }

    public class CloudinaryImageService : ICloudinaryImageService
    {
        private readonly StoreDBContext _context;
        private readonly ILogger<CloudinaryImageService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        private readonly string _cloudName;
        private readonly string _uploadPreset;
        private readonly string _apiKey;
        private readonly string _apiSecret;

        public CloudinaryImageService(
            StoreDBContext context,
            ILogger<CloudinaryImageService> logger,
            IConfiguration config,
            HttpClient http)
        {
            _context = context;
            _logger = logger;
            _config = config;
            _http = http;
            /**/
            _cloudName = _config["Cloudinary:CloudName"]!;
            _uploadPreset = _config["Cloudinary:UploadPreset"]!;
            _apiKey = _config["Cloudinary:ApiKey"]!;
            _apiSecret = _config["Cloudinary:ApiSecret"]!;
        }

        // ================= UPLOAD =================
        public async Task<ImageUploadResult> UploadImageAsync(
     IFormFile file,
     string entityType,
     long entityId,
     bool isPrimary = false)
        {
            if (file == null || file.Length == 0)
                return Fail("File không hợp lệ");

            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
                return Fail("Định dạng ảnh không hợp lệ");

            if (file.Length > 10 * 1024 * 1024)
                return Fail("File vượt quá 10MB");

            try
            {
                // Khởi tạo Cloudinary client với signed
                var account = new Account(_cloudName, _apiKey, _apiSecret);
                var cloudinary = new Cloudinary(account);

                var folderPath = $"{entityType.ToUpper()}/{entityId}"; // VD: CHARGER/10

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folderPath,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    // Có thể thêm tags nếu cần: Tags = new List<string> { "chargingpoint", entityType.ToLower() }
                };

                // Upload signed (tự động ký tên bằng api_key + api_secret)
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Cloudinary upload failed: {uploadResult.Error?.Message}");
                    return Fail("Cloudinary từ chối: " + uploadResult.Error?.Message);
                }

                // Lưu vào database
                if (isPrimary)
                    await ClearPrimaryImage(entityType, entityId);

                var image = new Image
                {
                    EntityType = entityType.ToUpper(),
                    EntityId = entityId,
                    ImageUrl = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    Caption = file.FileName,
                    IsPrimary = isPrimary,
                    DisplayOrder = await GetNextDisplayOrder(entityType, entityId),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return new ImageUploadResult { Success = true, Image = image };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi upload ảnh lên Cloudinary");
                return Fail(ex.Message);
            }
        }

        // ================= DELETE  =================
        public async Task<bool> DeleteImageAsync(long imageId)
        {
            var image = await _context.Images.FindAsync(imageId);
            if (image == null) return false;

            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var rawSignature = $"public_id={image.PublicId}&timestamp={timestamp}{_apiSecret}";
                var signature = GenerateHash(rawSignature);

                var form = new Dictionary<string, string>
                {
                    ["public_id"] = image.PublicId!,
                    ["api_key"] = _apiKey,
                    ["timestamp"] = timestamp,
                    ["signature"] = signature
                };

                var deleteUrl = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/destroy";
                var response = await _http.PostAsync(deleteUrl, new FormUrlEncodedContent(form));

                if (response.IsSuccessStatusCode)
                {
                    _context.Images.Remove(image);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }







        public async Task<bool> SetPrimaryImageAsync(long imageId, string entityType, long entityId)
        {
            await ClearPrimaryImage(entityType, entityId);
            var image = await _context.Images.FindAsync(imageId);
            if (image == null) return false;
            image.IsPrimary = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Image>> GetEntityImagesAsync(string entityType, long entityId)
        {
            return await _context.Images
                .Where(i => i.EntityType == entityType.ToUpper() && i.EntityId == entityId && i.IsActive)
                .OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder).ToListAsync();
        }

        public async Task<string?> GetPrimaryImageUrlAsync(string entityType, long entityId)
        {
            return await _context.Images
                .Where(i => i.EntityType == entityType.ToUpper() && i.EntityId == entityId && i.IsPrimary && i.IsActive)
                .Select(i => i.ImageUrl).FirstOrDefaultAsync();
        }

        private async Task ClearPrimaryImage(string entityType, long entityId)
        {
            var images = await _context.Images
                .Where(i => i.EntityType == entityType.ToUpper() && i.EntityId == entityId && i.IsPrimary).ToListAsync();
            foreach (var img in images) img.IsPrimary = false;
        }

        private async Task<int> GetNextDisplayOrder(string entityType, long entityId)
        {
            var max = await _context.Images
                .Where(i => i.EntityType == entityType.ToUpper() && i.EntityId == entityId)
                .MaxAsync(i => (int?)i.DisplayOrder);
            return (max ?? 0) + 1;
        }

        // Hàm băm Signature chuẩn SHA1 cho Cloudinary
        private string GenerateHash(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private ImageUploadResult Fail(string msg) => new() { Success = false, Message = msg };
    }

    public class CloudinaryUploadResponse
    {
        public string public_id { get; set; } = null!;
        public string secure_url { get; set; } = null!;
    }
}