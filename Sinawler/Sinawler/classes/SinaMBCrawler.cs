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

        private void AdjustLimit()
        {
            api.RemainingIPHits--;
            api.RemainingUserHits--;
            api.ResetTimeInSeconds = api.ResetTimeInSeconds - Convert.ToInt32((DateTime.Now - api.ResetTime).TotalSeconds);
            api.ResetTime = DateTime.Now;
            if (api.ResetTimeInSeconds <= 0 || api.RemainingIPHits < 0 || api.RemainingUserHits<0) api.ResetTimeInSeconds = 3;
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
            JsonIDs oJsonIDs = api.API.Friendships_Friends_Ids(lUid, 5000, iCursor);
            if (oJsonIDs == null)
            {
                ids.AddLast(-1);
                return ids;
            }
            if(oJsonIDs!=null && oJsonIDs.IDs!=null)
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
            if (oJsonIDs == null)
            {
                ids.AddLast(-1);
                return ids;
            }
            if (oJsonIDs != null && oJsonIDs.IDs != null)
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
        public int RelationExistBetween(long lSUID, long lTUID)
        {
            System.Threading.Thread.Sleep(iSleep);
            RelationShip rs = api.API.Friendships_Show(lSUID, lTUID);
            if (rs == null) return -1;  //forbidden
            if (rs.UserNotExist) return 0;
            if (!rs.source.followed_by && !rs.source.following)  //不存在
                return 0;
            if (blnStopCrawling)
                return 0;
            return 1;
        }

        //根据UserID抓取用户信息
        public User GetUserInfo(long lUid)
        {
            System.Threading.Thread.Sleep(iSleep);
            User user = api.API.Users_Show(lUid);
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
            //------------20120317加------------
            status.idstr = oJsonStatus.idstr;
            status.reposts_count = oJsonStatus.reposts_count;
            status.comments_count = oJsonStatus.comments_count;
            status.mlevel = oJsonStatus.mlevel;
            if (oJsonStatus.visible != null)
            {
                status.visible_type = oJsonStatus.visible.type;
                status.visible_list_id = oJsonStatus.visible.list_id;
            }
            //----------------------------------
            return status;
        }

        //transform a JsonComment object to a Comment object
        private Comment JsonCommentToComment(JsonComment oJsonComment)
        {
            Comment comment = new Comment();            
            comment.comment_id = oJsonComment.id;
            comment.content = oJsonComment.text;
            comment.created_at = PubHelper.ParseDateTime(oJsonComment.created_at);
            comment.idstr = oJsonComment.idstr;
            if(oJsonComment.mid!=null && oJsonComment.mid.Trim()!="")
                comment.mid = Convert.ToInt64(oJsonComment.mid);            
            if (oJsonComment.source != null)
            {
                comment.source_url = oJsonComment.source.Substring(9, oJsonComment.source.IndexOf("rel") - 11);
                comment.source_name = oJsonComment.source.Substring(oJsonComment.source.IndexOf('>') + 1, oJsonComment.source.IndexOf("</") - oJsonComment.source.IndexOf('>') - 1);
            }
            if(oJsonComment.status!=null)
                comment.status_id = oJsonComment.status.id;
            comment.user = oJsonComment.user;
            if(oJsonComment.reply_comment!=null)
               comment.reply_comment = JsonCommentToComment(oJsonComment.reply_comment);

            return comment;
        }

        /// <summary>
        /// 获取指定ID的微博
        /// </summary>
        /// <param name="lStatusID">要获取微博内容的微博ID</param>
        /// <returns>微博</returns>
        public Status GetStatus(long lStatusID)
        {
            System.Threading.Thread.Sleep(iSleep);
            JsonStatus oJsonStatus = api.API.Statuses_Show(lStatusID);
            if (oJsonStatus == null)
            {
                Status s = new Status();
                s.status_id = -1;
                return s;
            }
            return JsonStatusToStatus(oJsonStatus);
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
            JsonStatuses oJsonStatuses = api.API.Statuses_User_Timeline(lUid, lSinceSid, 0, 1, 200, 0, Feature.All);
            if (oJsonStatuses!=null && oJsonStatuses.Statuses != null)
                foreach (JsonStatus oJsonStatus in oJsonStatuses.Statuses)
                    lstStatuses.AddLast(JsonStatusToStatus(oJsonStatus));
            //Add a status with id as -1 to specify 403 forbidden
            if (oJsonStatuses == null)
            {
                Status s = new Status();
                s.status_id = -1;
                lstStatuses.AddLast(s);
            }

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
            JsonStatuses oJsonStatuses = api.API.Statuses_Repost_Timeline(lStatusID, 0, 0, iPageNum, 200, FilterByAuthor.All);
            //JsonStatus[] oJsonStatuses = api.API.Statuses_Repost_Timeline(lStatusID,0,0, iPageNum,200,FilterByAuthor.All);
            if (oJsonStatuses!=null && oJsonStatuses.Statuses != null)
                foreach (JsonStatus oJsonStatus in oJsonStatuses.Statuses)
                    lstStatuses.AddLast(JsonStatusToStatus(oJsonStatus));

            //Add a status with id as -1 to specify 403 forbidden
            if (oJsonStatuses == null)
            {
                Status s = new Status();
                s.status_id = -1;
                lstStatuses.AddLast(s);
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
            JsonComments oJsonComments = api.API.Comments_Show(lStatusID, 0, 0, iPageNum, 200, FilterByAuthor.All);
            if (oJsonComments != null && oJsonComments.Comments != null)
                foreach (JsonComment oJsonComment in oJsonComments.Comments)
                {
                    Comment comment = JsonCommentToComment(oJsonComment);
                    lstComments.AddLast(comment);
                }

            //Add a comment with id as -1 to specify 403 forbidden
            if (oJsonComments == null)
            {
                Comment c = new Comment();
                c.comment_id = -1;
                lstComments.AddLast(c);
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
            JsonTag[] oJsonTags = api.API.Tags(lUserID, 1, 50);
            
            if(oJsonTags==null && api.API.JsonResult.Contains("403"))
            {
                Tag t = new Tag();
                t.tag_id = -1;
                lstTags.AddLast(t);
            }
            else
                foreach (JsonTag oJsonTag in oJsonTags)
                    lstTags.AddLast(JsonTagToTag(oJsonTag));

            return lstTags;
        }
    };
}
