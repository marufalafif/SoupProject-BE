using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoupProject.Data;
using SoupProject.DTOs;
using SoupProject.Models;

namespace SoupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentData _paymentData;
        private readonly IWebHostEnvironment _environment;

        public PaymentController(PaymentData paymentData, IWebHostEnvironment environment)
        {
            _paymentData = paymentData;
            _environment = environment;
        }

        [HttpGet("GetAllPayments")]
        public IActionResult GetCategories()
        {
            try
            {
                List<Payment> payments = _paymentData.GetPayments();

                // Mengubah path gambar menjadi path lengkap
                payments.ForEach(payment => payment.ImagePath = GetImageUrl(payment.paymentImg));

                return StatusCode(200, payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetPayment")]
        public IActionResult GetCategoryById(int paymentId)
        {
            Payment? paymentById = _paymentData.GetPaymentById(paymentId);

            if (paymentById == null)
                return StatusCode(404, "Data Not Found");

            // Mengubah path gambar menjadi URL publik
            paymentById.ImagePath = GetImageUrl(paymentById.paymentImg);

            return StatusCode(200, paymentById);
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

        [HttpPost("AddPayment")]
        public IActionResult InsertNewCategory([FromForm] PaymentDTO paymentDto)
        {
            if (paymentDto == null || paymentDto.ImageFile == null)
                return BadRequest("Data and Image should be provided");

            // Dapatkan gambar 
            string imageName = SaveImage(paymentDto.ImageFile);

            Payment newPayment = new Payment
            {
                paymentName = paymentDto.paymentName,
                paymentStatus = paymentDto.paymentStatus,
                paymentImg = imageName
            };

            bool result = _paymentData.InsertNewPayment(newPayment);

            if (result)
            {
                // Menggunakan URL gambar langsung sebagai categoryImg
                newPayment.ImagePath = imageName;

                return StatusCode(200, $"{newPayment.paymentName} have been added");
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

        [HttpPut("UpdatePayment")]
        public IActionResult UpdateCategory(int paymentId, [FromForm] PaymentDTO paymentDto)
        {
            if (paymentDto == null)
                return BadRequest("Data should be inputed");

            Payment existingPayment = _paymentData.GetPaymentById(paymentId);

            if (existingPayment == null)
                return StatusCode(404, "Data Not Found");

            // Mengupdate properti yang diperlukan
            existingPayment.paymentName = paymentDto.paymentName;
            existingPayment.paymentStatus = paymentDto.paymentStatus;

            // Jika ada file gambar baru diunggah, simpan file yang baru
            if (paymentDto.ImageFile != null)
            {
                // Hapus file gambar yang lama jika ada
                if (!string.IsNullOrEmpty(existingPayment.paymentImg))
                {
                    string oldImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", existingPayment.paymentImg);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Simpan file gambar yang baru
                existingPayment.paymentImg = SaveImage(paymentDto.ImageFile);
            }

            // Update kategori dalam database
            bool result = _paymentData.UpdatePayment(paymentId, existingPayment);

            if (result)
            {
                // Mengubah path gambar menjadi URL publik
                existingPayment.ImagePath = GetImageUrl(existingPayment.paymentImg);

                return StatusCode(200, existingPayment);
            }
            else
            {
                return StatusCode(500, "Error Occur");
            }
        }

        [HttpDelete("DeletePayment")]
        public IActionResult DeleteCategory(int paymentId)
        {
            // Mendapatkan informasi kategori sebelum dihapus
            Payment? paymentToDelete = _paymentData.GetPaymentById(paymentId);

            if (paymentToDelete == null)
            {
                return StatusCode(404, "Data Not Found");
            }

            // Mendapatkan path gambar untuk dihapus
            string imagePathToDelete = Path.Combine(_environment.ContentRootPath, "wwwroot", "Images", paymentToDelete.paymentImg);

            bool result = _paymentData.DeletePaymentById(paymentId);

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
