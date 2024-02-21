using MailKit.Search;
using MySql.Data.MySqlClient;
using SoupProject.DTOs;
using SoupProject.DTOs.Cart;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class CheckoutData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public CheckoutData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public bool InsertToCart(Cart cart)
        {
            bool result = false;

            string query = $"INSERT INTO cart (courseId, userId, courseDate, isPaid) " +
                $"VALUES (@courseId, @userId, @courseDate, FALSE)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@courseId", cart.courseId.ToString());
                    command.Parameters.AddWithValue("@userId", cart.userId);
                    command.Parameters.AddWithValue("@courseDate", cart.courseDate.ToString());

                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery() > 0 ? true : false;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }

        public bool Checkout(Order order, int[] selectedCourses)
        {
            bool addToOrder = false;
            bool addToOrderDetail = false;
            bool confirmPayment = false;
            bool totalPriceUpdated = false;

            string queryOrder = "INSERT INTO `order` (orderId, invoice, userId, paymentMethod) " +
                "VALUES (@orderId, @invoice, @userId, @paymentMethod)";
            string queryOrderDetail = "INSERT INTO orderdetail (invoice, cartId) " +
               "VALUES (@invoice, @cartId)";
            string queryConfirmPayment = "UPDATE cart SET isPaid = TRUE " +
                "WHERE userId = @userId AND cartId = @cartId";
            string queryOrderTotalPrice = "UPDATE `order` SET totalPrice = (SELECT SUM(c.coursePrice) FROM " +
                "orderdetail od JOIN cart ca ON od.cartId = ca.cartId JOIN course c ON ca.courseId = c.courseId " +
                "WHERE od.invoice = @invoice) WHERE invoice = @invoice";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    MySqlTransaction transaction = null;

                    try
                    {
                        connection.Open();

                        transaction = connection.BeginTransaction();
                        command.Connection = connection;
                        command.Transaction = transaction;

                        command.Parameters.Clear();
                        command.CommandText = queryOrder;
                        command.Parameters.AddWithValue("@orderId", order.orderId);
                        command.Parameters.AddWithValue("@invoice", order.invoice);
                        command.Parameters.AddWithValue("@userId", order.userId);
                        command.Parameters.AddWithValue("@paymentMethod", order.paymentMethod);

                        addToOrder = command.ExecuteNonQuery() > 0;

                        foreach (int selectedCourse in selectedCourses)
                        {
                            command.Parameters.Clear();
                            command.CommandText = queryOrderDetail;
                            command.Parameters.AddWithValue("@invoice", order.invoice);
                            command.Parameters.AddWithValue("@cartId", selectedCourse);
                            addToOrderDetail = command.ExecuteNonQuery() > 0;

                            command.Parameters.Clear();
                            command.CommandText = queryConfirmPayment;
                            command.Parameters.AddWithValue("@userId", order.userId);
                            command.Parameters.AddWithValue("@cartId", selectedCourse);
                            confirmPayment = command.ExecuteNonQuery() > 0;
                        }

                        command.Parameters.Clear();
                        command.CommandText = queryOrderTotalPrice;
                        command.Parameters.AddWithValue("@userId", order.userId);
                        command.Parameters.AddWithValue("@invoice", order.invoice);
                        totalPriceUpdated = command.ExecuteNonQuery() > 0;

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        if (transaction != null) transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (transaction != null) transaction.Dispose();
                        connection.Close();
                    }
                }
            }

            return addToOrder && addToOrderDetail && confirmPayment && totalPriceUpdated;
        }

        public bool CheckoutByCourse(Order order, Guid courseId)
        {
            bool result = false;

            string queryInsertToCart = "INSERT INTO cart (courseId, userId, courseDate, isPaid) VALUES (@courseId, @userId, @courseDate, FALSE)";
            string queryGetCartId = "SELECT cartId FROM cart WHERE courseId = @courseId AND userId = @userId AND isPaid = FALSE";
            string queryInsertOrder = "INSERT INTO `order` (orderId, invoice, userId, paymentMethod, courseDate) VALUES (@orderId, @invoice, @userId, @paymentMethod, @courseDate)";
            string queryInsertOrderDetail = "INSERT INTO orderdetail (invoice, cartId) VALUES (@invoice, @cartId)";
            string queryUpdateIsPaid = "UPDATE cart SET isPaid = TRUE WHERE cartId = @cartId";
            string queryUpdateOrderTotalPrice = "UPDATE `order` SET totalPrice = (SELECT SUM(c.coursePrice) FROM " +
                "orderdetail od JOIN cart ca ON od.cartId = ca.cartId JOIN course c ON ca.courseId = c.courseId " +
                "WHERE od.invoice = @invoice) WHERE invoice = @invoice";

            MySqlConnection connection = null;
            MySqlTransaction transaction = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();

                // Step 1: Insert to cart
                using (MySqlCommand command = new MySqlCommand(queryInsertToCart, connection, transaction))
                {
                    command.Parameters.AddWithValue("@courseId", courseId.ToString());
                    command.Parameters.AddWithValue("@userId", order.userId);
                    command.Parameters.AddWithValue("@courseDate", order.courseDate?.ToString() ?? "TEST");
                    command.ExecuteNonQuery();
                }

                // Step 2: Insert order
                using (MySqlCommand command = new MySqlCommand(queryInsertOrder, connection, transaction))
                {
                    command.Parameters.AddWithValue("@orderId", order.orderId);
                    command.Parameters.AddWithValue("@invoice", order.invoice);
                    command.Parameters.AddWithValue("@userId", order.userId);
                    command.Parameters.AddWithValue("@paymentMethod", order.paymentMethod);
                    command.Parameters.AddWithValue("@courseDate", order.courseDate?.ToString() ?? "TEST");
                    command.ExecuteNonQuery();
                }

                // Step 3: Checkout
                // Get cartId
                int cartId;
                using (MySqlCommand command = new MySqlCommand(queryGetCartId, connection, transaction))
                {
                    command.Parameters.AddWithValue("@courseId", courseId.ToString());
                    command.Parameters.AddWithValue("@userId", order.userId);
                    object cartIdObj = command.ExecuteScalar();
                    cartId = cartIdObj != null && cartIdObj != DBNull.Value ? Convert.ToInt32(cartIdObj) : -1;
                }

                if (cartId != -1)
                {
                    // Insert to order detail
                    using (MySqlCommand command = new MySqlCommand(queryInsertOrderDetail, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@invoice", order.invoice);
                        command.Parameters.AddWithValue("@cartId", cartId);
                        command.ExecuteNonQuery();
                    }

                    // Update isPaid in cart
                    using (MySqlCommand command = new MySqlCommand(queryUpdateIsPaid, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@cartId", cartId);
                        command.ExecuteNonQuery();
                    }

                    // Update order total price
                    using (MySqlCommand command = new MySqlCommand(queryUpdateOrderTotalPrice, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@invoice", order.invoice);
                        command.ExecuteNonQuery();
                    }

                    // Commit transaction
                    transaction.Commit();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                // Rollback transaction if any exception occurs
                try
                {
                    transaction?.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                }
            }
            finally
            {
                transaction?.Dispose();
                connection?.Close();
            }

            return result;
        }

        public List<MyCourseDTO> GetMyCourseById(Guid userId)
        {
            List<MyCourseDTO> myCourses = new List<MyCourseDTO>();

            string query = @"
                SELECT ca.courseDate, c.categoryName, cr.courseName, cr.courseImg, ca.isPaid
                FROM cart ca
                JOIN course cr ON ca.courseId = cr.courseId
                JOIN category c ON cr.categoryId = c.categoryId
                JOIN user u ON u.userId = ca.userId
                WHERE ca.userId = @userId
            ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    command.Connection = connection;
                    command.CommandText = query;

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MyCourseDTO myCourse = new MyCourseDTO
                                {
                                    categoryName = reader["categoryName"].ToString(),
                                    courseName = reader["courseName"].ToString(),
                                    courseDate = reader["courseDate"].ToString(),
                                    courseImg = reader["courseImg"].ToString(),
                                    isPaid = Convert.ToBoolean(reader["isPaid"].ToString())

                                };
                                myCourses.Add(myCourse);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return myCourses;
        }

        public List<UserCartDTO> GetCartById(Guid userId)
        {
            List<UserCartDTO> myCarts = new List<UserCartDTO>();

            string query = @"
                  SELECT ca.cartId, ca.courseDate, c.categoryName, cr.courseName, cr.coursePrice, ca.isPaid, cr.courseImg
                  FROM cart ca
                  JOIN course cr ON ca.courseId = cr.courseId
                  JOIN category c ON cr.categoryId = c.categoryId
                  JOIN user u ON u.userId = ca.userId
                  WHERE ca.userId = @userId ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    command.Connection = connection;
                    command.CommandText = query;

                    try
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserCartDTO myCourse = new UserCartDTO
                                {
                                    cartId = Convert.ToInt32(reader["cartId"]),
                                    categoryName = reader["categoryName"].ToString(),
                                    courseName = reader["courseName"].ToString(),
                                    courseDate = reader["courseDate"].ToString(),
                                    coursePrice = Convert.ToDecimal(reader["coursePrice"]),
                                    isPaid = Convert.ToBoolean(reader["isPaid"]),
                                    courseImg = reader["courseImg"].ToString()
                                };
                                myCarts.Add(myCourse);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return myCarts;
        }

        public bool DeleteCartByIdCart(int cartId)
        {
            bool result = false;

            string query = "DELETE FROM cart WHERE cartId = @cartId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@cartId", cartId);

                    command.Connection = connection;
                    command.CommandText = query;

                    try
                    {
                        connection.Open();
                        result = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return result;
        }


    }
}
