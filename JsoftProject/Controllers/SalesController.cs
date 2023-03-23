using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        JsoftWeb db = new JsoftWeb();
        
        private int c = 0;

        private int vNo;   
        public ActionResult Create()
        {

            vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));
            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            ViewData["VoucherNo"] = value;

            ViewData["VoucherDate"] = DateTime.Now.ToShortDateString();

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["flag"] = "1st";

            return View();
        }

        [HttpPost]
        public ActionResult Create(trnSale obj)
        {
            obj.TotalAmount = Math.Round((decimal)obj.TotalAmount, 2);
            obj.CashAmount = Math.Round((decimal)obj.CashAmount, 2);
            obj.ChequeAmount = Math.Round((decimal)obj.ChequeAmount, 2);



            if (obj.AccountID.ToString() != null && c == 0)
            {
                vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));
                vNo++;

                string value = vNo.ToString().PadLeft(5, '0');

                obj.VoucherNo = value;
                obj.VoucherDate = DateTime.Now.Date;
                obj.AddDate = DateTime.Now;

                var user = (from mstUser in db.mstUsers
                            where mstUser.LoginID == User.Identity.Name
                            select mstUser.UserID).Single();

                obj.AddBy = Convert.ToInt32(user);
                db.trnSales.Add(obj);
                db.SaveChanges();
                var x = (from ma in db.mstAccounts
                         join tp in db.trnSales on ma.AccountID equals tp.AccountID
                         where ma.AccountID == obj.AccountID
                         select ma.AccountName).FirstOrDefault();

                ViewData["acc"] = x;
                c = 2;
            }

            if (c == 2)
            {
                obj.TotalAmount = Math.Round((decimal)obj.TotalAmount, 2);
                obj.CashAmount = Math.Round((decimal)obj.CashAmount, 2);
                obj.ChequeAmount = Math.Round((decimal)obj.ChequeAmount, 2);

                var state2 = (from state in db.mstStates
                              join company in db.mstCompanies on state.StateID equals company.StateID
                              select state.StateName).FirstOrDefault();


                var state1 = (from a in db.mstAccounts
                              join p in db.trnSales on a.AccountID equals p.AccountID
                              select a.StateName).FirstOrDefault();

                if (state1 == state2)
                {
                    obj.InvoiceType = "GST";
                }
                else
                {
                    obj.InvoiceType = "IGST";
                }

                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (obj.TotalAmount.ToString() != null && obj.CashAmount.ToString() != null && obj.ChequeAmount.ToString() != null)
            {
                obj.OutstandingAmount = obj.TotalAmount - obj.CashAmount - obj.ChequeAmount;
                obj.OutstandingAmount = Math.Round((decimal)obj.OutstandingAmount, 2);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            ViewData["flag"] = "2nd";


            return View(obj);
        }

        public ActionResult ViewDetails(string startDate, string endDate)
        {
            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);
                var fetch = db.trnSales.Where(x => x.VoucherDate >= start && x.VoucherDate <= end).ToList();

                if (fetch.Any())
                {
                    return View(fetch);
                }
                else
                {
                    ViewBag.Message = "No records found";
                    return View();
                }
            }
            else
            {
                var fetch = db.trnSales.ToList();
                return View(fetch);
            }

        }



        public ActionResult Edit(int id)
        {
            var result = db.trnSales.Where(y => y.SalesID == id).FirstOrDefault();
            var x = (from ma in db.mstAccounts
                     join tp in db.trnSales on ma.AccountID equals tp.AccountID
                     where ma.AccountID == result.AccountID
                     select ma.AccountName).FirstOrDefault();

            ViewData["acc"] = x;
            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(trnSale obj)
        {
            obj.TotalAmount = Math.Round((decimal)obj.TotalAmount, 2);
            obj.CashAmount = Math.Round((decimal)obj.CashAmount, 2);
            obj.ChequeAmount = Math.Round((decimal)obj.ChequeAmount, 2);

            obj.EditDate = DateTime.Now;

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj.EditBy = Convert.ToInt32(user);

            if (obj.TotalAmount.ToString() != null && obj.CashAmount.ToString() != null && obj.ChequeAmount.ToString() != null)
            {
                obj.OutstandingAmount = obj.TotalAmount - obj.CashAmount - obj.ChequeAmount;
                obj.OutstandingAmount = Math.Round((decimal)obj.OutstandingAmount, 2);
            }
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Delete(int id)
        {
            var result = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id, trnSale obj)
        {

            obj = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            db.trnSales.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Details(int id)
        {
            var result = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            return View(result);
        }

    }
}
