using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Open.Sina2SDK;
using System.Xml;
using Sinawler.Model;
using Newtonsoft.Json;

namespace Sinawler
{
    //SinaMBCrawler类是通过新浪微博API从微博中抓取数据的类
    public class SinaMBCrawler
    {
        private APIInfo api;
        ///默认请求之前等待3秒钟，此值根据每小时100次的限制算得（每次3.6秒），但鉴于日志操作也有等待时间，故此值应能保证请求次数不超限
        ///后经测试，此值还可缩小。2010-10-11定为最小值2000，可调整
        ///2010-10-12定为不设下限
        ///2011-02-23设定为下限500ms
        private int iSleep = 3000;
        private bool blnStopCrawling = false;   //是否停止爬行
        private int iTryTimes = 10;             //the times that try to get XML

        private void AdjustLimit()
        {
            api.RemainingHits--;
            api.ResetTimeInSeconds = api.ResetTimeInSeconds - Convert.ToInt32((DateTime.Now - api.LimitUpdateTime).TotalSeconds);
            api.LimitUpdateTime = DateTime.Now;
            if (api.ResetTimeInSeconds <= 0 || api.RemainingHits < 0) api.ResetTimeInSeconds = 3;
        }

        public SinaMBCrawler(SysArgFor crawlerType)
        {
            api = GlobalPool.GetAPI(crawlerType);
        }

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
            JsonIDs oJsonIDs = api.API.Friendships_Friends_Ids(lUid,5000,iCursor);
            for (int i = 0; i < oJsonIDs.IDs.Length; i++)
                ids.AddLast(oJsonIDs.IDs[i]);

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
            JsonIDs oJsonIDs = api.API.Friendships_Followers_Ids(lUid, 5000, iCursor);

            for (int i = 0; i < oJsonIDs.IDs.Length; i++)
                ids.AddLast(oJsonIDs.IDs[i]);

            return ids;
        }

        /// <summary>
        /// 判定两个用户之间的关系是否存在
        /// 注：此函数使用friendship/show接口。该接口可返回任意两个用户之间关系的详细信息，
        /// 但此函数只用它对初步根据social graph接口确认有关系的两个用户进行进一步的确认，
        /// 因为即使某个用户已经被系统清除，social graph接口依然能够返回其ID
        /// </summary>
        /// <param name="lSUID">源用户ID</param>
        /// <param name="lTUID">目标用户ID</param>
        /// <returns>结果</returns>
        public bool RelationExistBetween(long lSUID, long lTUID)
        {
            System.Threading.Thread.Sleep(iSleep);
            RelationShip rs = api.API.Friendships_Show(lSUID, lTUID);
            if (rs.UserNotExist) return false;
            if (!rs.source.followed_by && !rs.source.following)  //不存在
                return false;
            if (blnStopCrawling)
                return false;
            return true;
        }

