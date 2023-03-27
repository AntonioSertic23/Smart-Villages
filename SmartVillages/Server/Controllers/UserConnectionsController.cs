using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartVillages.Server.Data;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserConnectionsController : ControllerBase
    {
        private readonly DataContext _context;

        public UserConnectionsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/UserConnections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserConnection>>> GetUserConnection()
        {
            return await _context.UserConnection.ToListAsync();
        }

        // GET: api/UserConnections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserConnection>> GetUserConnection(int id)
        {
            var userConnection = await _context.UserConnection.FindAsync(id);
            if (userConnection == null)
            {
                return NotFound();
            }
            return userConnection;
        }

        [HttpPost("GetByConnectionId")]
        public async Task<UserConnection> GetByConnectionId([FromBody] string connectionId)
        {
            var userConnection = _context.UserConnection.Where(c => c.ConnectionId == connectionId).FirstOrDefault();
            return userConnection;
        }

        [HttpPost("GetByUserId")]
        public async Task<UserConnection> GetByUserId([FromBody] string userId)
        {
            var userConnection = await _context.UserConnection.Where(c => c.UserId == userId && c.IsActive == true).OrderBy(o => o.Id).LastOrDefaultAsync();
            return userConnection;
        }

        [HttpPost("PostUserConnection")]
        public async Task<ActionResult<UserConnection>> PostUserConnection(UserConnection userConnection)
        {
            var allActiveConnection = await _context.UserConnection.Where(c => c.UserId == userConnection.UserId.ToString() && c.IsActive == true).ToListAsync();
            if(allActiveConnection.Count > 0)
            {
                foreach (var con in allActiveConnection)
                {
                    con.IsActive = false;
                    _context.Entry(con).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }

            _context.UserConnection.Add(userConnection);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUserConnection", new { id = userConnection.Id }, userConnection);
        }

        [HttpPut("PutUserConnection")]
        public async Task<IActionResult> PutUserConnection([FromBody] int userId)
        {
            var connection = _context.UserConnection.Where(c => c.UserId == userId.ToString() && c.IsActive == true).FirstOrDefault();
            connection.IsActive = false;
            _context.Entry(connection).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
