using DataAccesLayer.Context;
using DataAccesLayer.Repository;
using DataAccessLayer.Model;
using DataAccessLayer.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.Amount = obj.Amount;
                objFromDb.PictureUrl = obj.PictureUrl;
            }
        }

        public void Delete(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                _db.Products.Remove(objFromDb);
            }
        }
    }
}
