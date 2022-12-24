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
    /* RegistrationModel to validate user's data */
    public class RegisrationModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter User Name")]
        [Remote("DoesUserNameExist", "Registration", ErrorMessage = "User name already exists, Please enter a different user name")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "User Name length must between 5 to 20")]
        [RegularExpression(("[^ ]+$"), ErrorMessage = "Space is not allowed in User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email")]
        [RegularExpression(("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$"), ErrorMessage = "Please Enter Valid Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [Display(Name = "Mobile")]
        [RegularExpression(("^[0-9]{10}$"), ErrorMessage = "Please enter valid Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password length must between 8 to 20")]
        [RegularExpression(("[^ ]+$"), ErrorMessage = "Space is not allowed in password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}