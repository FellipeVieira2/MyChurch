using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;

namespace MyChurch.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly MyChurchDbContext _context;

        public GenericRepository(MyChurchDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> ById(Guid id)
           => await _context.Set<TEntity>().FindAsync(id);

        public async Task<IEnumerable<TEntity>> List()
            => await _context.Set<TEntity>().ToListAsync();

        public async void Create(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);

        public IQueryable<TEntity> Query()
            => _context.Set<TEntity>().AsQueryable();
    }
}
