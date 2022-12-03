using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopzop.Models
{
    public class LoginModel
    {
        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
    }
}