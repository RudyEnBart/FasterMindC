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
        public static Color GetColor(color c)
        {
            Color result;
            switch ((byte)c)
            {
                case 1:
                    result = Color.Red;
                    break;
                case 2:
                    result = Color.Blue;
                    break;
                case 3:
                    result = Color.Green;
                    break;
                case 4:
                    result = Color.Yellow;
                    break;
                case 5:
                    result = Color.Pink;
                    break;
                case 6:
                    result = Color.Cyan;
                    break;
                default:
                    result = Color.Black;
                    break;
            }
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
