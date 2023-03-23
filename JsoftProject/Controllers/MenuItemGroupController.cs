using Antlr.Runtime.Misc;
using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace JsoftProject.Controllers
{
    [Authorize]
    public class MenuItemGroupController : Controller
    {
        // GET: Master

        JsoftWeb db=new JsoftWeb();
        public ActionResult Create()
        {
            

            ViewData["Metals"] = db.mstMetalTypes.ToList();

            ViewData["PurchaseRates"] = db.mstRateTypes.ToList();

            ViewData["SalesRates"] = db.mstRateTypes.ToList();

            ViewData["MesrUnits"] = db.mstMesrUnits.ToList();

            return View();

        }

        [HttpPost]
        public ActionResult Create(mstItemGroup obj)
        {
            obj.AddDate=DateTime.Now;

            var user = (from mstUser in db.mstUsers
                           where mstUser.LoginID == User.Identity.Name
                           select mstUser.UserID).Single();

            obj.AddBy = Convert.ToInt32(user);

         
            db.mstItemGroups.Add(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult ViewDetails(string SearchBy, string search)
        {

            if (SearchBy == "ItemGroupName")
            {
                var data = db.mstItemGroups.Where(x => x.GroupName.StartsWith(search) || search == null).ToList();
                return View(data);
            }
            
            else if (SearchBy == "MetalType")
            { 
                var result = (from mt in db.mstMetalTypes
                              join ig in db.mstItemGroups on mt.MetalType equals ig.MetalType
                              where mt.MetalTypeDesc.StartsWith(search) || search == null
                              select ig).ToList();
                return View(result);
            }
            else
            {
                var fetch = db.mstItemGroups.ToList();
                
                return View(fetch);
            }
            
        }

        public ActionResult Edit(int id)
        {
            var result=db.mstItemGroups.Where(x=>x.ItemGroupID==id).FirstOrDefault();
            ViewData["Metals"] = db.mstMetalTypes.ToList();

            ViewData["PurchaseRates"] = db.mstRateTypes.ToList();

            ViewData["SalesRates"] = db.mstRateTypes.ToList();

            ViewData["MesrUnits"] = db.mstMesrUnits.ToList();

            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(int id,mstItemGroup obj)
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
            var result = db.mstItemGroups.Where(x => x.ItemGroupID == id).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Delete(int id,mstItemGroup obj)
        {
            
            obj = db.mstItemGroups.Where(x => x.ItemGroupID == id).FirstOrDefault();
            db.mstItemGroups.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("ViewDetails");
        }

        public ActionResult Details(int id)
        {
            var result=db.mstItemGroups.Where(x=>x.ItemGroupID== id).FirstOrDefault();

            var a = (from mstRateType in db.mstRateTypes
                     where mstRateType.RateType == result.PurchaseRateType
                     select mstRateType.RateTypeDesc).ToString();

            ViewData["purchase"] = a;

            var b = (from mstRateType in db.mstRateTypes
                     where mstRateType.RateType == result.SalesRateType
                     select mstRateType.RateTypeDesc).ToString();

            ViewData["sales"] = b;
            //ViewData["PurchaseRate"] = db.mstRateTypes.Where(x => x.RateType == result.PurchaseRateType);
            //ViewData["SalesRate"] = db.mstRateTypes.Where(x=>x.RateType== result.SalesRateType);
            return View(result);
        }
    }
}