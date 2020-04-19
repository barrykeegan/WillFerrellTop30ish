using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using WillFerrellTop30ish.Models;

namespace WillFerrellTop30ish.Controllers
{
    public class CustomerController : Controller
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
            }
            else //If Session == null, then StaffSession is not valid
            {
                return false;
            }
            return true;
        }

        // GET: Customer
        public ActionResult Index()
        {
            if (Session != null && Session["userType"] != null && Session["userType"].ToString() == "customer")
            {
                Customer customer = dao.ShowOneCustomer(Session["email"].ToString());
                return View(customer);
            }
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                count = dao.InsertCustomer(customer);
                if (count == 1)
                {
                    Session.Clear();
                    Session["userType"] = "customer";
                    Session["email"] = customer.Email;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Status = "Error! " + dao.message;
                }
            }
            else
            {
                ViewBag.Status = "You have not entered all required fields, please fix and resubmit.";
            }
            return View(customer);
        }

        [HttpPost]
        public ActionResult SignIn(Customer customer)
        {
            ModelState.Remove("Name");
            ModelState.Remove("AddressLine1");
            ModelState.Remove("Country");
            if (ModelState.IsValid)
            {
                Customer customerRecord = dao.ShowOneCustomer(customer.Email);
                if (customerRecord != null && Crypto.VerifyHashedPassword(customerRecord.Password, customer.Password))
                {
                    Session.Clear();
                    Session["userType"] = "customer";
                    Session["email"] = customer.Email;
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ViewBag.Status = "Error " + dao.message;
                    return View();
                }
            }
            else
            {
                return View(customer);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (Session != null && Session["userType"] != null && Session["userType"].ToString() == "customer")
            {
                Session.Clear();
            }
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            Customer customerRecord = dao.ShowOneCustomer(Session["email"].ToString());

            if (Crypto.VerifyHashedPassword(customerRecord.Password, form["txtCurrPassword"]))
            {
                customerRecord.Password = Crypto.HashPassword(form["txtNewPassword1"]);
                dao.UpdateCustomer(customerRecord);
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
        public ActionResult CustomerAdministration()
        {
            List<Customer> custList = null;
            if (IsStaffSessionValid())
            {
                custList = dao.SelectAllCustomers();
                return View(custList);
            }
            else
            {
                return View(custList);
            }
        }

        [HttpGet]
        public ActionResult DeleteCustomer()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult DeleteCustomer(Customer customer)
        {
            IsStaffSessionValid();
            dao.DeleteCustomer(customer.Email);
            return RedirectToAction("CustomerAdministration");
        }

        [HttpGet]
        public ActionResult DeactivateCustomer()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult DeactivateCustomer(Customer customer)
        {
            IsStaffSessionValid();
            customer.IsActive = false;
            dao.UpdateCustomer(customer);
            return RedirectToAction("CustomerAdministration");
        }

        [HttpGet]
        public ActionResult ActivateCustomer()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult ActivateCustomer(Customer customer)
        {
            IsStaffSessionValid();
            customer.IsActive = true;
            dao.UpdateCustomer(customer);
            return RedirectToAction("CustomerAdministration");
        }

        [HttpGet]
        public ActionResult ResetPass()
        {
            IsStaffSessionValid();
            return View();
        }

        [HttpPost]
        public ActionResult ResetPass(Customer customer)
        {
            IsStaffSessionValid();
            customer.Password = Crypto.HashPassword("monkey");
            dao.UpdateCustomer(customer);
            return RedirectToAction("CustomerAdministration");
        }
    }
}