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
    public class TaxController : Controller
    {
        JsoftWeb db = new JsoftWeb();
        public ActionResult Create()
        {
           return View();
        }

        [HttpPost]
        public ActionResult Create(mstTax obj)
        {
            obj.AddDate = DateTime.Now;

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj.AddBy = Convert.ToInt32(user);


            db.mstTaxes.Add(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult ViewDetails()
        {
            var fetch = db.mstTaxes.ToList();
            return View(fetch);

        }



        public ActionResult Edit(int id)
        {
            var result = db.mstTaxes.Where(x => x.TaxID == id).FirstOrDefault();

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(int id, mstTax obj)
        {
            obj.EditDate = DateTime.Now;

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj.EditBy = Convert.ToInt32(user);

            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Delete(int id)
        {
            var result = db.mstTaxes.Where(x => x.TaxID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id, mstTax obj)
        {

            obj = db.mstTaxes.Where(x => x.TaxID == id).FirstOrDefault();
            db.mstTaxes.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Details(int id)
        {
            var result = db.mstTaxes.Where(x => x.TaxID == id).FirstOrDefault();
            return View(result);
        }

    }
}