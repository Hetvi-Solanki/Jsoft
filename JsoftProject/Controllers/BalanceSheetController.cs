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
    public class BalanceSheetController : Controller
    {
        JsoftWeb db = new JsoftWeb();
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(mstBsgrGroup obj)
        {
            obj.AddDate = DateTime.Now;

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj.AddBy = Convert.ToInt32(user);

            db.mstBsgrGroups.Add(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult ViewDetails()
        {
            var fetch = db.mstBsgrGroups.ToList();
            return View(fetch);
        }



        public ActionResult Edit(int id)
        {
            var result = db.mstBsgrGroups.Where(x => x.BsgrID == id).FirstOrDefault();

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(int id, mstBsgrGroup obj)
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
            var result = db.mstBsgrGroups.Where(x => x.BsgrID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id, mstBsgrGroup obj)
        {

            obj = db.mstBsgrGroups.Where(x => x.BsgrID == id).FirstOrDefault();
            db.mstBsgrGroups.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Details(int id)
        {
            var result = db.mstBsgrGroups.Where(x => x.BsgrID == id).FirstOrDefault();
            return View(result);
        }
    }
}