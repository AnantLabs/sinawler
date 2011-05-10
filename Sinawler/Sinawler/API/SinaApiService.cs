using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Web;
using Sinawler;

namespace Sina.Api
{
    public class SinaApiService : oAuthSina
    {
        private string strUserName = "";    //登录帐号
        private string strPassWord = "";         //登录密码

        public string UserName
        {
            get { return strUserName; }
        }

        public string PassWord
        {
            get { return strPassWord; }
        }

        public SinaApiService()
        {
            //default format
            Format = "xml";
        }

        //从新浪跳转回来，换取Access Token
        public bool oAuthWeb(string oauth_token, string oauth_verifier)
        {
            Verifier = oauth_verifier;
            AccessTokenGet();
            return true;
        }


        /// <summary>
        /// 这个是供桌面端应用调用的方法，需要用户提供用户名和密码
        /// </summary>
        /// <returns></returns>
        public bool oAuthDesktop(string userid, string passwd)
        {
            try
            {
                strUserName = userid;
                strPassWord = passwd;
                string authLink = AuthorizationGet();
                authLink += "&userId=" + userid + "&passwd=" + passwd + "&action=submit&oauth_callback=none";
                string html = WebRequest(Method.POST, authLink, null);
                string pin = ParseHtml(html);
                Verifier = pin;
                AccessTokenGet();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /**********************************************************************************************
         *************************************下行数据获取*********************************************
         **********************************************************************************************
         **********************************************************************************************/

        /*指定用户详细信息*/
        public string user_show (long user_id)
        {
            try
            {
                string url = "http://api.weibo.com/users/show." + Format + "?user_id=" + user_id.ToString();
                return oAuthWebRequest( Method.GET, url, String.Empty );;
            }
            catch(Exception ex) 
            {
                if (ex.Message.IndexOf( "(400)" ) > 0 || ex.Message.IndexOf("(500)")>0)   //返回400或500，认为用户不存在
                    return "User Not Exist";
                else
                    return null; 
            }
        }

        /*指定用户详细信息*/
        public string user_show ( string screen_name )
        {
            try
            {
                string url = "http://api.weibo.com/users/show." + Format + "?screen_name=" + screen_name;
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("(400)") > 0 || ex.Message.IndexOf("(500)") > 0)   //返回400或500，认为用户不存在
                    return "User Not Exist";
                else
                    return null;
            }
        }

        /*指定用户详细信息*/
        public string user_show ( long user_id, string screen_name )
        {
            try
            {
                string url = "http://api.weibo.com/users/show." + Format + "?user_id="+user_id.ToString()+"&screen_name=" + screen_name;
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf( "(400)" ) > 0)   //返回400，认为用户不存在
                    return "User Not Exist";
                else
                    return null;
            }
        }

