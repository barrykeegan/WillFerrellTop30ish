using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WillFerrellTop30ish.Models;

namespace WillFerrellTop30ish.Controllers
{
    public class OrderController : Controller
    {
        DAO dao = new DAO();

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }


        // GET: Cart
        [HttpGet]
        public ActionResult Cart()
        {
            List<Movie> list = dao.ShowAllMovies();
            return View(list);
        }

        [HttpPost]
        public ActionResult Cart(FormCollection orderItems)
        {
            Response.Cookies["cart"].Expires = DateTime.Now.AddDays(-1);
            string[] itemsFromCart = orderItems["hdnItems"].Split(':');
            List<Movie> movieList = dao.ShowAllMovies();
            Order newOrder = new Order();
            List<OrderItem> itemsInOrder = new List<OrderItem>();
            foreach (string item in itemsFromCart)
            {
                string[] titleQty = item.Split('_');
                foreach (Movie m in movieList)
                {
                    if (m.ID == titleQty[0])
                    {
                        OrderItem newOI = new OrderItem();
                        newOI.Price = m.Price;
                        newOI.Quantity = int.Parse(titleQty[1]);
                        newOI.ItemOrdered = m;
                        itemsInOrder.Add(newOI);
                        break;
                    }
                }
            }
            newOrder.OrderItems = itemsInOrder;
            newOrder.CustomerEmail = Session["email"].ToString();
            dao.InsertOrder(newOrder.CustomerEmail);
            newOrder.OrderID = dao.GetCurrentOrderID();

            foreach (OrderItem item in newOrder.OrderItems)
            {
                dao.InsertOrderItem(newOrder.OrderID, item);
            }

            return RedirectToAction("MostRecentOrderSummary", "Order", newOrder);
        }

        [HttpGet]
        public ActionResult MostRecentOrderSummary()
        {
            Order summarisedOrder = new Order();
            int orderID = dao.GetMostRecentOrderIDForCustomer(Session["email"].ToString());
            summarisedOrder = dao.GetOrder(orderID);
            foreach (OrderItem oi in summarisedOrder.OrderItems)
            {
                oi.ItemOrdered = dao.ShowOneMovie(oi.ItemOrdered.ID);
            }

            return View(summarisedOrder);
        }

        public ActionResult CustomerOrders()
        {
            if (Session != null && Session["userType"] != null && Session["userType"].ToString() == "customer")
            {
                List<Order> list = dao.GetCustomerOrders(Session["email"].ToString());
                return View(list);
            }
            return View();
        }

        public ActionResult CustomerOrder(int id)
        {
            Order summarisedOrder = new Order();
            int orderID = id;
            summarisedOrder = dao.GetOrder(orderID);
            foreach (OrderItem oi in summarisedOrder.OrderItems)
            {
                oi.ItemOrdered = dao.ShowOneMovie(oi.ItemOrdered.ID);
            }

            return View(summarisedOrder);
        }

        public ActionResult AdministratorOrders()
        {
            if (Session != null && Session["userType"] != null && Session["userType"].ToString() == "staff")
            {
                List<Order> list = dao.GetAllOrders();
                return View(list);
            }
            return View();
        }

        public ActionResult AdministratorOrder(int id)
        {
            Order summarisedOrder = new Order();
            int orderID = id;
            summarisedOrder = dao.GetOrder(orderID);
            foreach (OrderItem oi in summarisedOrder.OrderItems)
            {
                oi.ItemOrdered = dao.ShowOneMovie(oi.ItemOrdered.ID);
            }

            return View(summarisedOrder);
        }
    }
}