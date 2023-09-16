using DataAccesLayer.Context;
using DataAccesLayer.Repository.IRepository;
using DataAccesLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repository.IRepository;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;

namespace DataAccesLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            User = new UserRepository(_db);
            Product = new ProductRepository(_db);
            Orders = new OrderRepository(_db);
        }
        public IUserRepository User { get; set; }
        public IProductRepository Product { get; set; }
        public IOrderRepository Orders { get; set; }

        public void Save()
        {

            _db.SaveChanges();
        }
    }
}