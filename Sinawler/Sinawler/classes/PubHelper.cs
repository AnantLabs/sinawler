using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Sina.Api;

namespace Sinawler
{
    //此类用于实现一些公用的、常用的处理函数
    class PubHelper
    {
        /// <summary>
        /// 将抓取的created_at时间转换为数据库可接受的yyyy-mm-dd hh:mm:ss格式
        /// </summary>
        /// <param name="strDateTime">抓取的日期字符串</param>
        /// <returns>整理后的yyyy-mm-dd hh:mm:ss格式的日期和时间</returns>
        static public string ParseDateTime ( string strDateTime )
        {
            string[] s = strDateTime.Split( ' ' );
            string strBuffer = s[5] + "-";
            switch (s[1])
            {
                case "Jan": //一月
                    strBuffer += "01-";
                    break;
                case "Feb": //二月
                    strBuffer += "02-";
                    break;
                case "Mar": //三月
                    strBuffer += "03-";
                    break;
                case "Apr": //四月
                    strBuffer += "04-";
                    break;
                case "May": //五月
                    strBuffer += "05-";
                    break;
                case "Jun": //六月
                    strBuffer += "06-";
                    break;
                case "Jul": //七月
                    strBuffer += "07-";
                    break;
                case "Aug": //八月
                    strBuffer += "08-";
                    break;
                case "Sep": //九月
                    strBuffer += "09-";
                    break;
                case "Oct": //十月
                    strBuffer += "10-";
                    break;
                case "Nov": //十一月
                    strBuffer += "11-";
                    break;
                case "Dec": //十二月
                    strBuffer += "12-";
                    break;
                default:
                    return "0001-01-01 00:00:00";
            }
            strBuffer += s[2] + " " + s[3];
            return strBuffer;
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <returns>成功提示或者错误信息</returns>
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

        //整数到字节数组的转换   
        static public byte[] intToByte(int number)
        {
            int temp = number;
            byte[] b = new byte[4];
            for (int i = b.Length - 1; i > -1; i--)
            {
                b[i] = Convert.ToByte(temp & 0xff);             //将最高位保存在最低位   
                temp = temp >> 8;               //向右移8位   
            }
            return b;
        }

        //字节数组到整数的转换   
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
            if (b[3] >= 0)               //最后一个之所以不乘，是因为可能会溢出   
                s = s + b[3];
            else
                s = s + 256 + b[3];
            return s;
        }

        //发一条微博帮忙推广
        static public bool PostAdvertisement(SinaApiService api)
        {
            SettingItems settings=AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            string strResult = api.statuses_update("我正在使用开源应用“新浪微博爬虫”。Project页面：http://code.google.com/p/sinawler/；下载页面：http://code.google.com/p/sinawler/downloads/list");
            if (strResult == null) return false;
            else return true;
        }
    }
}
