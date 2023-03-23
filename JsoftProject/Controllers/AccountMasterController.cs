using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsoftProject.Controllers
{
    public class AccountMasterController : Controller
    {
        JsoftWeb db = new JsoftWeb();
        public ActionResult Create()
        {
            ViewData["BsgrGroup"] = db.mstBsgrGroups.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(mstAccount obj)
        {
            obj.AddDate = DateTime.Now;

            var user = (from mstUser in db.mstUsers
                        where mstUser.LoginID == User.Identity.Name
                        select mstUser.UserID).Single();

            obj.AddBy = Convert.ToInt32(user);


            db.mstAccounts.Add(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult ViewDetails(string SearchBy, string search)
        {
            if (SearchBy == "ItemName")
            {
                // var data1=(from m in db.mstItems)
                var data = db.mstItems.Where(x => x.ItemName.StartsWith(search) || search == null).ToList();
                return View(data);
            }
            else if (SearchBy == "MetalType")
            {
                var result = (from mt in db.mstMetalTypes
                              join ig in db.mstItemGroups on mt.MetalType equals ig.MetalType
                              join i in db.mstItems on ig.ItemGroupID equals i.ItemGroupID
                              where mt.MetalTypeDesc.StartsWith(search) || search == null
                              select i).ToList();
                return View(result);
            }
            else
            {
                var fetch = db.mstAccounts.ToList();
                return View(fetch);
            }
        }



        public ActionResult Edit(int id)
        {
            ViewData["BsgrGroup"] = db.mstBsgrGroups.ToList();
            var result = db.mstAccounts.Where(x => x.AccountID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(int id, mstAccount obj)
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
            var result = db.mstAccounts.Where(x => x.AccountID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id, mstAccount obj)
        {

            obj = db.mstAccounts.Where(x => x.AccountID == id).FirstOrDefault();
            db.mstAccounts.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Details(int id)
        {
            var result = db.mstAccounts.Where(x => x.AccountID == id).FirstOrDefault();
            return View(result);
        }
    }
}