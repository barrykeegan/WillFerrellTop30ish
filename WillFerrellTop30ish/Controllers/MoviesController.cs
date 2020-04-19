using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using WillFerrellTop30ish.Models;
using System.Xml;

namespace WillFerrellTop30ish.Controllers
{
    public class MoviesController : Controller
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

        // GET: Movies
        public ActionResult Index()
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
                        }
                    }
                    else
                    {
                        Session.Clear();
                    }
                }
            }
            List<Movie> list = dao.ShowAllMovies();
            return View(list);
        }


        [HttpGet]
        public ActionResult Add()
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
                        }
                    }
                    else
                    {
                        Session.Clear();
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add(Movie movie)
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
                            return View();
                        }
                    }
                    else
                    {
                        Session.Clear();
                        return View();
                    }
                }
            }
            int counter = 0;
            counter = dao.InsertMovie(movie);
            if (counter == 1)
            {
                ViewBag.Message = "Record inserted successfully";
                ModelState.Clear();
            }
            else
            {
                ViewBag.Message = "Error, " + dao.message;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Delete(string ID)
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
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        Session.Clear();
                        return RedirectToAction("Index");
                    }
                }
                //Guard against deletion request by customer
                else
                {
                    return RedirectToAction("Index");
                }
            }
            //do not delete if no session exists, return to index
            else
            {
                return RedirectToAction("Index");
            }
            dao.DeleteMovie(ID);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ShowOne(string id)
        {
            Movie movie = dao.ShowOneMovie(id);
            return View(movie);
        }

        [HttpGet]
        public ActionResult Edit(string id)
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
                        }
                    }
                    else
                    {
                        Session.Clear();
                    }
                }
            }
            Movie movie = dao.ShowOneMovie(id);
            return View(movie);
        }

        [HttpPost]
        public ActionResult Edit(Movie movie)
        {
            if (ModelState.IsValid)
            {
                dao.UpdateMovie(movie);
                return RedirectToAction("Index");
            }

            return View(movie);
        }

    }
}