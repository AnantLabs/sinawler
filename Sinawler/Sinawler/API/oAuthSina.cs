using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;
using Sinawler;

namespace Sina.Api
{
    public class oAuthSina : oAuthBase
    {
        public enum Method { GET, POST, PUT, DELETE };

        public const string REQUEST_TOKEN = "http://api.t.sina.com.cn/oauth/request_token";
        public const string AUTHORIZE = "http://api.t.sina.com.cn/oauth/authorize";
        public const string ACCESS_TOKEN = "http://api.t.sina.com.cn/oauth/access_token";

        private string _appKey = "";
        private string _appSecret = "";
        private string _token = "";
        private string _tokenSecret = "";
        private string _format = "json";

        #region Properties
        public string Format 
        { 
            get{return _format;}
            set{_format=value;}
        }
        public string appKey
        {
            get { return _appKey;  }
            set { _appKey = value; }
        }

        public string appSecret
        {
            get { return _appSecret;  }
            set { _appSecret = value; }
        }

        public string Token { get { return _token; } set { _token = value; } }
        public string TokenSecret { get { return _tokenSecret; } set { _tokenSecret = value; } }
        #endregion

        /// <summary>
        /// 获取未授权的Request Token【后台进行】
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string RequestTokenGet()
        {
            string response = oAuthWebRequest( Method.GET, REQUEST_TOKEN, String.Empty );
            if (response.Length > 0)
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.Token = qs["oauth_token"];
                    this.TokenSecret = qs["oauth_token_secret"];                    
                    //ret = AUTHORIZE + "?oauth_token=" + this.Token;
                }
            }
            return this.Token;
        }

        /// <summary>
        /// 获取用户授权的Request Token服务地址【用户参与】
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationGet()
        {
            string ret = null;
            ret = AUTHORIZE + "?oauth_token=" + RequestTokenGet();            
            return ret;
        }

        /// <summary>
        /// 用授权的Request Token换取Access Token【后台进行】
        /// </summary>
        /// <param name="authToken">oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public void AccessTokenGet()
        {
            string response = oAuthWebRequest(Method.GET, ACCESS_TOKEN, string.Empty);

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.Token = qs["oauth_token"];
                    //HttpContext.Current.Session["oauth_token"] = this.Token;
                }
                if (qs["oauth_token_secret"] != null)
                {
                    this.TokenSecret = qs["oauth_token_secret"];
                    //HttpContext.Current.Session["oauth_token_secret"] = this.TokenSecret;
                }
            }
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string oAuthWebRequest(Method method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";
            string filePath = "";

            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == Method.POST || method == Method.PUT)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        //判断是否包含图片参数
                        if (key == "pic")
                        { filePath = qs[key]; }
                        else
                        {
                            if (postData.Length > 0)
                            {
                                postData += "&";
                            }
                            qs[key] = HttpUtility.UrlEncode(qs[key]);
                            qs[key] = this.UrlEncode(qs[key]);
                            postData += (key + "=" + qs[key]);
                        }
                    }
                    if (url.IndexOf("?") > 0)
                    {
                        //url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }

            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                this.appKey,
                this.appSecret,
                this.Token,
                this.TokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                out outUrl,
                out querystring);


            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            //Convert the querystring to postData
            if (method == Method.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }
            if (filePath != "")
            { ret = WebRequestWithFile(method, outUrl + querystring, postData, filePath); }
            else if (method == Method.POST || method == Method.GET)
            { ret = WebRequest(method, outUrl + querystring, postData); }
            //else if (method == Method.PUT)
            //ret = WebRequestWithPut(Method.PUT,outUrl + querystring, postData);
            return ret;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequestWithPut(Method method, string url, string postData)
        {

            //oauth_consumer_key=5s5OSkySDF_mG_dLkduT3l7gokQ68hBtEpY5a9ebJVDH2r5BG8Opb6mUQhPgmQEB
            //oauth_nonce=9708457
            //oauth_signature_method=HMAC-SHA1
            //oauth_timestamp=1259135648
            //oauth_token=387026c7-27bd-4a11-b76e-fbe052ce88ad
            //oauth_verifier=31838
            //oauth_version=1.0
            //oauth_signature=4wYKoBy4ndrR4ziDNTd5mQV%2fcLY%3d

            //url = "http://api.linkedin.com/v1/people/~/current-status";
            Uri uri = new Uri(url);

            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            string outUrl, querystring;

            //Generate Signature
            string sig = this.GenerateSignatureBase(uri,
                this.appKey,
                this.appSecret,
                this.Token,
                this.TokenSecret,
                "PUT",
                timeStamp,
                nonce,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);
            NameValueCollection qs = HttpUtility.ParseQueryString(querystring);
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<current-status>is setting their status using the LinkedIn API.</current-status>";

            HttpWebRequest webRequest = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.ContentType = "text/xml";
            webRequest.Method = "PUT";
            webRequest.ServicePoint.Expect100Continue = false;

            webRequest.Headers.Add("Authorization", "OAuth realm=\"\"");
            webRequest.Headers.Add("oauth_consumer_key", this.appKey);
            webRequest.Headers.Add("oauth_token", this.Token);
            webRequest.Headers.Add("oauth_signature_method", "HMAC-SHA1");
            webRequest.Headers.Add("oauth_signature", sig);
            webRequest.Headers.Add("oauth_timestamp", timeStamp);
            webRequest.Headers.Add("oauth_nonce", nonce);
            webRequest.Headers.Add("oauth_verifier", this.Verifier);
            webRequest.Headers.Add("oauth_version", "1.0");

            //webRequest.KeepAlive = true;

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            try
            {
                requestWriter.Write(postData);
            }
            catch
            {
                throw;
            }
            finally
            {
                requestWriter.Close();
                requestWriter = null;
            }


            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string returnString = response.StatusCode.ToString();

            webRequest = null;

            return responseData;

        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            //webRequest.UserAgent  = "Identify your application please.";
            //webRequest.Timeout = 20000;

            if (method == Method.POST || method == Method.PUT)
            {
                if (method == Method.PUT)
                {
                    webRequest.ContentType = "text/xml";
                    webRequest.Method = "PUT";
                }
                else
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                //webRequest.ContentType = "multipart/form-data";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;

        }

        /// <summary>
        /// 带有文件/图片的请求
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string WebRequestWithFile(Method method, string url, string postData, string filePath)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            //webRequest.UserAgent  = "Identify your application please.";
            //webRequest.Timeout = 20000;

            string boundary = "--iLinkeeSinaApi";

            if (method == Method.POST || method == Method.PUT)
            {
                webRequest.ContentType = "multipart/form-data;boundary=" + boundary;

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                    //图片
                    //$multipartbody .= $MPboundary . "\r\n";
                    //$multipartbody .= 'Content-Disposition: form-data; name="pic"; filename="wiki.png"'. "\r\n";
                    //$multipartbody .= 'Content-Type: image/png'. "\r\n\r\n";
                    //$multipartbody .= $file. "\r\n";
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int length = (int)fs.Length;
                    byte[] bytes = br.ReadBytes(length);

                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append(boundary + "\r\n");
                    sb1.Append("Content-Disposition: form-data; name=\"pic\"; filename=\"test.jpg\"\r\n");
                    sb1.Append("Content-Type: image/jpeg\r\n\r\n");
                    sb1.Append(bytes.ToString() + "\r\n");
                    requestWriter.Write(sb1.ToString()); 

                    //$k = "source";
                    //这里改成 appkey
                    //$v = "1861823087";
                    //$multipartbody .= $MPboundary . "\r\n";
                    //$multipartbody.='content-disposition: form-data; name="'.$k."\"\r\n\r\n";
                    //$multipartbody.=$v."\r\n";
                    StringBuilder sb2 = new StringBuilder();
                    sb2.Append(boundary + "\r\n");
                    sb2.Append("content-disposition: form-data; name=\"source\"\r\n\r\n");
                    sb2.Append(appKey + "\r\n");
                    requestWriter.Write(sb2.ToString()); 

                    //$k = "status";
                    //$v = "要上传的文件，这里是描述文字";
                    //$multipartbody .= $MPboundary . "\r\n";
                    //$multipartbody.='content-disposition: form-data; name="'.$k."\"\r\n\r\n";
                    //$multipartbody.=$v."\r\n";
                    //$multipartbody .= "\r\n". $endMPboundary;
                    StringBuilder sb3 = new StringBuilder();
                    sb3.Append(boundary + "\r\n");
                    sb3.Append("content-disposition: form-data; name=\"status\"\r\n\r\n");
                    sb3.Append("要上传的文件，这里是描述文字\r\n\r\n");
                    sb3.Append(boundary + "--");
                    requestWriter.Write(sb3.ToString()); 

                    
                    fs.Close();
                    br.Close();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }

        public string ParseHtml(string html)
        {
            string strPin="";

            Regex htmlRegex = new Regex("<b>[0-9]{6}</b>");
            Match m = htmlRegex.Match(html);
            Regex pinRegex = new Regex("[0-9]{6}");
            Match m1 = pinRegex.Match(m.Value);

            Regex reg = new Regex("获取到的授权码：<span class=\"fb\">([0-9]+)");
            strPin= reg.Match(html).Groups[1].Value;

            if (strPin == "")
            {
                Regex reg2 = new Regex("获取到授权码：([0-9]+)");
                strPin = reg.Match(html).Groups[1].Value;
            }

            return strPin;
        }
    }
}