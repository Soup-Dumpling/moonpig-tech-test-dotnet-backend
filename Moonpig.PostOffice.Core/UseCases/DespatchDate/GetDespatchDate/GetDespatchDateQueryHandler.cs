using Moonpig.PostOffice.Core.Exceptions;
using Moonpig.PostOffice.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonpig.PostOffice.Core.UseCases.DespatchDate.GetDespatchDate
{
    public class GetDespatchDateQueryHandler : IGetDespatchDateQueryHandler
    {
        private readonly IGetDespatchDateRepository getDespatchDateRepository;

        public GetDespatchDateQueryHandler(IGetDespatchDateRepository getDespatchDateRepository)
        {
            this.getDespatchDateRepository = getDespatchDateRepository;
        }

        private static DateTime AddWorkingDays(DateTime date, int workingDays)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) 
            {
                date = date.AddDays(2);
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday) 
            {
                date = date.AddDays(1);
            }

            while (workingDays > 0)
            {
                date = date.AddDays(1);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays--;
                }
            }

            return date;
        }

        public async Task<Models.DTOs.DespatchDate> GetDespatchDate(List<int> productIds, DateTime orderDate)
        {
            var maxLeadTime = orderDate;
            var errors = new Dictionary<string, string[]>();

            if (productIds == null || productIds.Count == 0)
            {
                errors.Add("productIds", ["One or more product Id is required"]);
                throw new ValidationException(errors);
            }

            foreach (var productId in productIds)
            {
                var productExists = await getDespatchDateRepository.CheckIfProductExistsAsync(productId);
                if (!productExists)
                {
                    errors.Add("productId", [$"Product Id = {productId} not found"]);
                    throw new ValidationException(errors);
                }

                var supplierLeadTime = await getDespatchDateRepository.GetSupplierLeadTimeByProductIdAsync(productId);
                if (!supplierLeadTime.HasValue)
                {
                    errors.Add("productId", [$"Supplier for Product Id = {productId} not found"]);
                    throw new ValidationException(errors);
                }
                if (supplierLeadTime.Value < 0)
                {
                    errors.Add("productId", [$"Supplier for Product Id = {productId} has an invalid supplier lead time"]);
                    throw new ValidationException(errors);
                }
                var newLeadTime = AddWorkingDays(orderDate, supplierLeadTime.Value);
                if (newLeadTime > maxLeadTime)
                {
                    maxLeadTime = newLeadTime;
                }
            }
            return new Models.DTOs.DespatchDate { Date = maxLeadTime };
        }
    }
}
