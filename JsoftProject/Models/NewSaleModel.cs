using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsoftProject.Models
{
    public class NewSaleModel
    {
        public trnSale trnSale { get; set; }
        public List<trnSalesItem> trnSaleItems { get; set; }
    }
}