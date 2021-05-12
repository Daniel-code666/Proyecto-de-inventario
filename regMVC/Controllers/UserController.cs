using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using regMVC.Models;

namespace regMVC.Controllers
{
    public class UserController : Controller
    {
        //registro
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(user user)
        {
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                var isExist = cedula_check(user.cedula);

                var isExistCorreo = correo_check(user.correo);

                if (isExist)
                {
                    ModelState.AddModelError("CedulaExist", "Ya está registrada la cedula");
                    return View(user);
                }

                if(isExistCorreo)
                {
                    ModelState.AddModelError("CorreoExist", "El correo ya está registrado");
                    return View(user);
                }

                using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
                {
                    dc.user.Add(user);
                    dc.SaveChanges();

                    message = "Registro completado";

                    Status = true;
                }

            }
            else
            {
                message = "Petición inválida";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;

            return View();
        }


        //login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(userLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                //var v = dc.user.Where(a => a.fname == login.fname).FirstOrDefault();
                //var c = dc.user.Where(b => b.lname == login.lname).FirstOrDefault();

                var c = dc.user.Where(x => x.correo == login.correo).FirstOrDefault();

                if (c != null)
                {
                    if (string.Compare(login.pass, c.pass) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20;
                        var ticket = new FormsAuthenticationTicket(login.correo, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            #region
                            return RedirectToAction("Index", "Home");
                            #endregion
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("WrongPass", "Contraseña incorrecta");
                        message = "La credencial provisionada es inválida";
                    }
                }
                else
                {
                    
                    ModelState.Clear();
                    ModelState.AddModelError("UnknowUser", "Usuario no registrado");
                    message = "La credencial provisionada es inválida";
                }
            }

            ViewBag.Message = message;
            return View();
        }

        //listar

        public ActionResult edit2list()
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var us = System.Web.HttpContext.Current.User.Identity.Name;

                return View(dc.user.Where(a => a.correo == us).ToList());
            }

        }

        //editar
        public ActionResult edit(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.user.Where(a => a.id == id).FirstOrDefault();
                return View(u);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(user user)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                bool Status = false;

                var u = dc.user.Where(a => a.id == user.id).FirstOrDefault();

                dc.user.Remove(u);
                dc.user.Add(user);
                dc.SaveChanges();

                Status = true;

                ViewBag.Status = Status;

                Logout();

                return View();

            }
        }

        //eliminar usuario
        public ActionResult delete(int id)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.user.Where(a => a.id == id).FirstOrDefault();
                return View(u);
            }
        }

        [HttpPost]
        public ActionResult delete(user user)
        {
            bool Status = false;
            string message = "";

            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.user.Where(a => a.id == user.id).FirstOrDefault();

                dc.user.Remove(u);
                dc.SaveChanges();

                message = "registro de usuario eliminado";

                Status = true;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;

            return View();
        }


        //inventario op
        public ActionResult inventarioCtrl()
        {
            return View();
        }

        //logout

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "user");
        }

        //restablecer contraseña
        [HttpGet]
        public ActionResult recoverPass()
        {
            return View();
        }

        /*[HttpPost]
        public ActionResult recoverPass(userRecoverPass usuario)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {

                var u = dc.user.Where(a => a.cedula == usuario.cedula)?.FirstOrDefault();
                if (u != null) return View(usuario);

            }
            return View();
        }*/

        [HttpPost]
        public ActionResult recoverPass(userRecoverPass usuario)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var u = dc.user.Where(a => a.cedula == usuario.cedula)?.FirstOrDefault();

                string message = "";
                bool Status = false;

                if (u != null)
                {
                    u.pass = usuario.newpass;
                    u.check_pass = u.pass;
                    dc.SaveChanges();
                    message = "Actualización correcta";
                    Status = true;    
                }
                ViewBag.Message = message;
                ViewBag.Status = Status;
                return View();
            }
        }

        //verifica que la cédula no se repita
        [NonAction]
        public bool cedula_check(string cedula)
        {
            using (inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var v = dc.user.Where(a => a.cedula == cedula).FirstOrDefault();
                return v != null;
            }
        }

        //verifica que el correo no se repita
        public bool correo_check(string correo)
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var c = dc.user.Where(b => b.correo == correo).FirstOrDefault();
                return c != null;
            }
        }
    }
}