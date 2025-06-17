using CallProcess.Application.Features.CallPrefixes.Commands;
using CallProcess.Application.Features.CallPrefixes.Queries;
using CallProcess.Domain.Entities.CallPrefix;
using Microsoft.AspNetCore.Mvc;

namespace CallProcess.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallPrefixController : ControllerBase
    {
        #region Private Properties

        private readonly GetAllCallPrefixesHandler _getAllHandler;
        
        private readonly GetCallPrefixByCodeHandler _getByCodeHandler;
        
        private readonly AddOrUpdateCallPrefixHandler _addOrUpdateHandler;
        
        private readonly DeleteCallPrefixHandler _deleteHandler;

        #endregion

        public CallPrefixController(
            GetAllCallPrefixesHandler getAllHandler,
            GetCallPrefixByCodeHandler getByCodeHandler,
            AddOrUpdateCallPrefixHandler addOrUpdateHandler,
            DeleteCallPrefixHandler deleteHandler)
        {
            _getAllHandler = getAllHandler;
            _getByCodeHandler = getByCodeHandler;
            _addOrUpdateHandler = addOrUpdateHandler;
            _deleteHandler = deleteHandler;
        }

        #region Api Methods

        // GET: api/callprefix
        [HttpGet("GetAllCodeDetails")]
        public async Task<ActionResult<IEnumerable<CallPrefixDetails>>> GetAll()
        {
            var result = await _getAllHandler.Handle(new GetAllCallPrefixesQuery());
            return Ok(result);
        }

        // GET: api/callprefix/{prefix}
        [HttpGet("GetCountryByCode")]
        public async Task<ActionResult<CallPrefixDetails>> GetByPrefix(string prefix)
        {
            var result = await _getByCodeHandler.Handle(new GetCallPrefixByCodeQuery(prefix));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/callprefix
        [HttpPost("AddCountryDetails")]
        public async Task<IActionResult> AddOrUpdate([FromBody] AddOrUpdateCallPrefixCommand command)
        {
            await _addOrUpdateHandler.Handle(command);
            return Ok();
        }

        // DELETE: api/callprefix/{prefix}
        [HttpDelete("DeleteCountryDetailsByCode")]
        public async Task<IActionResult> Delete(string prefix)
        {
            var success = await _deleteHandler.Handle(new DeleteCallPrefixCommand(prefix));
            if (!success)
                return NotFound();

            return NoContent();
        }

        #endregion
    }
}
