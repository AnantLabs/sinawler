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
        private static byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6,
            7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2
        };
        private static byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6 };


        /// <summary>
        /// 将对象压缩加密到字节数据
        /// </summary>
        /// <param name="obj">要压缩加密的对象</param>
        /// <returns>处理后生成的字节数组</returns>
        public static byte[] CompressEncryptToBytes(object obj)
        {
            // 建立对称密码信息
            MemoryStream ms = new MemoryStream();
            RijndaelManaged RM = new RijndaelManaged();
            CryptoStream EnCrpStrm = new CryptoStream(ms, RM.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            DeflateStream zip = new DeflateStream(EnCrpStrm, CompressionMode.Compress, true);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(zip, obj);
                zip.Close();        // 在返回（return）这前一定要进行关闭
                EnCrpStrm.FlushFinalBlock();    // 在返回（return）这前一定要进行调用。
                return ms.ToArray();
            }
            catch (Exception e)
            {
                MsgBoxs.Alert(e.ToString());
                return null;
            }
            finally
            {
                EnCrpStrm.Close();
                ms.Close();
            }
        }

        /// <summary>
        /// 将字节数组进行解密解压还原成对象
        /// </summary>
        /// <param name="ary">要处理的字节数组</param>
        /// <returns>被还原的对象</returns>
        public static object DecompressDecryptToObject(byte[] ary)
        {
            MemoryStream ms = new MemoryStream(ary);
            RijndaelManaged RM = new RijndaelManaged();
            CryptoStream DeCrpStrm = new CryptoStream(ms, RM.CreateDecryptor(key, IV), CryptoStreamMode.Read);
            DeflateStream UnZip = new DeflateStream(DeCrpStrm, CompressionMode.Decompress);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                return serializer.Deserialize(UnZip);
            }
            catch (Exception e)
            {
                MsgBoxs.Alert(e.ToString());
                return null;
            }
            finally
            {
                UnZip.Close();
                DeCrpStrm.Close();
                ms.Close();
            }
        }

        /// <summary>
        /// 将对象压缩到字节数组
        /// </summary>
        /// <param name="obj">要压缩的对象</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] CompressedToBytes(object obj)
        {
            MemoryStream ms = new MemoryStream();
            DeflateStream zip = new DeflateStream(ms, CompressionMode.Compress, true);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(zip, obj);
                zip.Close();
                byte[] ary = ms.ToArray();
                ms.Close();
                return ary;
            }
            catch (Exception e)
            {
                zip.Close();
                ms.Close();
                MsgBoxs.Alert(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 解压缩后对象
        /// </summary>
        /// <param name="ary">字节数组</param>
        /// <returns>对象</returns>
        public static object DecompressToObject(byte[] ary)
        {
            MemoryStream ms = new MemoryStream(ary);
            DeflateStream UnZip = new DeflateStream(ms, CompressionMode.Decompress);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                object obj = serializer.Deserialize(UnZip);
                UnZip.Close();
                ms.Close();
                return obj;
            }
            catch (Exception e)
            {
                UnZip.Close();
                ms.Close();
                MsgBoxs.Alert(e.ToString());
                return null;
            }
        }
    }

}
