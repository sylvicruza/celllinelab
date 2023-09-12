﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Cell_line_laboratory.Models
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
