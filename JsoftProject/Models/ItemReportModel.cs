using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JsoftProject.Models
{
    public class ItemReportModel
    {

            [DisplayName("Item Name")]
            public string ItemName { get; set; }

            [DisplayName("Total Puchase Amount")]
            public decimal TotalPurchaseAmount { get; set; }

            [DisplayName("Total Sales Amount")]
            public decimal TotalSalesAmount { get; set; }

        
            public decimal Profit { get; set; }
        
    }
}