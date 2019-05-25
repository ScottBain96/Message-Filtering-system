using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messageSystem
{
    class Message
    {
        public string header { get; set; }
        public string body { get; set; }
        public string type  { get; set; }
        public string subject { get; set; }
        public string quarantine { get; set; }

        
    }
}
