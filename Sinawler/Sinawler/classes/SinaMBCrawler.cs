using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Xml;
using Sinawler.Model;
using System.Windows.Forms;

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
            return ids;
        }

        //根据UserID抓取用户信息
        public User GetUserInfo(long lUid)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(lUid);
            while (strResult == null && !blnStopCrawling)
                strResult = api.user_show(lUid);
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
            return user;
        }

        //根据用户昵称抓取用户信息
        public User GetUserInfo(string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(strScreenName);
            while (strResult == null && !blnStopCrawling)
                strResult = api.user_show(strScreenName);
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
            return user;
        }

        //同时根据UserID和用户昵称抓取用户信息
        public User GetUserInfo(long lUid, string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = new User();
            user.user_id = -1;
            string strResult = api.user_show(lUid, strScreenName);
            while (strResult == null && !blnStopCrawling)
                strResult = api.user_show(lUid, strScreenName);
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

        /// <summary>
        /// 获取指定ID的微博
        /// </summary>
        /// <param name="lStatusID">要获取微博内容的微博ID</param>
        /// <returns>微博</returns>
        public Status GetStatus(long lStatusID)
        {
            System.Threading.Thread.Sleep(iSleep);
            string strResult = api.statuses_show(lStatusID);
            if (strResult == null) return null;
            strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
            if (strResult.Trim() == "") return null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            Status status = new Status();
            XmlNode nodeStatus = xmlDoc.GetElementsByTagName("status")[0];
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
                        status.geo = node.InnerText;
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
                    case "user":
                        status.user.user_id = Convert.ToInt64(node.ChildNodes[0].InnerText);
                        status.user.screen_name = node.ChildNodes[1].InnerText;
                        status.user.name = node.ChildNodes[2].InnerText;
                        status.user.province = node.ChildNodes[3].InnerText;
                        status.user.city = node.ChildNodes[4].InnerText;
                        status.user.location = node.ChildNodes[5].InnerText;
                        status.user.description = node.ChildNodes[6].InnerText;
                        status.user.url = node.ChildNodes[7].InnerText;
                        status.user.profile_image_url = node.ChildNodes[8].InnerText;
                        status.user.domain = node.ChildNodes[9].InnerText;
                        status.user.gender = node.ChildNodes[10].InnerText;
                        status.user.followers_count = Convert.ToInt32(node.ChildNodes[11].InnerText);
                        status.user.friends_count = Convert.ToInt32(node.ChildNodes[12].InnerText);
                        status.user.statuses_count = Convert.ToInt32(node.ChildNodes[13].InnerText);
                        status.user.favourites_count = Convert.ToInt32(node.ChildNodes[14].InnerText);
                        status.user.created_at = PubHelper.ParseDateTime(node.ChildNodes[15].InnerText);
                        if (node.ChildNodes[16].InnerText == "false")
                            status.user.following = false;
                        else
                            status.user.following = true;
                        if (node.ChildNodes[17].InnerText == "false")
                            status.user.verified = false;
                        else
                            status.user.verified = true;
                        if (node.ChildNodes[18].InnerText == "false")
                            status.user.allow_all_act_msg = false;
                        else
                            status.user.allow_all_act_msg = true;
                        if (node.ChildNodes[19].InnerText == "false")
                            status.user.geo_enabled = false;
                        else
                            status.user.geo_enabled = true;
                        break;
                    case "retweeted_status":
                        status.retweeted_status = new Status();
                        status.retweeted_status.status_id = Convert.ToInt64(node.ChildNodes[1].InnerText);
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
                                    status.retweeted_status.source_name = node.ChildNodes[3].ChildNodes[0].InnerText;
                                    status.retweeted_status.source_url = node.ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
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
                                    status.retweeted_status.geo = retweeted_node.InnerText;
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
                                case "user":
                                    status.retweeted_status.user.user_id = Convert.ToInt64(retweeted_node.ChildNodes[0].InnerText);
                                    status.retweeted_status.user.screen_name = retweeted_node.ChildNodes[1].InnerText;
                                    status.retweeted_status.user.name = retweeted_node.ChildNodes[2].InnerText;
                                    status.retweeted_status.user.province = retweeted_node.ChildNodes[3].InnerText;
                                    status.retweeted_status.user.city = retweeted_node.ChildNodes[4].InnerText;
                                    status.retweeted_status.user.location = retweeted_node.ChildNodes[5].InnerText;
                                    status.retweeted_status.user.description = retweeted_node.ChildNodes[6].InnerText;
                                    status.retweeted_status.user.url = retweeted_node.ChildNodes[7].InnerText;
                                    status.retweeted_status.user.profile_image_url = retweeted_node.ChildNodes[8].InnerText;
                                    status.retweeted_status.user.domain = retweeted_node.ChildNodes[9].InnerText;
                                    status.retweeted_status.user.gender = retweeted_node.ChildNodes[10].InnerText;
                                    status.retweeted_status.user.followers_count = Convert.ToInt32(retweeted_node.ChildNodes[11].InnerText);
                                    status.retweeted_status.user.friends_count = Convert.ToInt32(retweeted_node.ChildNodes[12].InnerText);
                                    status.retweeted_status.user.statuses_count = Convert.ToInt32(retweeted_node.ChildNodes[13].InnerText);
                                    status.retweeted_status.user.favourites_count = Convert.ToInt32(retweeted_node.ChildNodes[14].InnerText);
                                    status.retweeted_status.user.created_at = PubHelper.ParseDateTime(retweeted_node.ChildNodes[15].InnerText);
                                    if (retweeted_node.ChildNodes[16].InnerText == "false")
                                        status.retweeted_status.user.following = false;
                                    else
                                        status.retweeted_status.user.following = true;
                                    if (retweeted_node.ChildNodes[17].InnerText == "false")
                                        status.retweeted_status.user.verified = false;
                                    else
                                        status.retweeted_status.user.verified = true;
                                    if (retweeted_node.ChildNodes[18].InnerText == "false")
                                        status.retweeted_status.user.allow_all_act_msg = false;
                                    else
                                        status.retweeted_status.user.allow_all_act_msg = true;
                                    if (retweeted_node.ChildNodes[19].InnerText == "false")
                                        status.retweeted_status.user.geo_enabled = false;
                                    else
                                        status.retweeted_status.user.geo_enabled = true;
                                    break;
                            }
                        }
                        break;
                }
            }
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
            while (strResult == null && !blnStopCrawling)
                strResult = api.user_timeline(lUid, lSinceSid);
            strResult = PubHelper.stripNonValidXMLCharacters(strResult);  //过滤XML中的无效字符
            if (strResult.Trim() == "") return lstStatuses;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            XmlNodeList nodes = xmlDoc.GetElementsByTagName("status");
            foreach (XmlNode nodeStatus in nodes)
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
                            status.source_name = nodeStatus.ChildNodes[3].ChildNodes[0].InnerText;
                            status.source_url = nodeStatus.ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
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
                            status.geo = node.InnerText;
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
                        case "user":
                            status.user.user_id = Convert.ToInt64(node.ChildNodes[0].InnerText);
                            status.user.screen_name = node.ChildNodes[1].InnerText;
                            status.user.name = node.ChildNodes[2].InnerText;
                            status.user.province = node.ChildNodes[3].InnerText;
                            status.user.city = node.ChildNodes[4].InnerText;
                            status.user.location = node.ChildNodes[5].InnerText;
                            status.user.description = node.ChildNodes[6].InnerText;
                            status.user.url = node.ChildNodes[7].InnerText;
                            status.user.profile_image_url = node.ChildNodes[8].InnerText;
                            status.user.domain = node.ChildNodes[9].InnerText;
                            status.user.gender = node.ChildNodes[10].InnerText;
                            status.user.followers_count = Convert.ToInt32(node.ChildNodes[11].InnerText);
                            status.user.friends_count = Convert.ToInt32(node.ChildNodes[12].InnerText);
                            status.user.statuses_count = Convert.ToInt32(node.ChildNodes[13].InnerText);
                            status.user.favourites_count = Convert.ToInt32(node.ChildNodes[14].InnerText);
                            status.user.created_at = PubHelper.ParseDateTime(node.ChildNodes[15].InnerText);
                            if (node.ChildNodes[16].InnerText == "false")
                                status.user.following = false;
                            else
                                status.user.following = true;
                            if (node.ChildNodes[17].InnerText == "false")
                                status.user.verified = false;
                            else
                                status.user.verified = true;
                            if (node.ChildNodes[18].InnerText == "false")
                                status.user.allow_all_act_msg = false;
                            else
                                status.user.allow_all_act_msg = true;
                            if (node.ChildNodes[19].InnerText == "false")
                                status.user.geo_enabled = false;
                            else
                                status.user.geo_enabled = true;
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
                                        status.retweeted_status.source_name = node.ChildNodes[3].ChildNodes[0].InnerText;
                                        status.retweeted_status.source_url = node.ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
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
                                        status.retweeted_status.geo = retweeted_node.InnerText;
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
                                    case "user":
                                        status.retweeted_status.user.user_id = Convert.ToInt64(retweeted_node.ChildNodes[0].InnerText);
                                        status.retweeted_status.user.screen_name = retweeted_node.ChildNodes[1].InnerText;
                                        status.retweeted_status.user.name = retweeted_node.ChildNodes[2].InnerText;
                                        status.retweeted_status.user.province = retweeted_node.ChildNodes[3].InnerText;
                                        status.retweeted_status.user.city = retweeted_node.ChildNodes[4].InnerText;
                                        status.retweeted_status.user.location = retweeted_node.ChildNodes[5].InnerText;
                                        status.retweeted_status.user.description = retweeted_node.ChildNodes[6].InnerText;
                                        status.retweeted_status.user.url = retweeted_node.ChildNodes[7].InnerText;
                                        status.retweeted_status.user.profile_image_url = retweeted_node.ChildNodes[8].InnerText;
                                        status.retweeted_status.user.domain = retweeted_node.ChildNodes[9].InnerText;
                                        status.retweeted_status.user.gender = retweeted_node.ChildNodes[10].InnerText;
                                        status.retweeted_status.user.followers_count = Convert.ToInt32(retweeted_node.ChildNodes[11].InnerText);
                                        status.retweeted_status.user.friends_count = Convert.ToInt32(retweeted_node.ChildNodes[12].InnerText);
                                        status.retweeted_status.user.statuses_count = Convert.ToInt32(retweeted_node.ChildNodes[13].InnerText);
                                        status.retweeted_status.user.favourites_count = Convert.ToInt32(retweeted_node.ChildNodes[14].InnerText);
                                        status.retweeted_status.user.created_at = PubHelper.ParseDateTime(retweeted_node.ChildNodes[15].InnerText);
                                        if (retweeted_node.ChildNodes[16].InnerText == "false")
                                            status.retweeted_status.user.following = false;
                                        else
                                            status.retweeted_status.user.following = true;
                                        if (retweeted_node.ChildNodes[17].InnerText == "false")
                                            status.retweeted_status.user.verified = false;
                                        else
                                            status.retweeted_status.user.verified = true;
                                        if (retweeted_node.ChildNodes[18].InnerText == "false")
                                            status.retweeted_status.user.allow_all_act_msg = false;
                                        else
                                            status.retweeted_status.user.allow_all_act_msg = true;
                                        if (retweeted_node.ChildNodes[19].InnerText == "false")
                                            status.retweeted_status.user.geo_enabled = false;
                                        else
                                            status.retweeted_status.user.geo_enabled = true;
                                        break;
                                }
                            }//for
                            break;
                    }
                }
                lstStatuses.AddLast(status);
            }
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
            while (strResult == null && !blnStopCrawling)
                strResult = api.comments(lStatusID, iPageNum);

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
                        case "user":
                            comment.user.user_id = Convert.ToInt64(node.ChildNodes[0].InnerText);
                            comment.user.screen_name = node.ChildNodes[1].InnerText;
                            comment.user.name = node.ChildNodes[2].InnerText;
                            comment.user.province = node.ChildNodes[3].InnerText;
                            comment.user.city = node.ChildNodes[4].InnerText;
                            comment.user.location = node.ChildNodes[5].InnerText;
                            comment.user.description = node.ChildNodes[6].InnerText;
                            comment.user.url = node.ChildNodes[7].InnerText;
                            comment.user.profile_image_url = node.ChildNodes[8].InnerText;
                            comment.user.domain = node.ChildNodes[9].InnerText;
                            comment.user.gender = node.ChildNodes[10].InnerText;
                            comment.user.followers_count = Convert.ToInt32(node.ChildNodes[11].InnerText);
                            comment.user.friends_count = Convert.ToInt32(node.ChildNodes[12].InnerText);
                            comment.user.statuses_count = Convert.ToInt32(node.ChildNodes[13].InnerText);
                            comment.user.favourites_count = Convert.ToInt32(node.ChildNodes[14].InnerText);
                            comment.user.created_at = PubHelper.ParseDateTime(node.ChildNodes[15].InnerText);
                            if (node.ChildNodes[16].InnerText == "false")
                                comment.user.following = false;
                            else
                                comment.user.following = true;
                            if (node.ChildNodes[17].InnerText == "false")
                                comment.user.verified = false;
                            else
                                comment.user.verified = true;
                            if (node.ChildNodes[18].InnerText == "false")
                                comment.user.allow_all_act_msg = false;
                            else
                                comment.user.allow_all_act_msg = true;
                            if (node.ChildNodes[19].InnerText == "false")
                                comment.user.geo_enabled = false;
                            else
                                comment.user.geo_enabled = true;
                            break;
                    }
                }
                lstComments.AddLast(comment);
            }
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
            return lstTags;
        }
    };
}
