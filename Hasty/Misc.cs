using System; 

namespace Hasty {
    class Misc {
        public static int UnixTime {
            get {
                var time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                return time;
            }
        }

        public static string UnixToString(int time = -1) {
            if (time == -1)
                time = UnixTime;

            var dateTime = new DateTime(1970, 1, 1);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            return dateTime.ToString("g");
        }


        public static bool ByteCompare(byte[] a1, byte[] b1) { 
            if (a1.Length != b1.Length) return false;
            var i = 0;
            while (i < a1.Length && (a1[i] == b1[i])) //Earlier it was a1[i]!=b1[i]
            {
                i++;
            }
            return i == a1.Length;
        }
    }
}
