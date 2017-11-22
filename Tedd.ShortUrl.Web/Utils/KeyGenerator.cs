using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tedd.ShortUrl.Web.Utils
{
    public static class KeyGenerator
    {
        private const string ValidChars = "abcdefghjkmnprstwxz2345789"; // Letters and numbers that are not easily mixed with others when reading
        //private const string ValidChars = "ABCDEFHJKLMNPRSTUWXYZ2345789";
        private static readonly Dictionary<long, bool> ValidCharLookup = new Dictionary<long, bool>();
        private static readonly Tedd.MoreRandom.Random Rnd = new Tedd.MoreRandom.Random();

        static KeyGenerator()
        {
            // Set up a quick lookup dictionary for all valid characters
            foreach (var c in ValidChars.ToUpperInvariant())
                ValidCharLookup.Add((long)c, true);
        }

        public static string Generate(int length)
        {
            var ret = new char[length];
            for (var i = 0; i < length; i++)
            {
                int c;
                lock (Rnd)
                    c = Rnd.Next(0, ValidChars.Length);
                ret[i] = ValidChars[c];
            }
            return new string(ret);
        }

        public static bool Validate(int maxLength, string key)
        {
            if (key.Length > maxLength)
                return false;

            foreach (var c in key.ToUpperInvariant())
            {
                if (!ValidCharLookup.ContainsKey((long)c))
                    return false;
            }
            return true;
        }
    }
}
