using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using WillFerrellTop30ish.Models;

namespace WillFerrellTop30ish.Controllers
{
    public class StaffController : Controller
    {
        DAO dao = new DAO();

        //IsStaffActive is made available in controllers that have access
        //to adminstrative Methods, this is to ensure that no deactivated
        //staff members, who were deactivated after a successful login and 
        //still have an active session. It will be used to contole the 
        //closure of their session in such a case.
        private bool IsStaffActive(string email)
        {
            Staff staff = dao.SelectOneStaff(email);
            return staff.IsActive;
        }

        //Method to ensure that Session state is valid (userType is staff with an email set).
        //Also checks IsActive state from the db to ensure that staff member was not
        //deactivated. If Session state is invalid or the currently logged in staff member
        //has since been deactivated, the session will be cleared. Thus ensuring only properly
        //logged in staff members will be able to access certain pages.
        private bool IsStaffSessionValid()
        {
            if (Session != null)
            {
                if (Session["userType"] != null)
                {
                    if (Session["userType"].ToString() == "staff")
                    {
                        if (Session["email"].ToString() != null)
                        {
                            if (!IsStaffActive(Session["email"].ToString()))
                            {
                                Session.Clear();
                                return false;
                            }
                        }
                        else
                        {
                            Session.Clear();
                            return false;
                        }
                    }
                }
                else
                {
                    Session.Clear();
                    return false;
                }
            }
            else //If Session == null, then StaffSession is not valid
            {
                return false;
            }
            return true;
        }

        // GET: Staff
        public ActionResult Index()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult Login(Staff staff)
        {
            ModelState.Remove("IsActive");
            if (ModelState.IsValid)
            {
                Staff staffRecord = dao.SelectOneStaff(staff.Email);
                if (staffRecord != null)
                {
                    if (staff.Email == staffRecord.Email)
                    {
                        //This will only happen on first time login at which time we want to hash the password
                        if (staff.Password == staffRecord.Password)
                        {
                            staffRecord.Password = Crypto.HashPassword(staff.Password);
                            dao.UpdateStaff(staffRecord);
                        }

                        if (Crypto.VerifyHashedPassword(staffRecord.Password, staff.Password))
                        {
                            if (staffRecord.IsActive)
                            {
                                Session.Clear();
                                Session["userType"] = "staff";
                                Session["email"] = staff.Email;
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewBag.Message = "This account was deactivated, please contact an admin.";
                            }
                        }
                        else
                        {
                            ViewBag.Message = "The Email and Password Combination entered do not match any Staff records.";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "The Email and Password Combination entered do not match any Staff records.";
                    }
                }
                else
                {
                    ViewBag.Message = "The Email and Password Combination entered do not match any Staff records.";
                }
            }
            else
            {
                ViewBag.Message = "You must enter valid data in the form.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (Session["userType"] != null && Session["userType"].ToString() == "staff")
            {
                Session.Clear();
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult List()
        {
            List<Staff> staffList = null;
            if (IsStaffSessionValid())
            {
                staffList = dao.SelectAllStaff();
                return View(staffList);
            }
            else
            {
                return View(staffList);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            Staff staffRecord = dao.SelectOneStaff(Session["email"].ToString());

            if (Crypto.VerifyHashedPassword(staffRecord.Password, form["txtCurrPassword"]))
            {
                staffRecord.Password = Crypto.HashPassword(form["txtNewPassword1"]);
                dao.UpdateStaff(staffRecord);
                ViewBag.Message = "Password Changed Successfully";
                return View();
            }
            else
            {
                ViewBag.Message = "The password you entered for Current Password does not match the stored password";
                return View();
            }
        }

        [HttpGet]
        public ActionResult AddStaff()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult AddStaff(Staff staff)
        {
            staff.IsActive = true;
            staff.Password = Crypto.HashPassword(staff.Password);
            if (dao.InsertStaff(staff) == 1)
            {
                ViewBag.Message = "Staff member successfully added.";
                return View();
            }
            else
            {
                ViewBag.Message = "Something went wrong... Staff member not added.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult DeleteStaff()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult DeleteStaff(Staff staff)
        {
            IsStaffSessionValid();
            dao.DeleteStaffMember(staff.Email);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult DeactivateStaff()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult DeactivateStaff(Staff staff)
        {
            IsStaffSessionValid();
            staff.IsActive = false;
            dao.UpdateStaff(staff);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult ActivateStaff()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult ActivateStaff(Staff staff)
        {
            IsStaffSessionValid();
            staff.IsActive = true;
            dao.UpdateStaff(staff);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult ResetPass()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult ResetPass(Staff staff)
        {
            IsStaffSessionValid();
            staff.Password = Crypto.HashPassword("admin");
            dao.UpdateStaff(staff);
            return RedirectToAction("List");
        }
    }
}