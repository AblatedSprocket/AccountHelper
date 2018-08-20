using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AccountHelper.Utilities
{
    public static class Encryptor
    {
        private const string masterKey = "9B2423BD-5134-4ECD-85B3-C43C4D040244";
        public static string GetSHA256Hash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            SHA256Managed hashString = new SHA256Managed();
            byte[] hash = hashString.ComputeHash(data);
            StringBuilder hashStringBuild = new StringBuilder();
            foreach (byte x in hash)
            {
                hashStringBuild.Append(string.Format("{0:x2}", x));
            }
            return hashStringBuild.ToString();
        }
        public static string GetHash(string input)
        {
            StringBuilder hexHashSB = new StringBuilder();
            MD5 cspMD5 = new MD5CryptoServiceProvider();
            byte[] bData = Encoding.UTF8.GetBytes(input);
            byte[] bResult = cspMD5.ComputeHash(bData);
            for (int i = 0; i < bResult.Length - 1; i++)
            {
                hexHashSB.Append(bResult[i].ToString("x"));
            }
            return hexHashSB.ToString();
        }
        public static string WriteEncryptedFile(string fileName, string data)
        {
            FileStream outputFile;
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            CryptoStream cryptoStream;
            TripleDESCryptoServiceProvider cspDES = new TripleDESCryptoServiceProvider();
            byte[] byteBuffer = new byte[4096];
            long longBytesProcessed = 0;
            long longFileLength;
            int byteBlockSize;
            byte[] byteKey;
            byte[] byteIV;
            byteKey = CreateKey(masterKey);
            byteIV = CreateIV(masterKey);
            using (MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                longFileLength = input.Length;
                using (outputFile = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write))
                using (cryptoStream = new CryptoStream(outputFile, cspDES.CreateEncryptor(byteKey, byteIV), CryptoStreamMode.Write))
                {
                    outputFile.SetLength(0);
                    while (longBytesProcessed < longFileLength)
                    {
                        byteBlockSize = input.Read(byteBuffer, 0, 4096);
                        cryptoStream.Write(byteBuffer, 0, byteBlockSize);
                        longBytesProcessed += byteBlockSize;
                    }
                }
            }
            return outputPath;
        }
        public static string DecryptFile(string fileName)
        {
            FileStream inputFile;
            MemoryStream output;
            string inputPath;
            CryptoStream cryptoStream;
            TripleDESCryptoServiceProvider cspDES = new TripleDESCryptoServiceProvider();
            byte[] byteBuffer = new byte[4096];
            long longBytesProcessed = 0;
            long longFileLength;
            int byteBlockSize;
            byte[] byteKey;
            byte[] byteIV;
            byteKey = CreateKey(masterKey);
            byteIV = CreateIV(masterKey);
            inputPath = fileName;
            if (File.Exists(inputPath))
            {
                using (inputFile = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                {
                    longFileLength = inputFile.Length;
                    using (output = new MemoryStream())
                    using (cryptoStream = new CryptoStream(output, cspDES.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Write))
                    {
                        while (longBytesProcessed < longFileLength)
                        {
                            byteBlockSize = inputFile.Read(byteBuffer, 0, 4096);
                            cryptoStream.Write(byteBuffer, 0, byteBlockSize);
                            longBytesProcessed += byteBlockSize;
                        }
                        cryptoStream.FlushFinalBlock();
                        return Encoding.UTF8.GetString(output.ToArray());
                    }
                }
            }
            return null;
        }
        private static byte[] CreateKey(string password)
        {
            try
            {
                byte[] byteSalt = Encoding.ASCII.GetBytes("C76ED6EE-50DD-4133-A0B2-31C80221CFC2");
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, byteSalt);
                return pdb.GetBytes(24);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static byte[] CreateIV(string password)
        {
            try
            {
                byte[] byteSalt = Encoding.ASCII.GetBytes("C76ED6EE-50DD-4133-A0B2-31C80221CFC2");
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, byteSalt);
                return pdb.GetBytes(8);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
