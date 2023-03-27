using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartVillages.Server.Data;
using SmartVillages.Shared;
using MimeKit;
using MailKit.Net.Smtp;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly Random _random = new Random();

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<User>>> GetUser(int id)
        {
            //var user = await _context.User.Include(i => i.UserType).FindAsync(id);
            var user = _context.Users.Where(t => t.Id == id).Include(i => i.UserType).Include(i => i.UserImage).Include(i => i.Place).ToList();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpPost("updateuserimage")]
        public async Task<IActionResult> UpdateUserImage(UserImage userImage)
        {
            _context.Entry(userImage).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("putimage")]
        public async Task<ActionResult<User>> PutImage(User user)
        {
            var Image = _context.UserImages.Where(i => i.Id == user.UserImage.Id).FirstOrDefault();
            user.UserImage = Image;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", user);
        }
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("postuser/{id}")]
        public async Task<ActionResult<User>> PostUser(int id, User user)
        {
            user.Place = _context.Places.Where(p => p.Id == user.Place.Id).FirstOrDefault();
            user.EmailConfirmationCode = GenerateCode();
            user.UserType = await _context.UserTypes.SingleOrDefaultAsync(t => t.UserTypeId == id);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", user);
        }

        [HttpPost("login/{id}")]
        public async Task<ActionResult<User>> Login(int id, UserSignIn user)
        {
            var type = await _context.UserTypes.SingleOrDefaultAsync(t => t.UserTypeId == id);
            var User = await _context.Users.Include(i => i.Place).Include(j => j.UserType).Include(k => k.UserImage).SingleOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password && u.SecretCode == user.SecretCode && u.UserType == type);

            if (User == null)
            {
                return NotFound();
            }

            return User;
        }

        // LOGOUT
        // await _localStorageService.RemoveItem("user");

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpPost("SendEmail")]
        public async Task SendEmail(User user, bool issecret = false)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Smart Villages", "elliotalderson050@gmail.com"));
            message.To.Add(new MailboxAddress(user.FirstName + " " + user.LastName, user.Email));
            message.Subject = issecret ? "Secret code" : "Email confirmation";

            var bodyBuilder = new BodyBuilder();
            var code = issecret ? user.SecretCode : user.EmailConfirmationCode;
            if(issecret)
            {
                bodyBuilder.HtmlBody = "<div style='text-align: center; height: 200px;'><h1>Welcome to Smart Villages <span style='color: lightseagreen;'>" + user.FirstName + "</span></h1><h3 style='margin-bottom: 0; margin-top: 20px'>Your secret code for login is below:</h3><br><p style='font-size: 50px;font-weight: bold;margin-top: 0;'>" + user.SecretCode + "</p></div>";
            }
            else
            {
                bodyBuilder.HtmlBody = "<div style='text-align: center;height: 150px;'><h1>Welcome to Smart Villages <span style='color: lightseagreen;'>" + user.FirstName + "</span></h1><h3>Please click on Confirm to confirm your email!</h3><br><a style='color: white; background-color: #31B58E;padding: 10px 20px;border-radius: 5px;font-size: 17px;text-decoration: none;' href='https://localhost:5001/emailconfirmation/" + code + "/" + user.OIB + "'>Confirm</a></div>";
            }

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("elliotalderson050@gmail.com", "EvilCorp");
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            /*
            {
                "Id": 13,
                "FirstName": "Antonio",
                "LastName":	"Sertić",
                "Email": "a@a",
                "Bio": "NULL",
                "Sex": "NULL",
                "OIB": "45789658965",
                "EmailConfirm": false,
                "Locked": true,
                "Country": "Cro",
                "City": "Na",
                "Address": "NULL",
                "Number": "4789595562",
                "SecretCode": "NULL",
                "BirthDate": "2020-02-07T00:00:00.0000000",
                "UserTypeId": 2,
                "DateCreated": "2021-05-05T13:51:58.3440000",
                "Password": "12345678",
                "TermsAndConditions": true,
                "EmailConfirmationCode": "NULL"
            }
             */
        }

        [HttpPost("ConfirmEmail/{oib}")]
        public async Task<ActionResult> ConfirmEmail(string oib, [FromBody] string code)
        {
            var User = await _context.Users.SingleOrDefaultAsync(u => u.EmailConfirmationCode == code && u.OIB == oib);

            if (User == null)
            {
                return NotFound();
            }
            else
            {
                User.EmailConfirm = true;
                User.Locked = false;
                User.SecretCode = GenerateCode();
                _context.Entry(User).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Opet poslati email a pravim SecretCod-om
                await SendEmail(User, true);

                return CreatedAtAction("ConfirmEmail", User.SecretCode);
            }
        }

        protected string GenerateCode()
        {
            string SecretCode = "";
            List<string> alphabet = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            for (var i = 0; i < 3; i++)
            {
                int num = RandomNumber(0, 9);
                int randchar = RandomNumber(0, 26);
                SecretCode += alphabet[randchar] + num;
            }

            return SecretCode;
        }

        protected int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }


        [HttpGet("getuserstat/{id}")]
        public async Task<ActionResult<UserProfileStat>> GetUserStat(int id)
        {
            var tempyear = _context.Users.Where(u => u.Id == id).Select(s => s.DateCreated).FirstOrDefault();
            var year = tempyear.Year;

            var temprates = _context.ProductRate.Where(u => u.Product.User.Id == id).Select(s => s.Rate).Sum();
            float rates = 0;
            if (temprates > 0)
            {
                var tmp = _context.ProductRate.Where(u => u.Product.User.Id == id).Count();
                rates = (float)temprates / (float)tmp;
            }

            var orders = _context.CartItems.Where(u => u.Product.User.Id == id).Count();

            var products = _context.Products.Where(u => u.User.Id == id).Count();

            UserProfileStat userProfileStat = new UserProfileStat { Orders = orders, Products = products, Rate = rates, Year = year };

            return userProfileStat;
        }
    }
}
