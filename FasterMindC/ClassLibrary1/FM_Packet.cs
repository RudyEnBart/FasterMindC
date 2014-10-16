namespace FMNetworkLibrary
{
    public class FM_Packet
    {
        public FM_Packet(string type, string message)
        {
            _type = type;
            _message = message;
        }

        public FM_Packet(string id, string type, string message)
        {
            _id = id;
            _type = type;
            _message = message;
        }

        public string _id { get; set; }
        public string _type { get; set; }
        public string _message { get; set; }

        public string toString()
        {
            return "ID: " + _id + " Type: " + _type + " \nMessage: " + _message;
        }
    }
}