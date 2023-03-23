using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    public class DailyRateController : Controller
    {
        JsoftWeb db = new JsoftWeb();
        public ActionResult CreateDailyRate()
        {
            
            ViewData["ItemGroupList"] = db.mstItemGroups.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateDailyRate(List<mstDailyRate> obj)
        {

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            var rateDate = Request.Form["RateDate"];

            var itemGrpId= db.mstItemGroups.ToList();


            int i = 0;
            foreach (mstDailyRate record in obj)
            {

                    record.ItemGroupID = itemGrpId[i].ItemGroupID;
                    record.RateDate = Convert.ToDateTime(rateDate);
                    record.AddDate = DateTime.Now;
                    record.AddBy = Convert.ToInt32(user);
                    db.mstDailyRates.Add(record);
                    db.SaveChanges();
                   
                i++;
            }
            
            return RedirectToAction("ViewDetailsDailyRate");
        }


          
        public ActionResult ViewDetailsDailyRate(string startDate, string endDate)
        {
            if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
            {
                var start = Convert.ToDateTime(startDate);
                var end = Convert.ToDateTime(endDate);
                var fetch = db.mstDailyRates.Where(x => x.RateDate >= start && x.RateDate <= end).ToList();

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
                var fetch = db.mstDailyRates.ToList();
                return View(fetch);
            }
        }


        public ActionResult EditDailyRate(string id)
        {
            string dateString = id;
            DateTime dateOnly = DateTime.Parse(dateString);
            dateOnly = dateOnly.Date;

            var result = db.mstDailyRates.Where(x => x.RateDate== dateOnly).ToList();

            ViewData["ItemGroupList"] = db.mstItemGroups.ToList();

            return View(result);
        }


        [HttpPost]
        public ActionResult EditDailyRate(string id, List<mstDailyRate> obj)
        {
            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();




          
            foreach (mstDailyRate record in obj)
            {

                record.EditDate = DateTime.Now;
                record.EditBy = Convert.ToInt32(user);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

            }

            return RedirectToAction("ViewDetailsDailyRate");

        }

        public ActionResult DeleteDailyRate(string id)
        {
            string dateString = id;
            DateTime dateOnly = DateTime.Parse(dateString);
            dateOnly = dateOnly.Date;

            var result = db.mstDailyRates.Where(x => x.RateDate == dateOnly).FirstOrDefault();
            
            return View(result);
        }

        [HttpPost]
        public ActionResult DeleteDailyRate(string id, mstDailyRate obj)
        {
            string dateString = id;
            DateTime dateOnly = DateTime.Parse(dateString);
            dateOnly = dateOnly.Date;

            obj = db.mstDailyRates.Where(x => x.RateDate == dateOnly).FirstOrDefault();
            db.mstDailyRates.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetailsDailyRate");
        }

        public ActionResult DetailsDailyRate(string id)
        {
            string dateString = id;
            DateTime dateOnly = DateTime.Parse(dateString);
            dateOnly = dateOnly.Date;

            var result = db.mstDailyRates.Where(x => x.RateDate == dateOnly).FirstOrDefault();
         
            return View(result);
        }

        public ActionResult HomeDailyRate()
        {
            ViewData["ItemGroupList"] = db.mstItemGroups.ToList();
            return View();
        }

    }
}