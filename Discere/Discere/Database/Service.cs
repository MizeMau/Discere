using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Discere.Database
{
    public class Service<T> where T : class
    {
        protected static DBContext CreateContext()
        {
            return new DBContext();
        }

        protected DbContext _context;
        public virtual IQueryable<T> GetQuery(bool withDeleted = false)
        {
            _context = CreateContext();
            if (withDeleted)
                return _context.Set<T>().AsQueryable();
            return _context.Set<T>().AsQueryable();
        }

        public virtual List<T> GetAll(bool withDeleted = false)
        {
            return GetQuery(withDeleted)
                .ToList();
        }

        public virtual T? GetById(long id)
        {
            using var context = CreateContext();
            return context.Set<T>().Find(id);
        }

        public virtual T Create(T entity)
        {
            using var context = CreateContext();
            context.Set<T>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public virtual T Update(T entity)
        {
            using var context = CreateContext();
            context.Set<T>().Update(entity);
            context.SaveChanges();
            return entity;
        }

        public virtual bool UpdateProperty<TProperty>(object key, Expression<Func<T, TProperty>> propertyExpression, TProperty value)
        {
            using var context = CreateContext();

            var entityType = context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType!.FindPrimaryKey()!;
            var keyProperty = primaryKey.Properties.First();

            var parameter = Expression.Parameter(typeof(T), "e");

            var propertyAccess = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { keyProperty.ClrType },
                parameter,
                Expression.Constant(keyProperty.Name)
            );

            var equals = Expression.Equal(
                propertyAccess,
                Expression.Constant(key)
            );

            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            var affected = context.Set<T>()
                .Where(lambda)
                .ExecuteUpdate(setters =>
                    setters.SetProperty(propertyExpression, value));

            return affected == 1;
        }

        public virtual bool Delete(long id)
        {
            using var context = CreateContext();
            var entity = context.Set<T>().Find(id);
            if (entity == null) return false;
            context.Set<T>().Remove(entity);
            context.SaveChanges();
            return true;
        }
    }
}
