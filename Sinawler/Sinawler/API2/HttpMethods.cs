using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Net;
//using log4net;

namespace Open.Sina2SDK
{    
    #region 提交方式
    class HttpMethods : IHttpMethod
    {
        //readonly ILog logger = log4net.LogManager.GetLogger(typeof(HttpMethods));

        #region POST
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public virtual string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }
        #endregion

        #region Get
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public virtual string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }
        #endregion

        #region Post With Pic
        private string HttpPost(string url, IDictionary<object, object> param, string filePath)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "pic", filePath, "text/plain");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr= reader2.ReadToEnd();
                //logger.Debug(string.Format("File uploaded, server response is: {0}", responseStr));
            }
            catch
            {
                //logger.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return responseStr;
        }
        #endregion
        
        #region Post With Pic
        /// <summary>
        /// HTTP POST方式请求数据(带图片)
        /// </summary>
        /// <param name="url">URL</param>        
        /// <param name="param">POST的数据</param>
        /// <param name="fileByte">图片</param>
        /// <returns></returns>
        public virtual string HttpPost(string url, IDictionary<object, object> param, byte[] fileByte)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "pic", fileByte, "text/plain");//image/jpeg
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();
                //logger.Error(string.Format("File uploaded, server response is: {0}", responseStr));
            }
            catch
            {
                //logger.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return responseStr;
        }
        #endregion
    }
    #endregion

    #region 枚举
    /// <summary>
    /// 请求方式
    /// </summary>
    public enum Method
    {
        GET,
        POST,
        DELETE
    }

    /// <summary>
    /// 返回格式
    /// </summary>
    public enum Format
    {
        xml,
        json,
    }

    /// <summary>
    /// 微博内容返回类型
    /// </summary>
    public enum Feature
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = 0,
        /// <summary>
        /// 原创
        /// </summary>
        Original = 1,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 2,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 3,
        /// <summary>
        /// 音乐
        /// </summary>
        Music = 4,
    }

    /// <summary>
    /// 作者筛选类型
    /// </summary>
    public enum FilterByAuthor
    {
        /// <summary>
        /// 全部
        /// </summary>
        All=0,
        /// <summary>
        /// 我关注的人
        /// </summary>
        Follow=1,
        /// <summary>
        /// 陌生人
        /// </summary>
        Strange=2
    }

    /// <summary>
    /// 来源筛选类型
    /// </summary>
    public enum FilterBySource
    {
        /// <summary>
        /// 全部
        /// </summary>
        All=0,
        /// <summary>
        /// 来自微博
        /// </summary>
        Status=1,
        /// <summary>
        /// 来自微群
        /// </summary>
        Group=2
    }

    public enum FilterByType
    {
        /// <summary>
        /// 全部微博
        /// </summary>
        All = 0,
        /// <summary>
        /// 原创微博
        /// </summary>
        Original = 1
    }

    public enum EmotionsType
    {
        /// <summary>
        /// 普通表情
        /// </summary>
        face,
        /// <summary>
        /// 魔法表情
        /// </summary>
        ani,
        /// <summary>
        /// 动漫表情
        /// </summary>
        cartoon
    }

    public enum EmotionsLanguage
    {
        /// <summary>
        /// 简体
        /// </summary>
        cnname,
        /// <summary>
        /// 繁体
        /// </summary>
        twname
    }

    public enum CommentType
    {
        /// <summary>
        /// 不发表评论
        /// </summary>
        None = 0,
        /// <summary>
        /// 发表评论给当前微博
        /// </summary>
        RepostToCurrent = 1,
        /// <summary>
        /// 表评论给原微博
        /// </summary>
        RepostToOri = 2,
        /// <summary>
        /// 发表评论给当前微博,原微博
        /// </summary>
        All = 3
    }

    public enum HotCategory
    {
        /// <summary>
        /// 人气关注
        /// </summary>
        @default,
        /// <summary>
        /// 影视名星
        /// </summary>
        ent,
        /// <summary>
        /// 港台名人
        /// </summary>
        hk_famous,
        /// <summary>
        /// 模特
        /// </summary>
        model,
        /// <summary>
        /// 美食健康
        /// </summary>
        cooking,
        /// <summary>
        /// 体育名人
        /// </summary>
        sport,
        /// <summary>
        /// 商界名人
        /// </summary>
        finance,
        /// <summary>
        /// IT互联网
        /// </summary>
        tech,
        /// <summary>
        /// 歌手
        /// </summary>
        singer,
        /// <summary>
        /// 作家
        /// </summary>
        writer,
        /// <summary>
        /// 主持人
        /// </summary>
        moderator,
        /// <summary>
        /// 媒体总编
        /// </summary>
        medium,
        /// <summary>
        /// 炒股高手
        /// </summary>
        stockplayer
    }
    #endregion
}
