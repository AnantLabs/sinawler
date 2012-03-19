using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sinawler
{
    public class Serialize
    {
        // 用于初始化对称密钥
        static private String encryptKey = "sizheng0320Sinawler";

        static private byte[] key = Encoding.ASCII.GetBytes(encryptKey.Substring(0, 8));
        static private byte[] IV = Encoding.ASCII.GetBytes(encryptKey);


        /// <summary>
        /// 将对象加密到字节数据
        /// </summary>
        /// <param name="obj">要加密的对象</param>
        /// <returns>处理后生成的字节数组</returns>
        public static byte[] EncryptToBytes ( object obj )
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream msPlaneText = new MemoryStream();

                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize( msPlaneText, obj );

                byte[] inputByteArray = msPlaneText.ToArray();
                msPlaneText.Close();
                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream cs = new CryptoStream( msEncrypt, des.CreateEncryptor(key,IV), CryptoStreamMode.Write );
                cs.Write( inputByteArray, 0, inputByteArray.Length );
                cs.FlushFinalBlock();
                byte[] byteEncrypt = msEncrypt.ToArray();
                cs.Close();
                return byteEncrypt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将字节数组进行解密还原成对象
        /// </summary>
        /// <param name="ary">要处理的字节数组</param>
        /// <returns>被还原的对象</returns>
        public static object DecryptToObject ( byte[] ary )
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream( ms, des.CreateDecryptor(key,IV), CryptoStreamMode.Write );
                cs.Write( ary, 0, ary.Length );
                cs.FlushFinalBlock();
                cs.Close();

                byte[] byteDecrypt = ms.ToArray();
                MemoryStream msDecrypt = new MemoryStream( byteDecrypt );
                BinaryFormatter serializer = new BinaryFormatter();
                Object obj = serializer.Deserialize( msDecrypt );
                msDecrypt.Close();
                return obj;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 将对象压缩到字节数组
        /// </summary>
        /// <param name="obj">要压缩的对象</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressedToBytes ( object obj )
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream zip = new DeflateStream( ms, CompressionMode.Compress, true );
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize( zip, obj );
                zip.Close();
                byte[] ary = ms.ToArray();
                ms.Close();
                return ary;
            }
            catch
            {
                zip.Close();
                ms.Close();
                return null;
            }
        }

        /// <summary>
        /// 解压缩后对象
        /// </summary>
        /// <param name="ary">字节数组</param>
        /// <returns>对象</returns>
        public static object DecompressToObject ( byte[] ary )
        {
            MemoryStream ms = new MemoryStream( ary );
            DeflateStream UnZip = new DeflateStream( ms, CompressionMode.Decompress );
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                object obj = serializer.Deserialize( UnZip );
                UnZip.Close();
                ms.Close();
                return obj;
            }
            catch
            {
                UnZip.Close();
                ms.Close();
                return null;
            }
        }
    }

}
