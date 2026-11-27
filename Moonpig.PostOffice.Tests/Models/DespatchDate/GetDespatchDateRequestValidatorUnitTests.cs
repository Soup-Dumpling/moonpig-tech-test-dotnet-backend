using Moonpig.PostOffice.Api.Models.DespatchDate;
using Shouldly;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Moonpig.PostOffice.Tests.Models.DespatchDate
{
    public class GetDespatchDateRequestValidatorUnitTests
    {
        [Fact]
        public void ValidQuery_PassesValidation()
        {
            // Arrange
            var request = new GetDespatchDateRequest
            {
                ProductIds = [1, 2],
                OrderDate = new DateTime(2018, 1, 1)
            };

            // Act & Assert
            Should.NotThrow(() => Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true));
        }

        [Fact]
        public void InvalidQuery_MissingProductIds_ThrowsValidationException()
        {
            // Arrange
            var request = new GetDespatchDateRequest
            {
                ProductIds = null,
                OrderDate = new DateTime(2018, 1, 1)
            };

            // Act
            var exception = Should.Throw<ValidationException>(() => Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true));

            // Assert
            exception.Message.ShouldContain("One or more product Id is required");
        }

        [Fact]
        public void InvalidQuery_EmptyProductIds_ThrowsValidationException()
        {
            // Arrange
            var request = new GetDespatchDateRequest
            {
                ProductIds = [],
                OrderDate = new DateTime(2018, 1, 1)
            };

            // Act
            var exception = Should.Throw<ValidationException>(() => Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true));

            // Assert
            exception.Message.ShouldContain("One or more product Id is required");
        }

        [Fact]
        public void InvalidQuery_MissingOrderDate_ThrowsValidationException()
        {
            // Arrange
            var request = new GetDespatchDateRequest
            {
                ProductIds = [1, 2],
                OrderDate = null
            };

            // Act
            var exception = Should.Throw<ValidationException>(() => Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true));

            // Assert
            exception.Message.ShouldContain("Order date is required");
        }
    }
}
