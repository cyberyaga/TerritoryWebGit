using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TerritoryWeb.Models;
using Microsoft.AspNet.Identity.Owin;

namespace TerritoryWeb.Controllers
{
    public class PublishersController : Controller
    {
        TerritoryWebEntities db = new TerritoryWebEntities();

        [AuthLog(Roles = "Admin, Manager")]
        public ActionResult Index()
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            var Publishers = db.AspNetUsers.Where(x => x.CongregationID == CongID).ToList();
                
            return View(Publishers);   

        }

        [AuthLog(Roles = "Admin, Manager")]
        public ActionResult Details(string id)
        {
            var p = db.AspNetUsers.Find(id);
            return View(p);
        }

        [AuthLog(Roles = "Admin, Manager")]
        public ActionResult Edit(string id)
        {
            var p = db.AspNetUsers.Find(id);
            
            return View(p);
        }

        [HttpGet]
        [AuthLog(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
            return View();
        }

        [AuthLog(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Email,FirstName,LastName,Address1,Address2,PhoneNumber,CellPhone")] CreatePublisherViewModel model, string Permissions)
        {
            if (ModelState.IsValid)
            {
                model.CongregationID = Common.IdentityExtensions.GetCongregationID(User);

                var user = new ApplicationUser { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    FirstName = model.FirstName, 
                    LastName = model.LastName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    PhoneNumber = model.PhoneNumber,
                    CellPhone = model.CellPhone,
                    CongregationID = model.CongregationID
                };


                ApplicationUserManager UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var result = await UserManager.CreateAsync(user);


                if (result.Succeeded)
                {
                    //Set permissions
                    if (Permissions != "" && Permissions != "None") //Check that a permission is to be set
                    {
                        if (db.AspNetRoles.Any(x => x.Name == Permissions)) //Check that user role exists
                        {
                            //Add Role
                            await UserManager.AddToRoleAsync(user.Id, Permissions);

                            //Send confirmation to new user to create a password.
                            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                            var callbackUrl = Url.Action(
                               "ConfirmEmail", "Account",
                               new { userId = user.Id, code = code },
                               protocol: Request.Url.Scheme);

                            await UserManager.SendEmailAsync(user.Id,
                               "Confirm your account",
                               "Please confirm your account by clicking this link: <a href=\""
                                                               + callbackUrl + "\">link</a>");

                        }
                    }

                    return RedirectToAction("Index", "Publishers");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}