using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.Mutual
{
    public class EmailData
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public string Towho { get; set; }

        public bool HtmlContent { get; set; }

        public List<string> ClientList { get; set; }

        public EmailData()
        {
            ClientList = new List<string>();
        }
    }
}
