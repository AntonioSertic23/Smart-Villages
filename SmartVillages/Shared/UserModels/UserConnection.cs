using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.UserModels
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public bool IsActive { get; set; }
    }
}
