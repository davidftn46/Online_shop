using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShop_back.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("allOrders")]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAll()
        {
            Answer<IEnumerable<OrderDTO>> response = _orderService.GetAll();
            if (response.Status == Feedback.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("myOrders")]
        public IActionResult GetCustomers(long id)
        {
            Answer<IEnumerable<OrderDTO>> response = _orderService.GetByUser(id);
            if (response.Status == Feedback.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("orderHistory")]
        public IActionResult GetSellersHistory(long id)
        {
            Answer<IEnumerable<OrderDTO>> response = _orderService.GetHistory(id);
            if (response.Status == Feedback.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }

        [HttpGet("newOrders")]
        public IActionResult GetSellersNew(long id)
        {
            Answer<IEnumerable<OrderDTO>> response = _orderService.GetNew(id);
            if (response.Status == Feedback.OK)
                return Ok(response.Data);
            else
                return Problem(response.Message);
        }


        [HttpPost("newOrder")]
        [Authorize(Roles = "Customer")]
        public IActionResult NewOrder([FromBody] NewOrderDTO orderDTO)
        {
            Answer<bool> response = _orderService.AddOrder(orderDTO);
            if (response.Status == Feedback.OK)
                return Ok(response.Message);
            else
                return Problem(response.Message);
        }

        [HttpPost("sendProduct")]
        public IActionResult SendProduct(long id)
        {
            Answer<bool> response = _orderService.UpdateOrder(id);
            if (response.Status == Feedback.OK)
                return Ok(response.Message);
            else
                return Problem(response.Message);
        }
    }
}
