using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _10X_ManagerPortal.Models;

namespace _10X_ManagerPortal.Services
{
    
        public interface IAuthService
        {
            bool ValidateLogin(LoginVM model);
        void Logout();
    }
    
}
