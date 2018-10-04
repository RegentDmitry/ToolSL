using System;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;

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
            var p = mc.Properties;
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

        private static string GetRegistryKey()
        {
            try
            {
                string x64Result = string.Empty;
                string x86Result = string.Empty;
                RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
                RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
                object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"");
                object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"");
                keyX64.Close();
                keyX86.Close();
                keyBaseX64.Close();
                keyBaseX86.Close();
                keyX64.Dispose();
                keyX86.Dispose();
                keyBaseX64.Dispose();
                keyBaseX86.Dispose();
                keyX64 = null;
                keyX86 = null;
                keyBaseX64 = null;
                keyBaseX86 = null;

                var s = resultObjX64.ToString();
                var s1 = resultObjX86.ToString();
                return s + s1;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetMachineToken()
        {
            var modelNo = _Identifier("Win32_DiskDrive", "Model");
            var computerId = _Identifier("Win32_ComputerSystemProduct", "IdentifyingNumber");
            var uuid = _Identifier("Win32_ComputerSystemProduct", "UUID");
            var registryKey = GetRegistryKey();
            return _GetSaltedString(modelNo + computerId + uuid + registryKey);
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
