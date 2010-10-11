using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Xml;
using Sinawler.Model;

namespace Sinawler
{
    //SinaMBCrawler类是通过新浪微博API从微博中抓取数据的类
    class SinaMBCrawler
    {
        private SinaApiService api;
        ///默认请求之前等待3.6秒钟，此值根据每小时100次的限制算得（每次3.6秒），但鉴于日志操作也有等待时间，故此值应能保证请求次数不超限
        ///后经测试，此值还可缩小。2010-10-11定为最小值2000，可调整
        ///2010-10-12定为不设下限
        private int iSleep = 3600;

        private int iRemainingHits = 1000; //当前小时内剩余请求次数
        private int iResetTimeInSeconds = 3600; //剩余秒数

        public SinaMBCrawler ( SinaApiService oApi )
        {
            api = oApi;
        }

        public int SleepTime
        {
            set { iSleep = value; }
            get { return iSleep; }
        }

        public int RemainingHits
        {
            get { return iRemainingHits; }
        }

        public int ResetTimeInSeconds
        {
            get { return iResetTimeInSeconds; }
        }

        private int CompareStatus ( Status x, Status y )
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y == null)  // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare their status_id
                    if (x.status_id > y.status_id)
                        return 1;
                    else if (x.status_id == y.status_id)
                        return 0;
                    else
                        return -1;
                }
            }
        }

        private int CompareComment ( Comment x, Comment y )
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y == null)  // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare their comment_id
                    if (x.comment_id > y.comment_id)
                        return 1;
                    else if (x.comment_id == y.comment_id)
                        return 0;
                    else
                        return -1;
                }
            }
        }

        /// <summary>
        /// 获取指定UID的指定个数的好友（即该用户所关注的人）ID列表
        /// </summary>
        /// <param name="lUid">要获取好友的用户ID</param>
        /// <param name="lCursor">分页时指示游标位置，详见API文档</param>
        /// <returns>好友ID列表</returns>
        public LinkedList<long> GetFriendsOf ( long lUid, int iCursor )
        {
            System.Threading.Thread.Sleep( iSleep );
            return api.friends_ids( lUid, iCursor );
        }

        /// <summary>
        /// 获取指定UID的指定个数的粉丝ID列表
        /// </summary>
        /// <param name="lUid">要获取好友的用户ID</param>
        /// <param name="lCursor">分页时指示游标位置，详见API文档</param>
        /// <returns>粉丝ID列表</returns>
        public LinkedList<long> GetFollowersOf ( long lUid, int iCursor )
        {
            System.Threading.Thread.Sleep( iSleep );
            return api.followers_ids( lUid, iCursor );
        }

        //根据UID抓取用户信息
        public User GetUserInfo ( long lUid )
        {
            System.Threading.Thread.Sleep( iSleep );
            string strResult = api.user_show( lUid );
            while (strResult == null)
                strResult = api.user_show( lUid );
            strResult = PubHelper.stripNonValidXMLCharacters( strResult );  //过滤XML中的无效字符
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );
            User user = new User();
            user.uid = Convert.ToInt64( xmlDoc.GetElementsByTagName( "id" )[0].InnerText );
            user.screen_name = xmlDoc.GetElementsByTagName( "screen_name" )[0].InnerText;
            user.name = xmlDoc.GetElementsByTagName( "name" )[0].InnerText;
            user.province = xmlDoc.GetElementsByTagName( "province" )[0].InnerText;
            user.city = xmlDoc.GetElementsByTagName( "city" )[0].InnerText;
            user.location = xmlDoc.GetElementsByTagName( "location" )[0].InnerText;
            user.description = xmlDoc.GetElementsByTagName( "description" )[0].InnerText;
            user.url = xmlDoc.GetElementsByTagName( "url" )[0].InnerText;
            user.profile_image_url = xmlDoc.GetElementsByTagName( "profile_image_url" )[0].InnerText;
            user.domain = xmlDoc.GetElementsByTagName( "domain" )[0].InnerText;
            user.gender = xmlDoc.GetElementsByTagName( "gender" )[0].InnerText;
            user.followers_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "followers_count" )[0].InnerText );
            user.friends_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "friends_count" )[0].InnerText );
            user.statuses_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "statuses_count" )[0].InnerText );
            user.favourites_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "favourites_count" )[0].InnerText );
            user.created_at = PubHelper.ParseDateTime( xmlDoc.GetElementsByTagName( "created_at" )[0].InnerText );
            if (xmlDoc.GetElementsByTagName( "following" )[0].InnerText == "false")
                user.following = false;
            else
                user.following = true;
            if (xmlDoc.GetElementsByTagName( "verified" )[0].InnerText == "false")
                user.verified = false;
            else
                user.verified = true;
            if (xmlDoc.GetElementsByTagName( "allow_all_act_msg" )[0].InnerText == "false")
                user.allow_all_act_msg = false;
            else
                user.allow_all_act_msg = true;
            if (xmlDoc.GetElementsByTagName( "geo_enabled" )[0].InnerText == "false")
                user.geo_enabled = false;
            else
                user.geo_enabled = true;
            return user;
        }

        //根据用户昵称抓取用户信息
        public User GetUserInfo ( string strScreenName )
        {
            System.Threading.Thread.Sleep( iSleep );
            string strResult = api.user_show( strScreenName );
            while (strResult == null)
                strResult = api.user_show( strScreenName );
            strResult = PubHelper.stripNonValidXMLCharacters( strResult );  //过滤XML中的无效字符
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );
            User user = new User();
            user.uid = Convert.ToInt64( xmlDoc.GetElementsByTagName( "id" )[0].InnerText );
            user.screen_name = xmlDoc.GetElementsByTagName( "screen_name" )[0].InnerText;
            user.name = xmlDoc.GetElementsByTagName( "name" )[0].InnerText;
            user.province = xmlDoc.GetElementsByTagName( "province" )[0].InnerText;
            user.city = xmlDoc.GetElementsByTagName( "city" )[0].InnerText;
            user.location = xmlDoc.GetElementsByTagName( "location" )[0].InnerText;
            user.description = xmlDoc.GetElementsByTagName( "description" )[0].InnerText;
            user.url = xmlDoc.GetElementsByTagName( "url" )[0].InnerText;
            user.profile_image_url = xmlDoc.GetElementsByTagName( "profile_image_url" )[0].InnerText;
            user.domain = xmlDoc.GetElementsByTagName( "domain" )[0].InnerText;
            user.gender = xmlDoc.GetElementsByTagName( "gender" )[0].InnerText;
            user.followers_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "followers_count" )[0].InnerText );
            user.friends_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "friends_count" )[0].InnerText );
            user.statuses_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "statuses_count" )[0].InnerText );
            user.favourites_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "favourites_count" )[0].InnerText );
            user.created_at = PubHelper.ParseDateTime( xmlDoc.GetElementsByTagName( "created_at" )[0].InnerText );
            if (xmlDoc.GetElementsByTagName( "following" )[0].InnerText == "false")
                user.following = false;
            else
                user.following = true;
            if (xmlDoc.GetElementsByTagName( "verified" )[0].InnerText == "false")
                user.verified = false;
            else
                user.verified = true;
            if (xmlDoc.GetElementsByTagName( "allow_all_act_msg" )[0].InnerText == "false")
                user.allow_all_act_msg = false;
            else
                user.allow_all_act_msg = true;
            if (xmlDoc.GetElementsByTagName( "geo_enabled" )[0].InnerText == "false")
                user.geo_enabled = false;
            else
                user.geo_enabled = true;
            return user;
        }

        //同时根据UID和用户昵称抓取用户信息
        public User GetUserInfo ( long lUid, string strScreenName )
        {
            System.Threading.Thread.Sleep( iSleep );
            string strResult = api.user_show( lUid, strScreenName );
            while (strResult == null)
                strResult = api.user_show( lUid, strScreenName );
            strResult = PubHelper.stripNonValidXMLCharacters( strResult );  //过滤XML中的无效字符
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );
            User user = new User();
            user.uid = Convert.ToInt64( xmlDoc.GetElementsByTagName( "id" )[0].InnerText );
            user.screen_name = xmlDoc.GetElementsByTagName( "screen_name" )[0].InnerText;
            user.name = xmlDoc.GetElementsByTagName( "name" )[0].InnerText;
            user.province = xmlDoc.GetElementsByTagName( "province" )[0].InnerText;
            user.city = xmlDoc.GetElementsByTagName( "city" )[0].InnerText;
            user.location = xmlDoc.GetElementsByTagName( "location" )[0].InnerText;
            user.description = xmlDoc.GetElementsByTagName( "description" )[0].InnerText;
            user.url = xmlDoc.GetElementsByTagName( "url" )[0].InnerText;
            user.profile_image_url = xmlDoc.GetElementsByTagName( "profile_image_url" )[0].InnerText;
            user.domain = xmlDoc.GetElementsByTagName( "domain" )[0].InnerText;
            user.gender = xmlDoc.GetElementsByTagName( "gender" )[0].InnerText;
            user.followers_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "followers_count" )[0].InnerText );
            user.friends_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "friends_count" )[0].InnerText );
            user.statuses_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "statuses_count" )[0].InnerText );
            user.favourites_count = Convert.ToInt32( xmlDoc.GetElementsByTagName( "favourites_count" )[0].InnerText );
            user.created_at = PubHelper.ParseDateTime( xmlDoc.GetElementsByTagName( "created_at" )[0].InnerText );
            if (xmlDoc.GetElementsByTagName( "following" )[0].InnerText == "false")
                user.following = false;
            else
                user.following = true;
            if (xmlDoc.GetElementsByTagName( "verified" )[0].InnerText == "false")
                user.verified = false;
            else
                user.verified = true;
            if (xmlDoc.GetElementsByTagName( "allow_all_act_msg" )[0].InnerText == "false")
                user.allow_all_act_msg = false;
            else
                user.allow_all_act_msg = true;
            if (xmlDoc.GetElementsByTagName( "geo_enabled" )[0].InnerText == "false")
                user.geo_enabled = false;
            else
                user.geo_enabled = true;
            return user;
        }

        //抓取当前登录用户的信息
        public User GetCurrentUserInfo ()
        {
            System.Threading.Thread.Sleep( iSleep );
            long lCurrentUID = api.account_verify_credentials();
            if (lCurrentUID == 0) return null;
            else return this.GetUserInfo( lCurrentUID );
        }

        /// <summary>
        /// 获取指定UID的指定微博ID之后的微博
        /// </summary>
        /// <param name="lUid">要获取微博内容的UID</param>
        /// <param name="lSinceSid">只返回ID比lSinceSid大（比lSinceSid时间晚的）的微博信息内容</param>
        /// <returns>微博列表</returns>
        public List<Status> GetStatusesOfSince ( long lUid, long lSinceSid )
        {
            System.Threading.Thread.Sleep( iSleep );
            string strResult = api.user_timeline( lUid, lSinceSid );
            while (strResult == null)
                strResult = api.user_timeline( lUid, lSinceSid );
            strResult = PubHelper.stripNonValidXMLCharacters( strResult );  //过滤XML中的无效字符
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );
            List<Status> lstStatuses = new List<Status>();
            XmlNodeList nodes = xmlDoc.GetElementsByTagName( "status" );
            foreach (XmlNode nodeStatus in nodes)
            {
                Status status = new Status();
                foreach (XmlNode node in nodeStatus.ChildNodes)
                {
                    switch (node.Name.ToLower())
                    {
                        case "created_at":
                            status.created_at = PubHelper.ParseDateTime( node.InnerText );
                            break;
                        case "id":
                            status.status_id = Convert.ToInt64( node.InnerText );
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
                                status.in_reply_to_status_id = Convert.ToInt64( node.InnerText );
                            break;
                        case "in_reply_to_user_id":
                            if (node.InnerText == "")
                                status.in_reply_to_user_id = 0;
                            else
                                status.in_reply_to_user_id = Convert.ToInt64( node.InnerText );
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
                            status.uid = Convert.ToInt64( node.ChildNodes[0].InnerText );
                            break;
                        case "retweeted_status":
                            status.retweeted_status_id = Convert.ToInt64( node.ChildNodes[1].InnerText );
                            break;
                    }
                }
                status.iteration = 0;
                lstStatuses.Add( status );
            }
            lstStatuses.Sort( CompareStatus );
            return lstStatuses;
        }

        /// <summary>
        /// 获取指定微博评论
        /// </summary>
        /// <param name="lStatusID">要获取评论内容的微博ID</param>
        /// <returns>评论列表</returns>
        public List<Comment> GetCommentsOf ( long lStatusID )
        {
            List<Comment> lstComments = new List<Comment>();
            int iPage = 1;
            System.Threading.Thread.Sleep( iSleep );
            string strResult = api.comments( lStatusID, iPage );
            while (strResult == null)
                strResult = api.comments( lStatusID, iPage );
            strResult = PubHelper.stripNonValidXMLCharacters( strResult );  //过滤XML中的无效字符
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );
            XmlNodeList nodes = xmlDoc.GetElementsByTagName( "comment" );
            while (nodes.Count > 0)
            {
                foreach (XmlNode nodeComment in nodes)
                {
                    Comment comment = new Comment();
                    comment.status_id = lStatusID;
                    foreach (XmlNode node in nodeComment.ChildNodes)
                    {
                        switch (node.Name.ToLower())
                        {
                            case "created_at":
                                comment.created_at = PubHelper.ParseDateTime( node.InnerText );
                                break;
                            case "id":
                                comment.comment_id = Convert.ToInt64( node.InnerText );
                                break;
                            case "text":
                                comment.content = node.InnerText;
                                break;
                            case "user":
                                comment.uid = Convert.ToInt64( node.ChildNodes[0].InnerText );
                                break;
                        }
                    }
                    comment.iteration = 0;
                    lstComments.Add( comment );
                }
                iPage++;
                System.Threading.Thread.Sleep( iSleep );
                strResult = api.comments( lStatusID, iPage );
                while (strResult == null)
                    strResult = api.comments( lStatusID, iPage );
                xmlDoc.LoadXml( strResult );
                nodes = xmlDoc.GetElementsByTagName( "comment" );
            }
            lstComments.Sort(CompareComment);
            return lstComments;
        }

        //检查请求限制剩余次数，并根据情况调整访问频度
        public void AdjustFreq()
        {
            string strResult = api.check_hits_limit();
            if (strResult == null) return;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( strResult );

            iRemainingHits = Convert.ToInt32( xmlDoc.GetElementsByTagName( "remaining-hits" )[0].InnerText );
            int iHourlyLimit = Convert.ToInt32( xmlDoc.GetElementsByTagName( "hourly-limit" )[0].InnerText );
            int iResetTimeInSeconds = Convert.ToInt32( xmlDoc.GetElementsByTagName( "reset-time-in-seconds" )[0].InnerText );
            string[] strBuffer = PubHelper.ParseDateTime( xmlDoc.GetElementsByTagName( "reset-time" )[0].InnerText ).Split( ' ' )[0].Split( '-' );
            string strResetTime = strBuffer[0] + "年" + strBuffer[1] + "月" + strBuffer[2] + "日";

            //计算
            //剩余时间可访问次数大于等于剩余次数，说明剩余次数不够用，将会超限，则加长等待时间
            if (iResetTimeInSeconds * 1000 / iSleep >= iRemainingHits)
            {
                while (iResetTimeInSeconds * 1000 / iSleep > iRemainingHits)
                {
                    //增加等待时间
                    iSleep += 200;

                    //重新获取信息
                    strResult = api.check_hits_limit();
                    if (strResult == null) return;
                    xmlDoc.LoadXml(strResult);

                    iRemainingHits = Convert.ToInt32(xmlDoc.GetElementsByTagName("remaining-hits")[0].InnerText);
                    iResetTimeInSeconds = Convert.ToInt32(xmlDoc.GetElementsByTagName("reset-time-in-seconds")[0].InnerText);
                }
            }
            else
            {
                //剩余时间可访问次数小于剩余次数，说明剩余次数够用，不会超限，则减少等待时间，但不低于3600毫秒下限
                iSleep -= 200;
                //2010-10-12定为不设下限
                if (iSleep <= 0) iSleep = 1;

                //重新获取信息
                strResult = api.check_hits_limit();
                if (strResult == null) return;
                xmlDoc.LoadXml( strResult );

                iRemainingHits = Convert.ToInt32( xmlDoc.GetElementsByTagName( "remaining-hits" )[0].InnerText );
                iResetTimeInSeconds = Convert.ToInt32( xmlDoc.GetElementsByTagName( "reset-time-in-seconds" )[0].InnerText );
            }
        }
    };
}
