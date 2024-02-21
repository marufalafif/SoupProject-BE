using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using SoupProject.Data;
using SoupProject.DTOs.Cart;
using SoupProject.Models;
using System.Data.Common;
using System.Security.Claims;

namespace SoupProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutData checkoutData;

        public CheckoutController(CheckoutData checkoutData)
        {
            this.checkoutData = checkoutData;
        }

        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] CartDTO cartDTO)
        {
            try
            {
                if (cartDTO == null) return BadRequest("Data should be inputed");

                Cart cart = new Cart
                {
                    userId = cartDTO.userId,
                    courseId = cartDTO.courseId,
                    courseDate = cartDTO.courseDate
                };

                bool result = checkoutData.InsertToCart(cart);

                if (result) return StatusCode(201);
                else return StatusCode(500, "Failed adding to cart.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("Checkout")]
        public IActionResult Checkout([FromBody] CheckoutDTO checkoutDTO)
        {
            try
            {
                if (checkoutDTO == null)
                {
                    return BadRequest("There is no order data");
                }

                string invoiceNumber = "INV" + DateTime.Today.ToString("ddMMyyyyhmmss") + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                Order order = new Order
                {
                    orderId = Guid.NewGuid(),
                    invoice = invoiceNumber,
                    userId = checkoutDTO.userId,
                    paymentMethod = checkoutDTO.paymentMethod
                };

                bool result = checkoutData.Checkout(order, checkoutDTO.selectedCourses);

                if (result)
                {
                    return StatusCode(201, "Success");
                }
                else
                {
                    return StatusCode(500, "Error occurred during checkout transaction.");
                }
            }
            catch (ArgumentNullException ex)
            {
                // Handle ArgumentNullException
                return BadRequest("Invalid input: " + ex.Message);
            }
            catch (DbException ex)
            {
                // Handle database-related exceptions
                return StatusCode(500, "Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("CheckoutByCourse")]
        public IActionResult CheckoutByCourse([FromBody] CheckoutByCourseDTO checkoutDTO)
        {
            try
            {
                if (checkoutDTO == null)
                {
                    return BadRequest("There is no order data");
                }

                string invoiceNumber = "INV" + DateTime.Today.ToString("ddMMyyyyhmmss") + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                Order order = new Order
                {
                    orderId = Guid.NewGuid(),
                    invoice = invoiceNumber,
                    userId = checkoutDTO.userId,
                    paymentMethod = checkoutDTO.paymentMethod,
                    courseDate = checkoutDTO.courseDate
                };

                bool result = checkoutData.CheckoutByCourse(order, checkoutDTO.courseId);

                if (result)
                {
                    return StatusCode(201, "Success");
                }
                else
                {
                    return StatusCode(500, "Error occurred during checkout transaction.");
                }
            }
            catch (ArgumentNullException ex)
            {
                // Handle ArgumentNullException
                return BadRequest("Invalid input: " + ex.Message);
            }
            catch (DbException ex)
            {
                // Handle database-related exceptions
                return StatusCode(500, "Database error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetCartById")]
        public IActionResult GetCartById(Guid userId)
        {
            try
            {
                List<UserCartDTO> cart = checkoutData.GetCartById(userId);
                return StatusCode(200, cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetMyCourseById")]
        public IActionResult GetMyCourseById(Guid userId)
        {
            try
            {
                List<MyCourseDTO> cart = checkoutData.GetMyCourseById(userId);
                return StatusCode(200, cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteCart")]
        public IActionResult DeleteCartByIdCart(int cartId)
        {
            try
            {
                bool result = checkoutData.DeleteCartByIdCart(cartId);
                if (result)
                {
                    return Ok(new { Message = "Cart deleted successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Cart not found." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

    }
}
