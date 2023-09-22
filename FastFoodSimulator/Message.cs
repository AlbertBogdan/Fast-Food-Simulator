using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary
{
    public class Message
    {
        public string Name { get; set; }
        public string Msg { get; set; }
        public Message(string name, string msg) 
        {
            this.Name = name;
            this.Msg = msg;
        }
    }
}
