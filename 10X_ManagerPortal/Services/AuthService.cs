using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using _10X_ManagerPortal.Models;


namespace _10X_ManagerPortal.Services
{
    public class AuthService : IAuthService
    {
        public bool ValidateLogin(LoginVM model)
        {
            var serviceLayer = new SAPRestServiceLayer();
            return serviceLayer.ValidateUserCredentials(model);
        }
        public void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }
    }
}