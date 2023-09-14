using DataAccesLayer.Context;
using DataAccesLayer.Repository.IRepository;
using DataAccesLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccesLayer.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            User = new UserRepository(_db);

        }
        public IUserRepository User { get; set; }

        public void Save()
        {

            _db.SaveChanges();
        }
    }
}