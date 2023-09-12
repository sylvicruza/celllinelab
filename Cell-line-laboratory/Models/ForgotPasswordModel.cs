using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cell_line_laboratory.Models
{
    public class ForgotPasswordModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public ForgotPasswordStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public enum ForgotPasswordStatus
        {
            None,
            Success,
            Error
        }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }

}
