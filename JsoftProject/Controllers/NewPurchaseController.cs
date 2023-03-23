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
    public class NewPurchaseController : Controller
    {
        JsoftWeb db = new JsoftWeb();

        private decimal finalamt = 0;

        // GET: NewPurchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var vNo = int.Parse(db.trnPurchases.Max(p => p.VoucherNo));

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
        public ActionResult Create(NewPurchaseModel pm, string variable)
        {

            return View();
        }

        public ActionResult MainView()
        {
            var vNo = int.Parse(db.trnPurchases.Max(p => p.VoucherNo));

            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            ViewData["VoucherNo"] = value;

            ViewData["VoucherDate"] = DateTime.Now.ToShortDateString();

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["ItemList"] = db.mstItems.ToList();

            ViewData["Taxes"] = db.mstTaxes.ToList();

            ViewData["flag"] = "1st";
            var model = new NewPurchaseModel();
            // set values for the model properties as needed
            return View(model);
          //  return View();
        }

        [HttpPost]
        public int MainView(NewPurchaseModel pm)
        {

            var obj1 = new trnPurchase();
            var obj2 = new trnPurchaseItem();

            obj1 = pm.trnPurchase;

            ViewData["hflah"] = "2nd";

            var vNo = int.Parse(db.trnPurchases.Max(p => p.VoucherNo));
            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            obj1.VoucherNo = value;
            obj1.VoucherDate = DateTime.Now.Date;
            obj1.AddDate = DateTime.Now.Date;


            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj1.AddBy = Convert.ToInt32(user);

            db.trnPurchases.Add(obj1);
            db.SaveChanges();

            var x = (from ma in db.mstAccounts
                     join tp in db.trnPurchases on ma.AccountID equals tp.AccountID
                     where ma.AccountID == obj1.AccountID
                     select ma.AccountName).FirstOrDefault();



            var state2 = (from state in db.mstStates
                          join company in db.mstCompanies on state.StateID equals company.StateID
                          select state.StateName).FirstOrDefault();


            var state1 = (from a in db.mstAccounts
                          join p in db.trnPurchases on a.AccountID equals p.AccountID
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
            obj2.PurchaseID = obj1.PurchaseID;

            string serializedObject = JsonConvert.SerializeObject(obj2);
            TempData["MyObject"] = serializedObject;
            TempData.Keep("MyObject");

            TempData["Invoice"] = obj1.InvoiceType;
            TempData.Keep("Invoice");

            string serializedObject1 = JsonConvert.SerializeObject(obj1);
            TempData["MyObject1"] = serializedObject1;
            TempData.Keep("MyObject1");
            //------------------------- finished2 -----------------------------

            //if (obj1.TotalAmount.ToString() != null && obj1.CashAmount.ToString() != null && obj1.ChequeAmount.ToString() != null)
            //{
            //    obj1.OutstandingAmount = obj1.TotalAmount - obj1.CashAmount - obj1.ChequeAmount;
            //    obj1.OutstandingAmount = Math.Round((decimal)obj1.OutstandingAmount, 2);
            //}
            //db.Entry(obj1).State = EntityState.Modified;
            //db.SaveChanges();
            //return RedirectToAction("ViewDetails");

            //--------------------------Finished 3 ------------------------------

            //return new EmptyResult();

            //return Content("Item saved.");

            // return Content("<h4>Account saved.</h4>", "text/html");
            return 1;

        }


        public ActionResult AddMorePartialView()
        {
            var vNo = int.Parse(db.trnPurchases.Max(p => p.VoucherNo));

            vNo++;

            string value = vNo.ToString().PadLeft(5, '0');

            ViewData["VoucherNo"] = value;

            ViewData["VoucherDate"] = DateTime.Now.ToShortDateString();

            ViewData["AccountList"] = db.mstAccounts.ToList();

            ViewData["ItemList"] = db.mstItems.ToList();

            ViewData["Taxes"] = db.mstTaxes.ToList();

            ViewData["flag"] = "1st";

            NewPurchaseModel model = new NewPurchaseModel();
            return PartialView("_AddMorePartialView", model);

        }

        [HttpPost]
        public ActionResult AddMorePartialView(NewPurchaseModel model)
        {
            var obj2 = new trnPurchaseItem();

            obj2 = model.trnPurchaseItems.ToList().FirstOrDefault();

            string serializedObject = TempData["MyObject"] as string;
            trnPurchaseItem myObject = JsonConvert.DeserializeObject<trnPurchaseItem>(serializedObject);

            obj2.AddBy = myObject.AddBy;

            obj2.PurchaseID = myObject.PurchaseID;

            obj2.AddDate = myObject.AddDate;

            TempData.Keep("MyObject");

            db.trnPurchaseItems.Add(obj2);
            db.SaveChanges();

            obj2.Rate = (from dailyRate in db.mstDailyRates
                         join item in db.mstItems on dailyRate.ItemGroupID equals item.ItemGroupID
                         join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                         where item.ItemID == obj2.ItemID && dailyRate.RateDate == obj2.AddDate
                         select dailyRate.PurchaseRate).FirstOrDefault();

            var purchaseRateType = (from item in db.mstItems
                                    join itemGroup in db.mstItemGroups on item.ItemGroupID equals itemGroup.ItemGroupID
                                    where item.ItemID == obj2.ItemID
                                    select itemGroup.PurchaseRateType).FirstOrDefault();

            if (purchaseRateType.ToString() == "N")
            {
                obj2.Amount = obj2.NetWt * obj2.Rate;
            }
            else if (purchaseRateType.ToString() == "G")
            {
                obj2.Amount = obj2.GrossWt * obj2.Rate;
            }
            else if (purchaseRateType.ToString() == "P")
            {
                obj2.Amount = obj2.Pcs * obj2.Rate;
            }
            else if (purchaseRateType.ToString() == "M")
            {
                obj2.Amount = obj2.Mrp;
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

            var totalAmount = db.trnPurchaseItems.Where(x => x.PurchaseID == myObject.PurchaseID)
                                  .Sum(x => x.TotalAmt);

            ViewData["FinalAmount"] = totalAmount;

             var a = (decimal)totalAmount;

            TempData["total"] = a;
            TempData.Keep("total");

            //return Json(new { MTotalAmount = a });
            // return Content("Item saved.");
            //return new EmptyResult();

              return Content("<h4>Item saved Successfully</h4><br>Please Go Back to Continue", "text/html");

            // return 0;
            // append previous state to the URL
            
        }

        [HttpGet]
        public decimal GetTotalAmt()
        {
            if (TempData["total"]!=null)
            {
                decimal a = (decimal)TempData["total"];
                TempData.Keep("total");
                return a;
            }
            return 0;
        }

        [HttpPost]
        public ActionResult AmountMethod(NewPurchaseModel pm)
        {
            var obj = new trnPurchase();
            obj = pm.trnPurchase;

            string serializedObject = TempData["MyObject"] as string;
            trnPurchaseItem myObject = JsonConvert.DeserializeObject<trnPurchaseItem>(serializedObject);

            TempData.Keep("MyObject");

            string serializedObject1 = TempData["MyObject1"] as string;
            trnPurchase obj1 = JsonConvert.DeserializeObject<trnPurchase>(serializedObject1);

            TempData.Keep("MyObject1");

            var pid = myObject.PurchaseID;

            obj1.CashAmount = obj.CashAmount;
            obj1.ChequeAmount = obj.ChequeAmount;

            obj1.TotalAmount = db.trnPurchaseItems.Where(x => x.PurchaseID == myObject.PurchaseID)
                                  .Sum(x => x.TotalAmt);


            if (obj1.TotalAmount.ToString() != null && obj1.CashAmount.ToString() != null && obj1.ChequeAmount.ToString() != null)
            {
                obj1.OutstandingAmount = obj1.TotalAmount - obj1.CashAmount - obj1.ChequeAmount;
                obj1.OutstandingAmount = Math.Round((decimal)obj1.OutstandingAmount, 2);
            }

            obj1.Remarks = obj.Remarks;
            db.Entry(obj1).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("viewInvoice", new { id = pid });
        }


        public ActionResult ViewDetails(string startDate, string endDate)
        {
           if(!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);


                var purchases = db.trnPurchases.Where(x => x.VoucherDate >= start && x.VoucherDate <= end).ToList();
                var purchaseItems = db.trnPurchaseItems.ToList();

                List<NewPurchaseModel> purchaseModels= new List<NewPurchaseModel>();

                foreach (var purchase in purchases)
                {
                    var purchaseItem = purchaseItems.Where(pi => pi.PurchaseID == purchase.PurchaseID).ToList();
                    if (purchaseItem != null)
                    {
                        purchaseModels.Add(new NewPurchaseModel { trnPurchase = purchase, trnPurchaseItems = purchaseItem });
                    }
                }

                if (purchaseModels.Any())
                {
                    return View(purchaseModels);
                }
                else
                {
                    ViewBag.Message = "No records found";
                    return View();
                }
            }
            else
            {
                var purchases = db.trnPurchases.ToList();
                var purchaseItems = db.trnPurchaseItems.ToList();

                List<NewPurchaseModel> purchaseModel = new List<NewPurchaseModel>();

                foreach (var purchase in purchases)
                {
                    var itemsForPurchase = purchaseItems.Where(item => item.PurchaseID == purchase.PurchaseID).ToList();

                    purchaseModel.Add(new NewPurchaseModel { trnPurchase = purchase, trnPurchaseItems = itemsForPurchase });
                }

                return View(purchaseModel);
            }

            

        }

        public ActionResult Delete(int id)
        {
            var purchase = db.trnPurchases.Where(x => x.PurchaseID == id).FirstOrDefault();
            var purchaseItems = db.trnPurchaseItems.Where(x => x.PurchaseID == id).ToList();

            if (purchase == null)
            {
                return HttpNotFound();
            }
            else if(purchaseItems == null)
            {
                return HttpNotFound();
            }

            db.trnPurchases.Remove(purchase);


            foreach(var item in purchaseItems)
            {
                db.trnPurchaseItems.Remove(item);
            }
            
            db.SaveChanges();

            return RedirectToAction("ViewDetails");
        }

        public ActionResult viewInvoice(int id)
        {
            var purchases = db.trnPurchases.Where(x => x.PurchaseID == id).FirstOrDefault();
            var purchaseItems = db.trnPurchaseItems.Where(x => x.PurchaseID == id).ToList();

            NewPurchaseModel purchaseModel = new NewPurchaseModel();

            purchaseModel.trnPurchase = purchases;

            purchaseModel.trnPurchaseItems = purchaseItems;
            

            return View(purchaseModel);
           
        }

    }
}