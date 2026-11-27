using Moonpig.PostOffice.Data;
using Moonpig.PostOffice.Infrastructure.Repository.DespatchDate;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Moonpig.PostOffice.Tests.Repository.DespatchDate
{
    public class GetDespatchDateRepositoryUnitTests
    {
        private readonly GetDespatchDateRepository getDespatchDateRepository;

        public GetDespatchDateRepositoryUnitTests()
        {
            var dbContext = new DbContext();
            getDespatchDateRepository = new GetDespatchDateRepository(dbContext);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(5, true)]
        [InlineData(8, true)]
        [InlineData(10, true)]
        [InlineData(20, false)]
        public async Task CheckIfProductExistsAsync_ReturnsExpectedResult(int productId, bool expectedResult) 
        {
            // Act
            var result = await getDespatchDateRepository.CheckIfProductExistsAsync(productId);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(4, 1)]
        [InlineData(6, 2)]
        [InlineData(10, 13)]
        public async Task GetSupplierLeadTimeByProductIdAsync_ReturnsExpectedSupplierLeadTime(int productId, int expectedSupplierLeadTime)
        {
            // Act
            var result = await getDespatchDateRepository.GetSupplierLeadTimeByProductIdAsync(productId);

            // Assert
            result.ShouldBe(expectedSupplierLeadTime);
        }

        [Fact]
        public async Task GetSupplierLeadTimeByProductIdAsync_ReturnsNullWhenSupplierDoesNotExist()
        {
            // Arrange
            var productId = 8;

            // Act
            var result = await getDespatchDateRepository.GetSupplierLeadTimeByProductIdAsync(productId);

            // Assert
            result.ShouldBeNull();
        }
    }
}
