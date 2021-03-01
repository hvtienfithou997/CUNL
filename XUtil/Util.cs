using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XMedia
{
    public class XUtil
    {
        private const string keyDefault = "c990d20480c64ef085782210eb678df3";

        public static class ConfigurationManager
        {
            public static IConfiguration AppSetting { get; }

            static ConfigurationManager()
            {
                AppSetting = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();
            }
        }

        public static long TimeInEpoch(DateTime? dt = null)
        {
            if (dt.HasValue)
                return (long)(dt.Value.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static long TimeInEpoch(DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTimeOffset EpochToTime(long ep)
        {
            return DateTimeOffset.FromUnixTimeSeconds(ep).ToLocalTime();
        }

        public static string EpochToTimeStringShortFomart(long ep)
        {
            return EpochToTime(ep).ToString("dd/MM/yyyy HH:mm");
        }

        public static string EpochToTimeString(long ep, string format = "dd/MM/yyyy HH:mm:ss")
        {
            return EpochToTime(ep).ToString(format);
        }

        private static byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }

        public static string Decrypt(string input, byte[] hash)
        {
            byte[] data = String_To_Bytes(input);

            TripleDESCryptoServiceProvider provider;
            provider = new TripleDESCryptoServiceProvider();

            provider.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            provider.Key = hash;

            ICryptoTransform transform = provider.CreateDecryptor();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms,
                transform, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                //return Encoding.UTF8.GetString(ms.ToArray());
                return Encoding.ASCII.GetString(ms.ToArray());
            }
        }

        public static string Decrypt(string input, string hex)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            if (string.IsNullOrEmpty(hex))
            {
                hex = keyDefault;
            }

            byte[] hash = String_To_Bytes(hex);
            byte[] data = String_To_Bytes(input);

            TripleDESCryptoServiceProvider provider;
            provider = new TripleDESCryptoServiceProvider();

            provider.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            provider.Key = hash;

            ICryptoTransform transform = provider.CreateDecryptor();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms,
                transform, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                //return Encoding.UTF8.GetString(ms.ToArray());
                return Encoding.ASCII.GetString(ms.ToArray());
            }
        }

        public static string Encrypt(string text, string md5Hex)
        {
            if (string.IsNullOrEmpty(md5Hex))
            {
                md5Hex = keyDefault;
            }
            byte[] hash = String_To_Bytes(md5Hex);

            TripleDESCryptoServiceProvider provider;
            provider = new TripleDESCryptoServiceProvider();

            provider.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            provider.Key = hash;

            byte[] encodedText = Encoding.ASCII.GetBytes(text);
            ICryptoTransform transform = provider.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream
                (ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(encodedText, 0, encodedText.Length);
                    cs.FlushFinalBlock();
                }
                return Bytes_To_String(ms.ToArray());
                //return ByteArrayToString(ms.ToArray());
            }
        }

        private static string Bytes_To_String(byte[] bytes_Input)
        {
            // convert the byte array back to a true string
            StringBuilder strTemp = new StringBuilder();
            for (int x = 0; x <= bytes_Input.GetUpperBound(0); x++)
            {
                int number = int.Parse(bytes_Input[x].ToString());
                strTemp.Append(number.ToString("X").PadLeft(2, '0'));
            }
            // return the finished string of hex values
            return strTemp.ToString();
        }

        public static string Encrypt(string text, byte[] hash)
        {
            TripleDESCryptoServiceProvider provider;
            provider = new TripleDESCryptoServiceProvider();

            provider.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            provider.Key = hash;

            byte[] encodedText = Encoding.ASCII.GetBytes(text);
            ICryptoTransform transform = provider.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream
                (ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(encodedText, 0, encodedText.Length);
                    cs.FlushFinalBlock();
                }
                return Bytes_To_String(ms.ToArray());
                //return ByteArrayToString(ms.ToArray());
            }
        }

        public static string Encode(string input)
        {
            try
            {
                string md5Key = GetConfigElement("md5key");

                return Encrypt(input, md5Key);
            }
            catch
            {
            }
            return "";
        }

        public static string DecodeToken(string token)
        {
            try
            {
                string md5Key = GetConfigElement("md5key");

                return Decrypt(token, md5Key);
            }
            catch
            {
            }
            return "";
        }

        public static string GetConfigElement(string element)
        {
            try
            {
                return keyDefault;
            }
            catch
            {
            }
            return "";
        }

        public static string GenSKU(string input)
        {
            input = TiengVietKhongDau(input);
            string sku = Regex.Replace(String.Join("-", input.ToLower().Split(' ')), @"(-)\1{1,}", "$1");

            return sku;
        }

        public static string TiengVietKhongDau(string input)
        {
            string[] p = new string[8];
            string[] pReplace = new string[8] { "a", "e", "o", "u", "i", "y", "d", " " };
            p[0] = "àảáạãâầấẩậẫăằẳắặẵ";
            p[1] = "ềếệễểêẹéèẻẽ";
            p[2] = "òỏóọõôồổốộỗơớợởờỡ";
            p[3] = "úùủụừứựửữưũ";
            p[4] = "ịỉìĩí";
            p[5] = "ỷýỵỳỹ";
            p[6] = "đ";
            p[7] = "^a-z0-9";

            for (int i = 0; i < 8; i++)
            {
                input = Regex.Replace(input, "[" + p[i] + "]", pReplace[i], RegexOptions.IgnoreCase);
            }
            return StringOptimal(input);
        }

        public static string StringOptimal(string input)
        {
            try
            {
                input = Regex.Replace(input, @"[\t\f]| ", " ", RegexOptions.Singleline, TimeSpan.FromSeconds(5));
                input = Regex.Replace(input, @"([\s\t\f]*[\r\n][\s\t\f]*){2,}", "\n", RegexOptions.Singleline, TimeSpan.FromSeconds(5));
                input = Regex.Replace(input, @"[ ]{2,}", " ", RegexOptions.Singleline, TimeSpan.FromSeconds(5));
                input = input.TrimStart().TrimEnd();
            }
            catch
            {
            }
            return input;
        }

        public static long UnixTime(DateTime? date = null)
        {
            try
            {
                if (date == null)
                    date = DateTime.Now;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var res = Convert.ToInt64((date.GetValueOrDefault().ToUniversalTime() - epoch).TotalSeconds);
                return res;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static DateTime UnixTime(long unixTimeStamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static string GetToken()
        {
            string ip = GetUserIP();
            try
            {
                long time = UnixTime(DateTime.Now);
                return Encrypt($"{time}-{ip}", "4a53afd6f286e316ad3d9f50579ec8fe");
            }
            catch
            {
                return "";
            }
        }

        public static string GetUserIP()
        {
            string ip = String.Empty;
            var firstUpInterface = NetworkInterface.GetAllNetworkInterfaces()
    .OrderByDescending(c => c.Speed)
    .FirstOrDefault(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up);
            if (firstUpInterface != null)
            {
                var props = firstUpInterface.GetIPProperties();
                // get first IPV4 address assigned to this interface
                ip = props.UnicastAddresses
                    .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(c => c.Address)
                    .FirstOrDefault().ToString();

                //
                //if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                //    ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                //else if (!String.IsNullOrWhiteSpace(context.Request.UserHostAddress))
                //    ip = context.Request.UserHostAddress;

                if (ip == "::1")
                    ip = "127.0.0.1";
            }
            return ip;
        }
    }
}