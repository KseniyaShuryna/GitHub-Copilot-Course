using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Interfaces;
using ToDo.Application.DTOs;
using ToDo.Api.Models;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/toDo")]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        /// <summary>
        /// Gets all ToDo items for the authenticated user.
        /// </summary>
        /// <returns>A list of ToDo items.</returns>
        /// <response code="200">Returns the list of ToDo items.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ToDoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ToDoDto>>> GetAll()
        {
            var todos = await _toDoService.GetToDosByUserIdAsync();
            return Ok(todos);
        }

        /// <summary>
        /// Gets a specific ToDo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <returns>The ToDo item.</returns>
        /// <response code="200">Returns the ToDo item.</response>
        /// <response code="404">If the ToDo item is not found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ToDoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ToDoDto>> GetById([FromRoute] int id)
        {
            var todo = await _toDoService.GetToDoByIdAsync(id);
            return Ok(todo);
        }

        /// <summary>        
        /// Adds a new ToDo item for the authenticated user.
        /// </summary>
        /// <param name="toDoDto">The ToDo item to add.</param>
        /// <returns>The created ToDo item.</returns>
        /// <response code="201">Returns the created ToDo item.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ToDoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ToDoDto>> Add([FromBody] ToDoDto toDoDto)
        {
            var created = await _toDoService.AddToDoAsync(toDoDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Toggles the completion status of a ToDo item.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <returns>The updated ToDo item.</returns>
        /// <response code="200">Returns the updated ToDo item.</response>
        /// <response code="404">If the ToDo item is not found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPut("{id:int}/toggle")]
        [ProducesResponseType(typeof(ToDoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ToDoDto>> Toggle(int id)
        {
            var updated = await _toDoService.ToggleToDoAsync(id);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a ToDo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <response code="204">If the ToDo item was deleted.</response>
        /// <response code="404">If the ToDo item is not found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            await _toDoService.DeleteToDoAsync(id);
            return NoContent();
        }
    }
}
