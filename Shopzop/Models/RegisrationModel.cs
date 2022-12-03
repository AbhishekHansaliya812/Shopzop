using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using CompareAttribute = System.Web.Mvc.CompareAttribute;

namespace Shopzop.Models
{
    public class RegisrationModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter User Name")]
        [Remote("doesUserNameExist", "Registration", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [Display(Name = "Mobile")]
        [StringLength(10,MinimumLength=10)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}