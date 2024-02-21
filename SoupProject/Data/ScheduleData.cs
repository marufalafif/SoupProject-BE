using MySql.Data.MySqlClient;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class ScheduleData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ScheduleData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Schedule> GetSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();

            string query = "SELECT * FROM schedule";

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
                                Schedule schedule = new Schedule
                                {
                                    scheduleId = dataReader.GetInt32("scheduleId"),
                                    courseId = Guid.Parse(dataReader.GetString("courseId")),
                                    startTime = dataReader.GetString("startTime"),
                                    isActive = Convert.ToInt32(dataReader["isActive"]),
                                };

                                schedules.Add(schedule);
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

            return schedules;
        }

        public Schedule GetScheduleById(int scheduleId)
        {
            Schedule schedule = null;

            string query = "SELECT * FROM schedule WHERE scheduleId = @scheduleId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@scheduleId", scheduleId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                schedule = new Schedule
                                {
                                    scheduleId = dataReader.GetInt32("scheduleId"),
                                    courseId = Guid.Parse(dataReader.GetString("courseId")),
                                    startTime = dataReader.GetString("startTime"),
                                    isActive = Convert.ToInt32(dataReader["isActive"]),
                                };
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

            return schedule;
        }

        public List<Schedule> GetSchedulesByCourse(Guid courseId)
        {
            List<Schedule> schedules = new List<Schedule>();

            string query = "SELECT * FROM schedule WHERE courseId = @courseId";

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
                                Schedule schedule = new Schedule
                                {
                                    scheduleId = dataReader.GetInt32("scheduleId"),
                                    courseId = Guid.Parse(dataReader.GetString("courseId")),
                                    startTime = dataReader.GetString("startTime"),
                                    isActive = Convert.ToInt32(dataReader["isActive"]),
                                };

                                schedules.Add(schedule);

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

            return schedules;
        }

        public bool InsertSchedule(Schedule schedule)
        {
            bool result = false;

            string query = $"INSERT INTO schedule (courseId, startTime) " +
                $"VALUES (@courseId, @startTime)";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@courseId", schedule.courseId);
                    command.Parameters.AddWithValue("@startTime", schedule.startTime);

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



        public bool DeleteSchedulesByCourse(Guid courseId)
        {
            bool result = false;

            string deleteSchedulesQuery = "DELETE FROM schedule WHERE courseId = @courseId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand deleteSchedulesCommand = new MySqlCommand(deleteSchedulesQuery, connection))
                {
                    deleteSchedulesCommand.Parameters.AddWithValue("@courseId", courseId);

                    try
                    {
                        connection.Open();
                        result = deleteSchedulesCommand.ExecuteNonQuery() > 0;
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


        public bool DeleteSchedule(int scheduleId)
        {
            bool result = false;

            string query = "DELETE FROM schedule WHERE scheduleId = @scheduleId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@scheduleId", scheduleId);

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

        public Schedule GetScheduleByStartTime(string startTime)
        {
            Schedule schedule = null;

            string query = "SELECT * FROM schedule WHERE startTime = @startTime";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@startTime", startTime);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                schedule = new Schedule
                                {
                                    scheduleId = dataReader.GetInt32("scheduleId"),
                                    courseId = Guid.Parse(dataReader.GetString("courseId")),
                                    startTime = dataReader.GetString("startTime"),
                                    isActive = Convert.ToInt32(dataReader["isActive"]),
                                };
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

            return schedule;
        }


        public bool UpdateSchedule(string startTime, Schedule updatedSchedule)
        {
            bool result = false;

            string updateScheduleQuery = "UPDATE schedule SET isActive = @isActive WHERE startTime = @startTime";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand updateScheduleCommand = new MySqlCommand(updateScheduleQuery, connection))
                {
                    updateScheduleCommand.Parameters.AddWithValue("@startTime", startTime);
                    updateScheduleCommand.Parameters.AddWithValue("@isActive", updatedSchedule.isActive);

                    try
                    {
                        connection.Open();
                        result = updateScheduleCommand.ExecuteNonQuery() > 0;
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
