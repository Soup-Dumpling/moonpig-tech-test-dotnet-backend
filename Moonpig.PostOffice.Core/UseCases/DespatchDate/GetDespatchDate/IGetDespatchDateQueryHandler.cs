using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonpig.PostOffice.Core.UseCases.DespatchDate.GetDespatchDate
{
    public interface IGetDespatchDateQueryHandler
    {
        Task<Models.DTOs.DespatchDate> GetDespatchDate(List<int> productIds, DateTime orderDate);
    }
}