        /*最新关注人微博*/
        public string friend_timeline()
        {
            try
            {
                string url = "http://api.weibo.com/statuses/friends_timeline." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch { return null; }
        }
        /*指定UserID的指定微博ID之后的微博列表*/
        public string user_timeline(long lUid, long lSinceID)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/user_timeline." + Format+"?user_id="+lUid.ToString()+"&count=200&since_id="+lSinceID.ToString();
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*最新n条@我的微博*/
        public string mentions()
        {
            try
            {
                string url = "http://api.weibo.com/statuses/mentions." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*最新评论*/
        public string comments_timeline()
        {
            try
            {
                string url = "http://api.weibo.com/statuses/comments_timeline." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*发出的评论*/
        public string comments_by_me()
        {
            try
            {
                string url = "http://api.weibo.com/statuses/comments_by_me." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*指定微博的指定页的评论列表（每页返回200条，要分页）*/
        public string comments(long status_id,int page)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/comments." + Format + "?id=" + status_id.ToString();
                url += "&count=200&page=" + page.ToString();
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch(Exception ex)
            { return null; }
        }
        /*批量获取一组微博的评论数及转发数*/
        public string counts(string ids)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/counts." + Format + "?ids=" + ids;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /**********************************************************************************************
         *************************************微博访问接口*********************************************
         **********************************************************************************************
         **********************************************************************************************/
        /*获取单条ID的微博信息*/
        public string statuses_show(long id)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/show/" + id.ToString() + "." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*获取单条ID的微博信息*/
        public string statuses_id(string id, string user_id)
        {
            try
            {
                string url = "http://api.weibo.com/" + user_id + "/statuses/" + id;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*发布一条微博信息*/
        public string statuses_update(string status)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/update." + Format + "?";
                return oAuthWebRequest(Method.POST, url, "status=" + HttpUtility.UrlEncode(status));
            }
            catch(Exception ex)
            { return null; }
        }
        /*上传图片并发布一条微博信息 */
        public string statuses_upload(string status, string pic)
        {
            try
            {
                string url = "http://api.weibo.com/statuses/upload." + Format + "?";
                return oAuthWebRequest(Method.POST, url, "status=" + HttpUtility.UrlEncode(status) + "&pic=" + HttpUtility.UrlEncode(pic));
            }
            catch
            { return null; }
        }
        /*验证当前用户身份是否合法，并返回用户ID*/
        public long account_verify_credentials ()
        {
            try
            {
                string url = "http://api.weibo.com/account/verify_credentials." + Format;
                string response = oAuthWebRequest( Method.GET, url, String.Empty );
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml( response );
                XmlNode nodeError = xmlDoc.GetElementsByTagName( "error" )[0];
                if (nodeError != null) return 0;   //出错，验证失败
                else return Convert.ToInt64( xmlDoc.GetElementsByTagName( "id" )[0].InnerText );
            }
            catch
            { return 0; }
        }

        /*关注某用户*/
        //领客康健网官方微博帐号ID:1679214941
        public string friendships_create(int user_id)
        {
            try
            {
                string url = "http://api.weibo.com/friendships/create." + Format + "?";
                return oAuthWebRequest(Method.POST, url, "user_id=" + user_id);
            }
            catch
            { return null; }
        }

        /*取消关注 */
        public string friendships_destroy(int user_id)
        {
            try
            {
                string url = "http://api.weibo.com/friendships/destroy." + Format + "?";
                return oAuthWebRequest(Method.POST, url, "user_id=" + user_id);
            }
            catch
            { return null; }
        }

        /*是否关注某用户 ,user_a关注user_b返回true*/
        public bool friendships_exists(int user_a, int user_b)
        {
            try
            {
                string url = "http://api.weibo.com/friendships/exists." + Format + "?";
                string response = oAuthWebRequest(Method.POST, url, "user_a=" + user_a + "&user_b=" + user_b);
                if (response.ToLower().Contains("true"))
                { return true; }
                else
                { return false; }
            }
            catch
            { return false; }
        }

        /*获取用户关注对象user_id列表 */
        /*
         * friends/ids 
        返回用户关注对象user_id列表 

        URL
        http://api.weibo.com/friends/ids.format 

        格式
        xml, json 

        HTTP请求方式
        GET 

        是否需要身份验证
        false 

        请求参数
        id. 选填参数. 要获取好友的UserID或微博昵称 
        o 示例: http://api.weibo.com/friends/ids/12345.xml or http://api.weibo.com/statuses/friends/bob.xml 

        user_id. 选填参数. 要获取的UserID 
        o 示例: http://api.weibo.com/friends/ids.xml?user_id=1401881 

        screen_name. 选填参数. 要获取的微博昵称 
        o 示例: http://api.weibo.com/friends/ids.xml?screen_name=101010 

        cursor. 选填参数. 单页只能包含5000个id，为了获取更多则cursor默认从-1开始，通过增加或减少cursor来获取更多的关注列表 
        o 示例: http://api.weibo.com/friends/ids.xml?cursor=-1 o 示例: http://api.weibo.com/friends/ids.xml?cursor=1300794057949944903 

        count. 可选参数. 每次返回的最大记录数（即页面大小），不大于5000，默认返回500。 
        o 示例: http://api.weibo.com/friends/ids.xml?&count=200 

        使用说明
        */
        public string friends_ids(long user_id,int cursor)
        {
            try
            {
                string url = "http://api.weibo.com/friends/ids/" + user_id.ToString() + "." + Format;
                url+="?count=5000&cursor="+cursor.ToString();
                return oAuthWebRequest(Method.GET,url,string.Empty);
            }
            catch(Exception ex)
            { return null; }
        }

        /*获取用户粉丝对象user_id列表 */
        /*
         * followers/ids 
        返回用户粉丝user_id列表，注意目前接口最多只返回5000个粉丝。 

        URL
        http://api.weibo.com/followers/ids.format 

        格式
        xml, json 

        HTTP请求方式
        GET 

        是否需要身份验证
        false 

        请求参数
        id. 选填参数. 要获取好友的UserID或微博昵称 
        o 示例: http://api.weibo.com/followers/ids/12345.xml or http://api.weibo.com/statuses/friends/bob.xml 
        user_id. 选填参数，要获取的UserID 
        o 示例: http://api.weibo.com/followers/ids.xml?user_id=1401881 
        screen_name. 选填参数，要获取的微博昵称 
        o 示例: http://api.weibo.com/followers/ids.xml?screen_name=101010 
        cursor. 选填参数. 单页只能包含5000个id，为了获取更多则cursor默认从-1开始，通过增加或减少cursor来获取更多的关注列表 
        o 示例: http://api.weibo.com/followers/ids.xml?cursor=-1 
        o 示例: http://api.weibo.com/followers/ids.xml?cursor=1300794057949944903 
        count. 可选参数. 每次返回的最大记录数（即页面大小），不大于5000，默认返回500。 
        o 示例: http://api.weibo.com/followers/ids.xml?&count=200 
        使用说明
        如果没有提供cursor参数，将只返回最前面的5000个粉丝id 
        */
        public string followers_ids ( long user_id, int cursor )
        {
            try
            {
                string url = "http://api.weibo.com/followers/ids/"+user_id.ToString()+"." + Format;
                url+="?cursor="+cursor.ToString()+"&count=5000";
                return oAuthWebRequest(Method.GET, url,string.Empty);
            }
            catch
            {
                return null;
            }
        }

        //发送一条私信
        //成功后返回私信ID，供删除用
        public int direct_messages_new(int user_id, string text)
        {
            try
            {
                string url = "http://api.weibo.com/direct_messages/new." + Format + "?";
                string response = oAuthWebRequest(Method.POST, url, "user_id=" + user_id + "&text=" + HttpUtility.UrlEncode(text));

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                int id = Convert.ToInt32(xmlDoc.GetElementsByTagName("id")[0].InnerText);
                return id;
            }
            catch
            { return 0; }
        }

        //删除一条私信
        public string direct_messages_destroy(int id)
        {
            try
            {
                string url = "http://api.weibo.com/direct_messages/destroy/" + id + "." + Format;
                return oAuthWebRequest(Method.POST, url, String.Empty);
            }
            catch
            { return null; }
        }

        //获取剩余请求次数
        public string check_hits_limit()
        {
            try
            {
                string url = "http://api.weibo.com/account/rate_limit_status." + Format;
                return oAuthWebRequest( Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }

        /*返回指定用户的标签列表 */
        /*
         URL
         http://api.weibo.com/tags.format 
         格式
         xml, json
         HTTP请求方式
         GET 
         是否需要登录:
         true 
         
         请求数限制
         true 
         
         请求参数
         user_id: 必填参数，查询用户的ID 
         示例: http://api.weibo.com/tags.xml?user_id=142272 
         count: 可选参数. 每次返回的最大记录数（即页面大小），不大于200，默认为20。 
         示例: http://api.weibo.com/tags.xml?user_id=12345&count=200 
         page: 可选参数. 返回结果的页序号。注意：有分页限制。 
         示例: http://api.weibo.com/tags.xml?user_id=12345&page=3 
         使用说明
         缺少参数，将返回400错误 
         用户ID不存在，将返回500错误 
        */
        public string tags_of(long lUid)
        {
            try
            {
                string url = "http://api.weibo.com/tags."+Format+"?user_id="+lUid.ToString();
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch
            { return null; }
        }
    }
}