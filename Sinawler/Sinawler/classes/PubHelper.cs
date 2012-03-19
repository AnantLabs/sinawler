using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Xml;
using System.Net;
using System.IO;
using System.Reflection;
using Sinawler;

public struct RequestFrequency
{
    public int RemainingHits;
    public int ResetTimeInSeconds;
    public int Interval;
}

//此类用于实现一些公用的、常用的处理函数
class PubHelper
{
    /// <summary>
    /// 将抓取的created_at时间转换为数据库可接受的yyyy-mm-dd hh:mm:ss格式
    /// </summary>
    /// <param name="strDateTime">抓取的日期字符串</param>
    /// <returns>整理后的yyyy-mm-dd hh:mm:ss格式的日期和时间</returns>
    static public string ParseDateTime(string strDateTime)
    {
        if (strDateTime.Trim() == "") return "";
        string[] s = strDateTime.Split(' ');
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
    static public string TestDataBase()
    {
        string strResult;
        Database db = DatabaseFactory.CreateDatabase();
        try
        {
            Assembly Asm = Assembly.GetExecutingAssembly();
            Stream strmUserRelationBuffer, strmUserInfoBuffer, strmUserTagBuffer, strmStatusBuffer, strmCommentBuffer;

            SettingItems settings = AppSettings.Load();
            if (settings == null) settings = AppSettings.LoadDefault();
            #region UserRelation
            switch (settings.DBType)
            {
                case "SQL Server":
                    strmUserRelationBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_userRelation.sql");
                    break;
                default:
                    strmUserRelationBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_userRelation.sql");
                    break;
            }
            StreamReader readerUserRelationBuffer = new StreamReader(strmUserRelationBuffer);
            string sql = "";
            string strLine = readerUserRelationBuffer.ReadLine();
            while (strLine != null)
            {
                if (strLine.Trim() != "")
                {
                    sql += " " + strLine;
                    if (strLine.LastIndexOf(';') == strLine.Length - 1)  //SQL语句结尾
                    {
                        sql = sql.Substring(0, sql.Length - 1);
                        sql = sql.Replace("queue_buffer_for_userRelation", GlobalPool.UserRelationBufferTable);
                        sql = sql.Replace("PK_queue_buffer_for_userRelation", "PK_queue_buffer_for_userRelation" + GlobalPool.TimeStamp.ToString());
                        sql = sql.Replace("index_enqueue_time", "index_enqueue_time" + GlobalPool.TimeStamp.ToString());
                        db.CountByExecuteSQL(sql);
                        sql = "";
                    }
                }
                strLine = readerUserRelationBuffer.ReadLine();
            }

            readerUserRelationBuffer.Close();
            readerUserRelationBuffer.Dispose();
            #endregion
            #region UserInfo
            switch (settings.DBType)
            {
                case "SQL Server":
                    strmUserInfoBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_userInfo.sql");
                    break;
                default:
                    strmUserInfoBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_userInfo.sql");
                    break;
            }
            if (GlobalPool.UserInfoRobotEnabled)
            {
                StreamReader readerUserInfoBuffer = new StreamReader(strmUserInfoBuffer);
                sql = "";
                strLine = readerUserInfoBuffer.ReadLine();
                while (strLine != null)
                {
                    if (strLine.Trim() != "")
                    {
                        sql += " " + strLine;
                        if (strLine.LastIndexOf(';') == strLine.Length - 1)  //SQL语句结尾
                        {
                            sql = sql.Substring(0, sql.Length - 1);
                            sql = sql.Replace("queue_buffer_for_userInfo", GlobalPool.UserInfoBufferTable);
                            sql = sql.Replace("PK_queue_buffer_for_userInfo", "PK_queue_buffer_for_userInfo" + GlobalPool.TimeStamp.ToString());
                            sql = sql.Replace("index_enqueue_time", "index_enqueue_time" + GlobalPool.TimeStamp.ToString());
                            db.CountByExecuteSQL(sql);
                            sql = "";
                        }
                    }
                    strLine = readerUserInfoBuffer.ReadLine();
                }

                readerUserInfoBuffer.Close();
                readerUserInfoBuffer.Dispose();
            }
            #endregion
            #region UserTags
            switch (settings.DBType)
            {
                case "SQL Server":
                    strmUserTagBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_tag.sql");
                    break;
                default:
                    strmUserTagBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_tag.sql");
                    break;
            }
            if (GlobalPool.TagRobotEnabled)
            {
                StreamReader readerUserTagBuffer = new StreamReader(strmUserTagBuffer);
                sql = "";
                strLine = readerUserTagBuffer.ReadLine();
                while (strLine != null)
                {
                    if (strLine.Trim() != "")
                    {
                        sql += " " + strLine;
                        if (strLine.LastIndexOf(';') == strLine.Length - 1)  //SQL语句结尾
                        {
                            sql = sql.Substring(0, sql.Length - 1);
                            sql = sql.Replace("queue_buffer_for_tag", GlobalPool.UserTagBufferTable);
                            sql = sql.Replace("PK_queue_buffer_for_tag", "PK_queue_buffer_for_tag" + GlobalPool.TimeStamp.ToString());
                            sql = sql.Replace("index_enqueue_time", "index_enqueue_time" + GlobalPool.TimeStamp.ToString());
                            db.CountByExecuteSQL(sql);
                            sql = "";
                        }
                    }
                    strLine = readerUserTagBuffer.ReadLine();
                }

                readerUserTagBuffer.Close();
                readerUserTagBuffer.Dispose();
            }
            #endregion
            #region Status
            switch (settings.DBType)
            {
                case "SQL Server":
                    strmStatusBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_status.sql");
                    break;
                default:
                    strmStatusBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_status.sql");
                    break;
            }
            if (GlobalPool.StatusRobotEnabled)
            {
                StreamReader readerStatusBuffer = new StreamReader(strmStatusBuffer);
                sql = "";
                strLine = readerStatusBuffer.ReadLine();
                while (strLine != null)
                {
                    if (strLine.Trim() != "")
                    {
                        sql += " " + strLine;
                        if (strLine.LastIndexOf(';') == strLine.Length - 1)  //SQL语句结尾
                        {
                            sql = sql.Substring(0, sql.Length - 1);
                            sql = sql.Replace("queue_buffer_for_status", GlobalPool.StatusBufferTable);
                            sql = sql.Replace("PK_queue_buffer_for_status", "PK_queue_buffer_for_status" + GlobalPool.TimeStamp.ToString());
                            sql = sql.Replace("index_enqueue_time", "index_enqueue_time" + GlobalPool.TimeStamp.ToString());
                            db.CountByExecuteSQL(sql);
                            sql = "";
                        }
                    }
                    strLine = readerStatusBuffer.ReadLine();
                }

                readerStatusBuffer.Close();
                readerStatusBuffer.Dispose();
            }
            #endregion
            #region Comment
            switch (settings.DBType)
            {
                case "SQL Server":
                    strmCommentBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_comment.sql");
                    break;
                default:
                    strmCommentBuffer = Asm.GetManifestResourceStream(Asm.GetName().Name + ".queue_buffer_for_comment.sql");
                    break;
            }
            if (GlobalPool.CommentRobotEnabled)
            {
                StreamReader readerCommentBuffer = new StreamReader(strmCommentBuffer);
                sql = "";
                strLine = readerCommentBuffer.ReadLine();
                while (strLine != null)
                {
                    if (strLine.Trim() != "")
                    {
                        sql += " " + strLine;
                        if (strLine.LastIndexOf(';') == strLine.Length - 1)  //SQL语句结尾
                        {
                            sql = sql.Substring(0, sql.Length - 1);
                            sql = sql.Replace("queue_buffer_for_comment", GlobalPool.CommentBufferTable);
                            sql = sql.Replace("PK_queue_buffer_for_comment", "PK_queue_buffer_for_comment" + GlobalPool.TimeStamp.ToString());
                            sql = sql.Replace("index_enqueue_time", "index_enqueue_time" + GlobalPool.TimeStamp.ToString());
                            db.CountByExecuteSQL(sql);
                            sql = "";
                        }
                    }
                    strLine = readerCommentBuffer.ReadLine();
                }

                readerCommentBuffer.Close();
                readerCommentBuffer.Dispose();
            }
            #endregion
            strResult = "OK";
        }
        catch (Exception ex)
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
    static public bool PostAdvertisement(int UserCount, int StatusCount)
    {
        string strResult = GlobalPool.GetAPI(SysArgFor.USER_RELATION).API.Statuses_Update("（" + DateTime.Now.ToString() + "）I'm using an open source application Sinawler v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
            ". There have been " + UserCount.ToString() + " users and " + StatusCount.ToString() + " statuses in queues！Project Homepage: http://code.google.com/p/sinawler/");
        if (strResult == null) return false;
        else return true;
    }

    /**
    * This method ensures that the output String has only valid XML unicode
    * characters as specified by the XML 1.0 standard. For reference, please
    * see <a href="http://www.w3.org/TR/2000/REC-xml-20001006#NT-Char">the
    * standard</a>. This method will return an empty String if the input is
    * null or empty.
    * 
    * @param value
    *            The String whose non-valid characters we want to remove.
    * @return The in String, stripped of non-valid characters.
    */
    static public string stripNonValidXMLCharacters(String value)
    {
        StringBuilder strResult = new StringBuilder(); // Used to hold the output.
        char current; // Used to reference the current character.

        if (value == null) return ""; // vacancy test.
        for (int i = 0; i < value.Length; i++)
        {
            current = value[i]; // NOTE: No IndexOutOfBoundsException caught
            // here; it should not happen.
            if ((current == 0x9) || (current == 0xA) || (current == 0xD)
                    || ((current >= 0x20) && (current <= 0xD7FF))
                    || ((current >= 0xE000) && (current <= 0xFFFD))
                    || ((current >= 0x10000) && (current <= 0x10FFFF)))
                strResult.Append(current);
        }
        return strResult.ToString();
    }

    //自己实现队列的Contains操作，从头尾同时找，效率提高一倍
    static public bool ContainsInQueue<T>(LinkedList<T> list, T value)
    {
        if (list == null || value == null) return false;
        if (list.Count == 0) return false;
        if (list.Count == 1) return list.First.Value.Equals(value);

        LinkedListNode<T> nodeHead = list.First;
        LinkedListNode<T> nodeTail = list.Last;
        if (nodeHead.Next == nodeTail) return (nodeHead.Value.Equals(value) || nodeTail.Value.Equals(value));

        while (nodeHead.Next != nodeTail && nodeHead != nodeTail)
        {
            if (nodeHead.Value.Equals(value)) return true;
            else nodeHead = nodeHead.Next;

            if (nodeTail.Value.Equals(value)) return true;
            else nodeTail = nodeTail.Previous;
        }
        return (nodeHead.Value.Equals(value) || nodeTail.Value.Equals(value));
    }

    //为新版的Tag结果转换json
    static public string ParseNewTags(string strJsonTags)
    {
        if (strJsonTags == null || strJsonTags.Trim() == "" || strJsonTags == "[]") return "[]";
        string result = "[";
        string[] jsons = strJsonTags.Trim('[').Trim(']').Split(',');
        foreach (string str in jsons)
        {
            string[] substrs = str.Trim('{').Trim('}').Split(':');
            if (substrs[0] != "\"weight\"")
                result += "{\"id\":" + substrs[0] + ",\"content\":" + substrs[1];
            else
                result += "," + substrs[0] + ":" + substrs[1] + "},";
        }
        result = result.TrimEnd(',') + "]";
        return result;
    }

    #region 转换成URL参数
    /// <summary>
    /// 转换成URL参数
    /// </summary>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static string ToQueryString(IDictionary<object, object> dictionary)
    {
        var sb = new StringBuilder();
        foreach (var key in dictionary.Keys)
        {
            var value = dictionary[key];
            if (value != null)
            {
                sb.Append(key + "=" + value + "&");
            }
        }
        return sb.ToString().TrimEnd('&');
    }

    public static string ToQueryString(IList<object> list, string key)
    {
        var sb = new StringBuilder();
        foreach (var val in list)
        {
            if (val != null)
            {
                sb.Append(key + "=" + Uri.EscapeDataString(val.ToString()) + "&");
            }
        }
        return sb.ToString().TrimEnd('&').Substring(key.Length + 1);
    }
    #endregion
}