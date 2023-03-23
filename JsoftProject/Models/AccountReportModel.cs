using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace JsoftProject.Models
{
    public class AccountReportModel
    {
        [DisplayName("Account Name")]
        public string AccountName { get; set; }

        [DisplayName("Account ID")]
        public int AccountID { get; set; }

        [DisplayName("Cash Amount")]
        public decimal Cash { get; set; }

        [DisplayName("Cheque Amount")]
        public decimal Cheque { get; set; }

        [DisplayName("Outstanding Amount")]
        public decimal Outstanding { get; set; }
    }
}