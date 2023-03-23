using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report

        JsoftWeb db = new JsoftWeb();

        public ActionResult salesRepot()
        {
            //var result = from p in db.trnPurchaseItems
            //             join s in db.trnSalesItems on p.ItemID equals s.ItemID into g
            //             from x in g.DefaultIfEmpty()
            //             group new { p, x } by p.ItemID into grp
            //             select new ItemReportModel
            //             {
            //                 ItemId = grp.Key,
            //                 TotalPurchaseAmount = (decimal)grp.Sum(x => x.p.TotalAmt),
            //                 TotalSalesAmount = grp.Sum(x => x.x == null ? 0 : x.x.TotalAmt),
            //                 Profit = (decimal)grp.Sum(x => (x.x == null ? 0 : x.x.TotalAmt) - x.p.TotalAmt)
            //             };

            var result = from p in db.trnPurchaseItems
                         join s in db.trnSalesItems on p.ItemID equals s.ItemID into g
                         from x in g.DefaultIfEmpty()
                         join i in db.mstItems on p.ItemID equals i.ItemID
                         group new { p, x, i } by p.ItemID into grp
                         select new ItemReportModel
                         {
                             ItemName = grp.FirstOrDefault().i.ItemName,
                             TotalPurchaseAmount = (decimal)grp.Sum(x => x.p.TotalAmt),
                             TotalSalesAmount = grp.Sum(x => x.x == null ? 0 : x.x.TotalAmt),
                             Profit = (decimal)grp.Sum(x => (x.x == null ? 0 : x.x.TotalAmt) - x.p.TotalAmt)
                             
                         };

            decimal prof = result.Sum(x => x.Profit);

            ViewData["Profit"] = prof;

            

            var reportData = result.ToList();
            return View(reportData);
        }


        //public ActionResult cashFlow()
        //{
        //    var query = (from t in
        //         (
        //             from s in db.trnSales
        //             select new AccountReportModel { AccountID = s.AccountID, AccountName=s.AccountID.ToString(), Cash = (decimal)s.CashAmount, Cheque = (decimal)s.ChequeAmount, Outstanding = (decimal)s.OutstandingAmount }
        //         ).Union(
        //             from p in db.trnPurchases
        //             select new AccountReportModel { AccountID = p.AccountID, AccountName = p.AccountID.ToString(), Cash = (decimal)p.CashAmount, Cheque = (decimal)p.ChequeAmount, Outstanding = (decimal)p.OutstandingAmount }
        //         )
        //                 join a in db.mstAccounts on t.AccountID equals a.AccountID // join with mstAccounts table
        //                 group t by new { t.AccountID, a.AccountName } into grouped // group by AccountID and AccountName
        //                 select new AccountReportModel
        //                 {
        //                     AccountID = grouped.Key.AccountID,
        //                     AccountName = grouped.Key.AccountName, // select AccountName from mstAccounts table
        //                     Cash = grouped.Sum(x => x.Cash),
        //                     Cheque = grouped.Sum(x => x.Cheque),
        //                     Outstanding = grouped.Sum(x => x.Outstanding)
        //                 }).ToList();




        //    return View(query);


        //}

        public ActionResult cashFlow()
        {
            var query = (from t in
                         (
                             from s in db.trnSales
                             select new AccountReportModel { AccountID = s.AccountID, AccountName = s.AccountID.ToString(), Cash = (decimal)s.CashAmount, Cheque = (decimal)s.ChequeAmount, Outstanding = (decimal)s.OutstandingAmount }
                         ).Union(
                             from p in db.trnPurchases
                             select new AccountReportModel { AccountID = p.AccountID, AccountName = p.AccountID.ToString(), Cash = (decimal)p.CashAmount, Cheque = (decimal)p.ChequeAmount, Outstanding = (decimal)p.OutstandingAmount }
                         )
                         join a in db.mstAccounts on t.AccountID equals a.AccountID // join with mstAccounts table
                         group t by new { t.AccountID, a.AccountName } into grouped // group by AccountID and AccountName
                         select new AccountReportModel
                         {
                             AccountID = grouped.Key.AccountID,
                             AccountName = grouped.Key.AccountName, // select AccountName from mstAccounts table
                             Cash = grouped.Sum(x => x.Cash),
                             Cheque = grouped.Sum(x => x.Cheque),
                             Outstanding = grouped.Sum(x => x.Outstanding)
                         }).ToList();

            decimal totalOutstanding = query.Sum(x => x.Outstanding);

            ViewData["TotalOutstanding"] = totalOutstanding;

            return View(query);
        }

    }
}