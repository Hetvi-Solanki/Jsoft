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

    public partial class mstItemGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public mstItemGroup()
        {
            this.mstDailyRates = new HashSet<mstDailyRate>();
            this.mstItems = new HashSet<mstItem>();
        }
    
        public int ItemGroupID { get; set; }

        [DisplayName("Metal Type")]
        [Required]
        public string MetalType { get; set; }

        [DisplayName("Group Name")]
        [Required]
        public string GroupName { get; set; }

        [DisplayName("Short Name")]
        [Required]
        public string ShortName { get; set; }

        [Required]
        public decimal Touch { get; set; }

        [DisplayName("Sales Rate Type")]
        [Required]
        public string SalesRateType { get; set; }

        [DisplayName("Purchase Rate Type")]
        [Required]
        public string PurchaseRateType { get; set; }

        [DisplayName("Unit")]
        [Required]
        public string MesrUnitCode { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public Nullable<int> AddBy { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mstDailyRate> mstDailyRates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mstItem> mstItems { get; set; }
        public virtual mstMetalType mstMetalType { get; set; }
        public virtual mstUser mstUser { get; set; }
        public virtual mstUser mstUser1 { get; set; }
        public virtual mstMesrUnit mstMesrUnit { get; set; }
        public virtual mstRateType mstRateType { get; set; }
        public virtual mstRateType mstRateType1 { get; set; }
    }
}
