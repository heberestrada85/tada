using System.Threading.Tasks;
using Tada.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Tada.Application.Commands;

namespace Tada.Controllers
{
    public class InvitationsController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetInvitationsQuery());

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetInvitationsByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]  InvitationsModel model)
        {
            var result = await Mediator.Send(new CreateInvitationsCommand(model));
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteInvitationsCommand(id));
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
