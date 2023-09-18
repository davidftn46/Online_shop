using DataAccesLayer.Context;
using DataAccesLayer.Repository;
using DataAccessLayer.Model;
using DataAccessLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class OrderRepository:Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Order> GetHistory(long Id)
        {
            List<Order> allOrders = _db.Orders.Include("OrderProducts").ToList();
            List<Order> retList = new List<Order>();
            foreach (Order order in allOrders)
                foreach (OrderProduct i in order.OrderProducts)
                    if (i.SellerId == Id && i.IsSent)
                    {
                        retList.Add(order);
                        break;
                    }
            return retList;
        }

        public IEnumerable<Order> GetNew(long Id)
        {
            List<Order> allOrders = _db.Orders.Include("OrderProducts").ToList();
            List<Order> retList = new List<Order>();
            foreach (Order order in allOrders)
                foreach (OrderProduct i in order.OrderProducts)
                    if (i.SellerId == Id && !i.IsSent)
                    {
                        retList.Add(order);
                        break;
                    }
            return retList;
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
