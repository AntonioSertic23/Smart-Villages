using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartVillages.Server.Data;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserImagesController : ControllerBase
    {
        private readonly DataContext _context;

        public UserImagesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/UserImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserImage>>> GetUserImage()
        {
            return await _context.UserImages.ToListAsync();
        }

        // GET: api/UserImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserImage>> GetUserImage(int id)
        {
            var userImage = await _context.UserImages.FindAsync(id);

            if (userImage == null)
            {
                return NotFound();
            }

            return userImage;
        }

        // PUT: api/UserImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserImage(int id, UserImage userImage)
        {
            if (id != userImage.Id)
            {
                return BadRequest();
            }

            _context.Entry(userImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserImageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserImage>> PostUserImage(UserImage userImage)
        {
            _context.UserImages.Add(userImage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserImage", new { id = userImage.Id }, userImage);
        }

        // DELETE: api/UserImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserImage(int id)
        {
            var userImage = await _context.UserImages.FindAsync(id);
            if (userImage == null)
            {
                return NotFound();
            }

            _context.UserImages.Remove(userImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserImageExists(int id)
        {
            return _context.UserImages.Any(e => e.Id == id);
        }
    }
}
