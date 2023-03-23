using JsoftProject.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    [Authorize]
    public class NewSaleController : Controller
    {
        JsoftWeb db = new JsoftWeb();

        private decimal finalamt = 0;

        
        public ActionResult Create()
        {
            var vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));

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
        public ActionResult Create(NewSaleModel pm, string variable)
        {

            return View();
        }

        public ActionResult MainView()
        {
            var vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));

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
        public int MainView(NewSaleModel sm)
        {

            var obj1 = new trnSale();
            var obj2 = new trnSalesItem();

            obj1 = sm.trnSale;



            var vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));
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
                     join tp in db.trnSales on ma.AccountID equals tp.AccountID
                     where ma.AccountID == obj1.AccountID
                     select ma.AccountName).FirstOrDefault();



            var state2 = (from state in db.mstStates
                          join company in db.mstCompanies on state.StateID equals company.StateID
                          select state.StateName).FirstOrDefault();


            var state1 = (from a in db.mstAccounts
                          join s in db.trnSales on a.AccountID equals s.AccountID
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

            //----------------finished 1 should be here--------------------


            //--------------- Finished1 ----------------------------
            obj2.AddBy = obj1.AddBy;
            obj2.AddDate = obj1.AddDate;
            obj2.SalesID = obj1.SalesID;

            string serializedObject = JsonConvert.SerializeObject(obj2);
            TempData["MyObject"] = serializedObject;
            TempData.Keep("MyObject");

            TempData["Invoice"] = obj1.InvoiceType;
            TempData.Keep("Invoice");

            string serializedObject1 = JsonConvert.SerializeObject(obj1);
            TempData["MyObject1"] = serializedObject1;
            TempData.Keep("MyObject1");
          

          //  return new EmptyResult();
            return 1;


        }


        public ActionResult AddMorePartialView()
        {
            var vNo = int.Parse(db.trnSales.Max(p => p.VoucherNo));

            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            ViewData["VoucherNo"] = value;

            ViewData["VoucherDate"] = DateTime.Now.ToShortDateString();

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["ItemList"] = db.mstItems.ToList();

            ViewData["Taxes"] = db.mstTaxes.ToList();

            ViewData["flag"] = "1st";

            NewSaleModel model = new NewSaleModel();
            return PartialView("_AddMorePartialView", model);

        }

        [HttpPost]
        public ActionResult AddMorePartialView(NewSaleModel model)
        {
            var obj2 = new trnSalesItem();

            obj2 = model.trnSaleItems.ToList().FirstOrDefault();

            string serializedObject = TempData["MyObject"] as string;
            trnSalesItem myObject = JsonConvert.DeserializeObject<trnSalesItem>(serializedObject);

            obj2.AddBy = myObject.AddBy;

            obj2.SalesID = myObject.SalesID;

            obj2.AddDate = myObject.AddDate;

            TempData.Keep("MyObject");

            db.trnSalesItems.Add(obj2);
            db.SaveChanges();

            obj2.Rate = (from dailyRate in db.mstDailyRates
                         join item in db.mstItems on dailyRate.ItemGroupID equals item.ItemGroupID
                         join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                         where item.ItemID == obj2.ItemID && dailyRate.RateDate == obj2.AddDate
                         select dailyRate.SalesRate).FirstOrDefault();

            var saleRateType = (from item in db.mstItems
                                    join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                                    where item.ItemID == obj2.ItemID
                                    select itemGroup.SalesRateType).FirstOrDefault();

            if (saleRateType.ToString() == "N")
            {
                obj2.Amount = obj2.NetWt * obj2.Rate;
            }
            else if (saleRateType.ToString() == "G")
            {
                obj2.Amount = obj2.GrossWt * obj2.Rate;
            }
            else if (saleRateType.ToString() == "P")
            {
                obj2.Amount = obj2.Pcs * obj2.Rate;
            }
          
            if (TempData["Invoice"] == "GST")
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
            else if (TempData["Invoice"] == "IGST")
            {
                obj2.IGSTPrc = db.mstTaxes.Where(tax => tax.TaxID == obj2.TaxID)
                  .Select(tax => tax.IGSTPrc)
                  .FirstOrDefault();

                obj2.IGSTAmount = (obj2.Amount * obj2.IGSTPrc) / 100;

                obj2.TotalAmt = obj2.IGSTAmount + obj2.Amount;

            }



            db.Entry(obj2).State = EntityState.Modified;
            db.SaveChanges();
            TempData.Keep("Invoice");

            var totalAmount = db.trnSalesItems.Where(x => x.SalesID == myObject.SalesID)
                                  .Sum(x => x.TotalAmt);
            var a = (decimal)totalAmount;

            TempData["totalsale"] = a;

            ViewData["FinalAmount"] = totalAmount;

         //   return new EmptyResult();
            return Content("<h4>Item saved Successfully</h4><br>Please Go Back to Continue", "text/html");
        }

        [HttpGet]
        public decimal GetTotalAmt()
        {
            if (TempData["totalsale"] != null)
            {
                decimal a = (decimal)TempData["totalsale"];
                TempData.Keep("totalsale");
                return a;
            }
            return 0;
        }

        [HttpPost]
        public ActionResult AmountMethod(NewSaleModel sm)
        {
            var obj = new trnSale();
            obj = sm.trnSale;

            string serializedObject = TempData["MyObject"] as string;
            trnSalesItem myObject = JsonConvert.DeserializeObject<trnSalesItem>(serializedObject);

            TempData.Keep("MyObject");

            string serializedObject1 = TempData["MyObject1"] as string;
            trnSale obj1 = JsonConvert.DeserializeObject<trnSale>(serializedObject1);

            TempData.Keep("MyObject1");

            var sid = myObject.SalesID;

            obj1.CashAmount = obj.CashAmount;
            obj1.ChequeAmount = obj.ChequeAmount;

            obj1.TotalAmount = db.trnSalesItems.Where(x => x.SalesID == myObject.SalesID)
                                  .Sum(x => x.TotalAmt);


            if (obj1.TotalAmount.ToString() != null && obj1.CashAmount.ToString() != null && obj1.ChequeAmount.ToString() != null)
            {
                obj1.OutstandingAmount = obj1.TotalAmount - obj1.CashAmount - obj1.ChequeAmount;
                obj1.OutstandingAmount = Math.Round((decimal)obj1.OutstandingAmount, 2);
            }

            obj1.Remarks = obj.Remarks;
            db.Entry(obj1).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("viewInvoice", new { id = sid });
        }


        public ActionResult ViewDetails(string startDate, string endDate)
        {
            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);


                var sales = db.trnSales.Where(x => x.VoucherDate >= start && x.VoucherDate <= end).ToList();
                var saleItems = db.trnSalesItems.ToList();

                List<NewSaleModel> saleModels = new List<NewSaleModel>();

                foreach (var sale in sales)
                {
                    var salesItem = saleItems.Where(pi => pi.SalesID == sale.SalesID).ToList();
                    if (salesItem != null)
                    {
                        saleModels.Add(new NewSaleModel { trnSale = sale, trnSaleItems = salesItem });
                    }
                }

                if (saleModels.Any())
                {
                    return View(saleModels);
                }
                else
                {
                    ViewBag.Message = "No records found";
                    return View();
                }
            }
            else
            {
                var sales = db.trnSales.ToList();
                var saleItems = db.trnSalesItems.ToList();

                List<NewSaleModel> saleModel = new List<NewSaleModel>();

                foreach (var sale in sales)
                {
                    var itemsForSale = saleItems.Where(item => item.SalesID == sale.SalesID).ToList();

                    saleModel.Add(new NewSaleModel { trnSale = sale, trnSaleItems = itemsForSale });
                }

                return View(saleModel);
            }



        }

        public ActionResult Delete(int id)
        {
            var purchase = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            var purchaseItems = db.trnSalesItems.Where(x => x.SalesID == id).ToList();

            if (purchase == null)
            {
                return HttpNotFound();
            }
            else if (purchaseItems == null)
            {
                return HttpNotFound();
            }

            db.trnSales.Remove(purchase);


            foreach (var item in purchaseItems)
            {
                db.trnSalesItems.Remove(item);
            }

            db.SaveChanges();

            return RedirectToAction("ViewDetails");
        }

        public ActionResult viewInvoice(int id)
        {
            var sales = db.trnSales.Where(x => x.SalesID == id).FirstOrDefault();
            var saleItems = db.trnSalesItems.Where(x => x.SalesID == id).ToList();

            NewSaleModel saleModel = new NewSaleModel();

            saleModel.trnSale = sales;

            saleModel.trnSaleItems = saleItems;


            return View(saleModel);

        }
    }
}