using goldStore.Areas.Panel.Models.Repository;
using goldStore.Models.ViewModel;
using System;
using goldStore.Areas.Panel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Net.Configuration;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Web.Security;

namespace goldStore.Controllers
{
    public class UserController : Controller
    {
        CategoryRepository repoCategory = new CategoryRepository(new Areas.Panel.Models.goldstoreEntities());
        BrandRepository repoBrand = new BrandRepository(new Areas.Panel.Models.goldstoreEntities());
        UserRepository repoUser = new UserRepository(new Areas.Panel.Models.goldstoreEntities());
        // GET: User
        public PartialViewResult PartialBrands()
        {
            return PartialView(repoBrand.GetAll());
        }


        public PartialViewResult PartialNewArrivals()
        {
            return PartialView();
        }
        public PartialViewResult PartialCategory()
        {
            return PartialView(repoCategory.GetAll());
        }
        public PartialViewResult PartialPrice()
        {
            return PartialView();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register Register)
        {   
            bool status = false;
            string message = "";
            if (ModelState.IsValid)
            { 
             if (isExistUser(Register.email))
                {
                    message = "Bu maile kayıtlı bir kullanıcı mevcuttur.";
                    ViewBag.message = message;
                        return View();
                }

            user user = new user();
                user.email = Register.email;
            user.password = Crypto.Hash(Register.password);
            user.rePassword = Crypto.Hash(Register.confirmPassword);

            user.activationCode = Guid.NewGuid().ToString();
            user.roleId = 2;
            user.isMailVerified = false;
            user.createdDate = DateTime.Now;

            repoUser.Save(user);
            SendVerificationLinkEmail(user.email, user.activationCode);

                message = "Kaydın oldu"
                    + user.email + "adresine gitti";

                status = true;
                ViewBag.status = status;
                ViewBag.message = message;

           }
            return View();
           
        }



        public ActionResult Index()
        {
            return View();
        }
        [NonAction]
        public bool isExistUser (string username)
        {
            var user = repoUser.GetAll().Where(a => a.email == username).FirstOrDefault();
            return user == null ? false : true;
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var verifyUrl = "/User/VerifyAccount/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "goldStore Üyeligi");
                var toEmail = new MailAddress(emailID);

                string subject = "goldStore Hosgeldiniz!";
                string body = "<br/><br/>goldStore hesabiniz basariyla olusturulmustur. Hesabiniz aktive etmek için asagidaki linke tiklayiniz" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                var smtp = new SmtpClient
                {
                    Host = network.Network.Host,
                    Port = network.Network.Port,
                    EnableSsl = network.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = network.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(network.Network.UserName, network.Network.Password)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            var result = repoUser.GetAll().Where(a=> a.activationCode == new Guid(id).ToString()).FirstOrDefault();
            if(result!=null)
            {
                result.isMailVerified = true;
                repoUser.Update(result);
                status = true;
                ViewBag.status = status;
                ViewBag.message = "Hesabınız aktive edildi.";

            }
            else
            {
                ViewBag.Status = status;
                ViewBag.message = "Geçersiz istek";
            }
            return View("Login");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login,string ReturnUrl)
        {
            string message = "";
            int sayac = 0;
            bool status = false;
            if(ModelState.IsValid)
            {
                user _user = repoUser.GetAll().Where(x => x.email == login.email).FirstOrDefault();
                if(_user==null)
                {
                    message = "Böyle br email kayıtlı değil";
                    ViewBag.message = message;
                    ViewBag.status = status;
                    
                }
                bool verify = _user.isMailVerified ?? false;
                if (!verify)
                {
                    message = "Mail doğrulaması yapılmamış";
                    ViewBag.message = message;
                    ViewBag.status = status;
                    sayac++;
                    _user.loginAttempt = sayac ;
                    repoUser.Update(_user);
                }
                if (_user.isActive == false) 
                {
                    sayac++;
                    message = "hesabınız askıya alınmıştır";
                    ViewBag.message = message;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);
                }
                login.password = Crypto.Hash(login.password);
                if (string.Compare(login.password, _user.password) == 0)
                {
                    _user.loginTime = DateTime.Now;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);

                    Session["username"] = _user.email;
                    int timeOut = login.rememberMe ? 60 : 10;

                    var ticket = new FormsAuthenticationTicket(login.email, login.rememberMe, timeOut);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeOut);
                    cookie.HttpOnly = true;

                    FormsAuthentication.SetAuthCookie("userName", login.rememberMe);
                    Response.Cookies.Add(cookie);

                    if (_user.roleId==1)
                        return Redirect("~/Panel/Category");

                    if(Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Shop");
                    }
                }
                else
                {
                    sayac++;
                    _user.loginAttempt = sayac;
                    repoUser.Update(_user);
                    message = "Giriş yapılamadı.Şifre yanlış";
                }
                
            }
            ViewBag.status = status;
            ViewBag.message = message;
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            string message = "";
            bool status = false;
            if(!string.IsNullOrEmpty(email))
            {
                user _user = repoUser.GetAll().Where(x => x.email == email).FirstOrDefault();
                if(_user!=null)
                {
                    Guid resetCode = Guid.NewGuid();
                    _user.resetCode = resetCode.ToString();
                    repoUser.Update(_user);

                    SendResetCodeEmail(email,resetCode.ToString());
                    status = true;
                    message = "Parola sıfırlama işlemi başarılı şekilde gerçekletirildi. Parola sıfırlma linki " + email + "adresinde gönderildi.";
                }
                else
                {
                    message = "Geçersiz Eposta";
                }
            }
            else
            {
                message = "Email alanı boş bırakılamaz";
            }
            ViewBag.message = message;
            ViewBag.status = status;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            ResetPassword rp = new ResetPassword()
            {
                resetCode = id
            };
            return View(rp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword rp)
        {
            string message = "";
            bool status = false;
            if(ModelState.IsValid)
            {
                user _user = repoUser.GetAll().Where(x => x.email == rp.resetCode).FirstOrDefault();
                if (_user != null)
                {
                    _user.password = Crypto.Hash(rp.newPassword);
                    _user.rePassword = Crypto.Hash(rp.confirmPassword);
                    repoUser.Update(_user);
                    status = true;
                    message = "Şifreniz başarıyla değiştirildi";

                }
                else
                    message = "Geçersiz şifre";
            }
            ViewBag.message = message;
            ViewBag.status = status;
            return View();
        }

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [NonAction]
        public void SendResetCodeEmail(string emailID, string ResetCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var resetUrl = "/User/ResetPassword/" + ResetCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, resetUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "goldStore Parola Sıfırlama");
                var toEmail = new MailAddress(emailID);

                string subject = "goldStore Parola Sıfırlama!";
                string body = "<br/><br/>isteğiniz üzere goldStore parola sıfırlama işleminiz talebi alınmıştır onaylamak için aşağıdaki bağlanıtya tıklayınız"+
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                var smtp = new SmtpClient
                {
                    Host = network.Network.Host,
                    Port = network.Network.Port,
                    EnableSsl = network.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = network.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(network.Network.UserName, network.Network.Password)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}