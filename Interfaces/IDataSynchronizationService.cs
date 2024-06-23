// IDataSynchronizationService.cs
using System.Threading.Tasks;

namespace UnitOfWorkDemo.Services
{
    public interface IDataSynchronizationService
    {
        Task SynchronizeProductsAsync();
        // Add other synchronization methods as needed
    }
}
