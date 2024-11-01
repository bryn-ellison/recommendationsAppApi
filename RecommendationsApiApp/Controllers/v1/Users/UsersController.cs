using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RecommendationsAppApiLibrary.DataAccess;
using RecommendationsAppApiLibrary.Models;


namespace RecommendationsApi.Controllers.v1.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserData _data;

        public UsersController(IUserData data)
        {
            _data = data;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<List<UserModel>>> GetAllUsers()
        {
            try
            {
                List<UserModel> output = await _data.LoadUsers();
                return Ok(output);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
