namespace Moonpig.PostOffice.Api.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Moonpig.PostOffice.Api.Models.DespatchDate;
    using Moonpig.PostOffice.Core.Models.DTOs;
    using Moonpig.PostOffice.Core.UseCases.DespatchDate.GetDespatchDate;

    [ApiController]
    [Route("api/[controller]")]
    public class DespatchDateController : ControllerBase
    {
        private readonly IGetDespatchDateQueryHandler getDespatchDateQueryHandler;

        public DespatchDateController(IGetDespatchDateQueryHandler getDespatchDateQueryHandler)
        {
            this.getDespatchDateQueryHandler = getDespatchDateQueryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<DespatchDate>> Get([FromQuery] GetDespatchDateRequest request)
        {
            return Ok(await getDespatchDateQueryHandler.GetDespatchDate(request.ProductIds, request.OrderDate.Value));
        }
    }
}
