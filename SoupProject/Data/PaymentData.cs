using MySql.Data.MySqlClient;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class PaymentData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public PaymentData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Payment> GetPayments()
        {
            List<Payment> payments = new List<Payment>();

            string query = "SELECT * FROM payment";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                Payment payment = new Payment
                                {
                                    paymentId = dataReader.GetInt32("paymentId"),
                                    paymentName = dataReader["paymentName"].ToString() ?? string.Empty,
                                    paymentImg = dataReader["paymentImg"].ToString() ?? string.Empty,
                                    paymentStatus = dataReader["paymentStatus"].ToString() ?? string.Empty
                                };

                                payments.Add(payment);
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

            return payments;
        }

        public Payment GetPaymentById(int paymentId)
        {
            Payment paymentById = null;

            string query = "SELECT * FROM payment WHERE paymentId = @paymentId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@paymentId", paymentId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                Payment payment = new Payment
                                {
                                    paymentId = dataReader.GetInt32("paymentId"),
                                    paymentName = dataReader["paymentName"].ToString() ?? string.Empty,
                                    paymentImg = dataReader["paymentImg"].ToString() ?? string.Empty,
                                    paymentStatus = dataReader["paymentStatus"].ToString() ?? string.Empty
                                };

                                paymentById = payment;
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

            return paymentById;
        }

        public bool InsertNewPayment(Payment newPayment)
        {
            bool result = false;

            string query = "INSERT INTO payment (paymentName, paymentImg, paymentStatus) VALUES(@paymentName, @paymentImg, @paymentStatus)";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@paymentName", newPayment.paymentName);
                    command.Parameters.AddWithValue("@paymentImg", newPayment.paymentImg);
                    command.Parameters.AddWithValue("@paymentStatus", newPayment.paymentStatus);

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

        public bool UpdatePayment(int paymentId, Payment updatedPayment)
        {
            bool result = false;

            string query = "UPDATE payment SET paymentName = @paymentName, paymentImg = @paymentImg, paymentStatus = @paymentStatus WHERE paymentId = @paymentId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@paymentId", paymentId);
                    command.Parameters.AddWithValue("@paymentName", updatedPayment.paymentName);
                    command.Parameters.AddWithValue("@paymentImg", updatedPayment.paymentImg);
                    command.Parameters.AddWithValue("@paymentStatus", updatedPayment.paymentStatus);

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

        public bool DeletePaymentById(int paymentId)
        {
            bool result = false;

            string query = "DELETE FROM payment WHERE paymentId = @paymentId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@paymentId", paymentId);

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
