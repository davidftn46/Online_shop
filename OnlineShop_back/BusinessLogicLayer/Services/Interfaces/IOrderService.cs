using Addition.DTO;
using Addition.Mutual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IOrderService
    {
        Answer<bool> AddOrder(NewOrderDTO orderDTO);
        Answer<bool> UpdateOrder(long id);
        Answer<IEnumerable<OrderDTO>> GetAll();
        Answer<IEnumerable<OrderDTO>> GetByUser(long UserId);
        Answer<IEnumerable<OrderDTO>> GetHistory(long UserId);
        Answer<IEnumerable<OrderDTO>> GetNew(long UserId);
        Answer<bool> CancelOrder(long id);
        bool MarkAsShiped(long id);
    }
}
