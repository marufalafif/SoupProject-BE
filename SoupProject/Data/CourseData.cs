using MySql.Data.MySqlClient;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class CourseData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public CourseData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();

            string query = "SELECT course.*, category.categoryName " +
                           "FROM course " +
                           "INNER JOIN category ON course.categoryId = category.categoryId";

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
                                Course course = new Course
                                {
                                    courseId = Guid.Parse(dataReader["courseId"].ToString() ?? string.Empty),
                                    courseName = dataReader["courseName"].ToString() ?? string.Empty,
                                    categoryId = Convert.ToInt32(dataReader["categoryId"]),
                                    courseImg = dataReader["courseImg"].ToString() ?? string.Empty,
                                    courseDesc = dataReader["courseDesc"].ToString() ?? string.Empty,
                                    coursePrice = Convert.ToInt32(dataReader["coursePrice"]),
                                    courseStatus = dataReader["courseStatus"].ToString() ?? string.Empty,
                                    categoryName = dataReader["categoryName"].ToString() ?? string.Empty
                                };

                                courses.Add(course);
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

            return courses;
        }

        public Course GetCourseById(Guid courseId)
        {
            Course courseById = null;

            string query = "SELECT course.*, category.categoryName " +
                           "FROM course " +
                           "INNER JOIN category ON course.categoryId = category.categoryId " +
                           "WHERE course.courseId = @courseId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@courseId", courseId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                Course course = new Course
                                {
                                    courseId = Guid.Parse(dataReader["courseId"].ToString() ?? string.Empty),
                                    courseName = dataReader["courseName"].ToString() ?? string.Empty,
                                    categoryId = Convert.ToInt32(dataReader["categoryId"]),
                                    courseImg = dataReader["courseImg"].ToString() ?? string.Empty,
                                    courseDesc = dataReader["courseDesc"].ToString() ?? string.Empty,
                                    coursePrice = Convert.ToInt32(dataReader["coursePrice"]),
                                    courseStatus = dataReader["courseStatus"].ToString() ?? string.Empty,
                                    categoryName = dataReader["categoryName"].ToString() ?? string.Empty
                                };

                                courseById = course;
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

            return courseById;
        }

        public int GetCartCount(Guid courseId)
        {
            int cartCount = 0;

            string query = "SELECT COUNT(*) FROM cart WHERE courseId = @courseId AND isPaid = 0";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@courseId", courseId);

                    try
                    {
                        connection.Open();
                        cartCount = Convert.ToInt32(command.ExecuteScalar());
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

            return cartCount;
        }


        public List<Course> GetCoursesByCategoryId(int categoryId)
        {
            List<Course> courses = new List<Course>();

            string query = "SELECT course.*, category.categoryName " +
                           "FROM course " +
                           "INNER JOIN category ON course.categoryId = category.categoryId " +
                           "WHERE course.categoryId = @categoryId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@categoryId", categoryId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                Course course = new Course
                                {
                                    courseId = Guid.Parse(dataReader["courseId"].ToString() ?? string.Empty),
                                    courseName = dataReader["courseName"].ToString() ?? string.Empty,
                                    categoryId = Convert.ToInt32(dataReader["categoryId"]),
                                    courseImg = dataReader["courseImg"].ToString() ?? string.Empty,
                                    courseDesc = dataReader["courseDesc"].ToString() ?? string.Empty,
                                    coursePrice = Convert.ToInt32(dataReader["coursePrice"]),
                                    courseStatus = dataReader["courseStatus"].ToString() ?? string.Empty,
                                    categoryName = dataReader["categoryName"].ToString() ?? string.Empty
                                };

                                courses.Add(course);
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

            return courses;
        }

        public bool InsertNewCourse(Course newCourse)
        {
            bool result = false;

            string query = "INSERT INTO course (courseId, courseName, categoryId, courseImg, courseDesc, coursePrice, courseStatus) " +
                           "VALUES(@courseId, @courseName, @categoryId, @courseImg, @courseDesc, @coursePrice, @courseStatus)";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@courseId", newCourse.courseId);
                    command.Parameters.AddWithValue("@courseName", newCourse.courseName);
                    command.Parameters.AddWithValue("@categoryId", newCourse.categoryId);
                    command.Parameters.AddWithValue("@courseImg", newCourse.courseImg);
                    command.Parameters.AddWithValue("@courseDesc", newCourse.courseDesc);
                    command.Parameters.AddWithValue("@coursePrice", newCourse.coursePrice);
                    command.Parameters.AddWithValue("@courseStatus", newCourse.courseStatus);

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

        public bool UpdateCourse(Guid courseId, Course updatedCourse)
        {
            bool result = false;

            string query = "UPDATE course SET courseName = @courseName, categoryId = @categoryId, courseImg = @courseImg, " +
                           "courseDesc = @courseDesc, coursePrice = @coursePrice, courseStatus = @courseStatus " +
                           "WHERE courseId = @courseId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@courseName", updatedCourse.courseName);
                    command.Parameters.AddWithValue("@categoryId", updatedCourse.categoryId);
                    command.Parameters.AddWithValue("@courseImg", updatedCourse.courseImg);
                    command.Parameters.AddWithValue("@courseDesc", updatedCourse.courseDesc);
                    command.Parameters.AddWithValue("@coursePrice", updatedCourse.coursePrice);
                    command.Parameters.AddWithValue("@courseStatus", updatedCourse.courseStatus);

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

        public bool DeleteCourseById(Guid courseId)
        {
            bool result = false;

            string query = "DELETE FROM course WHERE courseId = @courseId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@courseId", courseId);

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
