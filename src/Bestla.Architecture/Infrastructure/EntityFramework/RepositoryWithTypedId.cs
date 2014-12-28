namespace Bestla.Architecture.Infrastructure.EntityFramework
{
    using System;
    using System.Data;
    using Microsoft.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Bestla.Architecture.Domain.Contracts.Repositories;

    public class RepositoryWithTypedId<T> : IEntityFrameworkRepositoryWithTypedId<T>
        where T : class
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public RepositoryWithTypedId(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public DbContext DbContext { get; set; }

        public int Count
        {
            get
            {
                return this.DbSet.Count();
            }
        }

        protected DbSet<T> DbSet
        {
            get
            {
                return this.DbContext.Set<T>();
            }
        }

        public IQueryable<T> GetAll()
        {
            return this.DbSet.AsQueryable();
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate).AsQueryable();
        }

        public IQueryable<T> Filter<TKey>(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var resetSet = filter != null ? this.DbSet.Where(filter).AsQueryable() :
                this.DbSet.AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) :
                resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Count(predicate) > 0;
        }

        public T Find(params object[] keys)
        {
            return this.DbSet.Single(x => keys.Contains(x));
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.FirstOrDefault(predicate);
        }

        public T Create(T entity)
        {
            var newEntry = this.DbSet.Add(entity);
            return newEntry.Entity;
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = this.Filter(predicate);
            foreach (var obj in objects)
            {
                this.DbSet.Remove(obj);
            }
        }

        public T Update(T entity)
        {
            var entry = this.DbContext.Entry(entity);
            this.DbSet.Attach(entity);
            entry.State = EntityState.Modified;

            return entity;
        }

        public T SaveOrUpdate(T entity)
        {
            return this.Update(entity);
        }

        public void Delete(T entity)
        {
            this.DbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            if (DbContext == null)
            {
                return;
            }

            try
            {
                DbContext.SaveChanges();
            }
            finally
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}