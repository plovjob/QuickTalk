using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTalk.Messages.Domain.Dto
{
    public class MessageDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public string TimeOfSend { get; set; }
    }
}
