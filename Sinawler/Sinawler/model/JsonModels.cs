using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sinawler.Model;

namespace Sinawler
{
    //the classes that receive the deserialized objects from Json
    //some ones can use model classes directly
    class JsonIDs
    {
        private long[] ids;

        public long[] IDs
        { 
            get { return ids; }
            set { ids = value; }
        }
    }

    class JsonGEO
    {
        private string _type;
        private double[] _coordinates = new double[2];

        public string type
        {
            set { _type = value; }
            get { return _type; }
        }

        public double[] coordinates
        {
            set { _coordinates = value; }
            get { return _coordinates; }
        }
    }

    class JsonStatus
    {
        private long _id;
        private string _created_at;
        private string _text;
        private string _source;
        private bool _favorited;
        private bool _truncated;
        private JsonGEO _geo;
        private string _in_reply_to_status_id;
        private string _in_reply_to_user_id;
        private string _in_reply_to_screen_name;
        private string _thumbnail_pic = "";
        private string _bmiddle_pic = "";
        private string _original_pic = "";
        private string _mid;
        private User _user = new User();
        private JsonStatus _retweeted_status = null;

        #region properties
        /// <summary>
        /// 微博ID（XML中为id）
        /// </summary>
        public long id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string created_at
        {
            set { _created_at = value; }
            get { return _created_at; }
        }
        /// <summary>
        /// 微博信息内容(XML中为text)
        /// </summary>
        public string text
        {
            set { _text = value; }
            get { return _text; }
        }
        /// <summary>
        /// 微博来源
        /// </summary>
        public string source
        {
            set { _source = value; }
            get { return _source; }
        }
        /// <summary>
        /// 是否已收藏（应该是对于当前登录账号）
        /// </summary>
        public bool favorited
        {
            set { _favorited = value; }
            get { return _favorited; }
        }
        /// <summary>
        /// 是否被截断
        /// </summary>
        public bool truncated
        {
            set { _truncated = value; }
            get { return _truncated; }
        }
        /// <summary>
        /// 地理信息（暂不明类型）
        /// </summary>
        public JsonGEO geo
        {
            set { _geo = value; }
            get { return _geo; }
        }
        /// <summary>
        /// 回复ID
        /// </summary>
        public string in_reply_to_status_id
        {
            set { _in_reply_to_status_id = value; }
            get { return _in_reply_to_status_id; }
        }
        /// <summary>
        /// 回复人UserID
        /// </summary>
        public string in_reply_to_user_id
        {
            set { _in_reply_to_user_id = value; }
            get { return _in_reply_to_user_id; }
        }
        /// <summary>
        /// 回复人昵称
        /// </summary>
        public string in_reply_to_screen_name
        {
            set { _in_reply_to_screen_name = value; }
            get { return _in_reply_to_screen_name; }
        }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string thumbnail_pic
        {
            set { _thumbnail_pic = value; }
            get { return _thumbnail_pic; }
        }
        /// <summary>
        /// 中型图片
        /// </summary>
        public string bmiddle_pic
        {
            set { _bmiddle_pic = value; }
            get { return _bmiddle_pic; }
        }
        /// <summary>
        /// 原始图片
        /// </summary>
        public string original_pic
        {
            set { _original_pic = value; }
            get { return _original_pic; }
        }
        /// <summary>
        /// mid
        /// </summary>
        public string mid
        {
            set { _mid = value; }
            get { return _mid; }
        }
        /// <summary>
        /// 用户
        /// </summary>
        public User user
        {
            set { _user = value; }
            get { return _user; }
        }
        /// <summary>
        /// 同时转发的微博
        /// </summary>
        public JsonStatus retweeted_status
        {
            set { _retweeted_status = value; }
            get { return _retweeted_status; }
        }
        #endregion

        public JsonStatus()
        {
            _id = 0;
        }
    }

    class JsonComment
    {
        private long _id;
        private string _text;
        private string _created_at;
        private string _source;
        private string _mid;
        private User _user = new User();
        private JsonStatus _status;
        
        #region property
        /// <summary>
        /// 评论ID（XML中为id）
        /// </summary>
        public long id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 评论内容（XML中为text）
        /// </summary>
        public string text
        {
            set { _text = value; }
            get { return _text; }
        }
        /// <summary>
        /// 评论时间
        /// </summary>
        public string created_at
        {
            set { _created_at = value; }
            get { return _created_at; }
        }
        /// <summary>
        /// 评论来源
        /// </summary>
        public string source
        {
            set { _source = value; }
            get { return _source; }
        }
        /// <summary>
        /// 评论人
        /// </summary>
        public User user
        {
            set { _user = value; }
            get { return _user; }
        }
        /// <summary>
        /// 评论的微博
        /// </summary>
        public JsonStatus status
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// mid
        /// </summary>
        public string mid
        {
            set{_mid=value;}
            get { return _mid; }
        }
        #endregion
    }
}
