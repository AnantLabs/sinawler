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
    public class SinaApiService : oAuthSina//,ISinaApiService
    {
        public SinaApiService()
        {
            //default format
            Format = "xml";
        }

        //从新浪跳转回来，换取Access Token
        public bool oAuthWeb(string oauth_token, string oauth_verifier)
        {
            Verifier = oauth_verifier;
            AccessTokenGet(oauth_token);
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
                string authLink = AuthorizationGet();
                authLink += "&userId=" + userid + "&passwd=" + passwd + "&action=submit&oauth_callback=none";
                string html = WebRequest(Method.POST, authLink, null);
                string pin = ParseHtml(html);
                Verifier = pin;
                AccessTokenGet(Token);
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
                string url = "http://api.t.sina.com.cn/users/show." + Format + "?user_id=" + user_id.ToString();
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch { return null; }
        }

        /*指定用户详细信息*/
        public string user_show ( string screen_name )
        {
            try
            {
                string url = "http://api.t.sina.com.cn/users/show." + Format + "?screen_name=" + screen_name;
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch { return null; }
        }

        /*指定用户详细信息*/
        public string user_show ( long user_id, string screen_name )
        {
            try
            {
                string url = "http://api.t.sina.com.cn/users/show." + Format + "?user_id="+user_id.ToString()+"&screen_name=" + screen_name;
                return oAuthWebRequest( Method.GET, url, String.Empty );
            }
            catch { return null; }
        }

        /*最新关注人微博*/
        public string friend_timeline()
        {
            try
            {
                string url = "http://api.t.sina.com.cn/statuses/friends_timeline." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch { return null; }
        }
        /*指定UID的指定微博ID之后的微博列表*/
        public string user_timeline(long lUid, long lSinceID)
        {
            try
            {
                string url = "http://api.t.sina.com.cn/statuses/user_timeline." + Format+"?user_id="+lUid.ToString()+"&count=200&since_id="+lSinceID.ToString();
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
                string url = "http://api.t.sina.com.cn/statuses/mentions." + Format;
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
                string url = "http://api.t.sina.com.cn/statuses/comments_timeline." + Format;
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
                string url = "http://api.t.sina.com.cn/statuses/comments_by_me." + Format;
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
                string url = "http://api.t.sina.com.cn/statuses/comments." + Format + "?id=" + status_id.ToString();
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
                string url = "http://api.t.sina.com.cn/statuses/counts." + Format + "?ids=" + ids;
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
                string url = "http://api.t.sina.com.cn/statuses/show/" + id.ToString() + "." + Format;
                return oAuthWebRequest(Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
        /*获取单条ID的微博信息*/
        public string statuses_id(string id, string uid)
        {
            try
            {
                string url = "http://api.t.sina.com.cn/" + uid + "/statuses/" + id;
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
                string url = "http://api.t.sina.com.cn/statuses/update." + Format + "?";
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
                string url = "http://api.t.sina.com.cn/statuses/upload." + Format + "?";
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
                string url = "http://api.t.sina.com.cn/account/verify_credentials." + Format;
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
                string url = "http://api.t.sina.com.cn/friendships/create." + Format + "?";
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
                string url = "http://api.t.sina.com.cn/friendships/destroy." + Format + "?";
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
                string url = "http://api.t.sina.com.cn/friendships/exists." + Format + "?";
                string response = oAuthWebRequest(Method.POST, url, "user_a=" + user_a + "&user_b=" + user_b);
                if (response.ToLower().Contains("true"))
                { return true; }
                else
                { return false; }
            }
            catch
            { return false; }
        }

        /*获取用户关注对象uid列表 */
        /*
         * friends/ids 
        返回用户关注对象uid列表 

        URL
        http://api.t.sina.com.cn/friends/ids.format 

        格式
        xml, json 

        HTTP请求方式
        GET 

        是否需要身份验证
        false 

        请求参数
        id. 选填参数. 要获取好友的UID或微博昵称 
        o 示例: http://api.t.sina.com.cn/friends/ids/12345.xml or http://api.t.sina.com.cn/statuses/friends/bob.xml 

        user_id. 选填参数. 要获取的UID 
        o 示例: http://api.t.sina.com.cn/friends/ids.xml?user_id=1401881 

        screen_name. 选填参数. 要获取的微博昵称 
        o 示例: http://api.t.sina.com.cn/friends/ids.xml?screen_name=101010 

        cursor. 选填参数. 单页只能包含5000个id，为了获取更多则cursor默认从-1开始，通过增加或减少cursor来获取更多的关注列表 
        o 示例: http://api.t.sina.com.cn/friends/ids.xml?cursor=-1 o 示例: http://api.t.sina.com.cn/friends/ids.xml?cursor=1300794057949944903 

        count. 可选参数. 每次返回的最大记录数（即页面大小），不大于5000，默认返回500。 
        o 示例: http://api.t.sina.com.cn/friends/ids.xml?&count=200 

        使用说明
        */
        public LinkedList<long> friends_ids(long user_id,int cursor)
        {
            LinkedList<long> ids = new LinkedList<long>();
            try
            {
                string url = "http://api.t.sina.com.cn/friends/ids/" + user_id.ToString() + "." + Format;
                url+="?count=5000&cursor="+cursor.ToString();
                string response = oAuthWebRequest(Method.GET,url,string.Empty);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                
                XmlNodeList nodes = xmlDoc.GetElementsByTagName("id");
                for (int i = 0; i < nodes.Count; i++)
                {
                    ids.AddLast(Convert.ToInt64(nodes[i].InnerText));
                }
            }
            catch(Exception ex)
            {  }
            return ids;
        }

        /*获取用户粉丝对象uid列表 */
        /*
         * followers/ids 
        返回用户粉丝uid列表，注意目前接口最多只返回5000个粉丝。 

        URL
        http://api.t.sina.com.cn/followers/ids.format 

        格式
        xml, json 

        HTTP请求方式
        GET 

        是否需要身份验证
        false 

        请求参数
        id. 选填参数. 要获取好友的UID或微博昵称 
        o 示例: http://api.t.sina.com.cn/followers/ids/12345.xml or http://api.t.sina.com.cn/statuses/friends/bob.xml 
        user_id. 选填参数，要获取的UID 
        o 示例: http://api.t.sina.com.cn/followers/ids.xml?user_id=1401881 
        screen_name. 选填参数，要获取的微博昵称 
        o 示例: http://api.t.sina.com.cn/followers/ids.xml?screen_name=101010 
        cursor. 选填参数. 单页只能包含5000个id，为了获取更多则cursor默认从-1开始，通过增加或减少cursor来获取更多的关注列表 
        o 示例: http://api.t.sina.com.cn/followers/ids.xml?cursor=-1 
        o 示例: http://api.t.sina.com.cn/followers/ids.xml?cursor=1300794057949944903 
        count. 可选参数. 每次返回的最大记录数（即页面大小），不大于5000，默认返回500。 
        o 示例: http://api.t.sina.com.cn/followers/ids.xml?&count=200 
        使用说明
        如果没有提供cursor参数，将只返回最前面的5000个粉丝id 
        */
        public LinkedList<long> followers_ids ( long user_id, int cursor )
        {
            LinkedList<long> ids = new LinkedList<long>();
            try
            {
                string url = "http://api.t.sina.com.cn/followers/ids/"+user_id.ToString()+"." + Format;
                url+="?cursor="+cursor.ToString()+"&count=5000";
                string response = oAuthWebRequest(Method.GET, url,string.Empty);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                
                XmlNodeList nodes = xmlDoc.GetElementsByTagName("id");
                for (int i = 0; i < nodes.Count; i++)
                {
                    ids.AddLast( Convert.ToInt64( nodes[i].InnerText ) );
                }
            }
            catch{}
            return ids;
        }

        //发送一条私信
        //成功后返回私信ID，供删除用
        public int direct_messages_new(int user_id, string text)
        {
            try
            {
                string url = "http://api.t.sina.com.cn/direct_messages/new." + Format + "?";
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
                string url = "http://api.t.sina.com.cn/direct_messages/destroy/" + id + "." + Format;
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
                string url = "http://api.t.sina.com.cn/account/rate_limit_status." + Format;
                return oAuthWebRequest( Method.GET, url, String.Empty);
            }
            catch
            { return null; }
        }
    }
}