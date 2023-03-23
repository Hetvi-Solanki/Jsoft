using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JsoftProject.Models
{
    public class MememberLogin
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z]).{5,}$", ErrorMessage = "Password must be at least 5 characters long and contain at least one capital letter.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z]).{5,}$", ErrorMessage = "Password must be at least 5 characters long and contain at least one capital letter.")]
        [DisplayName("Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; }
    }
}