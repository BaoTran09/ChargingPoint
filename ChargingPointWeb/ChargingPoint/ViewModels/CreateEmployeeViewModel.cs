using System.ComponentModel.DataAnnotations;

namespace ChargingPoint.ViewModels
{
    public class CreateEmployeeViewModel
    {
        // Account Info
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        // Employee Info
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        
        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; }

        [StringLength(200)]
        public string Status { get; set; } = "Active";
    }
}