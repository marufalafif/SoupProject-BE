using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoupProject.Data;
using SoupProject.DTOs;
using SoupProject.Models;

namespace SoupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseData _courseData;
        private readonly CategoryData _categoryData;
        private readonly IWebHostEnvironment _environment;

        public CourseController(CourseData courseData, CategoryData categoryData, IWebHostEnvironment environment)
        {
            _courseData = courseData;
            _categoryData = categoryData;
            _environment = environment;
        }

        [HttpGet("GetAllCourses")]
        public IActionResult GetCourses()
        {
            try
            {
                List<Course> courses = _courseData.GetCourses();

                // looping course untuk dapatkan category dan image 
                foreach (var course in courses)
                {
                    // dapatkan category menggunkan id
                    Category? category = _categoryData.GetCategoryById(course.categoryId);

                    // tetapkan nama untuk course categoryName
                    course.categoryName = category?.categoryName ?? string.Empty;

                    // set url gambar
                    course.ImagePath = GetImageUrl(course.courseImg);

                    // mendapatkan jumlah cart untuk setiap course
                    int cartCount = _courseData.GetCartCount(course.courseId);
                    course.CartCount = cartCount;
                }

                return StatusCode(200, courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetCourse")]
        public IActionResult GetCourseById(Guid courseId)
        {
            Course? courseById = _courseData.GetCourseById(courseId);

            if (courseById == null)
                return StatusCode(404, "Data Not Found");

            Category? category = _categoryData.GetCategoryById(courseById.categoryId);

            courseById.categoryName = category?.categoryName ?? string.Empty;

            courseById.ImagePath = GetImageUrl(courseById.courseImg);

            return StatusCode(200, courseById);
        }

        [HttpGet("GetCoursesByCategoryId")]
        public IActionResult GetCoursesByCategoryId(int categoryId)
        {
            try
            {
                List<Course> courses;

                if (categoryId == 0)
                {
                    return BadRequest("CategoryId must be provided");
                }
                else
                {
                    // Jika categoryId != 0, ambil kursus berdasarkan kategori yang dipilih
                    courses = _courseData.GetCoursesByCategoryId(categoryId);
                }

                // looping course untuk dapatkan category dan image 
                foreach (var course in courses)
                {
                    // dapatkan category menggunakan id
                    Category? category = _categoryData.GetCategoryById(course.categoryId);

                    // tetapkan nama untuk course categoryName
                    course.categoryName = category?.categoryName ?? string.Empty;

                    // set url gambar
                    course.ImagePath = GetImageUrl(course.courseImg);
                }

                return StatusCode(200, courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Metode bantuan untuk mendapatkan URL publik dari path gambar
        private string GetImageUrl(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                // Return URL lengkap gambar
                return $"{Request.Scheme}://{Request.Host}/Images/{imagePath}";
            }
            return string.Empty;
        }

        [HttpPost("AddCourse")]
        public IActionResult InsertNewCourse([FromForm] CourseDTO courseDTO)
        {
            if (courseDTO == null || courseDTO.courseImage == null)
                return BadRequest("Data and Image should be provided");

            // dapatkan nama gambar
            string imageName = SaveImage(courseDTO.courseImage);

            Course newCourse = new Course
            {
                courseId = Guid.NewGuid(),
                courseName = courseDTO.courseName,
                categoryId = courseDTO.categoryId,
                courseImg = imageName,
                courseDesc = courseDTO.courseDesc,
                coursePrice = courseDTO.coursePrice,
                courseStatus = courseDTO.courseStatus
            };

            bool result = _courseData.InsertNewCourse(newCourse);

            if (result)
            {
                Category? category = _categoryData.GetCategoryById(newCourse.categoryId);

                newCourse.categoryName = category?.categoryName ?? string.Empty;

                newCourse.ImagePath = GetImageUrl(newCourse.courseImg);

                return StatusCode(200, newCourse);
            }
            else
            {
                return StatusCode(500, "Error Occurred");
            }
        }


 
        //function untuk ambil gambar
        private string SaveImage(IFormFile imageFile)
        {
            // Direktori tempat gambar akan disimpan 
            string uploadPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images");

            // Tampung gambar
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string fileExtension = Path.GetExtension(imageFile.FileName);

            // Generate nama file yang unik dengan timestamp
            string uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";

            // Gabungkan path dengan nama file untuk mendapatkan path lengkap
            string filePath = Path.Combine(uploadPath, uniqueFileName);

            // Simpan gambar ke server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return uniqueFileName;
        }


        [HttpPut("UpdateCourse")]
        public IActionResult UpdateCourse(Guid courseId, [FromForm] CourseDTO courseDTO)
        {
            if (courseDTO == null)
                return BadRequest("Data should be inputed");

            Course existingCourse = _courseData.GetCourseById(courseId);

            if (existingCourse == null)
                return StatusCode(404, "Data Not Found");

            existingCourse.courseName = courseDTO.courseName;
            existingCourse.categoryId = courseDTO.categoryId;
            existingCourse.courseDesc = courseDTO.courseDesc;
            existingCourse.coursePrice = courseDTO.coursePrice;
            existingCourse.courseStatus = courseDTO.courseStatus;

            // Jika file gambar baru diunggah, simpan file baru tersebut
            if (courseDTO.courseImage != null)
            {
                // Hapus file gambar lama jika ada
                if (!string.IsNullOrEmpty(existingCourse.courseImg))
                {
                    string oldImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", existingCourse.courseImg);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // simpan gambar baru
                existingCourse.courseImg = SaveImage(courseDTO.courseImage);

                // Set url gambar
                existingCourse.ImagePath = GetImageUrl(existingCourse.courseImg);
            }

            // Update course di database
            bool result = _courseData.UpdateCourse(courseId, existingCourse);

            if (result)
            {
                Category? category = _categoryData.GetCategoryById(existingCourse.categoryId);

                existingCourse.categoryName = category?.categoryName ?? string.Empty;

                return StatusCode(200, existingCourse);
            }
            else
            {
                return StatusCode(500, "Error Occur");
            }
        }

        [HttpDelete("DeleteCourse")]
        public IActionResult DeleteCourse(Guid courseId)
        {
            Course? courseToDelete = _courseData.GetCourseById(courseId);

            if (courseToDelete == null)
            {
                return StatusCode(404, "Data Not Found");
            }

            // dapatkan path gambar untuk di hapus
            string imagePathToDelete = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", courseToDelete.courseImg);

            bool result = _courseData.DeleteCourseById(courseId);

            if (result)
            {
                if (System.IO.File.Exists(imagePathToDelete))
                {
                    System.IO.File.Delete(imagePathToDelete);
                }

                return StatusCode(200);
            }
            else
            {
                return StatusCode(500, "Error Occur");
            }
        }
    }
}
