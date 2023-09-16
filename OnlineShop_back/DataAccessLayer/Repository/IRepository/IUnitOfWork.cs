using DataAccessLayer.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccesLayer.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        IProductRepository Product { get; }
        IOrderRepository Orders { get; }
        void Save();
    }
}