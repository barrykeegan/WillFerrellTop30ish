using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using System.IO;
using System.Xml;
using System.Web.Helpers;
using WillFerrellTop30ish.Models;

namespace WillFerrellTop30ish.Models
{
    public class DAO
    {
        SqlConnection con;
        public string message = "";

        public DAO()
        {
            con = new SqlConnection(WebConfigurationManager.ConnectionStrings["conString"].ConnectionString);
        }

        public int InsertMovie(Movie movie)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspInsert_tbl_Movie", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TitleId", movie.ID);
            cmd.Parameters.AddWithValue("@Name", movie.Name);
            cmd.Parameters.AddWithValue("@XmlData", movie.XmlData);
            cmd.Parameters.AddWithValue("@Stock", movie.Stock);
            cmd.Parameters.AddWithValue("@Price", movie.Price);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return count;
        }

        public int UpdateMovie(Movie movie)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspEditMovie", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TitleId", movie.ID);
            cmd.Parameters.AddWithValue("@Name", movie.Name);
            cmd.Parameters.AddWithValue("@XmlData", movie.XmlData);
            cmd.Parameters.AddWithValue("@Stock", movie.Stock);
            cmd.Parameters.AddWithValue("@Price", movie.Price);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return count;
        }

        public List<Movie> ShowAllMovies()
        {
            SqlDataReader reader;
            List<Movie> list = new List<Movie>();

            SqlCommand cmd = new SqlCommand("uspShowAllMovies", con);
            cmd.CommandType = CommandType.StoredProcedure;


            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Movie movie = new Movie();
                    movie.ID = reader["MovieTitleId"].ToString();
                    movie.Name = reader["MovieName"].ToString();
                    movie.Price = decimal.Parse(reader["Price"].ToString());
                    movie.Stock = int.Parse(reader["Stock"].ToString());
                    movie.XmlData = reader["MovieData"].ToString();
                    list.Add(movie);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public Movie ShowOneMovie(string id)
        {
            SqlDataReader reader;
            Movie movie = null;

            SqlCommand cmd = new SqlCommand("uspShowOneMovie", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TitleId", id);


            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movie = new Movie();
                    movie.ID = reader["MovieTitleId"].ToString();
                    movie.Name = reader["MovieName"].ToString();
                    movie.Price = decimal.Parse(reader["Price"].ToString());
                    movie.Stock = int.Parse(reader["Stock"].ToString());
                    movie.XmlData = reader["MovieData"].ToString();
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return movie;
        }

        public void DeleteMovie(string ID)
        {
            SqlCommand cmd = new SqlCommand("uspDeleteMovie", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter paramId = new SqlParameter();
            paramId.ParameterName = "@MovieTitleId";
            paramId.Value = ID;
            cmd.Parameters.Add(paramId);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public int InsertCustomer(Customer customer)
        {
            int count = 0;
            string password;
            SqlCommand cmd = new SqlCommand("uspInsert_tbl_Customer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@custName", customer.Name);
            cmd.Parameters.AddWithValue("@custEmail", customer.Email);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
            password = Crypto.HashPassword(customer.Password);
            cmd.Parameters.AddWithValue("@pass", password);
            cmd.Parameters.AddWithValue("@addressLine1", customer.AddressLine1);
            cmd.Parameters.AddWithValue("@addressLine2", customer.AddressLine2);
            cmd.Parameters.AddWithValue("@addressLine3", customer.AddressLine3);
            cmd.Parameters.AddWithValue("@country", customer.Country);
            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return count;
        }

        public int UpdateCustomer(Customer customer)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspUpdateCustomer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@custName", customer.Name);
            cmd.Parameters.AddWithValue("@custEmail", customer.Email);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
            //password is hashed before this method is called
            cmd.Parameters.AddWithValue("@pass", customer.Password);
            cmd.Parameters.AddWithValue("@addressLine1", customer.AddressLine1);
            cmd.Parameters.AddWithValue("@addressLine2", customer.AddressLine2);
            cmd.Parameters.AddWithValue("@addressLine3", customer.AddressLine3);
            cmd.Parameters.AddWithValue("@country", customer.Country);
            cmd.Parameters.AddWithValue("@active", customer.IsActive);
            cmd.Parameters.AddWithValue("@DateRegistered", customer.DateRegistered);
            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return count;
        }



        public List<Customer> SelectAllCustomers()
        {
            List<Customer> custList = null;
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspShowAllCustomers", con);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    custList = new List<Customer>();
                }

                while (reader.Read())
                {
                    Customer customer = new Customer();
                    customer.Email = reader["CustomerEmail"].ToString();
                    customer.Name = reader["CustomerName"].ToString();
                    customer.Password = reader["Pass"].ToString();
                    customer.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    customer.Phone = reader["Phone"].ToString();
                    customer.DateRegistered = (DateTime)reader["DateRegistered"];
                    customer.AddressLine1 = reader["AddressLine1"].ToString();
                    customer.AddressLine2 = reader["AddressLine2"].ToString();
                    customer.AddressLine3 = reader["AddressLine3"].ToString();
                    customer.Country = reader["Country"].ToString();
                    custList.Add(customer);
                }

            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            catch (FormatException ex1)
            {
                message = ex1.Message;
            }
            finally
            {
                con.Close();
            }

            return custList;
        }

        public Customer ShowOneCustomer(string email)
        {
            Customer customer = null;
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspShowOneCustomer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@custEmail", email);

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customer = new Customer();
                    customer.Email = reader["CustomerEmail"].ToString();
                    customer.Name = reader["CustomerName"].ToString();
                    customer.Password = reader["Pass"].ToString();
                    customer.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    customer.Phone = reader["Phone"].ToString();
                    customer.DateRegistered = (DateTime)reader["DateRegistered"];
                    customer.AddressLine1 = reader["AddressLine1"].ToString();
                    customer.AddressLine2 = reader["AddressLine2"].ToString();
                    customer.AddressLine3 = reader["AddressLine3"].ToString();
                    customer.Country = reader["Country"].ToString();
                }

            }
            catch (SqlException ex)
            {
                message = ex.Message;
            }
            catch (FormatException ex1)
            {
                message = ex1.Message;
            }
            finally
            {
                con.Close();
            }

            return customer;
        }

        public int DeleteCustomer(string email)
        {
            int counter = 0;
            SqlCommand cmd = new SqlCommand("uspDeleteCustomer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerEmail", email);

            try
            {
                con.Open();
                counter = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return counter;
        }

        public int InsertStaff(Staff staff)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspInsert_tbl_Staff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", staff.Email);
            cmd.Parameters.AddWithValue("@pass", staff.Password);
            cmd.Parameters.AddWithValue("@active", staff.IsActive);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }

            finally
            {
                con.Close();
            }
            return count;
        }

        public int UpdateStaff(Staff staff)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspUpdateStaff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", staff.Email);
            cmd.Parameters.AddWithValue("@pass", staff.Password);
            cmd.Parameters.AddWithValue("@active", staff.IsActive);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return count;
        }

        public Staff SelectOneStaff(string email)
        {
            SqlDataReader reader;
            Staff staff = null;
            SqlCommand cmd = new SqlCommand("uspSelectOneStaff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    staff = new Staff();
                    staff.Email = reader["StaffEmail"].ToString();
                    staff.Password = reader["Pass"].ToString();
                    staff.IsActive = Convert.ToBoolean(reader["IsActive"]);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return staff;
        }


        public List<Staff> SelectAllStaff()
        {
            SqlDataReader reader;
            List<Staff> list = new List<Staff>();

            SqlCommand cmd = new SqlCommand("uspSelectAllStaff", con);
            cmd.CommandType = CommandType.StoredProcedure;


            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Staff staff = new Staff();
                    staff.Email = reader["StaffEmail"].ToString();
                    staff.Password = reader["Pass"].ToString();
                    staff.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    list.Add(staff);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public int DeleteStaffMember(string email)
        {
            int counter = 0;
            SqlCommand cmd = new SqlCommand("uspDeleteOneStaff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                counter = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return counter;
        }

        public int InsertOrder(string email)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspInsertOrder", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }

            finally
            {
                con.Close();
            }
            return count;
        }

        public int GetCurrentOrderID()
        {
            SqlDataReader reader;
            int orderID = -1;
            SqlCommand cmd = new SqlCommand("uspCurrentOrder", con);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    orderID = int.Parse(reader["ID"].ToString());
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return orderID;
        }

        public int InsertOrderItem(int orderID, OrderItem orderItem)
        {
            int count = 0;
            SqlCommand cmd = new SqlCommand("uspInsertOrderItem", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@orderID", orderID);
            cmd.Parameters.AddWithValue("@movieID", orderItem.ItemOrdered.ID);
            cmd.Parameters.AddWithValue("@quantity", orderItem.Quantity);
            cmd.Parameters.AddWithValue("@price", orderItem.Price);

            try
            {
                con.Open();
                count = cmd.ExecuteNonQuery();
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }

            finally
            {
                con.Close();
            }
            return count;
        }

        public int GetMostRecentOrderIDForCustomer(string email)
        {
            int orderID = -1;
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspGetMostRecentOrderIDForCustomer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    orderID = int.Parse(reader["OrderID"].ToString());
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }

            return orderID;
        }

        public Order GetOrder(int orderID)
        {
            Order retrievedOrder = new Order();
            retrievedOrder.OrderID = orderID;

            //Get Order Record for OrderID
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspSelectOneOrder", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@orderId", orderID);

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    retrievedOrder.CustomerEmail = reader["CustomerEmail"].ToString();
                    retrievedOrder.OrderDate = (DateTime)reader["DateOfOrder"];
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }

            //Get OrderItem Records for OrderID
            cmd = new SqlCommand("uspGetOrderItems", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@orderId", orderID);
            retrievedOrder.OrderItems = new List<OrderItem>();

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OrderItem oi = new OrderItem();
                    oi.ItemOrdered = new Movie();
                    oi.Price = decimal.Parse(reader["Price"].ToString());
                    oi.Quantity = int.Parse(reader["Quantity"].ToString());
                    oi.ItemOrdered.ID = reader["MovieTitleId"].ToString();
                    retrievedOrder.OrderItems.Add(oi);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }

            return retrievedOrder;
        }

        public List<Order> GetCustomerOrders(string email)
        {
            List<Order> list = null;

            //Get Order Record for OrderID
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspGetCustomerOrders", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    list = new List<Order>();
                }
                while (reader.Read())
                {
                    Order ord = new Order();
                    ord.CustomerEmail = email;
                    ord.OrderID = int.Parse(reader["OrderId"].ToString());
                    ord.OrderDate = (DateTime)reader["DateOfOrder"];
                    list.Add(ord);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public List<Order> GetAllOrders()
        {
            List<Order> list = null;

            //Get Order Record for OrderID
            SqlDataReader reader;
            SqlCommand cmd = new SqlCommand("uspGetAllOrders", con);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    list = new List<Order>();
                }
                while (reader.Read())
                {
                    Order ord = new Order();
                    ord.CustomerEmail = reader["CustomerEmail"].ToString();
                    ord.OrderID = int.Parse(reader["OrderId"].ToString());
                    ord.OrderDate = (DateTime)reader["DateOfOrder"];
                    list.Add(ord);
                }
            }
            catch (SystemException ex)
            {
                message = ex.Message;
            }
            finally
            {
                con.Close();
            }
            return list;
        }
    }
}