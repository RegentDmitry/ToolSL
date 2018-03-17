using System;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;

namespace ToolSL
{
    public static class Utils
    {
        public static readonly string MachineToken = GetMachineToken();

        private const string _salt = "HJl290123_a";

        private static string _Identifier(string wmiClass, string wmiProperty)
        {
            var result = "";
            var mc = new ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if (result != "")
                    continue;

                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                {
                }
            }
            return result;
        }

        private static string _GetSaltedString(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input + _salt);
            var hash = md5.ComputeHash(inputBytes);
            var code = Convert.ToBase64String(hash);
            return code;
        }

        public static string GetMachineToken()
        {
            var modelNo = _Identifier("Win32_DiskDrive", "Model");
            return _GetSaltedString(modelNo);
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }
}
