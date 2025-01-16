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
    public class AdminController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        // GET: Admin
        [AuthLog(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(); 

        }

        [AuthLog(Roles = "Admin")]
        public ActionResult ManageUsers()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                return View(db.AspNetUsers);
            }
            else
            {
                int CongID = Common.IdentityExtensions.GetCongregationID(User);
                return View(db.AspNetUsers.Where(x => x.CongregationID == CongID));
            }

        }

        [AuthLog(Roles = "Admin")]
        public ActionResult CreateUser()
        {
            CreatePublisherViewModel model = new CreatePublisherViewModel();
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            ViewBag.Roles = db.AspNetRoles.Where(x => x.Name != "SuperAdmin").ToList();
            ViewBag.PublisherTypes = db.PublisherTypes.ToList();
            if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.Congregations = new SelectList(db.Congregations, "Id", "Description", CongID);
            }
            else
            {
                ViewBag.Congregations = new SelectList(db.Congregations.Where(x => x.Id == CongID), "Id", "Description", CongID);
            }
            return View(model);
        }

        [AuthLog(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser([Bind(Include = "UserName,Email,FirstName,LastName,Address1,Address2,PhoneNumber,CellPhone,CongregationID")] CreatePublisherViewModel model, string Permissions, string PublisherType)
        {
            if (ModelState.IsValid)
            {
                //model.CongregationID = Common.IdentityExtensions.GetCongregationID(User);

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    PhoneNumber = model.PhoneNumber,
                    CellPhone = model.CellPhone,
                    CongregationID = model.CongregationID
                };

                //Set publisher type
                if (PublisherType != "")
                {
                    var pt = db.PublisherTypes.Single(x => x.Description == PublisherType);
                    user.TypeID = pt.Id;
                }


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
                               "ResetPassword", "Account",
                               new { code = code },
                               protocol: Request.Url.Scheme);

                            string smallURL = Common.Methods.ShrinkURL(callbackUrl);
                            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                            {

                                await UserManager.SendSmsAsync(user.Id, "Please confirm your account by clicking this link: \r\n\r\n"
                                                                                               + smallURL);

                            }
                            else if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                await UserManager.SendEmailAsync(user.Id,
                                                               "Confirm your account",
                                                               "Please confirm your account by clicking this link: \r\n\r\n"
                                                                                               + smallURL);
                            }
                        }
                    }

                    return RedirectToAction("ManageUsers", "Admin");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AuthLog(Roles = "Admin")]
        public ActionResult EditUser(string UserID)
        {
            AspNetUser u = db.AspNetUsers.Find(UserID);
            CreatePublisherViewModel model = new CreatePublisherViewModel();
            model.SetModelFromUser(u); //Set properties


            ViewBag.Roles = db.AspNetRoles.Where(x => x.Name != "SuperAdmin").ToList();
            ViewBag.PublisherTypes = db.PublisherTypes.ToList();
            if (User.IsInRole("SuperAdmin"))
            {
                ViewBag.Congregations = new SelectList(db.Congregations, "Id", "Description", u.CongregationID);
            }
            else
            {
                ViewBag.Congregations = new SelectList(db.Congregations.Where(x => x.Id == u.CongregationID), "Id", "Description", u.CongregationID);
            }

            return View(model);
        }

        [AuthLog(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser([Bind(Include = "Id,UserName,Email,FirstName,LastName,Address1,Address2,PhoneNumber,CellPhone,CongregationID")] CreatePublisherViewModel model, string Permissions, string PublisherType)
        {
            if (ModelState.IsValid)
            {
                int CongID = (model.CongregationID == 0) ? Common.IdentityExtensions.GetCongregationID(User) : model.CongregationID;

                AspNetUser user = db.AspNetUsers.Find(model.Id);
                
                //If username is changed, then check unique
                if (user.UserName != model.UserName)
                {
                    //if another person has this ID, stop everything
                    if (db.AspNetUsers.Any(x => x.Id != model.Id && x.UserName == model.UserName))
                    {
                        // If we got this far, something failed, redisplay form
                        CreatePublisherViewModel newmodel = new CreatePublisherViewModel();
                        newmodel.SetModelFromUser(user);
                        model.Roles = newmodel.Roles;
                        model.PublisherType = newmodel.PublisherType;
                        ViewBag.Roles = db.AspNetRoles.Where(x => x.Name != "SuperAdmin").ToList();
                        ViewBag.PublisherTypes = db.PublisherTypes.ToList();
                        ViewBag.Congregations = new SelectList(db.Congregations, "Id", "Description", model.CongregationID);
                        ViewBag.ErrorMessage = "ERROR! - Username is already taken. Please use a different username.";
                        return View(model);
                    }
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address1 = model.Address1;
                user.Address2 = model.Address2;
                user.PhoneNumber = model.PhoneNumber;
                user.CellPhone = model.CellPhone;
                user.CongregationID = CongID;

                //Set publisher type
                if (!string.IsNullOrWhiteSpace(PublisherType))
                {
                    var pt = db.PublisherTypes.Single(x => x.Description == PublisherType);
                    user.TypeID = pt.Id;
                }

                db.SaveChanges();


                ApplicationUserManager UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();


                //Set permissions
                if (Permissions != "") //Check that a permission is to be set
                {
                    foreach (AspNetRole r in user.AspNetUserRoles.Select(s => s.AspNetRole)) //Remove Roles
                    {
                        await UserManager.RemoveFromRoleAsync(user.Id, r.Name);
                    }

                    if (db.AspNetRoles.Any(x => x.Name == Permissions)) //Check that user role exists
                    {
                        //Add Role
                        await UserManager.AddToRoleAsync(user.Id, Permissions);
                    }
                }

                return RedirectToAction("ManageUsers", "Admin");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Roles = db.AspNetRoles.Where(x => x.Name != "SuperAdmin").ToList();
            ViewBag.PublisherTypes = db.PublisherTypes.ToList();
            ViewBag.Congregations = new SelectList(db.Congregations, "Id", "Description", model.CongregationID);
            return View(model);
        }

        [AuthLog(Roles = "Admin")]
        public async Task<JsonResult> SendPasswordReset(string UserID)
        {
            AspNetUser p = db.AspNetUsers.SingleOrDefault(x => x.Id == UserID);
            if (p == null)
            {
                return null;
            }



            //Send Email
            ApplicationUserManager UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //string TerritoryURL = Url.Action("Details", "Territory", new { id = TerritoryID }, Request.Url.Scheme);
            //string body = "You have been assigned a new territory.\r\n\r\nTo View it, please click this link:\r\n " + TerritoryURL;
            //UserManager.SendEmail(PublisherID, "Territory Assigned", body);


            //Send confirmation to new user to create a password.
            var code =  await UserManager.GeneratePasswordResetTokenAsync(p.Id);
            var callbackUrl = Url.Action(
               "ResetPassword", "Account",
               new { code = code },
               protocol: Request.Url.Scheme);

            string smallURL = Common.Methods.ShrinkURL(callbackUrl);

            if (!string.IsNullOrWhiteSpace(p.PhoneNumber))
            {
                await UserManager.SendSmsAsync(p.Id, "Please Reset your account password by clicking this link: \r\n\r\n"
                                                                               + smallURL);

            }
            else if (!string.IsNullOrWhiteSpace(p.Email))
            {
                await UserManager.SendEmailAsync(p.Id,
                    "Password Reset",
                    "Please Reset your account password by clicking this link: \r\n\r\n"
                                                                               + smallURL);
            }


            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult DeleteUser(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (TerritoryWebEntities db = new TerritoryWebEntities())
                {
                    AspNetUser p = db.AspNetUsers.SingleOrDefault(x => x.Id == id);
                    if (p != null)
                    {
                        //remove territories
                        var tersAssigned = db.Territories.Where(x => x.AssignedPublisherID == id).ToList();
                        var tersLastCheckedIn = db.Territories.Where(x => x.LastCheckedInBy == id).ToList();
                        tersAssigned.ForEach(u => u.AssignedPublisherID = null);
                        tersLastCheckedIn.ForEach(u => u.LastCheckedInBy = null);                        

                        db.AspNetUsers.Remove(p);
                        int result = db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("ManageUsers");

        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}