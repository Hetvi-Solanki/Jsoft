using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsoftProject.Models
{
    public class NewPurchaseModel
    {
        public trnPurchase trnPurchase { get; set; }
        public List<trnPurchaseItem> trnPurchaseItems { get; set; }

        public static decimal MTotalAmount { get; set; } = 0;
    }
}