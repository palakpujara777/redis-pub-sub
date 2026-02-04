using CallProcess.Application.Features.CallPrefixes.Commands;
using CallProcess.Application.Features.CallPrefixes.Queries;
using CallProcess.Domain.Entities.CallPrefix;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CallProcess.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallPrefixController : ControllerBase
    {
        #region Private Properties

        private readonly GetAllCallPrefixesHandler _getAllHandler;
        
        private readonly CheckCallPrefixExistsHandler _existsHandler;

        private readonly GetCallPrefixByCodeHandler _getByCodeHandler;
        
        private readonly AddOrUpdateCallPrefixHandler _addOrUpdateHandler;
        
        private readonly DeleteCallPrefixHandler _deleteHandler;

        private readonly ILogger<CallPrefixController> _logger;

        #endregion

        public CallPrefixController(
            GetAllCallPrefixesHandler getAllHandler,
            CheckCallPrefixExistsHandler existsHandler,
            GetCallPrefixByCodeHandler getByCodeHandler,
            AddOrUpdateCallPrefixHandler addOrUpdateHandler,
            DeleteCallPrefixHandler deleteHandler,
            ILogger<CallPrefixController> logger)
        {
            _getAllHandler = getAllHandler;
            _existsHandler = existsHandler;
            _getByCodeHandler = getByCodeHandler;
            _addOrUpdateHandler = addOrUpdateHandler;
            _deleteHandler = deleteHandler;
            _logger = logger;
        }

        #region Api Methods

        // GET: api/callprefix
        [HttpGet("GetAllCodeDetails")]
        public async Task<ActionResult<IEnumerable<CallPrefixDetails>>> GetAll()
        {
            var result = await _getAllHandler.Handle(new GetAllCallPrefixesQuery());
            return Ok(result);
        }

        // GET: api/callprefix/exists?code=91
        [HttpGet("ExistsByCode")]
        public async Task<ActionResult<bool>> ExistsByCode([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("ExistsByCode request rejected due to missing code.");
                return BadRequest("Code is required.");
            }

            var exists = await _existsHandler.Handle(new CheckCallPrefixExistsQuery(code));
            return Ok(exists);
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
