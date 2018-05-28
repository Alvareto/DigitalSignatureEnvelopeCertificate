using System;
using System.Text;

namespace DigitalSignature.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Converts BYTE[] to HEX STRING
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToHex(this byte[] input)
        {
            StringBuilder hex = new StringBuilder(input.Length * 2);

            foreach (byte b in input)
            {
                if (b < 0x10)
                    hex.Append("0" + Convert.ToString(b, 16));
                else
                    hex.Append(Convert.ToString(b, 16));
            }
            return hex.ToString();
        }

        /// <summary>
        /// Converts HEX to BYTE[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this string input)
        {
            byte[] text = new byte[input.Length / 2];

            for (int i = 0; i < (input.Length / 2); i++)
            {
                string word = input.Substring(i * 2, 2);
                text[i] = (byte)Convert.ToInt16(word, 16);
            }
            return text;
        }
    }
}
