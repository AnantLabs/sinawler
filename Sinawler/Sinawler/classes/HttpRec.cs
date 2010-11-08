using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

public class HttpRec
{
    public string _Url = "http://login.xiaonei.com/Login.do";
    private string _strErr;
    private CookieContainer _CookieContainer = new CookieContainer();

    public string GetCode ()
    {
        HttpWebRequest rqq = (HttpWebRequest)HttpWebRequest.Create( _Url );
        rqq.Method = "Get";
        rqq.KeepAlive = true;
        if (rqq.CookieContainer == null)
            rqq.CookieContainer = _CookieContainer;
        HttpWebResponse rpp = (HttpWebResponse)rqq.GetResponse();
        return "";
    }

    public string LoginWeb ( string PostData )
    {
        string str = string.Empty;
        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create( _Url );//创建req
        req.Accept = "*/*"; //接受任意文件
        req.UserAgent = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727)"; // 模拟使用IE在浏览
        req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.8.1.16) Gecko/20080702 Firefox/2.0.0.16";
        req.KeepAlive = true;
        req.CookieContainer = _CookieContainer;

        if ((PostData != null & PostData.Length > 0) || _Url.Contains( "?" ))
        {
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] b = Encoding.Default.GetBytes( PostData );
            req.ContentLength = b.Length;
            req.AutomaticDecompression = DecompressionMethods.GZip;
            System.IO.Stream sw = null;
            try
            {
                sw = req.GetRequestStream();
                sw.Write( b, 0, b.Length );
            }
            catch (System.Exception ex)
            {
                this._strErr = ex.Message;
            }
            finally
            {
                if (sw != null) { sw.Close(); }
            }
        }

        HttpWebResponse rep = null;
        System.IO.StreamReader sr = null;
        try
        {
            req.Method = "GET";
            rep = (HttpWebResponse)req.GetResponse();
            sr = new System.IO.StreamReader( rep.GetResponseStream(), Encoding.UTF8 );
            str = sr.ReadToEnd();

            if (sr != null) sr.Close();
        }
        catch (Exception e)
        { }//MessageBox.Show( e.Message ); }

        return str;
    }
}