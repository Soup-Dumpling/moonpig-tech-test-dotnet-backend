using System.Threading.Tasks;

namespace Moonpig.PostOffice.Core.Interfaces
{
    public interface IGetDespatchDateRepository
    {
        Task<bool> CheckIfProductExistsAsync(int productId);
        Task<int?> GetSupplierLeadTimeByProductIdAsync(int productId);
    }
}
