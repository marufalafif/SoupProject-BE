using Microsoft.AspNetCore.Mvc;
using SoupProject.Data;
using SoupProject.DTOs;
using SoupProject.Models;

namespace SoupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryData _categoryData;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(CategoryData categoryData, IWebHostEnvironment environment)
        {
            _categoryData = categoryData;
            _environment = environment;
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetCategories()
        {
            try
            {
                List<Category> categories = _categoryData.GetCategories();

                // Mengubah path gambar menjadi path lengkap
                categories.ForEach(category => category.ImagePath = GetImageUrl(category.categoryImg));

                return StatusCode(200, categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetCategory")]
        public IActionResult GetCategoryById(int categoryId)
        {
            Category? categoryById = _categoryData.GetCategoryById(categoryId);

            if (categoryById == null)
                return StatusCode(404, "Data Not Found");

            // Mengubah path gambar menjadi URL publik
            categoryById.ImagePath = GetImageUrl(categoryById.categoryImg);

            return StatusCode(200, categoryById);
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

        [HttpPost("AddCategory")]
        public IActionResult InsertNewCategory([FromForm] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null || categoryDTO.ImageFile == null)
                return BadRequest("Data and Image should be provided");

            // Dapatkan gambar 
            string imageName = SaveImage(categoryDTO.ImageFile);

            Category newCategory = new Category
            {
                categoryName = categoryDTO.categoryName,
                categoryDesc = categoryDTO.categoryDesc,
                categoryStatus = categoryDTO.categoryStatus,
                categoryImg = imageName 
            };

            bool result = _categoryData.InsertNewCategory(newCategory);

            if (result)
            {
                // Menggunakan URL gambar langsung sebagai categoryImg
                newCategory.ImagePath = imageName;

                return StatusCode(200, $"{newCategory.categoryName} have been added");
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

        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory(int categoryId, [FromForm] CategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                return BadRequest("Data should be inputed");

            Category existingCategory = _categoryData.GetCategoryById(categoryId);

            if (existingCategory == null)
                return StatusCode(404, "Data Not Found");

            // Mengupdate properti yang diperlukan
            existingCategory.categoryName = categoryDTO.categoryName;
            existingCategory.categoryDesc = categoryDTO.categoryDesc;
            existingCategory.categoryStatus = categoryDTO.categoryStatus;

            // Jika ada file gambar baru diunggah, simpan file yang baru
            if (categoryDTO.ImageFile != null)
            {
                // Hapus file gambar yang lama jika ada
                if (!string.IsNullOrEmpty(existingCategory.categoryImg))
                {
                    string oldImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", existingCategory.categoryImg);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Simpan file gambar yang baru
                existingCategory.categoryImg = SaveImage(categoryDTO.ImageFile);
            }

            // Update kategori dalam database
            bool result = _categoryData.UpdateCategory(categoryId, existingCategory);

            if (result)
            {
                // Mengubah path gambar menjadi URL publik
                existingCategory.ImagePath = GetImageUrl(existingCategory.categoryImg);

                return StatusCode(200, existingCategory);
            }
            else
            {
                return StatusCode(500, "Error Occur");
            }
        }

        [HttpDelete("DeleteCategory")]
        public IActionResult DeleteCategory(int categoryId)
        {
            // Mendapatkan informasi kategori sebelum dihapus
            Category? categoryToDelete = _categoryData.GetCategoryById(categoryId);

            if (categoryToDelete == null)
            {
                return StatusCode(404, "Data Not Found");
            }

            // Mendapatkan path gambar untuk dihapus
            string imagePathToDelete = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", categoryToDelete.categoryImg);

            bool result = _categoryData.DeleteById(categoryId);

            if (result)
            {
                // Menghapus gambar secara fisik jika kategori berhasil dihapus
                if (System.IO.File.Exists(imagePathToDelete))
                {
                    System.IO.File.Delete(imagePathToDelete);
                }

                return StatusCode(200, "has been deleted");
            }
            else
            {
                return StatusCode(500, "Error Occur");
            }
        }
    }
}
