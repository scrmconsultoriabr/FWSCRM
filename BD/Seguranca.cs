using System;
using System.Security.Cryptography;
using System.Text;

namespace FWSCRM.BD
{
    public class Seguranca
    {
        // Methods
        public static string DecryptTripleDES(string sOut)
        {
            string sKey = "FWSCRM.123.kuatcha";
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            sKey = ScrambleKey(sKey);
            DES.Key = hashMD5.ComputeHash(Encoding.ASCII.GetBytes(sKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(sOut);
            return Encoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string DecryptTripleDES(string sOut, string sKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            sKey = ScrambleKey(sKey);
            DES.Key = hashMD5.ComputeHash(Encoding.ASCII.GetBytes(sKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(sOut);
            return Encoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string EncryptTripleDES(string sIn)
        {
            string sKey = "FWSCRM.123.kuatcha";
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            sKey = ScrambleKey(sKey);
            DES.Key = hashMD5.ComputeHash(Encoding.ASCII.GetBytes(sKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.ASCII.GetBytes(sIn);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string EncryptTripleDES(string sIn, string sKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
            sKey = ScrambleKey(sKey);
            DES.Key = hashMD5.ComputeHash(Encoding.ASCII.GetBytes(sKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.ASCII.GetBytes(sIn);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        private static string ScrambleKey(string v_strKey)
        {
            StringBuilder sbKey = new StringBuilder();
            for (int intPtr = 1; intPtr <= v_strKey.Length; intPtr++)
            {
                int intIn = (v_strKey.Length - intPtr) + 1;
                sbKey.Append(v_strKey.Substring(intIn - 1, 1));
            }
            string strKey = sbKey.ToString();
            return sbKey.ToString();
        }

        public static string SeguracaSql(string filtro)
        {
            string filtroaux = filtro;
            return filtroaux.Replace("'", "''").Replace("--", "").Replace(";", "").Replace("%", "");
        }
    }

}