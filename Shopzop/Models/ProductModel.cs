using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Shopzop.Models
{
    /* ProductModel to validate details of Product */
    public class ProductModel
    {
        [Display(Name = "Product Id")]
        public int ProductId { get; set; }

        [Display(Name = "Product Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Product Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Price")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Category Id")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Category Id")]
        public int CategoryId { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
}
