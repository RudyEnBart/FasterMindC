using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FMNetworkLibrary
{
    public class ENUMS
    {
        public enum color : byte
        {
            [ByteValue(1)]
            RED,
            [ByteValue(2)]
            BLUE,
            [ByteValue(3)]
            GREEN,
            [ByteValue(4)]
            YELLOW,
            [ByteValue(5)]
            PINK,
            [ByteValue(6)]
            CYAN
        }
        public static byte GetByteValue(color c)
        {
            byte result = 0;
            result = GetByteValue(c);
            return result;
        }

        public class ByteValue : System.Attribute
        {
            private byte _value;

            public ByteValue(byte value)
            {
                _value = value;
            }

            public byte Value
            {
                get { return _value; }
            }

        }
    }
}
