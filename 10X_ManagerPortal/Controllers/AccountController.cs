using _10X_ManagerPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using _10X_ManagerPortal.Helpers;


namespace _10X_ManagerPortal.Controllers
{
    public class AccountController : Controller
    {
        SAPRestServiceLayer serviceLayer;
        public ActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            serviceLayer = new SAPRestServiceLayer();
       
            if (!serviceLayer.ValidateUserCredentials(model))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }
          
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult Logout()
        {
            
            Session.Clear();
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }


    }
}
