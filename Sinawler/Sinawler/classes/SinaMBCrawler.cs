using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Xml;
using Sinawler.Model;
using Newtonsoft.Json;

namespace Sinawler
{
    //SinaMBCrawler类是通过新浪微博API从微博中抓取数据的类
    public class SinaMBCrawler
    {
        private SinaApiService api = GlobalPool.API;
        ///默认请求之前等待3.6秒钟，此值根据每小时100次的限制算得（每次3.6秒），但鉴于日志操作也有等待时间，故此值应能保证请求次数不超限
        ///后经测试，此值还可缩小。2010-10-11定为最小值2000，可调整
        ///2010-10-12定为不设下限
        ///2011-02-23设定为下限500ms
        private int iSleep = 3000;
        private bool blnStopCrawling = false;   //是否停止爬行
        private int iTryTimes = 10;             //the times that try to get XML

        public SinaMBCrawler()
        { }

        public int SleepTime
        {
            set { iSleep = value; }
            get { return iSleep; }
        }

        public bool StopCrawling
        {
            set { blnStopCrawling = value; }
            get { return blnStopCrawling; }
        }

        /// <summary>
        /// 获取指定UserID的指定个数的好友（即该用户所关注的人）ID列表
        /// </summary>
        /// <param name="lUid">要获取好友的用户ID</param>
        /// <param name="lCursor">分页时指示游标位置，详见API文档</param>
        /// <returns>好友ID列表</returns>
        public LinkedList<long> GetFriendsOf(long lUid, int iCursor)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<long> ids = new LinkedList<long>();
            string strResult = api.friends_ids(lUid, iCursor);
            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return ids;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.friends_ids(lUid, iCursor);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);
                if (strResult != null && strResult != "")
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(strResult);

