using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    public class SlsMergeController : Controller
    {
        JsoftWeb db = new JsoftWeb();

        private int c = 1;
        private int z = 1;
        //private int y;
        private int y = 12;
        private int k = 1;
        private int vNo;
        private int first = 1;
        private int EditID;


        public ActionResult Create()
        {
            vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));

            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            ViewData["VoucherNo"] = value;

            ViewData["VoucherDate"] = DateTime.Now.ToShortDateString();

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["ItemList"] = db.mstItems.ToList();

            ViewData["Taxes"] = db.mstTaxes.ToList();

            ViewData["flag"] = "1st";

            return View();
        }



        [HttpPost]
        public ActionResult Create(SalesMerge salesMerge, string variable)
        {

            var obj1 = new trnSale();
            var obj2 = new trnSalesItem();

            obj1 = salesMerge.trnSale;
            obj2 = salesMerge.trnSalesItem;

            if (c == 1 && obj1.AccountID.ToString() != null && variable != "2nd" && z == 1)
            {
                z = 0;

                vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));
                vNo++;

                string value = vNo.ToString().PadLeft(5, '0');

                obj1.VoucherNo = value;
                obj1.VoucherDate = DateTime.Now.Date;
                obj1.AddDate = DateTime.Now.Date;


                var user = (from mstUser in db.mstUsers
                            where mstUser.LoginID == User.Identity.Name
                            select mstUser.UserID).Single();

                obj1.AddBy = Convert.ToInt32(user);

                db.trnSales.Add(obj1);
                db.SaveChanges();

                var x = (from ma in db.mstAccounts
                         join ts in db.trnSales on ma.AccountID equals ts.AccountID
                         where ma.AccountID == obj1.AccountID
                         select ma.AccountName).FirstOrDefault();



                var state2 = (from state in db.mstStates
                              join company in db.mstCompanies on state.StateID equals company.StateID
                              select state.StateName).FirstOrDefault();


                var state1 = (from a in db.mstAccounts
                              join p in db.trnSales on a.AccountID equals p.AccountID
                              select a.StateName).FirstOrDefault();

                if (state1 == state2)
                {
                    obj1.InvoiceType = "GST";
                }
                else
                {
                    obj1.InvoiceType = "IGST";
                }

                db.Entry(obj1).State = EntityState.Modified;
                db.SaveChanges();

                obj2.SalesID = obj1.SalesID;
                obj2.AddDate = obj1.AddDate;
                obj2.AddBy = Convert.ToInt32(user);
                db.trnSalesItems.Add(obj2);
                db.SaveChanges();

                obj2.Rate = (from dailyRate in db.mstDailyRates
                             join item in db.mstItems on dailyRate.ItemGroupID equals item.ItemGroupID
                             join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                             where item.ItemID == obj2.ItemID && dailyRate.RateDate == obj2.AddDate
                             select dailyRate.SalesRate).FirstOrDefault();

                var salesRateType = (from item in db.mstItems
                                        join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                                        where item.ItemID == obj2.ItemID
                                        select itemGroup.SalesRateType).FirstOrDefault();

                if (salesRateType.ToString() == "N")
                {
                    obj2.Amount = obj2.NetWt * obj2.Rate;
                }
                else if (salesRateType.ToString() == "G")
                {
                    obj2.Amount = obj2.GrossWt * obj2.Rate;
                }
                else if (salesRateType.ToString() == "P")
                {
                    obj2.Amount = obj2.Pcs * obj2.Rate;
                }
               

                if (obj1.InvoiceType == "GST")
                {
                    obj2.SGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.SGSTPrc)
                      .FirstOrDefault();

                    obj2.CGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.CGSTPrc)
                      .FirstOrDefault();

                    obj2.SGSTAmount = (obj2.Amount * obj2.SGSTPrc) / 100;
                    obj2.CGSTAmount = (obj2.Amount * obj2.CGSTPrc) / 100;

                    obj2.TotalAmt = obj2.SGSTAmount + obj2.CGSTAmount + obj2.Amount;

                }
                else if (obj1.InvoiceType == "IGST")
                {
                    obj2.IGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.IGSTPrc)
                      .FirstOrDefault();

                    obj2.IGSTAmount = (obj2.Amount * obj2.IGSTPrc) / 100;

                    obj2.TotalAmt = obj2.IGSTAmount + obj2.Amount;

                }



                obj1.TotalAmount = obj2.TotalAmt;

                db.Entry(obj1).State = EntityState.Modified;
                db.Entry(obj2).State = EntityState.Modified;
                db.SaveChanges();

                ViewData["flag"] = "2nd";
                ViewData["acc"] = x;
                ViewData["totalAmt"] = obj1.TotalAmount;
                c = 2;

            }

            else if (true)
            {
                if (obj1.TotalAmount.ToString() != null && obj1.CashAmount.ToString() != null && obj1.ChequeAmount.ToString() != null)
                {
                    obj1.OutstandingAmount = obj1.TotalAmount - obj1.CashAmount - obj1.ChequeAmount;
                    obj1.OutstandingAmount = Math.Round((decimal)obj1.OutstandingAmount, 2);
                }
                db.Entry(obj1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewDetails");
            }
            k = c;
            return View(new SalesMerge { trnSale = obj1, trnSalesItem = obj2 });
        }


        public ActionResult ViewDetails(string startDate, string endDate)
        {

            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);


                var saless = db.trnSales.Where(x => x.VoucherDate >= start && x.VoucherDate <= end).ToList();
                var salesItems = db.trnSalesItems.ToList();

                List<SalesMerge> salesMerges = new List<SalesMerge>();
                foreach (var sales in saless)
                {
                    var salesItem = salesItems.FirstOrDefault(pi => pi.SalesID == sales.SalesID);
                    if (salesItem != null)
                    {
                        salesMerges.Add(new SalesMerge { trnSale = sales, trnSalesItem = salesItem });
                        
                    }
                }

                if (salesMerges.Any())
                {
                    return View(salesMerges);
                }
                else
                {
                    ViewBag.Message = "No records found";
                    return View();
                }
            }
            else
            {
                var saless = db.trnSales.ToList();
                var salesItems = db.trnSalesItems.ToList();

                List<SalesMerge> salesMerges = new List<SalesMerge>();
                foreach (var sales in saless)
                {
                    var salesItem = salesItems.FirstOrDefault(pi => pi.SalesID == sales.SalesID);
                    if (salesItem != null)
                    {
                        salesMerges.Add(new SalesMerge { trnSale = sales, trnSalesItem = salesItem });
                    }
                }

                return View(salesMerges);
            }



        }

        public ActionResult Edit(int id)
        {
            EditID = id;
            var sales = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            var salesItem = db.trnSalesItems.Where(x => x.SalesID == id).FirstOrDefault();
            var result = new SalesMerge { trnSale = sales, trnSalesItem = salesItem };

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["ItemList"] = db.mstItems.ToList();

            ViewData["Taxes"] = db.mstTaxes.ToList();

            ViewData["flag"] = "1st";

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(SalesMerge salesMerge, string variable, int id)
        {

            var obj1 = new trnSale();
            var obj2 = new trnSalesItem();

            obj1 = salesMerge.trnSale;
            obj2 = salesMerge.trnSalesItem;

            obj1.SalesID = id;
            obj2.SalesID = id;



            obj2.SalesItemID = db.trnSalesItems
                       .Where(p => p.SalesID == id)
                       .Select(p => p.SalesItemID)
                       .FirstOrDefault();


            if (c == 1 && obj1.AccountID.ToString() != null && variable != "2nd" && z == 1)
            {
                z = 0;
                y = 9;
                vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));
                vNo++;

                string value = vNo.ToString().PadLeft(5, '0');

                obj1.VoucherNo = value;
                obj1.VoucherDate = DateTime.Now.Date;
                obj1.AddDate = DateTime.Now.Date;


                var user = (from mstUser in db.mstUsers
                            where mstUser.LoginID == User.Identity.Name
                            select mstUser.UserID).Single();

                obj1.AddBy = Convert.ToInt32(user);
                try
                {
                    db.Entry(obj1).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {

                }

                var x = (from ma in db.mstAccounts
                         join tp in db.trnSales on ma.AccountID equals tp.AccountID
                         where ma.AccountID == obj1.AccountID
                         select ma.AccountName).FirstOrDefault();



                var state2 = (from state in db.mstStates
                              join company in db.mstCompanies on state.StateID equals company.StateID
                              select state.StateName).FirstOrDefault();


                var state1 = (from a in db.mstAccounts
                              join p in db.trnSales on a.AccountID equals p.AccountID
                              select a.StateName).FirstOrDefault();

                if (state1 == state2)
                {
                    obj1.InvoiceType = "GST";
                }
                else
                {
                    obj1.InvoiceType = "IGST";
                }

                try
                {
                    db.Entry(obj1).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {

                }

                obj2.SalesID = obj1.SalesID;
                obj2.AddDate = obj1.AddDate;
                obj2.AddBy = Convert.ToInt32(user);

                try
                {
                    db.Entry(obj2).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch { }


                obj2.Rate = (from dailyRate in db.mstDailyRates
                             join item in db.mstItems on dailyRate.ItemGroupID equals item.ItemGroupID
                             join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                             where item.ItemID == obj2.ItemID && dailyRate.RateDate == obj2.AddDate
                             select dailyRate.SalesRate).FirstOrDefault();

                var salesRateType = (from item in db.mstItems
                                        join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                                        where item.ItemID == obj2.ItemID
                                        select itemGroup.SalesRateType).FirstOrDefault();

                if (salesRateType.ToString() == "N")
                {
                    obj2.Amount = obj2.NetWt * obj2.Rate;
                }
                else if (salesRateType.ToString() == "G")
                {
                    obj2.Amount = obj2.GrossWt * obj2.Rate;
                }
                else if (salesRateType.ToString() == "P")
                {
                    obj2.Amount = obj2.Pcs * obj2.Rate;
                }
                

                if (obj1.InvoiceType == "GST")
                {
                    obj2.SGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.SGSTPrc)
                      .FirstOrDefault();

                    obj2.CGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.CGSTPrc)
                      .FirstOrDefault();

                    obj2.SGSTAmount = (obj2.Amount * obj2.SGSTPrc) / 100;
                    obj2.CGSTAmount = (obj2.Amount * obj2.CGSTPrc) / 100;

                    obj2.TotalAmt = obj2.SGSTAmount + obj2.CGSTAmount + obj2.Amount;

                }
                else if (obj1.InvoiceType == "IGST")
                {
                    obj2.IGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                      .Select(tax => tax.IGSTPrc)
                      .FirstOrDefault();

                    obj2.IGSTAmount = (obj2.Amount * obj2.IGSTPrc) / 100;

                    obj2.TotalAmt = obj2.IGSTAmount + obj2.Amount;

                }



                obj1.TotalAmount = obj2.TotalAmt;

                try
                {
                    db.Entry(obj1).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch { }


                db.Entry(obj2).State = EntityState.Modified;
                db.SaveChanges();





                ViewData["flag"] = "2nd";
                ViewData["acc"] = x;
                ViewData["totalAmt"] = obj1.TotalAmount;
                c = 2;

            }
            else if (true)
            {
                if (obj1.TotalAmount.ToString() != null && obj1.CashAmount.ToString() != null && obj1.ChequeAmount.ToString() != null)
                {
                    obj1.OutstandingAmount = obj1.TotalAmount - obj1.CashAmount - obj1.ChequeAmount;
                    obj1.OutstandingAmount = Math.Round((decimal)obj1.OutstandingAmount, 2);
                }
                db.Entry(obj1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewDetails");


            }
            k = c;
            return View(new SalesMerge { trnSale = obj1, trnSalesItem = obj2 });
        }


        public ActionResult Delete(int id)
        {
            var sales = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            var salesItem = db.trnSalesItems.Where(x => x.SalesID == id).FirstOrDefault();
            var result = new SalesMerge { trnSale = sales, trnSalesItem = salesItem };
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id, SalesMerge salesMerge)
        {
            var sales = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            var salesItem = db.trnSalesItems.Where(x => x.SalesID == id).FirstOrDefault();

            db.trnSales.Remove(sales);
            db.trnSalesItems.Remove(salesItem);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

    }
}