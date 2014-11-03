using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMNetworkLibrary
{
    public class DataHandling
    {
        private static long DEFAULTTIME = 10 * 60 * 1000;
        private static string DEFAULTNAME = "Anonymous";
        public const byte MOSTWINS = 0;
        public const byte MOSTWINSNAME = 1;
        public const byte BESTTIMES = 2;
        public const byte BESTTIMESNAMES = 3;
        public const long DEFAULTMOSTWINS = 0;
        public const string DEFAULTMOSTWINSNAME = "Anonymous";
        public static long[] DEFAULTBESTTIMES = { DEFAULTTIME, DEFAULTTIME, DEFAULTTIME, DEFAULTTIME, DEFAULTTIME
                                                      , DEFAULTTIME, DEFAULTTIME, DEFAULTTIME, DEFAULTTIME, DEFAULTTIME};
        public static string[] DEFAULTBESTTIMESNAMES = { DEFAULTNAME, DEFAULTNAME, DEFAULTNAME, DEFAULTNAME, DEFAULTNAME 
                                                             , DEFAULTNAME, DEFAULTNAME, DEFAULTNAME, DEFAULTNAME, DEFAULTNAME};
        private const string _dir = @"Highscores\";
        private const string _fileExtension = ".dat";
        private const string _fileName = "Highscores";
        public static void SaveData(long score, string name)
        {
            string[] data = File.ReadAllLines(Path.Combine(_dir, _fileName + _fileExtension));
            data[0] = "" + score;
            data[1] = name;
            File.WriteAllLines(Path.Combine(_dir, _fileName + _fileExtension), data);
        }

        public static void SaveData(long[] scores, string[] names)
        {
            string[] data = File.ReadAllLines(Path.Combine(_dir, _fileName + _fileExtension));
            for (int i = 0; i < scores.Length; i++ )
            {
                data[2 + i] = "" + scores[i];
                data[12 + i] = names[i];
            }
            File.WriteAllLines(Path.Combine(_dir, _fileName + _fileExtension), data);
        }

        public static object ReadData(byte type)
        {
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
            if (!File.Exists(Path.Combine(_dir, _fileName + _fileExtension)))
            {
                File.Create(Path.Combine(_dir, _fileName + _fileExtension)).Close();
                //TODO write standard highscores to file
                using (StreamWriter sw = File.AppendText(Path.Combine(_dir, _fileName + _fileExtension)))
                {
                    sw.WriteLine(DEFAULTMOSTWINS);
                    sw.WriteLine(DEFAULTMOSTWINSNAME);
                    for (int i = 0; i < DEFAULTBESTTIMES.Length; i++)
                    {
                        sw.WriteLine(DEFAULTBESTTIMES[i]);
                    } for (int i = 0; i < DEFAULTBESTTIMESNAMES.Length; i++)
                    {
                        sw.WriteLine(DEFAULTBESTTIMESNAMES[i]);
                    }
                }
            }
            string[] data = File.ReadAllLines(Path.Combine(_dir, _fileName + _fileExtension));
            switch (type)
            {
                case MOSTWINS:
                    return Int16.Parse(data[0]);
                case MOSTWINSNAME:
                    //TODO
                    return data[1];
                case BESTTIMES:
                    //TODO
                    return StringArrayToLongArray(SubArray(data, 2, 10));
                case BESTTIMESNAMES:
                    //TODO
                    return SubArray(data, 12, 10);
                default:
                    return null;
            }
        }
        private static string[] SubArray(string[] data, int index, int length)
        {
            string[] result = new string[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        private static long[] StringArrayToLongArray(string[] data)
        {
            long[] result = new long[data.Length];
            byte index = 0;
            foreach(string s in data)
            {
                result[index] = long.Parse(s);
                index++;
            }
            return result;
        }
    }
}
