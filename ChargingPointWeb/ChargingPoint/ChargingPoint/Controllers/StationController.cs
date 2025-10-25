using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChargingPoint.DB;

namespace ChargingPoint.Controllers
{
    public class StationController : Controller
    {
        private readonly StoreDBContext _context;

        // Định nghĩa range tọa độ cho 3 miền của Việt Nam
        private const decimal NORTH_MIN_LAT = 19.8m;
        private const decimal NORTH_MAX_LAT = 23.5m;
        private const decimal NORTH_MIN_LON = 102.0m;
        private const decimal NORTH_MAX_LON = 109.5m;
        private const decimal CENTER_MIN_LAT = 11.5m;
        private const decimal CENTER_MAX_LAT = 19.8m;
        private const decimal CENTER_MIN_LON = 106.0m;
        private const decimal CENTER_MAX_LON = 110.0m;
        private const decimal SOUTH_MIN_LAT = 8.5m;
        private const decimal SOUTH_MAX_LAT = 11.5m;
        private const decimal SOUTH_MIN_LON = 104.0m;
        private const decimal SOUTH_MAX_LON = 109.0m;

        public StationController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Station - Hiển thị danh sách tất cả stations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Station.ToListAsync());
        }

        // GET: Station/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Station
                .FirstOrDefaultAsync(m => m.StationId == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Station/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Station/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StationId,Tag,Name,StationType,Address,Latitude,Longitude,Notes,CreatedAt,BuiltDate")] Station store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Station/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Station.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        // POST: Station/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("StationId,Tag,Name,StationType,Address,Latitude,Longitude,Notes,CreatedAt,BuiltDate")] Station store)
        {
            if (id != store.StationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.StationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(store);
        }

        // GET: Station/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Station
                .FirstOrDefaultAsync(m => m.StationId == id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Station/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var store = await _context.Station.FindAsync(id);
            if (store != null)
            {
                _context.Station.Remove(store);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(long id)
        {
            return _context.Station.Any(e => e.StationId == id);
        }

        [HttpGet]
        [Route("GetStores")]
        public IActionResult GetStores()
        {
            var stores = _context.Station.ToList();
            return Json(stores);
        }

        // API endpoint để lấy gợi ý tìm kiếm (autocomplete)
        [HttpGet]
        [Route("Station/SearchSuggestions")]
        public IActionResult SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            try
            {
                var suggestions = _context.Station
                    .Where(s => (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(term.ToLower())) ||
                               (!string.IsNullOrEmpty(s.Address) && s.Address.ToLower().Contains(term.ToLower())) ||
                               (!string.IsNullOrEmpty(s.Tag) && s.Tag.ToLower().Contains(term.ToLower())))
                    .Select(s => new
                    {
                        id = s.StationId,
                        label = s.Name ?? "N/A",
                        value = s.Name ?? "N/A",
                        address = s.Address ?? "N/A",
                        latitude = s.Latitude,
                        longitude = s.Longitude,
                        stationType = s.StationType ?? "N/A",
                        notes = s.Notes,
                        tag = s.Tag
                    })
                    .Take(10)
                    .ToList();

                return Json(suggestions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchSuggestions: {ex.Message}");
                return Json(new List<object>());
            }
        }

        // Helper method để xác định miền địa lý
        private string GetRegionByLatitudeLongitude(decimal? latitude, decimal? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return "Unknown";
            }

            decimal lat = latitude.Value;
            decimal lon = longitude.Value;
            Console.WriteLine($"Checking region for Lat: {lat}, Lon: {lon}");
            if (lat >= NORTH_MIN_LAT && lat <= NORTH_MAX_LAT && lon >= NORTH_MIN_LON && lon <= NORTH_MAX_LON)
                return "North";
            else if (lat >= CENTER_MIN_LAT && lat < CENTER_MAX_LAT && lon >= CENTER_MIN_LON && lon <= CENTER_MAX_LON)
                return "Center";
            else if (lat >= SOUTH_MIN_LAT && lat < SOUTH_MAX_LAT && lon >= SOUTH_MIN_LON && lon <= SOUTH_MAX_LON)
                return "South";
            else
                return "Unknown";
        }

        // Main Search Action - Xử lý tìm kiếm với nhiều filter
        [HttpGet]
        public IActionResult Search(string searchString, bool searchNorth = false, bool searchCenter = false, bool searchSouth = false)
        {
            var stations = _context.Station.AsQueryable();

            // Filter theo text search nếu có
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var searchTerm = searchString.ToLower().Trim();
                stations = stations.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(s.Address) && s.Address.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(s.Tag) && s.Tag.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(s.StationType) && s.StationType.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(s.Notes) && s.Notes.ToLower().Contains(searchTerm))
                );
            }

            // Filter theo miền địa lý nếu có ít nhất 1 miền được chọn
            if (searchNorth || searchCenter || searchSouth)
            {
                stations = stations.Where(s =>
                    (searchNorth && s.Latitude.HasValue && s.Longitude.HasValue &&
                     s.Latitude.Value >= NORTH_MIN_LAT && s.Latitude.Value <= NORTH_MAX_LAT &&
                     s.Longitude.Value >= NORTH_MIN_LON && s.Longitude.Value <= NORTH_MAX_LON) ||
                    (searchCenter && s.Latitude.HasValue && s.Longitude.HasValue &&
                     s.Latitude.Value >= CENTER_MIN_LAT && s.Latitude.Value < CENTER_MAX_LAT &&
                     s.Longitude.Value >= CENTER_MIN_LON && s.Longitude.Value <= CENTER_MAX_LON) ||
                    (searchSouth && s.Latitude.HasValue && s.Longitude.HasValue &&
                     s.Latitude.Value >= SOUTH_MIN_LAT && s.Latitude.Value < SOUTH_MAX_LAT &&
                     s.Longitude.Value >= SOUTH_MIN_LON && s.Longitude.Value <= SOUTH_MAX_LON)
                );
            }

            var results = stations.OrderBy(s => s.Name).ToList();

            // Debug log
            Console.WriteLine($"Search executed - Term: '{searchString}', Regions: N:{searchNorth}/C:{searchCenter}/S:{searchSouth}, Results: {results.Count}");
            foreach (var station in results)
            {
                Console.WriteLine($"Station: {station.Name}, Lat: {station.Latitude}, Lon: {station.Longitude}, Region: {GetRegionByLatitudeLongitude(station.Latitude, station.Longitude)}");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Headers["Content-Type"].ToString().Contains("application/json"))
            {
                return Json(results);
            }
            else
            {
                ViewBag.SearchString = searchString;
                ViewBag.SearchNorth = searchNorth;
                ViewBag.SearchCenter = searchCenter;
                ViewBag.SearchSouth = searchSouth;
                ViewBag.ResultCount = results.Count;

                return View("Index", results);
            }
        }
     




    }
}