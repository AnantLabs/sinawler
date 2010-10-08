using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

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
    }
}
