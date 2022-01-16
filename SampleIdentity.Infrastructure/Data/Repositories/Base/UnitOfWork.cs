using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using SampleIdentity.Core.Repositories.Interfaces;
using SampleIdentity.Infrastructure.Data.Context;

namespace SampleIdentity.Infrastructure.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public bool Commit()
        {
            return _dbContext.SaveChanges() > 0;
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        #region Private Methods


        #endregion
    }
}
