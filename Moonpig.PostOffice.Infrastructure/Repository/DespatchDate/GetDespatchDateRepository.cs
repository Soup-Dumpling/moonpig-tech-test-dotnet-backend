using Moonpig.PostOffice.Core.Interfaces;
using Moonpig.PostOffice.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moonpig.PostOffice.Infrastructure.Repository.DespatchDate
{
    public class GetDespatchDateRepository : IGetDespatchDateRepository
    {
        private readonly DbContext dbContext;

        public GetDespatchDateRepository(DbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        public Task<bool> CheckIfProductExistsAsync(int productId)
        {
            return Task.FromResult(dbContext.Products.Any(x => x.ProductId == productId));
        }

        public Task<int?> GetSupplierLeadTimeByProductIdAsync(int productId)
        {
            var product = dbContext.Products.Single(x => x.ProductId == productId);
            var supplier = dbContext.Suppliers.SingleOrDefault(x => x.SupplierId == product.SupplierId);
            return Task.FromResult(supplier?.LeadTime);
        }
    }
}
