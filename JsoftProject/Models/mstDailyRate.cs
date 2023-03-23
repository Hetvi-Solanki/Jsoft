//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JsoftProject.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class mstDailyRate
    {
        [DisplayName("Rate Date")]
        public System.DateTime RateDate { get; set; }
        public int ItemGroupID { get; set; }

        [DisplayName("Purchase Rate")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Purchase rate must be greater than zero.")]
        public decimal PurchaseRate { get; set; }

        [DisplayName("Sales Rate")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sales rate must be greater than zero.")]
        public decimal SalesRate { get; set; }
        public Nullable<int> AddBy { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
    
        public virtual mstItemGroup mstItemGroup { get; set; }
        public virtual mstUser mstUser { get; set; }
        public virtual mstUser mstUser1 { get; set; }
    }
}
