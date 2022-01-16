using SampleIdentity.Core.Common.Specifications;
using SampleIdentity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Services.Account.Specifications
{
    public class RefreshTokenByTicketSpecifications : BaseSpecification<RefreshToken>
    {
        private readonly string _protectedTicket;

        public RefreshTokenByTicketSpecifications(string protectedTicket)
        {
            _protectedTicket = protectedTicket;
        }

        public override Expression<Func<RefreshToken, bool>> IsSatisifiedBy()
        {
            return x => x.ProtectedTicket == _protectedTicket;
        }
    }
}
