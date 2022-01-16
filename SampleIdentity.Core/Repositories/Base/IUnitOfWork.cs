using System.Threading.Tasks;

namespace SampleIdentity.Core.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        bool Commit();

        Task<int> CommitAsync();
    }
}
