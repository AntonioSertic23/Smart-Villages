using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartVillages.Shared.UserModels;

namespace SmartVillages.Shared.MessageModels
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public User PersonOne { get; set; }
        public User PersonTwo { get; set; }
        public string MessageContent { get; set; }
        public bool Seen { get; set; }
    }
}