        //根据UserID抓取用户信息
        public User GetUserInfo(long lUid)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = api.API.Users_Show(lUid);
            if (user == null && api.API.JsonResult.Contains("400"))    //用户不存在
                return new User();
            if (user == null && api.API.JsonResult.Contains("403"))    //服务已禁止
            {
                user = new User();
                user.user_id = -1;
            }
            user.created_at = PubHelper.ParseDateTime(user.created_at);
            return user;
        }

        //根据用户昵称抓取用户信息
        public User GetUserInfo(string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = api.API.Users_Show(strScreenName);
            if (user == null && api.API.JsonResult.Contains("400"))    //用户不存在
                return null;
            if (user == null && api.API.JsonResult.Contains("403"))    //服务已禁止
            {
                user = new User();
                user.user_id = -1;
            }
            user.created_at = PubHelper.ParseDateTime(user.created_at);
            return user;
        }

        //同时根据UserID和用户昵称抓取用户信息
        public User GetUserInfo(long lUid, string strScreenName)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = api.API.Users_Show(lUid, strScreenName);
            if (user == null && api.API.JsonResult.Contains("400"))    //用户不存在
                return null;
            if (user == null && api.API.JsonResult.Contains("403"))    //服务已禁止
            {
                user = new User();
                user.user_id = -1;
            }
            user.created_at = PubHelper.ParseDateTime(user.created_at);
            return user;
        }

        //抓取当前登录用户的信息
        public User GetCurrentUserInfo()
        {
            System.Threading.Thread.Sleep(iSleep);
            //long lCurrentUserID = api.API.account_verify_credentials();
            long lCurrentUserID = api.API.Account_Get_Uid();
            if (lCurrentUserID == 0) return new User();
            else return this.GetUserInfo(lCurrentUserID);
        }

        //transform a JsonStatus object to a Status object
        private Status JsonStatusToStatus(JsonStatus oJsonStatus)
        {
            Status status = new Status();
            status.created_at = PubHelper.ParseDateTime(oJsonStatus.created_at);
            status.status_id = oJsonStatus.id;
            status.content = oJsonStatus.text;
            if (oJsonStatus.source != null)
            {
                status.source_url = oJsonStatus.source.Substring(9, oJsonStatus.source.IndexOf("rel") - 11);
                status.source_name = oJsonStatus.source.Substring(oJsonStatus.source.IndexOf('>') + 1, oJsonStatus.source.IndexOf("</") - oJsonStatus.source.IndexOf('>') - 1);
            }
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
            if (status.user.created_at != null)
                status.user.created_at = PubHelper.ParseDateTime(status.user.created_at);
            if (oJsonStatus.retweeted_status != null)
            {
                status.retweeted_status = new Status();
                status.retweeted_status.created_at = PubHelper.ParseDateTime(oJsonStatus.retweeted_status.created_at);
                status.retweeted_status.status_id = Convert.ToInt64(oJsonStatus.retweeted_status.id);
                status.retweeted_status.content = oJsonStatus.retweeted_status.text;
                if (oJsonStatus.retweeted_status.source != null)
                {
                    status.retweeted_status.source_url = oJsonStatus.retweeted_status.source.Substring(9, oJsonStatus.retweeted_status.source.IndexOf("rel") - 11);
                    status.retweeted_status.source_name = oJsonStatus.retweeted_status.source.Substring(oJsonStatus.retweeted_status.source.IndexOf('>') + 1, oJsonStatus.retweeted_status.source.IndexOf("</") - oJsonStatus.retweeted_status.source.IndexOf('>') - 1);
                }
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
                if (oJsonStatus.retweeted_status.mid != null && oJsonStatus.retweeted_status.mid.Trim() != "")
                    status.retweeted_status.mid = Convert.ToInt64(oJsonStatus.retweeted_status.mid);
                status.retweeted_status.user = oJsonStatus.retweeted_status.user;
                if (status.retweeted_status.user.created_at != null)
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
                        string[] strCoordinates = node.InnerText.Split(' ');
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
            return JsonStatusToStatus(api.API.Statuses_Show(lStatusID));
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
            JsonStatus[] oJsonStatuses = api.API.Statuses_User_Timeline(lUid, lSinceSid,0,1,200,0,Feature.All);
            foreach (JsonStatus oJsonStatus in oJsonStatuses)
                lstStatuses.AddLast(JsonStatusToStatus(oJsonStatus));

            return lstStatuses;
        }

        /// <summary>
        /// 获取指定ID的转发微博
        /// </summary>
        /// <param name="lStatusID">要获取转发微博内容的微博ID</param>
        /// <returns>微博</returns>
        public LinkedList<Status> GetRepostedStatusOf(long lStatusID, int iPageNum)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<Status> lstStatuses = new LinkedList<Status>();
            JsonStatus[] oJsonStatuses = api.API.Statuses_Repost_Timeline(lStatusID,0,0, iPageNum,200,FilterByAuthor.All);
            foreach (JsonStatus oJsonStatus in oJsonStatuses)
                lstStatuses.AddLast(JsonStatusToStatus(oJsonStatus));

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
            JsonComment[] oJsonComments = api.API.Comments_Show(lStatusID,0, iPageNum,200,FilterBySource.All);
            foreach (JsonComment oJsonComment in oJsonComments)
            {
                Comment comment = new Comment();
                comment.status_id = lStatusID;
                comment.created_at = PubHelper.ParseDateTime(oJsonComment.created_at);
                comment.comment_id = oJsonComment.id;
                comment.content = oJsonComment.text;
                if (oJsonComment.source != null)
                {
                    comment.source_url = oJsonComment.source.Substring(9, oJsonComment.source.IndexOf("rel") - 11);
                    comment.source_name = oJsonComment.source.Substring(oJsonComment.source.IndexOf('>') + 1, oJsonComment.source.IndexOf("</") - oJsonComment.source.IndexOf('>') - 1);
                }
                comment.mid = Convert.ToInt64(oJsonComment.mid);
                comment.user = oJsonComment.user;
                comment.user.created_at = PubHelper.ParseDateTime(comment.user.created_at);

                lstComments.AddLast(comment);
            }

            return lstComments;
        }

        private Tag JsonTagToTag(JsonTag oJsonTag)
        {
            Tag tag = new Tag();
            tag.tag_id = Convert.ToInt64(oJsonTag.id);
            tag.tag = oJsonTag.content;
            tag.weight = Convert.ToInt32(oJsonTag.weight);
            return tag;
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
            JsonTag[] oJsonTags = api.API.Tags(lUserID,1,50);
            foreach (JsonTag oJsonTag in oJsonTags)
                lstTags.AddLast(JsonTagToTag(oJsonTag));

            return lstTags;
        }
    };
}
