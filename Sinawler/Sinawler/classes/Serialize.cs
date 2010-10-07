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
        // ���ڳ�ʼ���Գ���Կ
        private static byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6,
            7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2
        };
        private static byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6 };


        /// <summary>
        /// ������ѹ�����ܵ��ֽ�����
        /// </summary>
        /// <param name="obj">Ҫѹ�����ܵĶ���</param>
        /// <returns>��������ɵ��ֽ�����</returns>
        public static byte[] CompressEncryptToBytes(object obj)
        {
            // �����Գ�������Ϣ
            MemoryStream ms = new MemoryStream();
            RijndaelManaged RM = new RijndaelManaged();
            CryptoStream EnCrpStrm = new CryptoStream(ms, RM.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            DeflateStream zip = new DeflateStream(EnCrpStrm, CompressionMode.Compress, true);
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(zip, obj);
                zip.Close();        // �ڷ��أ�return����ǰһ��Ҫ���йر�
                EnCrpStrm.FlushFinalBlock();    // �ڷ��أ�return����ǰһ��Ҫ���е��á�
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
        /// ���ֽ�������н��ܽ�ѹ��ԭ�ɶ���
        /// </summary>
        /// <param name="ary">Ҫ������ֽ�����</param>
        /// <returns>����ԭ�Ķ���</returns>
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
        /// ������ѹ�����ֽ�����
        /// </summary>
        /// <param name="obj">Ҫѹ���Ķ���</param>
        /// <returns>ѹ������ֽ�����</returns>
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
        /// ��ѹ�������
        /// </summary>
        /// <param name="ary">�ֽ�����</param>
        /// <returns>����</returns>
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
