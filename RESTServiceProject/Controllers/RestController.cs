using HelloWorldService;
using Microsoft.AspNetCore.Mvc;
using RESTServiceProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RESTServiceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestController : ControllerBase
    {
        private static int currentId = 101;

        private static List<UserObject> users = new List<UserObject>();


        // GET: api/<RestController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(users);
        }

        // GET api/<RestController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = users.FirstOrDefault(t => t.userID == id);

            if (user == null)
            {
                return NotFound(null);
            }
            else
            {
                return Ok(user);
            }
        }

        // POST api/<RestController>
        [HttpPost]
        public IActionResult Post([FromBody] UserObject user)
        {

            if (string.IsNullOrEmpty(user.userEmail))
            {
                var badRequest1 = new BadRequest
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Email",
                    FieldName = "userEmail",
                };
                return BadRequest(badRequest1);
            }

            if (string.IsNullOrEmpty(user.userPassword))
            {
                var badRequest2 = new BadRequest
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Password",
                    FieldName = "userPassword",
                };
                return BadRequest(badRequest2);
            }

            user.userID = currentId++;
            user.createdDate = DateTime.UtcNow;
            users.Add(user);

            return CreatedAtAction(nameof(Get), new { id = user.userID }, user);
        }

        // PUT api/<RestController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserObject value)
        {
            var user = users.FirstOrDefault(t => t.userID == id);

            if (string.IsNullOrEmpty(user.userEmail))
            {
                var badRequest1 = new BadRequest
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Email",
                    FieldName = "userEmail",
                };
                return BadRequest(badRequest1);
            }

            if (string.IsNullOrEmpty(user.userPassword))
            {
                var badRequest2 = new BadRequest
                {
                    DBCode = ErrorType.MissingField,
                    Message = "Null or Empty Password",
                    FieldName = "userPassword",
                };
                return BadRequest(badRequest2);
            }

            user.userEmail = value.userEmail;
            user.userPassword = value.userPassword;
            user.createdDate = DateTime.UtcNow;

            return Ok(user);
        }

        // DELETE api/<RestController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rowsDeleted = users.RemoveAll(t => t.userID == id);

            if (rowsDeleted == 0)
            {
                return NotFound(null);
            }
            else
            {
                return Ok();
            }
        }
    }
}
