using DataAccesLayer.Repository.IRepository;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.IRepository
{
    public interface IProductRepository: IRepository<Product>
    {
        void Update(Product obj);
        void Delete(Product obj);
        void Save();
    }
}
