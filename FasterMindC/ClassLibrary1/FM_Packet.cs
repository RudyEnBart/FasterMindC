using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMNetworkLibrary
{
    public class FM_Packet
    {
        public string _id { get; set; }
        public string _type { get; set; }
        public string _message { get; set; }

        public FM_Packet(string id, string type, string message)
        {
            this._id = id;
            this._type = type;
            this._message = message;
        }

        public string toString()
        {
            return "ID: " + _id + " Type: " + _type + " \nMessage: " + _message; 
        }
    }
}