                    XmlNodeList nodes = xmlDoc.GetElementsByTagName("id");
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        ids.AddLast(Convert.ToInt64(nodes[i].InnerText));
                    }
                }
            }//format=xml
            else
            {
                JsonIDs oJsonIDs = (JsonIDs)JsonConvert.DeserializeObject(strResult, typeof(JsonIDs));
                for (int i = 0; i < oJsonIDs.IDs.Length; i++)
                    ids.AddLast(oJsonIDs.IDs[i]);
            }//json
            return ids;
        }

        /// <summary>
        /// 获取指定UserID的指定个数的粉丝ID列表
        /// </summary>
        /// <param name="lUid">要获取好友的用户ID</param>
        /// <param name="lCursor">分页时指示游标位置，详见API文档</param>
        /// <returns>粉丝ID列表</returns>
        public LinkedList<long> GetFollowersOf(long lUid, int iCursor)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<long> ids = new LinkedList<long>();
            string strResult = api.followers_ids(lUid, iCursor);
            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return ids;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.followers_ids(lUid, iCursor);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);
                if (strResult != null && strResult != "")
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(strResult);

                    XmlNodeList nodes = xmlDoc.GetElementsByTagName("id");
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        ids.AddLast(Convert.ToInt64(nodes[i].InnerText));
                    }
                }
            }//format=xml
            else
            {
                JsonIDs oJsonIDs = (JsonIDs)JsonConvert.DeserializeObject(strResult, typeof(JsonIDs));
                for (int i = 0; i < oJsonIDs.IDs.Length; i++)
                    ids.AddLast(oJsonIDs.IDs[i]);
            }//json
            return ids;
        }

        //根据UserID抓取用户信息
        public User GetUserInfo(long lUid)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(lUid);
            if (strResult == "User Not Exist")  //用户不存在
                return null;
            if (blnStopCrawling)
                return user;

            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return null;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.user_show(lUid);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;

                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return user;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);

                user.user_id = Convert.ToInt64(xmlDoc.GetElementsByTagName("id")[0].InnerText);
                user.screen_name = xmlDoc.GetElementsByTagName("screen_name")[0].InnerText;
                user.name = xmlDoc.GetElementsByTagName("name")[0].InnerText;
                user.province = xmlDoc.GetElementsByTagName("province")[0].InnerText;
                user.city = xmlDoc.GetElementsByTagName("city")[0].InnerText;
                user.location = xmlDoc.GetElementsByTagName("location")[0].InnerText;
                user.description = xmlDoc.GetElementsByTagName("description")[0].InnerText;
                user.url = xmlDoc.GetElementsByTagName("url")[0].InnerText;
                user.profile_image_url = xmlDoc.GetElementsByTagName("profile_image_url")[0].InnerText;
                user.domain = xmlDoc.GetElementsByTagName("domain")[0].InnerText;
                user.gender = xmlDoc.GetElementsByTagName("gender")[0].InnerText;
                user.followers_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("followers_count")[0].InnerText);
                user.friends_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("friends_count")[0].InnerText);
                user.statuses_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("statuses_count")[0].InnerText);
                user.favourites_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("favourites_count")[0].InnerText);
                user.created_at = PubHelper.ParseDateTime(xmlDoc.GetElementsByTagName("created_at")[0].InnerText);
                if (xmlDoc.GetElementsByTagName("following")[0].InnerText == "false")
                    user.following = false;
                else
                    user.following = true;
                if (xmlDoc.GetElementsByTagName("verified")[0].InnerText == "false")
                    user.verified = false;
                else
                    user.verified = true;
                if (xmlDoc.GetElementsByTagName("allow_all_act_msg")[0].InnerText == "false")
                    user.allow_all_act_msg = false;
                else
                    user.allow_all_act_msg = true;
                if (xmlDoc.GetElementsByTagName("geo_enabled")[0].InnerText == "false")
                    user.geo_enabled = false;
                else
                    user.geo_enabled = true;
            }//format=xml
            else
            {
                user = (User)JsonConvert.DeserializeObject(strResult, typeof(User));
                user.created_at = PubHelper.ParseDateTime(user.created_at);
            }//json
            return user;
        }

        //根据用户昵称抓取用户信息
        public User GetUserInfo(string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(strScreenName);
            if (strResult == "User Not Exist")  //用户不存在
                return null;
            if (blnStopCrawling)
                return user;

            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return null;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.user_show(strScreenName);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                if (strResult == "User Not Exist")  //用户不存在
                    return null;
                if (blnStopCrawling)
                    return user;

                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return user;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);

                user.user_id = Convert.ToInt64(xmlDoc.GetElementsByTagName("id")[0].InnerText);
                user.screen_name = xmlDoc.GetElementsByTagName("screen_name")[0].InnerText;
                user.name = xmlDoc.GetElementsByTagName("name")[0].InnerText;
                user.province = xmlDoc.GetElementsByTagName("province")[0].InnerText;
                user.city = xmlDoc.GetElementsByTagName("city")[0].InnerText;
                user.location = xmlDoc.GetElementsByTagName("location")[0].InnerText;
                user.description = xmlDoc.GetElementsByTagName("description")[0].InnerText;
                user.url = xmlDoc.GetElementsByTagName("url")[0].InnerText;
                user.profile_image_url = xmlDoc.GetElementsByTagName("profile_image_url")[0].InnerText;
                user.domain = xmlDoc.GetElementsByTagName("domain")[0].InnerText;
                user.gender = xmlDoc.GetElementsByTagName("gender")[0].InnerText;
                user.followers_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("followers_count")[0].InnerText);
                user.friends_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("friends_count")[0].InnerText);
                user.statuses_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("statuses_count")[0].InnerText);
                user.favourites_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("favourites_count")[0].InnerText);
                user.created_at = PubHelper.ParseDateTime(xmlDoc.GetElementsByTagName("created_at")[0].InnerText);
                if (xmlDoc.GetElementsByTagName("following")[0].InnerText == "false")
                    user.following = false;
                else
                    user.following = true;
                if (xmlDoc.GetElementsByTagName("verified")[0].InnerText == "false")
                    user.verified = false;
                else
                    user.verified = true;
                if (xmlDoc.GetElementsByTagName("allow_all_act_msg")[0].InnerText == "false")
                    user.allow_all_act_msg = false;
                else
                    user.allow_all_act_msg = true;
                if (xmlDoc.GetElementsByTagName("geo_enabled")[0].InnerText == "false")
                    user.geo_enabled = false;
                else
                    user.geo_enabled = true;
            }//format=xml
            else
            {
                user = (User)JsonConvert.DeserializeObject(strResult, typeof(User));
                user.created_at = PubHelper.ParseDateTime(user.created_at);
            }//json
            return user;
        }

        //同时根据UserID和用户昵称抓取用户信息
        public User GetUserInfo(long lUid, string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(lUid, strScreenName);
            if (strResult == "User Not Exist")  //用户不存在
                return null;
            if (blnStopCrawling)
                return user;

            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return null;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.user_show(lUid, strScreenName);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                if (strResult == "User Not Exist")  //用户不存在
                    return null;
                if (blnStopCrawling)
                    return user;

                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return user;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);

                user.user_id = Convert.ToInt64(xmlDoc.GetElementsByTagName("id")[0].InnerText);
                user.screen_name = xmlDoc.GetElementsByTagName("screen_name")[0].InnerText;
                user.name = xmlDoc.GetElementsByTagName("name")[0].InnerText;
                user.province = xmlDoc.GetElementsByTagName("province")[0].InnerText;
                user.city = xmlDoc.GetElementsByTagName("city")[0].InnerText;
                user.location = xmlDoc.GetElementsByTagName("location")[0].InnerText;
                user.description = xmlDoc.GetElementsByTagName("description")[0].InnerText;
                user.url = xmlDoc.GetElementsByTagName("url")[0].InnerText;
                user.profile_image_url = xmlDoc.GetElementsByTagName("profile_image_url")[0].InnerText;
                user.domain = xmlDoc.GetElementsByTagName("domain")[0].InnerText;
                user.gender = xmlDoc.GetElementsByTagName("gender")[0].InnerText;
                user.followers_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("followers_count")[0].InnerText);
                user.friends_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("friends_count")[0].InnerText);
                user.statuses_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("statuses_count")[0].InnerText);
                user.favourites_count = Convert.ToInt32(xmlDoc.GetElementsByTagName("favourites_count")[0].InnerText);
                user.created_at = PubHelper.ParseDateTime(xmlDoc.GetElementsByTagName("created_at")[0].InnerText);
                if (xmlDoc.GetElementsByTagName("following")[0].InnerText == "false")
                    user.following = false;
                else
                    user.following = true;
                if (xmlDoc.GetElementsByTagName("verified")[0].InnerText == "false")
                    user.verified = false;
                else
                    user.verified = true;
                if (xmlDoc.GetElementsByTagName("allow_all_act_msg")[0].InnerText == "false")
                    user.allow_all_act_msg = false;
                else
                    user.allow_all_act_msg = true;
                if (xmlDoc.GetElementsByTagName("geo_enabled")[0].InnerText == "false")
                    user.geo_enabled = false;
                else
                    user.geo_enabled = true;
            }//format=xml
            else
            {
                user = (User)JsonConvert.DeserializeObject(strResult, typeof(User));
                user.created_at = PubHelper.ParseDateTime(user.created_at);
            }//json
            return user;
        }

        //抓取当前登录用户的信息
        public User GetCurrentUserInfo()
        {
            System.Threading.Thread.Sleep(iSleep);
            long lCurrentUserID = api.account_verify_credentials();
            if (lCurrentUserID == 0) return null;
            else return this.GetUserInfo(lCurrentUserID);
        }

        //transform a JsonStatus object to a Status object
        private Status JsonStatusToStatus(JsonStatus oJsonStatus)
        {
            Status status = new Status();
            status.created_at = PubHelper.ParseDateTime(oJsonStatus.created_at);
            status.status_id = oJsonStatus.id;
            status.content = oJsonStatus.text;
            status.source_url = oJsonStatus.source.Substring(9, oJsonStatus.source.IndexOf("rel") - 11);
            status.source_name = oJsonStatus.source.Substring(oJsonStatus.source.IndexOf('>') + 1, oJsonStatus.source.IndexOf("</") - oJsonStatus.source.IndexOf('>') - 1);
            status.favorited = oJsonStatus.favorited;
            status.truncated = oJsonStatus.truncated;
            if (oJsonStatus.geo != null)
            {
                status.geo_type = oJsonStatus.geo.type.ToLower();
                status.geo_coordinates_x = oJsonStatus.geo.coordinates[0];
                status.geo_coordinates_y = oJsonStatus.geo.coordinates[1];
            }
            if (oJsonStatus.in_reply_to_status_id != null && oJsonStatus.in_reply_to_status_id != "")
                status.in_reply_to_status_id = Convert.ToInt64(oJsonStatus.in_reply_to_status_id);
            if (oJsonStatus.in_reply_to_user_id != null && oJsonStatus.in_reply_to_user_id != "")
                status.in_reply_to_user_id = Convert.ToInt64(oJsonStatus.in_reply_to_user_id);
            status.in_reply_to_screen_name = oJsonStatus.in_reply_to_screen_name;
            status.mid = Convert.ToInt64(oJsonStatus.mid);
            status.user = oJsonStatus.user;
            status.user.created_at = PubHelper.ParseDateTime(status.user.created_at);
            if (oJsonStatus.retweeted_status != null)
            {
                status.retweeted_status = new Status();
                status.retweeted_status.created_at = PubHelper.ParseDateTime(oJsonStatus.retweeted_status.created_at);
                status.retweeted_status.status_id = Convert.ToInt64(oJsonStatus.retweeted_status.id);
                status.retweeted_status.content = oJsonStatus.retweeted_status.text;
                status.retweeted_status.source_url = oJsonStatus.retweeted_status.source.Substring(9, oJsonStatus.retweeted_status.source.IndexOf("rel") - 11);
                status.retweeted_status.source_name = oJsonStatus.retweeted_status.source.Substring(oJsonStatus.retweeted_status.source.IndexOf('>') + 1, oJsonStatus.retweeted_status.source.IndexOf("</") - oJsonStatus.retweeted_status.source.IndexOf('>') - 1);
                status.retweeted_status.favorited = oJsonStatus.retweeted_status.favorited;
                status.retweeted_status.truncated = oJsonStatus.retweeted_status.truncated;
                if (oJsonStatus.retweeted_status.geo != null)
                {
                    status.retweeted_status.geo_type = oJsonStatus.retweeted_status.geo.type.ToLower();
                    status.retweeted_status.geo_coordinates_x = oJsonStatus.retweeted_status.geo.coordinates[0];
                    status.retweeted_status.geo_coordinates_y = oJsonStatus.retweeted_status.geo.coordinates[1];
                }
                if (oJsonStatus.retweeted_status.in_reply_to_status_id != null && oJsonStatus.retweeted_status.in_reply_to_status_id != "")
                    status.retweeted_status.in_reply_to_status_id = Convert.ToInt64(oJsonStatus.retweeted_status.in_reply_to_status_id);
                if (oJsonStatus.retweeted_status.in_reply_to_user_id != null && oJsonStatus.retweeted_status.in_reply_to_user_id != "")
                    status.retweeted_status.in_reply_to_user_id = Convert.ToInt64(oJsonStatus.retweeted_status.in_reply_to_user_id);
                status.retweeted_status.in_reply_to_screen_name = oJsonStatus.retweeted_status.in_reply_to_screen_name;
                status.retweeted_status.mid = Convert.ToInt64(oJsonStatus.retweeted_status.mid);
                status.retweeted_status.user = oJsonStatus.retweeted_status.user;
                status.retweeted_status.user.created_at = PubHelper.ParseDateTime(status.retweeted_status.user.created_at);
            }
            return status;
        }

        //generate a Status object from an XML node
        private Status XMLNodeToStatus(XmlNode nodeStatus)
        {
            Status status = new Status();
            foreach (XmlNode node in nodeStatus.ChildNodes)
            {
                switch (node.Name.ToLower())
                {
                    case "created_at":
                        status.created_at = PubHelper.ParseDateTime(node.InnerText);
                        break;
                    case "id":
                        status.status_id = Convert.ToInt64(node.InnerText);
                        break;
                    case "text":
                        status.content = node.InnerText;
                        break;
                    case "source":
                        status.source_name = node.InnerText;
                        status.source_url = node.ChildNodes[0].Attributes["href"].Value;
                        break;
                    case "favorited":
                        if (node.InnerText == "false")
                            status.favorited = false;
                        else
                            status.favorited = true;
                        break;
                    case "truncated":
                        if (node.InnerText == "false")
                            status.truncated = false;
                        else
                            status.truncated = true;
                        break;
                    case "geo":
                        status.geo_type = node.ChildNodes[0].LocalName.ToLower();
                        string[] strCoordinates=node.InnerText.Split(' ');
                        status.geo_coordinates_x = Convert.ToDouble(strCoordinates[0]);
                        status.geo_coordinates_y = Convert.ToDouble(strCoordinates[1]);
                        break;
                    case "in_reply_to_status_id":
                        if (node.InnerText == "")
                            status.in_reply_to_status_id = 0;
                        else
                            status.in_reply_to_status_id = Convert.ToInt64(node.InnerText);
                        break;
                    case "in_reply_to_user_id":
                        if (node.InnerText == "")
                            status.in_reply_to_user_id = 0;
                        else
                            status.in_reply_to_user_id = Convert.ToInt64(node.InnerText);
                        break;
                    case "in_reply_to_screen_name":
                        status.in_reply_to_screen_name = node.InnerText;
                        break;
                    case "thumbnail_pic":
                        status.thumbnail_pic = node.InnerText;
                        break;
                    case "bmiddle_pic":
                        status.bmiddle_pic = node.InnerText;
                        break;
                    case "original_pic":
                        status.original_pic = node.InnerText;
                        break;
                    case "mid":
                        status.mid = Convert.ToInt64(node.InnerText);
                        break;
                    case "user":
                        foreach (XmlNode nodeUser in node.ChildNodes)
                        {
                            switch (nodeUser.Name.ToLower())
                            {
                                case "id":
                                    status.user.user_id = Convert.ToInt64(nodeUser.InnerText);
                                    break;
                                case "screen_name":
                                    status.user.screen_name = nodeUser.InnerText;
                                    break;
                                case "name":
                                    status.user.name = nodeUser.InnerText;
                                    break;
                                case "province":
                                    status.user.province = nodeUser.InnerText;
                                    break;
                                case "city":
                                    status.user.city = nodeUser.InnerText;
                                    break;
                                case "location":
                                    status.user.location = nodeUser.InnerText;
                                    break;
                                case "description":
                                    status.user.description = nodeUser.InnerText;
                                    break;
                                case "url":
                                    status.user.url = nodeUser.InnerText;
                                    break;
                                case "profile_image_url":
                                    status.user.profile_image_url = nodeUser.InnerText;
                                    break;
                                case "domain":
                                    status.user.domain = nodeUser.InnerText;
                                    break;
                                case "gender":
                                    status.user.gender = nodeUser.InnerText;
                                    break;
                                case "followers_count":
                                    status.user.followers_count = Convert.ToInt32(nodeUser.InnerText);
                                    break;
                                case "friends_count":
                                    status.user.friends_count = Convert.ToInt32(nodeUser.InnerText);
                                    break;
                                case "statuses_count":
                                    status.user.statuses_count = Convert.ToInt32(nodeUser.InnerText);
                                    break;
                                case "favourites_count":
                                    status.user.favourites_count = Convert.ToInt32(nodeUser.InnerText);
                                    break;
                                case "created_at":
                                    status.user.created_at = PubHelper.ParseDateTime(nodeUser.InnerText);
                                    break;
                                case "following":
                                    if (nodeUser.InnerText == "false")
                                        status.user.following = false;
                                    else
                                        status.user.following = true;
                                    break;
                                case "verified":
                                    if (nodeUser.InnerText == "false")
                                        status.user.verified = false;
                                    else
                                        status.user.verified = true;
                                    break;
                                case "allow_all_act_msg":
                                    if (nodeUser.InnerText == "false")
                                        status.user.allow_all_act_msg = false;
                                    else
                                        status.user.allow_all_act_msg = true;
                                    break;
                                case "geo_enabled":
                                    if (nodeUser.InnerText == "false")
                                        status.user.geo_enabled = false;
                                    else
                                        status.user.geo_enabled = true;
                                    break;
                            }//switch
                        }//for
                        break;
                    case "retweeted_status":
                        status.retweeted_status = new Status();
                        foreach (XmlNode retweeted_node in node.ChildNodes)
                        {
                            switch (retweeted_node.Name.ToLower())
                            {
                                case "created_at":
                                    status.retweeted_status.created_at = PubHelper.ParseDateTime(retweeted_node.InnerText);
                                    break;
                                case "id":
                                    status.retweeted_status.status_id = Convert.ToInt64(retweeted_node.InnerText);
                                    break;
                                case "text":
                                    status.retweeted_status.content = retweeted_node.InnerText;
                                    break;
                                case "source":
                                    status.retweeted_status.source_name = retweeted_node.ChildNodes[0].InnerText;
                                    status.retweeted_status.source_url = retweeted_node.ChildNodes[0].Attributes["href"].Value;
                                    break;
                                case "favorited":
                                    if (retweeted_node.InnerText == "false")
                                        status.retweeted_status.favorited = false;
                                    else
                                        status.retweeted_status.favorited = true;
                                    break;
                                case "truncated":
                                    if (retweeted_node.InnerText == "false")
                                        status.retweeted_status.truncated = false;
                                    else
                                        status.retweeted_status.truncated = true;
                                    break;
                                case "geo":
                                    status.retweeted_status.geo_type = retweeted_node.ChildNodes[0].LocalName.ToLower();
                                    strCoordinates = retweeted_node.InnerText.Split(' ');
                                    status.retweeted_status.geo_coordinates_x = Convert.ToDouble(strCoordinates[0]);
                                    status.retweeted_status.geo_coordinates_y = Convert.ToDouble(strCoordinates[1]);
                                    break;
                                case "in_reply_to_status_id":
                                    if (retweeted_node.InnerText == "")
                                        status.retweeted_status.in_reply_to_status_id = 0;
                                    else
                                        status.retweeted_status.in_reply_to_status_id = Convert.ToInt64(retweeted_node.InnerText);
                                    break;
                                case "in_reply_to_user_id":
                                    if (retweeted_node.InnerText == "")
                                        status.retweeted_status.in_reply_to_user_id = 0;
                                    else
                                        status.retweeted_status.in_reply_to_user_id = Convert.ToInt64(retweeted_node.InnerText);
                                    break;
                                case "in_reply_to_screen_name":
                                    status.retweeted_status.in_reply_to_screen_name = retweeted_node.InnerText;
                                    break;
                                case "thumbnail_pic":
                                    status.retweeted_status.thumbnail_pic = retweeted_node.InnerText;
                                    break;
                                case "bmiddle_pic":
                                    status.retweeted_status.bmiddle_pic = retweeted_node.InnerText;
                                    break;
                                case "original_pic":
                                    status.retweeted_status.original_pic = retweeted_node.InnerText;
                                    break;
                                case "mid":
                                    status.retweeted_status.mid = Convert.ToInt64(retweeted_node.InnerText);
                                    break;
                                case "user":
                                    foreach (XmlNode nodeUser in retweeted_node.ChildNodes)
                                    {
                                        switch (nodeUser.Name.ToLower())
                                        {
                                            case "id":
                                                status.retweeted_status.user.user_id = Convert.ToInt64(nodeUser.InnerText);
                                                break;
                                            case "screen_name":
                                                status.retweeted_status.user.screen_name = nodeUser.InnerText;
                                                break;
                                            case "name":
                                                status.retweeted_status.user.name = nodeUser.InnerText;
                                                break;
                                            case "province":
                                                status.retweeted_status.user.province = nodeUser.InnerText;
                                                break;
                                            case "city":
                                                status.retweeted_status.user.city = nodeUser.InnerText;
                                                break;
                                            case "location":
                                                status.retweeted_status.user.location = nodeUser.InnerText;
                                                break;
                                            case "description":
                                                status.retweeted_status.user.description = nodeUser.InnerText;
                                                break;
                                            case "url":
                                                status.retweeted_status.user.url = nodeUser.InnerText;
                                                break;
                                            case "profile_image_url":
                                                status.retweeted_status.user.profile_image_url = nodeUser.InnerText;
                                                break;
                                            case "domain":
                                                status.retweeted_status.user.domain = nodeUser.InnerText;
                                                break;
                                            case "gender":
                                                status.retweeted_status.user.gender = nodeUser.InnerText;
                                                break;
                                            case "followers_count":
                                                status.retweeted_status.user.followers_count = Convert.ToInt32(nodeUser.InnerText);
                                                break;
                                            case "friends_count":
                                                status.retweeted_status.user.friends_count = Convert.ToInt32(nodeUser.InnerText);
                                                break;
                                            case "statuses_count":
                                                status.retweeted_status.user.statuses_count = Convert.ToInt32(nodeUser.InnerText);
                                                break;
                                            case "favourites_count":
                                                status.retweeted_status.user.favourites_count = Convert.ToInt32(nodeUser.InnerText);
                                                break;
                                            case "created_at":
                                                status.retweeted_status.user.created_at = PubHelper.ParseDateTime(nodeUser.InnerText);
                                                break;
                                            case "following":
                                                if (nodeUser.InnerText == "false")
                                                    status.retweeted_status.user.following = false;
                                                else
                                                    status.retweeted_status.user.following = true;
                                                break;
                                            case "verified":
                                                if (nodeUser.InnerText == "false")
                                                    status.retweeted_status.user.verified = false;
                                                else
                                                    status.retweeted_status.user.verified = true;
                                                break;
                                            case "allow_all_act_msg":
                                                if (nodeUser.InnerText == "false")
                                                    status.retweeted_status.user.allow_all_act_msg = false;
                                                else
                                                    status.retweeted_status.user.allow_all_act_msg = true;
                                                break;
                                            case "geo_enabled":
                                                if (nodeUser.InnerText == "false")
                                                    status.retweeted_status.user.geo_enabled = false;
                                                else
                                                    status.retweeted_status.user.geo_enabled = true;
                                                break;
                                        }//switch
                                    }//for
                                    break;
                            }
                        }
                        break;
                }
            }
            return status;
        }

        /// <summary>
        /// 获取指定ID的微博
        /// </summary>
        /// <param name="lStatusID">要获取微博内容的微博ID</param>
        /// <returns>微博</returns>
        public Status GetStatus(long lStatusID)
        {
            System.Threading.Thread.Sleep(iSleep);
            Status status = new Status();
            string strResult = api.statuses_show(lStatusID);
            if (strResult == null) return null;
            if (blnStopCrawling || strResult.Contains("target weibo does not exist"))   //微博不存在
                return null;

            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return null;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.statuses_show(lStatusID);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return null;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);
                
                XmlNode nodeStatus = xmlDoc.GetElementsByTagName("status")[0];
                status=XMLNodeToStatus(nodeStatus);
            }//format=xml
            else
            {
                status = JsonStatusToStatus((JsonStatus)JsonConvert.DeserializeObject(strResult, typeof(JsonStatus)));
            }//json
            return status;
        }
        
        /// <summary>
        /// 获取指定UserID的指定微博ID之后的微博
        /// </summary>
        /// <param name="lUid">要获取微博内容的UserID</param>
        /// <param name="lSinceSid">只返回ID比lSinceSid大（比lSinceSid时间晚的）的微博信息内容</param>
        /// <returns>微博列表</returns>
        public LinkedList<Status> GetStatusesOfSince(long lUid, long lSinceSid)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<Status> lstStatuses = new LinkedList<Status>();
            string strResult = api.user_timeline(lUid, lSinceSid);
            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return lstStatuses;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.user_timeline(lUid, lSinceSid);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return lstStatuses;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);

                XmlNodeList nodes = xmlDoc.GetElementsByTagName("status");
                foreach (XmlNode nodeStatus in nodes)
                    lstStatuses.AddLast(XMLNodeToStatus(nodeStatus));
            }//format=xml
            else
            {
                JsonStatus[] oJsonStatuses = (JsonStatus[])JsonConvert.DeserializeObject(strResult, typeof(JsonStatus[]));
                foreach (JsonStatus oJsonStatus in oJsonStatuses)
                    lstStatuses.AddLast(JsonStatusToStatus(oJsonStatus));
            }//json
            return lstStatuses;
        }
        
        /// <summary>
        /// 获取指定微博评论
        /// </summary>
        /// <param name="lStatusID">要获取评论内容的微博ID</param>
        /// <returns>评论列表</returns>
        public LinkedList<Comment> GetCommentsOf(long lStatusID, int iPageNum)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<Comment> lstComments = new LinkedList<Comment>();
            string strResult = api.comments(lStatusID, iPageNum);
            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return lstComments;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.comments(lStatusID, iPageNum);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
                if (strResult.Trim() == "") return lstComments;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strResult);
                XmlNodeList nodes = xmlDoc.GetElementsByTagName("comment");

                foreach (XmlNode nodeComment in nodes)
                {
                    Comment comment = new Comment();
                    comment.status_id = lStatusID;
                    foreach (XmlNode node in nodeComment.ChildNodes)
                    {
                        switch (node.Name.ToLower())
                        {
                            case "created_at":
                                comment.created_at = PubHelper.ParseDateTime(node.InnerText);
                                break;
                            case "id":
                                comment.comment_id = Convert.ToInt64(node.InnerText);
                                break;
                            case "text":
                                comment.content = node.InnerText;
                                break;
                            case "mid":
                                comment.mid = Convert.ToInt64(node.InnerText);
                                break;
                            case "source":
                                comment.source_name = node.ChildNodes[0].InnerText;
                                comment.source_url = node.ChildNodes[0].Attributes["href"].Value;
                                break;
                            case "user":
                                foreach (XmlNode nodeUser in node.ChildNodes)
                                {
                                    switch (nodeUser.Name.ToLower())
                                    {
                                        case "id":
                                            comment.user.user_id = Convert.ToInt64(nodeUser.InnerText);
                                            break;
                                        case "screen_name":
                                            comment.user.screen_name = nodeUser.InnerText;
                                            break;
                                        case "name":
                                            comment.user.name = nodeUser.InnerText;
                                            break;
                                        case "province":
                                            comment.user.province = nodeUser.InnerText;
                                            break;
                                        case "city":
                                            comment.user.city = nodeUser.InnerText;
                                            break;
                                        case "location":
                                            comment.user.location = nodeUser.InnerText;
                                            break;
                                        case "description":
                                            comment.user.description = nodeUser.InnerText;
                                            break;
                                        case "url":
                                            comment.user.url = nodeUser.InnerText;
                                            break;
                                        case "profile_image_url":
                                            comment.user.profile_image_url = nodeUser.InnerText;
                                            break;
                                        case "domain":
                                            comment.user.domain = nodeUser.InnerText;
                                            break;
                                        case "gender":
                                            comment.user.gender = nodeUser.InnerText;
                                            break;
                                        case "followers_count":
                                            comment.user.followers_count = Convert.ToInt32(nodeUser.InnerText);
                                            break;
                                        case "friends_count":
                                            comment.user.friends_count = Convert.ToInt32(nodeUser.InnerText);
                                            break;
                                        case "statuses_count":
                                            comment.user.statuses_count = Convert.ToInt32(nodeUser.InnerText);
                                            break;
                                        case "favourites_count":
                                            comment.user.favourites_count = Convert.ToInt32(nodeUser.InnerText);
                                            break;
                                        case "created_at":
                                            comment.user.created_at = PubHelper.ParseDateTime(nodeUser.InnerText);
                                            break;
                                        case "following":
                                            if (nodeUser.InnerText == "false")
                                                comment.user.following = false;
                                            else
                                                comment.user.following = true;
                                            break;
                                        case "verified":
                                            if (nodeUser.InnerText == "false")
                                                comment.user.verified = false;
                                            else
                                                comment.user.verified = true;
                                            break;
                                        case "allow_all_act_msg":
                                            if (nodeUser.InnerText == "false")
                                                comment.user.allow_all_act_msg = false;
                                            else
                                                comment.user.allow_all_act_msg = true;
                                            break;
                                        case "geo_enabled":
                                            if (nodeUser.InnerText == "false")
                                                comment.user.geo_enabled = false;
                                            else
                                                comment.user.geo_enabled = true;
                                            break;
                                    }//switch
                                }//for
                                break;
                        }
                    }
                    lstComments.AddLast(comment);
                }
            }//xml
            else
            {
                JsonComment[] oJsonComments = (JsonComment[])JsonConvert.DeserializeObject(strResult, typeof(JsonComment[]));
                foreach (JsonComment oJsonComment in oJsonComments)
                {
                    Comment comment = new Comment();
                    comment.status_id = lStatusID;
                    comment.comment_id = oJsonComment.id;
                    comment.content = oJsonComment.text;
                    comment.source_url = oJsonComment.source.Substring(9, oJsonComment.source.IndexOf("rel") - 11);
                    comment.source_name = oJsonComment.source.Substring(oJsonComment.source.IndexOf('>') + 1, oJsonComment.source.IndexOf("</") - oJsonComment.source.IndexOf('>') - 1);
                    comment.mid = Convert.ToInt64(oJsonComment.mid);
                    comment.user = oJsonComment.user;
                    comment.user.created_at = PubHelper.ParseDateTime(comment.user.created_at);

                    lstComments.AddLast(comment);
                }
            }//json
            return lstComments;
        }

        /// <summary>
        /// 获取指定用户的标签
        /// </summary>
        /// <param name="lUserID">要获取标签的用户ID</param>
        /// <returns>标签</returns>
        public LinkedList<Tag> GetTagsOf(long lUserID)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<Tag> lstTags = new LinkedList<Tag>();
            string strResult = api.tags_of(lUserID);
            if (api.Format == "xml")
            {
                while ((strResult == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" || strResult == null) && !blnStopCrawling)
                {
                    if (iTryTimes == 0) return lstTags;
                    System.Threading.Thread.Sleep(100);
                    strResult = api.tags_of(lUserID);
                    iTryTimes--;
                    GlobalPool.RemainingHits--;
                }
                iTryTimes = 10;
                strResult = PubHelper.stripNonValidXMLCharacters(strResult);
                if (strResult != null && strResult != "")
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(strResult);

                    XmlNodeList nodes = xmlDoc.GetElementsByTagName("tag");
                    foreach (XmlNode node in nodes)
                    {
                        Tag tag = new Tag();
                        foreach (XmlNode attr in node.ChildNodes)
                        {
                            switch (attr.Name.ToLower())
                            {
                                case "id":
                                    tag.tag_id = Convert.ToInt64(attr.InnerText);
                                    break;
                                case "value":
                                    tag.tag = attr.InnerText;
                                    break;
                            }
                        }
                        lstTags.AddLast(tag);
                    }
                }
            }//xml
            else
            {
                Hashtable[] oJsonTags = (Hashtable[])JsonConvert.DeserializeObject(strResult, typeof(Hashtable[]));
                foreach (Hashtable oJsonTag in oJsonTags)
                {
                    Tag tag = new Tag();
                    foreach (DictionaryEntry de in oJsonTag)
                    {
                        tag.tag_id = Convert.ToInt64(de.Key);
                        tag.tag = de.Value.ToString();
                    }
                    lstTags.AddLast(tag);
                }
            }//json
            return lstTags;
        }
    };
}
