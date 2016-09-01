using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TreeViewTest
{
    public enum CRYPTO_RESULT
    {
        SUCCESS,
        KEY_SIZE_ERROR,
        CRYPTO_ERROR,
        UNKNOWN_ERROR
    }
    public interface ICryptoBase
    {
        CRYPTO_RESULT Encrypt(byte[] data, out byte[] result);
        CRYPTO_RESULT Decrypt(byte[] data, out byte[] result);

        byte[] Key { get; set; }
        byte[] IV { get; set; }
        int KeySize { get; }
        PaddingMode Padd { get; set; }
    }

    public class AESCryptProvider : ICryptoBase
    {
        private byte[] key;
        private byte[] iv;
        private int keySize;
        private AesCryptoServiceProvider crypto;

        public AESCryptProvider()
        {
            crypto = new AesCryptoServiceProvider();
            crypto.Padding = PaddingMode.PKCS7;
            crypto.Mode = CipherMode.CBC;
        }

        public CRYPTO_RESULT Encrypt(byte[] data, out byte[] result)
        {
            var encryptor = crypto.CreateEncryptor();
            if (this.key.Length != this.keySize)
            {
                result = null;
                return CRYPTO_RESULT.KEY_SIZE_ERROR;
            }
            try
            {
                result = encryptor.TransformFinalBlock(data, 0, data.Length);
                return CRYPTO_RESULT.SUCCESS;
            }
            catch
            {
                result = null;
                return CRYPTO_RESULT.CRYPTO_ERROR;
            }
        }
        public CRYPTO_RESULT Decrypt(byte[] data, out byte[] result)
        {
            var encryptor = crypto.CreateDecryptor();
            if (this.key.Length != this.keySize)
            {
                result = null;
                return CRYPTO_RESULT.KEY_SIZE_ERROR;
            }
            try
            {
                result = encryptor.TransformFinalBlock(data, 0, data.Length);
                return CRYPTO_RESULT.SUCCESS;
            }
            catch
            {
                result = null;
                return CRYPTO_RESULT.CRYPTO_ERROR;
            }
        }

        public byte[] Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                this.keySize = key.Length;
                this.crypto.Key = this.key;
            }
        }
        public byte[] IV
        {
            get
            {
                return iv;
            }
            set
            {
                iv = value;
                this.crypto.IV = this.iv;
            }
        }

        public PaddingMode Padd
        {
            get
            {
                return this.crypto.Padding;
            }
            set
            {
                this.crypto.Padding = value;
            }
        }
        public int KeySize
        {
            get
            {
                return this.keySize;
            }
        }
    }

    public class AESCrypt : AESCryptProvider
    {
        private AESCryptProvider _AESCP;

        public AESCrypt(byte[] init_key, byte[] init_iv)
        {
            _AESCP = new AESCryptProvider();

            _AESCP.Key = init_key;
            _AESCP.IV = init_iv;

        }
        public string EncryptString(string plain)
        {
            byte[] result;
            CRYPTO_RESULT rc = _AESCP.Encrypt(Tools.getBytes(plain), out result);
            if (rc != CRYPTO_RESULT.SUCCESS)
                return "";
            else
                return Tools.exportData(result);
        }
        public string DecryptString(string crypted)
        {
            byte[] result;
            CRYPTO_RESULT rc = _AESCP.Decrypt(Tools.importData(crypted), out result);
            if (rc != CRYPTO_RESULT.SUCCESS)
                return "";
            else
                return Tools.getString(result);
        }

        public int EncryptFile(string filePath)
        {
            byte[] result;
            string path = filePath + ".enc";

            using (System.IO.FileStream x = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read), encWriter = new System.IO.FileStream(path, System.IO.FileMode.Append))
            using (System.IO.BinaryReader y = new System.IO.BinaryReader(x))
            {
                long numBytes = new System.IO.FileInfo(filePath).Length;
                int loops = (int)(numBytes / 1048576);
                for (int i = 0; i <= loops; i++)
                {
                    if (i == loops)
                    {
                        if (_AESCP.Encrypt(y.ReadBytes((int)(numBytes % 1048576)), out result) != CRYPTO_RESULT.SUCCESS)
                            return -1;
                        encWriter.Write(result, 0, result.Length);
                        encWriter.Flush();
                        break;
                    }
                    if (_AESCP.Encrypt(y.ReadBytes(1048576), out result) != CRYPTO_RESULT.SUCCESS)
                        return -1;
                    encWriter.Write(result, 0, result.Length);
                    encWriter.Flush();
                }
                return 0;
            }

        }

        public int DecryptFile(string filePath)
        {
            byte[] result;
            string path = filePath.Substring(0, filePath.Length - 4);
            using (System.IO.FileStream x = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read), decWriter = new System.IO.FileStream(path, System.IO.FileMode.Append))
            using (System.IO.BinaryReader y = new System.IO.BinaryReader(x))
            {
                long numBytes = new System.IO.FileInfo(filePath).Length;
                int loops = (int)(numBytes / (1048576 + 16));
                for (int i = 0; i <= loops; i++)
                {
                    if (i == loops)
                    {
                        if (_AESCP.Decrypt(y.ReadBytes((int)(numBytes % (1048576 + 16))), out result) != CRYPTO_RESULT.SUCCESS)
                            return -1;
                        decWriter.Write(result, 0, result.Length);
                        decWriter.Flush();
                        break;
                    }
                    if (_AESCP.Decrypt(y.ReadBytes(1048576 + 16), out result) != CRYPTO_RESULT.SUCCESS)
                        return -1;
                    decWriter.Write(result, 0, result.Length);
                    decWriter.Flush();
                }
                return 0;
            }
        }
    }
}
