using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace TreeViewTest
{
    public static class Consts
    {
        public const string file_path = @"bin\key_pref";

    }

    class Tools
    {
        public static byte[] getBytes(string str)
        {
            return UTF8Encoding.UTF8.GetBytes(str);
        }
        public static string getString(byte[] buffer)
        {
            return UTF8Encoding.UTF8.GetString(buffer);
        }

        public static string exportData(byte[] buffer)
        {
            return Convert.ToBase64String(buffer);
        }
        public static byte[] importData(string str)
        {
            return Convert.FromBase64String(str);
        }
        public static long calibrate(long target, long range)
        {
            long k = target % range;
            if (k >= (range / 2))
                return target + range - k;
            return target - k;
        }
        private static AESCrypt generalAES;
        public static void generateAESModule(List<long> answer)
        {
            generalAES = new AESCrypt(generateKey(answer[0]), generateIV((answer.LongCount() + answer[0]) ^ 10293847));
        }
        public static AESCrypt getAESModule()
        {
            return generalAES;
        }

        public static bool isDirectory(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return true;
                else
                    return false;
            }
            catch(FileNotFoundException error)
            {
                return false;
            }
        }
        public static string[] getAllFileList(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
        }

        public static byte[] generateKey(long init)
        {
            //Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Tools.exportData(getFileHash(Consts.file_path)), Tools.getBytes((init ^ 20394820).ToString() + "aoskdnfodmw"));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes("thisisfortest", Tools.getBytes((init ^ 20394820).ToString() + "aoskdnfodmw"));
            return key.GetBytes(128 / 8);
        }

        public static byte[] generateIV(long init)
        {
            //Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Tools.exportData(getFileHash(Consts.file_path)), Tools.getBytes(init.ToString() + "aoskdnfodmw"));
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes("thisisfortest", Tools.getBytes(init.ToString() + "aoskdnfodmw"));
            return key.GetBytes(128 / 8);
        }

        private static byte[] getFileHash(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                using (SHA1Managed sha = new SHA1Managed())
                {
                    return sha.ComputeHash(stream);
                }
            }
        }

        public static string genHash(string str)
        {
            using (SHA1Managed sha = new SHA1Managed())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(str)));
            }
        }
        public static string getFileNameFromPath(string path)
        {
            string[] k = path.Split('\\');
            return k[k.Length - 1];
        }
    }
}
