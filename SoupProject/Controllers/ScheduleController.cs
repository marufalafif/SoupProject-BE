using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoupProject.Data;
using SoupProject.DTOs;
using SoupProject.Models;

namespace SoupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleData _scheduleData;
        private readonly IWebHostEnvironment _environment;

        public ScheduleController(ScheduleData scheduleData, IWebHostEnvironment environment)
        {
            _scheduleData = scheduleData;
            _environment = environment;
        }

        // GET api/schedule
        [HttpGet("GetSchedule")]
        public IActionResult GetSchedule()
        {
            try
            {
                List<Schedule> schedules = _scheduleData.GetSchedules();
                return StatusCode(200, schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetScheduleByCourse")]
        public IActionResult GetSchedulesByCourse(Guid courseId)
        {
            try
            {
                List<Schedule> schedules = _scheduleData.GetSchedulesByCourse(courseId);
                return StatusCode(200, schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/schedule
        [HttpPost("addSchedule")]
        public IActionResult InsertSchedule([FromForm] ScheduleDTO scheduleDTO)
        {
            if (scheduleDTO == null)
                return BadRequest("Data kosong");

            Schedule schedule = new Schedule
            {
                courseId = scheduleDTO.courseId,
                startTime = scheduleDTO.startTime
            };

            bool result = _scheduleData.InsertSchedule(schedule);

            if (result)
            {
                return StatusCode(200, $"Schedule on CourseId {schedule.courseId} have been added");
            }
            else
            {
                return StatusCode(500, "Error Occurred");
            }
        }

        // DELETE api/schedule/{id}
        [HttpDelete("DeleteSchedule")]
        public ActionResult Delete(int id)
        {
            try
            {
                var result = _scheduleData.DeleteSchedule(id);

                if (result)
                    return Ok("Schedule deleted successfully");
                else
                    return NotFound("Schedule not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteScheduleByCourseId")]
        public IActionResult DeleteSchedulesByCourse(Guid courseId)
        {
            try
            {
                bool result = _scheduleData.DeleteSchedulesByCourse(courseId);
                if (result)
                {
                    return Ok("Schedules deleted successfully.");
                }
                else
                {
                    return NotFound("No schedules found for the given courseId.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
           
          }

        [HttpPut("DeactivateSchedule")]
        public IActionResult DeactivateScheduleByStartTime(string startTime)
        {
            try
            {
                Schedule existingSchedule = _scheduleData.GetScheduleByStartTime(startTime);

                if (existingSchedule == null)
                {
                    return NotFound("No schedule found for the given startTime.");
                }

                // Mengupdate properti isActive menjadi 0
                existingSchedule.isActive = 0;

                // Panggil metode untuk melakukan pembaruan di database
                bool result = _scheduleData.UpdateSchedule(startTime, existingSchedule);

                if (result)
                {
                    return Ok("Schedule deactivated successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to deactivate schedule.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("ActivateSchedule")]
        public IActionResult ActivateScheduleByStartTime(string startTime)
        {
            try
            {
                Schedule existingSchedule = _scheduleData.GetScheduleByStartTime(startTime);

                if (existingSchedule == null)
                {
                    return NotFound("No schedule found for the given startTime.");
                }

                // Mengupdate properti isActive menjadi 0
                existingSchedule.isActive = 1;

                // Panggil metode untuk melakukan pembaruan di database
                bool result = _scheduleData.UpdateSchedule(startTime, existingSchedule);

                if (result)
                {
                    return Ok("Schedule activated successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to deactivate schedule.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }

}
