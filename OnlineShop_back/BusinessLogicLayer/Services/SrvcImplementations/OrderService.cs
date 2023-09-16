using Addition.Constants;
using Addition.DTO;
using Addition.Mutual;
using AutoMapper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccesLayer.Repository.IRepository;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.SrvcImplementations
{
    public class OrderService: IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShipmentService _shipmentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IShipmentService shipmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shipmentService = shipmentService;
        }

        public Answer<bool> AddOrder(NewOrderDTO orderDTO)
        {
            Order order = new Order();
            order.UserId = orderDTO.UserId;
            order.Address = orderDTO.Address;
            order.City = orderDTO.City;
            order.Zip = orderDTO.Zip;
            order.Comment = orderDTO.Comment;
            order.OrderProducts = new List<OrderProduct>();
            foreach (NewOrderProductDTO oi in orderDTO.Products)
            {
                try
                {
                    Product i = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == oi.Id);
                    if (i.Amount < oi.Amount)
                        throw new Exception("Not enough items");
                    order.OrderProducts.Add(new OrderProduct()
                    {
                        ProductId = i.Id,
                        IsSent = false,
                        SellerId = i.UserId,
                        Amount = oi.Amount,
                    });
                    i.Amount -= oi.Amount;
                    _unitOfWork.Product.Update(i);

                }
                catch (Exception ex)
                {
                    return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while adding an order:" + ex.Message);
                }
            }

            try
            {
                _unitOfWork.Orders.Add(order);
                _unitOfWork.Save();

                int arrivalMinutes = _shipmentService.ScheduleShipment(order.Id);

                return new Answer<bool>(true, Feedback.OK, $"Order successfully made. Items will be shipped in {arrivalMinutes} minutes.");
            }
            catch (Exception ex)
            {
                return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while adding an order:" + ex.Message);
            }
        }


        public Answer<IEnumerable<OrderDTO>> GetAll()
        {
            var list = _unitOfWork.Orders.GetAll(includeProperties: "OrderItems");
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderProductDTO product in retOrder.OrderProducts)
                {
                    Product i = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == product.ProductId);
                    product.Product = _mapper.Map<ProductDTO>(i);
                    product.SellerId = i.UserId;
                }
                retOrder.Canceled = order.Canceled;
                retOrder.Shipped = retOrder.Shipped;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new Answer<IEnumerable<OrderDTO>>(retList, Feedback.OK, "All orders");
        }

       
        public Answer<IEnumerable<OrderDTO>> GetByUser(long UserId)
        {
            var list = _unitOfWork.Orders.GetAll(o => o.UserId == UserId && !o.Canceled, includeProperties: "OrderItems");
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderProductDTO product in retOrder.OrderProducts)
                {
                    Product i = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == product.ProductId);
                    product.Product = _mapper.Map<ProductDTO>(i);
                    product.SellerId = i.UserId;
                }
              
                TimeSpan t = _shipmentService.GetRemainingTime(order.Id);
                retOrder.Minutes = (int)t.TotalMinutes;

                retOrder.Shipped = order.Shipped;
                retOrder.Canceled = order.Canceled;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new Answer<IEnumerable<OrderDTO>>(retList, Feedback.OK, "All orders for user" + UserId);
        }

        
        public Answer<IEnumerable<OrderDTO>> GetHistory(long UserId)
        {
            var list = _unitOfWork.Orders.GetHistory(UserId);
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderProductDTO product in retOrder.OrderProducts)
                {
                    if (product.SellerId != UserId)
                        retOrder.OrderProducts.Remove(product);
                    else
                    {
                        Product i = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == product.ProductId);
                        product.Product = _mapper.Map<ProductDTO>(i);
                        product.SellerId = i.UserId;
                    }
                }
                retOrder.Canceled = order.Canceled;
                retOrder.Shipped = retOrder.Shipped;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new Answer<IEnumerable<OrderDTO>>(retList, Feedback.OK, "All previous orders for user" + UserId);
        }

       
        public Answer<IEnumerable<OrderDTO>> GetNew(long UserId)
        {
            var list = _unitOfWork.Orders.GetNew(UserId);
            var retList = new List<OrderDTO>();
            foreach (var order in list)
            {
                OrderDTO retOrder = _mapper.Map<OrderDTO>(order);
                foreach (OrderProductDTO product in retOrder.OrderProducts)
                {
                    Product i = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == product.ProductId);
                    product.Product = _mapper.Map<ProductDTO>(i);
                    product.SellerId = i.UserId;
                }
                
                retOrder.Minutes = (int)_shipmentService.GetRemainingTime(order.Id).TotalMinutes;

                retOrder.Shipped = order.Shipped;
                retOrder.Canceled = order.Canceled;
                retOrder.Username = _unitOfWork.User.GetFirstOrDefault(u => u.Id == order.UserId).UserName;
                retList.Add(retOrder);
            }

            return new Answer<IEnumerable<OrderDTO>>(retList, Feedback.OK, "All new orders for user" + UserId);
        }

        
        public Answer<bool> UpdateOrder(long id)
        {
            List<Order> orders = _unitOfWork.Orders.GetAll(includeProperties: "OrderItems").ToList();
            foreach (Order order in orders)
            {
                foreach (OrderProduct orderProduct in order.OrderProducts)
                {
                    if (orderProduct.Id == id)
                    {
                        orderProduct.IsSent = true;
                        _unitOfWork.Orders.Save();
                        return new Answer<bool>(true, Feedback.OK, "Item sent");
                    }
                }
            }
            return new Answer<bool>(false, Feedback.NotFound, "Item Not Found");
        }

        
        public Answer<bool> CancelOrder(long id)
        {
            try
            {
                if (_shipmentService.CancelShipment(id))
                {
                    _unitOfWork.Orders.GetFirstOrDefault(o => o.Id == id).Canceled = true;
                    _unitOfWork.Save();
                    return new Answer<bool>(true, Feedback.OK, "Order canceled");
                }
                else
                    return new Answer<bool>(false, Feedback.InternalServerError, "Order can't be canceled");

            }
            catch
            {
                return new Answer<bool>(false, Feedback.InternalServerError, "There was an error while canceling the order");
            }
        }

      
        public bool MarkAsShiped(long id)
        {
            try
            {
                _unitOfWork.Orders.GetFirstOrDefault(o => o.Id == id).Shipped = true;
                _unitOfWork.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
