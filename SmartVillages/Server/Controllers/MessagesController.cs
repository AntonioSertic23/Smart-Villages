using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SmartVillages.Server.Data;
using SmartVillages.Shared;
using SmartVillages.Shared.MessageModels;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHubContext<ConnectionHub> _hubContext;

        public MessagesController(DataContext context, IHubContext<ConnectionHub> connectionHub)
        {
            _context = context;
            _hubContext = connectionHub;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessage()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("postmessage/{personone}/{persontwo}")]
        public async Task<ActionResult<Message>> PostMessage(Message message, int personone, int persontwo)
        {
            message.PersonOne = await _context.Users.SingleOrDefaultAsync(t => t.Id == personone);
            message.PersonTwo = await _context.Users.SingleOrDefaultAsync(t => t.Id == persontwo);
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            SendMessageToUser(persontwo);

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }

        [HttpGet("getmessagesbyuser/{personone}/{persontwo}")]
        public async Task<ActionResult<List<Message>>> GetMessagesByUser(int personone, int persontwo)
        {
            var First = await _context.Users.SingleOrDefaultAsync(t => t.Id == personone);
            var Second = await _context.Users.SingleOrDefaultAsync(t => t.Id == persontwo);
            var Messages = await _context.Messages.Where(u => (u.PersonOne == First && u.PersonTwo == Second) || (u.PersonTwo == First && u.PersonOne == Second)).OrderBy(d => d.Date).ToListAsync();

            if (User == null)
            {
                return NotFound();
            }

            return Messages;
        }

        [HttpGet("getalllastmessages/{user}")]
        public async Task<ActionResult<List<LastMessage>>> GetAllLastMessages(int user)
        {
            /* moguć error ako korisnik nema ni jedne poruke.. */
            List<LastMessage> LastMessages = new List<LastMessage>();
            List<User> AllUsers = new List<User>();
            var User = await _context.Users.SingleOrDefaultAsync(t => t.Id == user);
            var AllUsersOne = await _context.Messages.Where(u => u.PersonOne == User).Include(i => i.PersonOne.UserImage).Include(i => i.PersonTwo.UserImage).Select(n => n.PersonTwo).Distinct().ToListAsync();
            var AllUsersTwo = await _context.Messages.Where(u => u.PersonTwo == User).Include(i => i.PersonOne.UserImage).Include(i => i.PersonTwo.UserImage).Select(n => n.PersonOne).Distinct().ToListAsync();

            foreach (var item in AllUsersOne)
                AllUsers.Add(item);
            foreach (var item in AllUsersTwo)
                AllUsers.Add(item);
            AllUsers = AllUsers.DistinctBy(x => x.Id).ToList();

            foreach (var singleUser in AllUsers)
            {
                // dohvatit da li je aktivan..
                var activeOrNot = _context.UserConnections.Where(c => c.UserId == singleUser.Id.ToString() && c.IsActive == true).OrderBy(o => o.Id).LastOrDefault();
                bool isActive = activeOrNot != null ? true : false;

                var lastMessage = _context.Messages.Where(u => (u.PersonOne == User && u.PersonTwo == singleUser) || (u.PersonOne == singleUser && u.PersonTwo == User)).OrderBy(o => o.Id).Last();
                var numOfUnread = _context.Messages.Where(u => (u.PersonOne == singleUser && u.PersonTwo == User) && u.Seen == false).Count();
                bool isLastSeen = false;
                lastMessage.MessageContent = lastMessage.MessageContent.Count() > 80 ? lastMessage.MessageContent.Substring(0, 37) + "..." : lastMessage.MessageContent;
                if (lastMessage.PersonOne == User) 
                {
                    isLastSeen = true;
                }
                else if(lastMessage.PersonTwo == User && lastMessage.Seen)
                {
                    isLastSeen = true;
                }
                LastMessages.Add(new LastMessage { 
                    MessageID = lastMessage.Id,
                    User = singleUser,
                    MessageContent = lastMessage.MessageContent,
                    LastIsSeen = isLastSeen,
                    UnreadMessages = numOfUnread,
                    Date = lastMessage.Date,
                    IsUserActive = isActive
                });
            }
            List<LastMessage> LastMessagesOrdered = LastMessages.OrderBy(o => o.Date, OrderByDirection.Descending).ToList();
            return LastMessagesOrdered;
        }

        [HttpPost("setasseen")]
        public async Task<IActionResult> SetAsSeen(LastMessage LastMessage)
        {
            // nade tu zadnju poruku i preko nje sve druge
            var one = _context.Messages.Where(m => m.Id == LastMessage.MessageID).Select(s => s.PersonOne);
            var two = _context.Messages.Where(m => m.Id == LastMessage.MessageID).Select(s => s.PersonTwo);
            var messages = _context.Messages.Where(u => (u.PersonOne == one.FirstOrDefault() && u.PersonTwo == two.FirstOrDefault()) && u.Seen == false).ToList();
            foreach (var m in messages)
            {
                m.Seen = true;
                _context.Entry(m).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        protected async Task<IActionResult> SendMessageToUser(int userId)
        {
            var connectionId = _context.UserConnections.Where(u => u.UserId == userId.ToString() && u.IsActive == true).Select(s => s.ConnectionId).FirstOrDefault();
            await _hubContext.Clients.Client(connectionId).SendAsync("new_message");
            return Ok("message sent successfully");
        }
    }
}
