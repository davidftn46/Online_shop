using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShop_back.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("allProducts")]
        public IActionResult GetAll()
        {
            Answer<IEnumerable<ProductDTO>> response = _productService.GetAll();
            if (response.Status == Feedback.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpPost("addNew")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddNewProduct([FromForm] NewProductDTO itemDTO, IFormFile? file = null)
        {
            if (ModelState.IsValid)
            {
                string filePath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string uploadPath = "Images";
                    Console.WriteLine(uploadPath);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                    filePath = Path.Combine(filePath, "NoPhoto.jpg");
                }

                var task = _productService.AddProduct(itemDTO, filePath);
                if (task.Status == Feedback.OK)
                    return Ok(task.Message);
                else if (task.Status == Feedback.InternalServerError)
                {
                    if (System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                    return Problem(task.Message, statusCode: ((int)task.Status));
            }
            return Problem();
        }




    }
}
