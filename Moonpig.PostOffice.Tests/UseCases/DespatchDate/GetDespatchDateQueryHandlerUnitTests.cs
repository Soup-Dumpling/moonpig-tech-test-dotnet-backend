using Moonpig.PostOffice.Core.Exceptions;
using Moonpig.PostOffice.Core.Interfaces;
using Moonpig.PostOffice.Core.UseCases.DespatchDate.GetDespatchDate;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Moonpig.PostOffice.Tests.UseCases.DespatchDate
{
    public class GetDespatchDateQueryHandlerUnitTests
    {
        private readonly GetDespatchDateQueryHandler getDespatchDateQueryHandler;
        private readonly IGetDespatchDateRepository getDespatchDateRepositoryMock = Substitute.For<IGetDespatchDateRepository>();

        public GetDespatchDateQueryHandlerUnitTests()
        {
            getDespatchDateQueryHandler = new GetDespatchDateQueryHandler(getDespatchDateRepositoryMock);
        }

        public static IEnumerable<object[]> ValidExpectedResults
        {
            get
            {
                yield return new object[] { new int[] { 1 }, new int[] { 1 }, new DateTime(2018, 1, 1), new DateTime(2018, 1, 2) };
                yield return new object[] { new int[] { 2 }, new int[] { 2 }, new DateTime(2018, 1, 1), new DateTime(2018, 1, 3) };
                yield return new object[] { new int[] { 1, 2 }, new int[] { 1, 2 }, new DateTime(2018, 1, 1), new DateTime(2018, 1, 3) };
                yield return new object[] { new int[] { 1 }, new int[] { 1 }, new DateTime(2018, 1, 5), new DateTime(2018, 1, 8) };
                yield return new object[] { new int[] { 1 }, new int[] { 1 }, new DateTime(2018, 1, 6), new DateTime(2018, 1, 9) };
                yield return new object[] { new int[] { 1 }, new int[] { 1 }, new DateTime(2018, 1, 7), new DateTime(2018, 1, 9) };
                yield return new object[] { new int[] { 3 }, new int[] { 6 }, new DateTime(2018, 1, 5), new DateTime(2018, 1, 15) };
                yield return new object[] { new int[] { 4, 8, 10 }, new int[] { 11, 6, 4 }, new DateTime(2018, 1, 5), new DateTime(2018, 1, 22) };
            }
        }

        [Theory]
        [MemberData(nameof(ValidExpectedResults))]
        public async Task ValidProduct_ReturnsCorrectDespatchDate(int[] productIds, int[] supplierLeadTimes, DateTime orderDate, DateTime despatchDate)
        {
            // Arrange
            getDespatchDateRepositoryMock.CheckIfProductExistsAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
            for (int i = 0; i < productIds.Length; i++) 
            {
                getDespatchDateRepositoryMock.GetSupplierLeadTimeByProductIdAsync(productIds[i]).Returns(Task.FromResult<int?>(supplierLeadTimes[i]));
            }

            // Act
            var result = await getDespatchDateQueryHandler.GetDespatchDate([..productIds], orderDate);

            // Assert
            result.Date.ShouldBe(despatchDate);
            foreach (var productId in productIds) 
            {
                await getDespatchDateRepositoryMock.Received(1).CheckIfProductExistsAsync(productId);
                await getDespatchDateRepositoryMock.Received(1).GetSupplierLeadTimeByProductIdAsync(productId);
            }
        }

        [Fact]
        public async Task InvalidProduct_ThrowsValidationException()
        {
            // Arrange
            var productId = 999;
            getDespatchDateRepositoryMock.CheckIfProductExistsAsync(Arg.Any<int>()).Returns(Task.FromResult(false));

            // Act
            var exception = await Should.ThrowAsync<ValidationException>(() => getDespatchDateQueryHandler.GetDespatchDate([productId], new DateTime(2018, 1, 1)));

            // Assert
            exception.Errors.Keys.ShouldContain("productId");
            exception.Errors.Values.Single().ShouldContain($"Product Id = {productId} not found");
            await getDespatchDateRepositoryMock.Received(1).CheckIfProductExistsAsync(productId);
            await getDespatchDateRepositoryMock.DidNotReceive().GetSupplierLeadTimeByProductIdAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task MissingSupplier_ThrowsValidationException()
        {
            // Arrange
            var productId = 8;
            getDespatchDateRepositoryMock.CheckIfProductExistsAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
            getDespatchDateRepositoryMock.GetSupplierLeadTimeByProductIdAsync(Arg.Any<int>()).Returns(Task.FromResult<int?>(null));

            // Act
            var exception = await Should.ThrowAsync<ValidationException>(() => getDespatchDateQueryHandler.GetDespatchDate([productId], new DateTime(2018, 1, 1)));

            // Assert
            exception.Errors.Keys.ShouldContain("productId");
            exception.Errors.Values.Single().ShouldContain($"Supplier for Product Id = {productId} not found");
            await getDespatchDateRepositoryMock.Received(1).CheckIfProductExistsAsync(productId);
            await getDespatchDateRepositoryMock.Received(1).GetSupplierLeadTimeByProductIdAsync(productId);
        }

        [Fact]
        public async Task NegativeSupplierLeadTime_ThrowsValidationException()
        {
            // Arrange
            var productId = 15;
            getDespatchDateRepositoryMock.CheckIfProductExistsAsync(Arg.Any<int>()).Returns(Task.FromResult(true));
            getDespatchDateRepositoryMock.GetSupplierLeadTimeByProductIdAsync(Arg.Any<int>()).Returns(Task.FromResult<int?>(-1));

            // Act
            var exception = await Should.ThrowAsync<ValidationException>(() => getDespatchDateQueryHandler.GetDespatchDate([productId], new DateTime(2018, 1, 1)));

            // Assert
            exception.Errors.Keys.ShouldContain("productId");
            exception.Errors.Values.Single().ShouldContain($"Supplier for Product Id = {productId} has an invalid supplier lead time");
            await getDespatchDateRepositoryMock.Received(1).CheckIfProductExistsAsync(productId);
            await getDespatchDateRepositoryMock.Received(1).GetSupplierLeadTimeByProductIdAsync(productId);
        }

        [Fact]
        public async Task NullProductIds_ThrowsValidationException()
        {
            // Arrange
            List<int> productIds = null;

            // Act
            var exception = await Should.ThrowAsync<ValidationException>(() => getDespatchDateQueryHandler.GetDespatchDate(productIds, new DateTime(2018, 1, 1)));

            // Assert
            exception.Errors.Keys.ShouldContain("productIds");
            exception.Errors.Values.Single().ShouldContain("One or more product Id is required");
            await getDespatchDateRepositoryMock.DidNotReceive().CheckIfProductExistsAsync(Arg.Any<int>());
            await getDespatchDateRepositoryMock.DidNotReceive().GetSupplierLeadTimeByProductIdAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task EmptyProductIds_ThrowsValidationException()
        {
            // Arrange
            var productIds = new List<int>();

            // Act
            var exception = await Should.ThrowAsync<ValidationException>(() => getDespatchDateQueryHandler.GetDespatchDate(productIds, new DateTime(2018, 1, 1)));

            // Assert
            exception.Errors.Keys.ShouldContain("productIds");
            exception.Errors.Values.Single().ShouldContain("One or more product Id is required");
            await getDespatchDateRepositoryMock.DidNotReceive().CheckIfProductExistsAsync(Arg.Any<int>());
            await getDespatchDateRepositoryMock.DidNotReceive().GetSupplierLeadTimeByProductIdAsync(Arg.Any<int>());
        }
    }
}
