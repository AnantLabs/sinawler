using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sinawler.Model;

namespace Sinawler
{
    //the classes that receive the deserialized objects from Json
    //some ones can use model classes directly

    public class JsonIDs
    {
        private long[] ids;
        private int _next_cursor;
        private int _previous_cursor;
        private int _total_number;

        public long[] IDs
        { 
            get { return ids; }
            set { ids = value; }
        }

        public int next_cursor
        {
            get { return _next_cursor; }
            set { _next_cursor = value; }
        }

        public int previous_cursor
        {
            get { return _previous_cursor; }
            set { _previous_cursor = value; }
        }

        public int total_number
        {
            get { return _total_number; }
            set { _total_number = value; }
        }
    }

    public class JsonGEO
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

    public class JsonVisible
    {
        private int _type = 0;
        private int _list_id = 0;

        public int type
        { get { return _type; } set { _type = value; } }

        public int list_id
        { get { return _list_id; } set { _list_id = value; } }
    }

    public class JsonStatus
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
        //------------20120317加------------
        private string _idstr = "";
        private int _reposts_count = 0;
        private int _comments_count = 0;
        private int _mlevel = 0;
        private JsonVisible _visible=null;
        //----------------------------------

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
        //------------20120317加------------
        public string idstr
        { get { return _idstr; } set { _idstr = value; } }
        public int reposts_count
        { get { return _reposts_count; } set { _reposts_count = value; } }
        public int comments_count
        { get { return _comments_count; } set { _comments_count = value; } }
        public int mlevel
        { get { return _mlevel; } set { _mlevel = value; } }
        public JsonVisible visible
        { get { return _visible; } set { _visible = value; } }
        //----------------------------------
        #endregion

        public JsonStatus()
        {
            _id = 0;
        }
    }

    public class JsonStatuses
    {
        private JsonStatus[] _oJsonStatuses;
        private bool _hasvisible = false;
        private long _previous_cursor = 0;
        private long _next_cursor = 0;
        private int _total_number = 0;

        public JsonStatus[] Statuses
        { get { return _oJsonStatuses; } set { _oJsonStatuses = value; } }
        public bool hasvisible
        { get { return _hasvisible; } set { _hasvisible = value; } }
        public long previous_cursor
        { get { return _previous_cursor; } set { _previous_cursor = value; } }
        public long next_cursor
        { get { return _next_cursor; } set { _next_cursor = value; } }
        public int total_number
        { get { return _total_number; } set { _total_number = value; } }
    }

    public class JsonComment
    {
        private long _id;
        private string _text;
        private string _created_at;
        private string _source;
        private string _mid;
        private User _user = new User();
        private JsonStatus _status;
        //------------20120318加---------------
        private string _idstr;
        private JsonComment _reply_comment;
        //-------------------------------------
        
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
        //------------20120318加---------------
        public string idstr
        { get { return _idstr; } set { _idstr = value; } }
        public JsonComment reply_comment
        { get { return _reply_comment; } set { _reply_comment = value; } }
        //-------------------------------------
        #endregion
    }

    public class JsonComments
    {
        private JsonComment[] _oJsonComments;
        private bool _hasvisible = false;
        private long _previous_cursor = 0;
        private long _next_cursor = 0;
        private int _total_number = 0;

        public JsonComment[] Comments
        { get { return _oJsonComments; } set { _oJsonComments = value; } }
        public bool hasvisible
        { get { return _hasvisible; } set { _hasvisible = value; } }
        public long previous_cursor
        { get { return _previous_cursor; } set { _previous_cursor = value; } }
        public long next_cursor
        { get { return _next_cursor; } set { _next_cursor = value; } }
        public int total_number
        { get { return _total_number; } set { _total_number = value; } }
    }

    public class JsonWriteLimit
    {
        private string _api;
        private int _limit;
        private string _limit_time_unit;
        private int _remaining_hits;

        public string api
        { get { return _api; } set { _api = value; } }

        public int limit
        { get { return _limit; } set { _limit = value; } }

        public string limit_time_unit
        { get { return _limit_time_unit; } set { _limit_time_unit = value; } }

        public int remaining_hits
        { get { return _remaining_hits; } set { _remaining_hits = value; } }
    }

    public class JsonRateLimit
    {
        private JsonWriteLimit[] _api_rate_limits;
        private int _ip_limit;
        private string _limit_time_unit;
        private int _remaining_ip_hits;
        private int _remaining_user_hits;
        private string _reset_time;
        private int _reset_time_in_seconds;
        private int _user_limit;

        public JsonWriteLimit[] api_rate_limite
        {get{return _api_rate_limits;} set{_api_rate_limits=value;}}

        public int ip_limit
        { get { return _ip_limit; } set { _ip_limit = value; } }

        public string limit_time_unit
        { get { return _limit_time_unit; } set { _limit_time_unit = value; } }

        public int remaining_ip_hits
        { get { return _remaining_ip_hits; } set { _remaining_ip_hits = value; } }

        public int remaining_user_hits
        { get { return _remaining_user_hits; } set { _remaining_user_hits = value; } }

        public string reset_time
        { get { return _reset_time; } set { _reset_time = value; } }

        public int reset_time_in_seconds
        { get { return _reset_time_in_seconds; } set { _reset_time_in_seconds = value; } }

        public int user_limit
        { get { return _user_limit; } set { _user_limit = value; } }
    }

    public class JsonAccountUID
    {
        private long _uid;

        public long uid
        { get { return _uid; } set { _uid = value; } }
    }

    public class JsonError
    {
        private string _error="";
        private int _error_code=0;
        private string _request="";

        public string error
        { get { return _error; } set { _error = value; } }

        public int error_code
        { get { return _error_code; } set { _error_code = value; } }

        public string request
        { get { return _request; } set { _request = value; } }

        public void Initial()
        {
            _error = "";
            _error_code = 0;
            _request = "";
        }
    }

    public class JsonTag
    {
        private string _id="";
        private string _content = "";
        private string _weight = "";

        public string id
        { get { return _id; } set { _id = value; } }

        public string content
        { get { return _content; } set { _content = value; } }

        public string weight
        { get { return _weight; } set { _weight = value; } }
    }
}
