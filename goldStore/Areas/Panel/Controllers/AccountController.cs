using goldStore.Areas.Panel.Models;
using goldStore.Areas.Panel.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace goldStore.Areas.Panel.Controllers
{
    
    public class AccountController : Controller
    {
        UserRepository repoUser = new UserRepository(new goldstoreEntities());
        [Authorize(Roles = "admin")]
        // GET: Panel/Account
        public ActionResult Index()
        {
            return RedirectToAction("MyProfile");
        }
        public ActionResult MyProfile()
        {
            if(User.Identity.IsAuthenticated)
            {
                user _user = repoUser.GetAll().Where(x => x.email == User.Identity.Name).FirstOrDefault();

            }
            return View();
        }
    }
}