using Microsoft.Data.Entity;

namespace Bestla.Architecture.Infrastructure.EntityFramework
{
    public class Repository<T> : RepositoryWithTypedId<T>
        where T : class
    {
        public Repository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}