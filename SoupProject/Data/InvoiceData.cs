using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SoupProject.DTOs.Invoice;
using SoupProject.Models;
using System;

namespace SoupProject.Data
{
    public class InvoiceData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public InvoiceData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<InvoiceDTO> GetAllInvoice()
        {
            List<InvoiceDTO> invoiceDTO = new List<InvoiceDTO>();
            string query = $"SELECT o.userId, u.username, o.invoice, o.transactionDate, COUNT(od.orderDetailId) as totalCourse, SUM(c.coursePrice) as totalPrice FROM `order` o " +
               $"JOIN orderdetail od ON o.invoice = od.invoice " +
               $"JOIN user u ON o.userId = u.userId " +
               $"JOIN cart ca ON od.cartId = ca.cartId " +
               $"JOIN course c ON ca.courseId = c.courseId " +
               $"GROUP BY o.invoice";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceDTO invoice = new InvoiceDTO
                                {
                                    userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                    username = Convert.ToString(reader["username"]),
                                    invoice = Convert.ToString(reader["invoice"]),
                                    transactionDate = reader["transactionDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["transactionDate"]),
                                    totalCourse = reader["totalCourse"] == DBNull.Value ? 0 : Convert.ToInt32(reader["totalCourse"]),
                                    totalPrice = reader["totalPrice"] == DBNull.Value ? 0 : Convert.ToInt32(reader["totalPrice"])
                                };
                                invoiceDTO.Add(invoice);
                            }
                        }
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoiceDTO;
        }

        public List<InvoiceDTO> GetInvoiceByUser(Guid userId)
        {
            List<InvoiceDTO> invoiceDTO = new List<InvoiceDTO>();
            string query = $"SELECT o.userId, u.username, o.invoice, o.transactionDate, COUNT(od.orderDetailId) as totalCourse, SUM(c.coursePrice) as totalPrice FROM `order` o " +
               $"JOIN orderdetail od ON o.invoice = od.invoice " +
               $"JOIN user u ON o.userId = u.userId " +
               $"JOIN cart ca ON od.cartId = ca.cartId " +
               $"JOIN course c ON ca.courseId = c.courseId " +
               $"WHERE o.userId = @userId " +
               $"GROUP BY o.invoice";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceDTO invoice = new InvoiceDTO
                                {
                                    userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                    username = Convert.ToString(reader["username"]),
                                    invoice = Convert.ToString(reader["invoice"]),
                                    transactionDate = reader["transactionDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["transactionDate"]),
                                    totalCourse = reader["totalCourse"] == DBNull.Value ? 0 : Convert.ToInt32(reader["totalCourse"]),
                                    totalPrice = reader["totalPrice"] == DBNull.Value ? 0 : Convert.ToInt32(reader["totalPrice"])
                                };
                                invoiceDTO.Add(invoice);
                            }
                        }
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoiceDTO;
        }

        public List<InvoiceDetailDTO> GetAllInvoiceDetail()
        {
            List<InvoiceDetailDTO> invoiceDetailDTO = new List<InvoiceDetailDTO>();
            string query = $"SELECT o.userId, u.username, c.courseName, cc.categoryName, o.transactionDate, ca.courseDate, c.coursePrice FROM `order` o " +
                $"JOIN orderDetail od ON o.invoice = od.invoice " +
                $"JOIN user u ON o.userId = u.userId " +
                $"JOIN cart ca ON od.cartId = ca.cartId " +
                $"JOIN course c ON ca.courseId = c.courseId " +
                $"JOIN category cc ON c.categoryId = cc.categoryId ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceDetailDTO invoiceDetail = new InvoiceDetailDTO
                                {
                                    userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                    username = Convert.ToString(reader["username"]),
                                    courseName = Convert.ToString(reader["courseName"]),
                                    category = Convert.ToString(reader["categoryName"]),
                                    transactionDate = Convert.ToDateTime(reader["transactionDate"]),
                                    schedule = reader["courseDate"].ToString() ?? string.Empty,
                                    coursePrice = Convert.ToInt32(reader["coursePrice"])
                                };
                                invoiceDetailDTO.Add(invoiceDetail);
                            }
                        }
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoiceDetailDTO;
        }

        public List<InvoiceDetailDTO> GetInvoiceDetailByInvoice(string invoice)
        {
            List<InvoiceDetailDTO> invoiceDetailDTO = new List<InvoiceDetailDTO>();
            string query = $"SELECT o.userId, u.username, c.courseName, cc.categoryName, o.transactionDate, ca.courseDate, c.coursePrice FROM `order` o " +
                $"JOIN orderDetail od ON o.invoice = od.invoice " +
                $"JOIN user u ON o.userId = u.userId " +
                $"JOIN cart ca ON od.cartId = ca.cartId " +
                $"JOIN course c ON ca.courseId = c.courseId " +
                $"JOIN category cc ON c.categoryId = cc.categoryId " +
                $"WHERE o.invoice = @invoice";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoice", invoice);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceDetailDTO invoiceDetail = new InvoiceDetailDTO
                                {
                                    userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                    username = Convert.ToString(reader["username"]),
                                    courseName = Convert.ToString(reader["courseName"]),
                                    category = Convert.ToString(reader["categoryName"]),
                                    transactionDate = reader["transactionDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["transactionDate"]),
                                    schedule = reader["courseDate"].ToString() ?? string.Empty,
                                    coursePrice = Convert.ToInt32(reader["coursePrice"])
                                };
                                invoiceDetailDTO.Add(invoiceDetail);
                            }
                        }
                    }
                    catch (MySqlException)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoiceDetailDTO;
        }
    }
}
