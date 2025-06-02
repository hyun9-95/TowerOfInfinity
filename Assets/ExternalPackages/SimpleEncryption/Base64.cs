using System;
using System.Text;

namespace Assets.SimpleEncryption
{
    /// <summary>
    /// Base64 helper.
    /// </summary>
	public static class Base64
    {
        /// <summary>
        /// Encode bytes to Base64.
        /// </summary>
        public static string Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decode encoded bytes.
        /// </summary>
        public static byte[] Decode(string base64EncodedString)
        {
            return Convert.FromBase64String(base64EncodedString);
        }

        /// <summary>
        /// Encode plain string to Base64.
        /// </summary>
        public static string Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Decode encoded Base64-string.
        /// </summary>
        public static string DecodeText(string base64EncodedString)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedString));
        }
    }
}