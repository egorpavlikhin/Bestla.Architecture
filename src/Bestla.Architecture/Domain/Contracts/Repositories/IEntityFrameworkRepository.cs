namespace Bestla.Architecture.Domain.Contracts.Repositories
{
    using System;

    public interface IEntityFrameworkRepository<T> : IEntityFrameworkRepositoryWithTypedId<T>
        where T : class
    {

    }
}