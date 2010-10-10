using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Sina.Api;

namespace Sinawler
{
    //��������ʵ��һЩ���õġ����õĴ�����
    class PubHelper
    {
        /// <summary>
        /// ��ץȡ��created_atʱ��ת��Ϊ���ݿ�ɽ��ܵ�yyyy-mm-dd hh:mm:ss��ʽ
        /// </summary>
        /// <param name="strDateTime">ץȡ�������ַ���</param>
        /// <returns>������yyyy-mm-dd hh:mm:ss��ʽ�����ں�ʱ��</returns>
        static public string ParseDateTime ( string strDateTime )
        {
            string[] s = strDateTime.Split( ' ' );
            string strBuffer = s[5] + "-";
            switch (s[1])
            {
                case "Jan": //һ��
                    strBuffer += "01-";
                    break;
                case "Feb": //����
                    strBuffer += "02-";
                    break;
                case "Mar": //����
                    strBuffer += "03-";
                    break;
                case "Apr": //����
                    strBuffer += "04-";
                    break;
                case "May": //����
                    strBuffer += "05-";
                    break;
                case "Jun": //����
                    strBuffer += "06-";
                    break;
                case "Jul": //����
                    strBuffer += "07-";
                    break;
                case "Aug": //����
                    strBuffer += "08-";
                    break;
                case "Sep": //����
                    strBuffer += "09-";
                    break;
                case "Oct": //ʮ��
                    strBuffer += "10-";
                    break;
                case "Nov": //ʮһ��
                    strBuffer += "11-";
                    break;
                case "Dec": //ʮ����
                    strBuffer += "12-";
                    break;
                default:
                    return "0001-01-01 00:00:00";
            }
            strBuffer += s[2] + " " + s[3];
            return strBuffer;
        }

        /// <summary>
        /// �������ݿ�����
        /// </summary>
        /// <returns>�ɹ���ʾ���ߴ�����Ϣ</returns>
        static public string TestDataBase ()
        {
            string strResult;
            Database db = DatabaseFactory.CreateDatabase();
            try
            {
                db.Test();
                strResult = "OK";
            }
            catch(Exception ex)
            {
                strResult = ex.Message;
            }
            return strResult;
        }

        //�������ֽ������ת��   
        static public byte[] intToByte(int number)
        {
            int temp = number;
            byte[] b = new byte[4];
            for (int i = b.Length - 1; i > -1; i--)
            {
                b[i] = Convert.ToByte(temp & 0xff);             //�����λ���������λ   
                temp = temp >> 8;               //������8λ   
            }
            return b;
        }

        //�ֽ����鵽������ת��   
        static public int byteToInt(byte[] b)
        {
            int s = 0;
            for (int i = 0; i < 3; i++)
            {
                if (b[i] >= 0)
                    s = s + b[i];
                else
                    s = s + 256 + b[i];
                s = s * 256;
            }
            if (b[3] >= 0)               //���һ��֮���Բ��ˣ�����Ϊ���ܻ����   
                s = s + b[3];
            else
                s = s + 256 + b[3];
            return s;
        }

        //��һ��΢����æ�ƹ�
        static public bool PostAdvertisement(SinaApiService api)
        {
            SettingItems settings=AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            string strResult = api.statuses_update("������ʹ�ÿ�ԴӦ�á�����΢�����桱��Projectҳ�棺http://code.google.com/p/sinawler/������ҳ�棺http://code.google.com/p/sinawler/downloads/list");
            if (strResult == null) return false;
            else return true;
        }
    }
}
