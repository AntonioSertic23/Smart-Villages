using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.MessageModels
{
    public class LastMessage
    {
        public int MessageID { get; set; }
        public User User { get; set; }
        public string MessageContent { get; set; }
        public bool LastIsSeen { get; set; }
        public int UnreadMessages { get; set; }
        public DateTime Date { get; set; }
        public bool IsUserActive { get; set; }
    }
}
