using JsoftProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JsoftProject.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        JsoftWeb db = new JsoftWeb();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(MememberLogin m)
        {
            bool isValid = db.mstUsers.Any(x => x.LoginID == m.UserName && x.Password == m.Password);

            if (isValid)
            {
                FormsAuthentication.SetAuthCookie(m.UserName, false);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid LoginID or password");
            return View();

        }

        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUp(mstUser usr)
        {
            
                usr.IsActive = true;
                usr.AddDate = DateTime.Now;
                usr.EditDate = DateTime.Now;

                db.mstUsers.Add(usr);
                db.SaveChanges();
                return RedirectToAction("Login");
            
            
           
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}