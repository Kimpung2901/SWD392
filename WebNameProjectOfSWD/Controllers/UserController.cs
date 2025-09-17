using BLL.Services.User;
using DAL.DTO.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly UserService _userService;
      
        public UserController(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("get_all_user")]
        public async Task<ActionResult<List<GetUserRespone>>> GetAllUser()
        {
            return await _userService.getAllUser();
        }

        [HttpGet("get_user_by_id/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.getUserById(id);
                if (user == null)
                {
                    return NotFound("No vaccine found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("update_user/{id}")]
        public async Task<IActionResult> updateUser(int id, [FromBody] UpdateUser rq)
        {
            try
            {
                var result = await _userService.updateUser(id, rq);
                return Ok(result); 
            }
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex) { 
                return NotFound(ex.Message); 
            }
            catch (Exception ex) { 
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("add_ user")]
        public async Task<IActionResult> AddUser([FromBody] AddUser rq)
        {
            try
            {
                var rs = await _userService.AddUser(rq);
                return Ok("Create staff successful");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPatch("soft_delete_user/{id}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                var rs = await _userService.SoftDeleteUser(id);
                return Ok("Delete successfully");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }




    }
    
}
