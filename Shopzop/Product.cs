//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shopzop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Product
    {
        public int ProductId { get; set; }

        [Display(Name = "Product Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Product Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Product Price")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Please Enter Valid Price")]
        public int ProductPrice { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is Required")]
        public int CategoryId { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; } = DateTime.Now;

        public virtual Category Category { get; set; }
    }
}