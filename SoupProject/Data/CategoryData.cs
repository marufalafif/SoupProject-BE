using MySql.Data.MySqlClient;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class CategoryData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public CategoryData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            string query = "SELECT c.*, COUNT(cr.courseId) as CourseCount " +
                           "FROM category c " +
                           "LEFT JOIN course cr ON c.categoryId = cr.categoryId " +
                           "GROUP BY c.categoryId";

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
                                Category category = new Category
                                {
                                    categoryId = Convert.ToInt32(dataReader["categoryId"]),
                                    categoryName = dataReader["categoryName"].ToString() ?? string.Empty,
                                    categoryImg = dataReader["categoryImg"].ToString() ?? string.Empty,
                                    categoryDesc = dataReader["categoryDesc"].ToString() ?? string.Empty,
                                    categoryStatus = dataReader["categoryStatus"].ToString() ?? string.Empty,
                                    courseCount = Convert.ToInt32(dataReader["CourseCount"])
                                };

                                categories.Add(category);
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

            return categories;
        }


        //GetByID
        public Category? GetCategoryById(int categoryId)
        {
            Category? categoryById = null;
            string query = "SELECT c.*, COUNT(cr.courseId) as CourseCount " +
                           "FROM category c " +
                           "LEFT JOIN course cr ON c.categoryId = cr.categoryId " +
                           "WHERE c.categoryId = @categoryId " +
                           "GROUP BY c.categoryId";

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
                                Category category = new Category
                                {
                                    categoryId = Convert.ToInt32(dataReader["categoryId"]),
                                    categoryName = dataReader["categoryName"].ToString() ?? string.Empty,
                                    categoryImg = dataReader["categoryImg"].ToString() ?? string.Empty,
                                    categoryDesc = dataReader["categoryDesc"].ToString() ?? string.Empty,
                                    categoryStatus = dataReader["categoryStatus"].ToString() ?? string.Empty,
                                    courseCount = Convert.ToInt32(dataReader["CourseCount"])
                                };

                                categoryById = category;
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

            return categoryById;
        }

        //Insert/Post
        public bool InsertNewCategory(Category newCategory)
        {
            bool result = false;

            string query = $"INSERT INTO category (categoryName, categoryImg, categoryDesc, categoryStatus) VALUES(@categoryName, @categoryImg, @categoryDesc, @categoryStatus)";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@categoryName", newCategory.categoryName);
                    command.Parameters.AddWithValue("@categoryImg", newCategory.categoryImg);
                    command.Parameters.AddWithValue("@categoryDesc", newCategory.categoryDesc);
                    command.Parameters.AddWithValue("@categoryStatus", newCategory.categoryStatus);

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

        //Update/Put
        public bool UpdateCategory(int categoryId, Category newCategory)
        {
            bool result = false;

            string query = $"UPDATE category SET categoryName = @categoryName, categoryImg = @categoryImg, categoryDesc = @categoryDesc, categoryStatus = @categoryStatus WHERE categoryId = @categoryId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    command.Parameters.AddWithValue("@categoryName", newCategory.categoryName);
                    command.Parameters.AddWithValue("@categoryImg", newCategory.categoryImg);
                    command.Parameters.AddWithValue("@categoryDesc", newCategory.categoryDesc);
                    command.Parameters.AddWithValue("@categoryStatus", newCategory.categoryStatus);

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

        //Delete
        public bool DeleteById(int categoryId)
        {
            bool result = false;

            string query = "DELETE FROM category WHERE categoryId = @categoryId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@categoryId", categoryId);

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
    }
}
