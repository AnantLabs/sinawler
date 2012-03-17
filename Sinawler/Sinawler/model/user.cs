using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
    /// <summary>
    /// 类user。此类中的方法，都是与数据库交互，从中读取或向其中写入数据
    /// </summary>

    //<id>88888888</id> 
    //<screen_name>领客康健网</screen_name> 
    //<name>领客康健网</name> 
    //<province>50</province> 
    //<city>1000</city> 
    //<location>重庆</location> 
    //<description></description> 
    //<url>http://www.ilinkee.com</url> 
    //<profile_image_url></profile_image_url> 
    //<domain>ilinkee</domain> 
    //<gender>f</gender> 
    //<followers_count>46</followers_count> 
    //<friends_count>28</friends_count> 
    //<statuses_count>93</statuses_count> 
    //<favourites_count>0</favourites_count> 
    //<created_at>Fri Jan 08 00:00:00 +0800 2010</created_at> 
    //<following>false</following> 
    //<verified>false</verified> 
    //<allow_all_act_msg>false</allow_all_act_msg> 
    //<geo_enabled>false</geo_enabled> 

    //id: 用户UserID
    //idstr: string类型的id    -20120222
    //screen_name: 微博昵称
    //name: 友好显示名称，如Tim Yang(此特性暂不支持)
    //province:省份编码（参考省份编码表）
    //city: 城市编码（参考城市编码表）
    //location：地址
    //description: 个人描叙
    //url: 用户博客地址
    //profile_image_url: 自定义图像
    //domain: 用户个性化域名
    //weihao: 微号   -20120222 
    //gender: 性别,m--男，f--女,n--未知
    //followers_count: 粉丝数
    //friends_count: 关注数
    //statuses_count: 微博数
    //favourites_count: 收藏数
    //bi_followers_count：双向关注数  -20120222
    //created_at: 创建时间
    //following: 是否已关注(此特性暂不支持)
    //follow_me：是否关注我   -20120222
    //online_status：在线状态（int）   -20120222
    //allow_all_act_msg: 未知
    //geo_enabled: 未知（允许地理信息？）
    //verified: 加V标示
    //verified_type：加V类型（-20120222通过对比，现在可以确定，该值为-1表示未认证，0表示个人认证，220表示微博达人，其它表示企业认证。但企业认证的值五花八门，有2、5、6、7等等，不知咋区分？）
    //verified_reason：认证说明  -20120222
    //allow_all_comment：允许评论    -20120222
    //avatar_large：头像URL    -20120222
    //lang：语言   -20120222
    //iteration: 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据

    public class User
    {
        public User()
        { }

        #region Model
        private long _user_id = 0;
        private string _screen_name;
        private string _name;
        private string _province;
        private string _city;
        private string _location;
        private string _description;
        private string _url;
        private string _profile_image_url;
        private string _domain;
        private string _gender;
        private int _followers_count;
        private int _friends_count;
        private int _statuses_count;
        private int _favourites_count;
        private string _created_at;
        private bool _following;
        private bool _verified;
        private bool _allow_all_act_msg;
        private bool _geo_enabled;
        private int _iteration;
        private string _update_time;

        //-20120222加
        private string _idstr;
        private string _weihao;
        private int _bi_followers_count;
        private int _online_status;
        private int _verified_type;
        private string _verified_reason;
        private bool _allow_all_comment;
        private string _avatar_large;
        private string _lang;

        /// <summary>
        /// 用户UserID（XML中为id）
        /// </summary>
        public long user_id
        {
            set { _user_id = value; }
            get { return _user_id; }
        }
        /// <summary>
        /// 用户UserID（XML中为id），用于Json的反序列化
        /// </summary>
        public long id
        {
            set { _user_id = value; }
        }
        /// <summary>
        /// 微博昵称
        /// </summary>
        public string screen_name
        {
            set { _screen_name = value; }
            get { return _screen_name; }
        }
        /// <summary>
        /// 友好显示名称，如Bill Gates(此特性暂不支持) 
        /// </summary>
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        /// <summary>
        /// 省份编码（参考省份编码表）
        /// </summary>
        public string province
        {
            set { _province = value; }
            get { return _province; }
        }
        /// <summary>
        /// 城市编码（参考城市编码表）
        /// </summary>
        public string city
        {
            set { _city = value; }
            get { return _city; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string location
        {
            set { _location = value; }
            get { return _location; }
        }
        /// <summary>
        /// 个人描述
        /// </summary>
        public string description
        {
            set { _description = value; }
            get { return _description; }
        }
        /// <summary>
        /// 用户博客地址
        /// </summary>
        public string url
        {
            set { _url = value; }
            get { return _url; }
        }
        /// <summary>
        /// 自定义图像
        /// </summary>
        public string profile_image_url
        {
            set { _profile_image_url = value; }
            get { return _profile_image_url; }
        }
        /// <summary>
        /// 用户个性化URL
        /// </summary>
        public string domain
        {
            set { _domain = value; }
            get { return _domain; }
        }
        /// <summary>
        /// 性别,m--男，f--女,n--未知
        /// </summary>
        public string gender
        {
            set { _gender = value; }
            get { return _gender; }
        }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int followers_count
        {
            set { _followers_count = value; }
            get { return _followers_count; }
        }
        /// <summary>
        /// 关注数
        /// </summary>
        public int friends_count
        {
            set { _friends_count = value; }
            get { return _friends_count; }
        }
        /// <summary>
        /// 微博数
        /// </summary>
        public int statuses_count
        {
            set { _statuses_count = value; }
            get { return _statuses_count; }
        }
        /// <summary>
        /// 收藏数
        /// </summary>
        public int favourites_count
        {
            set { _favourites_count = value; }
            get { return _favourites_count; }
        }
        /// <summary>
        /// 创建时间，格式为yyyy-mm-dd hh:mm:ss
        /// </summary>
        public string created_at
        {
            set { _created_at = value; }
            get { return _created_at; }
        }
        /// <summary>
        /// 是否已关注(此特性暂不支持)
        /// </summary>
        public bool following
        {
            set { _following = value; }
            get { return _following; }
        }
        /// <summary>
        /// 加V标示，是否微博认证用户
        /// </summary>
        public bool verified
        {
            set { _verified = value; }
            get { return _verified; }
        }
        /// <summary>
        /// 未知
        /// </summary>
        public bool allow_all_act_msg
        {
            set { _allow_all_act_msg = value; }
            get { return _allow_all_act_msg; }
        }
        /// <summary>
        /// 未知（允许地理信息？）
        /// </summary>
        public bool geo_enabled
        {
            set { _geo_enabled = value; }
            get { return _geo_enabled; }
        }
        /// <summary>
        /// 迭代次数。默认为0，每迭代一次，就加1，则为0的为最近的数据
        /// </summary>
        public int iteration
        {
            get { return _iteration; }
        }
        /// <summary>
        /// 记录更新时间
        /// </summary>
        public string update_time
        {
            set { _update_time = value; }
            get { return _update_time; }
        }

        //-20120222加
        public string idstr
        {
            set { _idstr = value; }
            get { return _idstr; }
        }

        public string weihao
        {
            set { _weihao = value; }
            get { return _weihao; }
        }

        public int bi_followers_count
        {
            set { _bi_followers_count = value; }
            get { return _bi_followers_count; }
        }

        public int online_status
        {
            set { _online_status = value; }
            get { return _online_status; }
        }

        public int verified_type
        {
            set { _verified_type = value; }
            get { return _verified_type; }
        }

        public string verified_reason
        {
            set { _verified_reason = value; }
            get { return _verified_reason; }
        }

        public bool allow_all_comment
        {
            set { _allow_all_comment = value; }
            get { return _allow_all_comment; }
        }

        public string avatar_large
        {
            set { _avatar_large = value; }
            get { return _avatar_large; }
        }

        public string lang
        {
            set { _lang = value; }
            get { return _lang; }
        }
        #endregion Model

        #region  成员方法

        /// <summary>
        /// 根据指定的Uid得到一个对象实体
        /// </summary>
        public User(long lUid)
        {
            this.GetModel(lUid);
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool ExistInDB(long lUid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect("select count(user_id) from users where user_id=" + lUid.ToString());
            return count > 0;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString("u").Replace("Z", "") + "'";
                htValues.Add("user_id", _user_id);
                htValues.Add("screen_name", "'" + _screen_name.Replace("'", "''") + "'");
                htValues.Add("name", "'" + _name.Replace("'", "''") + "'");
                htValues.Add("province", "'" + _province + "'");
                htValues.Add("city", "'" + _city + "'");
                htValues.Add("location", "'" + _location.Replace("'", "''") + "'");
                htValues.Add("description", "'" + _description.Replace("'", "''") + "'");
                htValues.Add("url", "'" + _url.Replace("'", "''") + "'");
                htValues.Add("profile_image_url", "'" + _profile_image_url.Replace("'", "''") + "'");
                htValues.Add("domain", "'" + _domain.Replace("'", "''") + "'");
                htValues.Add("gender", "'" + _gender + "'");
                htValues.Add("followers_count", _followers_count);
                htValues.Add("friends_count", _friends_count);
                htValues.Add("statuses_count", _statuses_count);
                htValues.Add("favourites_count", _favourites_count);
                htValues.Add("created_at", "'" + _created_at + "'");
                if (_following)
                    htValues.Add("following", 1);
                else
                    htValues.Add("following", 0);
                if (_verified)
                    htValues.Add("verified", 1);
                else
                    htValues.Add("verified", 0);
                if (_allow_all_act_msg)
                    htValues.Add("allow_all_act_msg", 1);
                else
                    htValues.Add("allow_all_act_msg", 0);
                if (_geo_enabled)
                    htValues.Add("geo_enabled", 1);
                else
                    htValues.Add("geo_enabled", 0);
                htValues.Add("iteration", 0);
                htValues.Add("update_time", _update_time);

                db.Insert("users", htValues);
            }
            catch
            { return; }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void Update()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString("u").Replace("Z", "") + "'";
                htValues.Add("screen_name", "'" + _screen_name.Replace("'", "''") + "'");
                htValues.Add("name", "'" + _name.Replace("'", "''") + "'");
                htValues.Add("province", "'" + _province + "'");
                htValues.Add("city", "'" + _city + "'");
                htValues.Add("location", "'" + _location.Replace("'", "''") + "'");
                htValues.Add("description", "'" + _description.Replace("'", "''") + "'");
                htValues.Add("url", "'" + _url.Replace("'", "''") + "'");
                htValues.Add("profile_image_url", "'" + _profile_image_url.Replace("'", "''") + "'");
                htValues.Add("domain", "'" + _domain.Replace("'", "''") + "'");
                htValues.Add("gender", "'" + _gender + "'");
                htValues.Add("followers_count", _followers_count);
                htValues.Add("friends_count", _friends_count);
                htValues.Add("statuses_count", _statuses_count);
                htValues.Add("favourites_count", _favourites_count);
                htValues.Add("created_at", "'" + _created_at + "'");
                //-------------20120222加--------------
                htValues.Add("idstr", "'" + _idstr + "'");
                htValues.Add("weihao", "'" + _weihao + "'");
                htValues.Add("bi_followers_count", _bi_followers_count);
                htValues.Add("online_status", _online_status);
                htValues.Add("verified_type", _verified_type);
                htValues.Add("verified_reason", "'" + _verified_reason + "'");
                if (_allow_all_comment)
                    htValues.Add("allow_all_comment", 1);
                else
                    htValues.Add("allow_all_comment", 0);
                htValues.Add("avatar_large", "'" + _avatar_large.Replace("'", "''") + "'");
                htValues.Add("lang", "'" + _lang.Replace("'", "''") + "'");
                //--------------------------------------
                if (_following)
                    htValues.Add("following", 1);
                else
                    htValues.Add("following", 0);
                if (_verified)
                    htValues.Add("verified", 1);
                else
                    htValues.Add("verified", 0);
                if (_allow_all_act_msg)
                    htValues.Add("allow_all_act_msg", 1);
                else
                    htValues.Add("allow_all_act_msg", 0);
                if (_geo_enabled)
                    htValues.Add("geo_enabled", 1);
                else
                    htValues.Add("geo_enabled", 0);
                htValues.Add("iteration", 0);
                htValues.Add("update_time", _update_time);

                db.Update("users", htValues, "user_id=" + _user_id.ToString());
            }
            catch
            { return; }
        }

        /// <summary>
        /// 根据UserID得到一个对象实体
        /// </summary>
        public bool GetModel(long lUid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * FROM users ");
            strSql.Append(" where user_id=" + lUid.ToString());

            DataRow dr = db.GetDataRow(strSql.ToString());
            if (dr != null)
            {
                _user_id = Convert.ToInt64(dr["user_id"]);
                _screen_name = dr["screen_name"].ToString();
                _name = dr["name"].ToString();
                _province = dr["province"].ToString();
                _city = dr["city"].ToString();
                _location = dr["location"].ToString();
                _description = dr["description"].ToString();
                _url = dr["url"].ToString();
                _profile_image_url = dr["profile_image_url"].ToString();
                _domain = dr["domain"].ToString();
                _gender = dr["gender"].ToString();
                if (dr["followers_count"].ToString() != "")
                {
                    _followers_count = Convert.ToInt32(dr["followers_count"]);
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32(dr["friends_count"]);
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32(dr["statuses_count"]);
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32(dr["favourites_count"]);
                }
                _created_at = dr["created_at"].ToString();
                if (dr["following"].ToString() != "")
                {
                    if (dr["following"].ToString() == "1")
                    {
                        _following = true;
                    }
                    else
                    {
                        _following = false;
                    }
                }
                if (dr["verified"].ToString() != "")
                {
                    if (dr["verified"].ToString() == "1")
                    {
                        _verified = true;
                    }
                    else
                    {
                        _verified = false;
                    }
                }
                if (dr["allow_all_act_msg"].ToString() != "")
                {
                    if (dr["allow_all_act_msg"].ToString() == "1")
                    {
                        _allow_all_act_msg = true;
                    }
                    else
                    {
                        _allow_all_act_msg = false;
                    }
                }
                if (dr["geo_enabled"].ToString() != "")
                {
                    if (dr["geo_enabled"].ToString() == "1")
                    {
                        _geo_enabled = true;
                    }
                    else
                    {
                        _geo_enabled = false;
                    }
                }
                _iteration = Convert.ToInt32(dr["iteration"]);
                _update_time = dr["update_time"].ToString();
                //-------------20120222加--------------
                _idstr = dr["idstr"].ToString();
                _weihao = dr["weihao"].ToString();
                if (dr["bi_followers_count"].ToString() != "")
                {
                    _bi_followers_count = Convert.ToInt32(dr["bi_followers_count"]);
                }
                if (dr["online_status"].ToString() != "")
                {
                    _online_status = Convert.ToInt32(dr["online_status"]);
                }
                if (dr["verified_type"].ToString() != "")
                {
                    _verified_type = Convert.ToInt32(dr["verified_type"]);
                }
                _verified_reason = dr["verified_reason"].ToString();
                if (dr["allow_all_comment"].ToString() != "")
                {
                    if (dr["allow_all_comment"].ToString() == "1")
                    {
                        _allow_all_comment = true;
                    }
                    else
                    {
                        _allow_all_comment = false;
                    }
                }
                _avatar_large = dr["avatar_large"].ToString();
                _lang = dr["lang"].ToString();
                //--------------------------------------
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 根据用户昵称得到一个对象实体
        /// </summary>
        public bool GetModel(string strScreenName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * FROM users ");
            strSql.Append(" where screen_name='" + strScreenName + "'");

            DataRow dr = db.GetDataRow(strSql.ToString());
            if (dr != null)
            {
                _user_id = Convert.ToInt64(dr["user_id"]);
                _screen_name = dr["screen_name"].ToString();
                _name = dr["name"].ToString();
                _province = dr["province"].ToString();
                _city = dr["city"].ToString();
                _location = dr["location"].ToString();
                _description = dr["description"].ToString();
                _url = dr["url"].ToString();
                _profile_image_url = dr["profile_image_url"].ToString();
                _domain = dr["domain"].ToString();
                _gender = dr["gender"].ToString();
                if (dr["followers_count"].ToString() != "")
                {
                    _followers_count = Convert.ToInt32(dr["followers_count"]);
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32(dr["friends_count"]);
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32(dr["statuses_count"]);
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32(dr["favourites_count"]);
                }
                _created_at = dr["created_at"].ToString();
                if (dr["following"].ToString() != "")
                {
                    if (dr["following"].ToString() == "1")
                    {
                        _following = true;
                    }
                    else
                    {
                        _following = false;
                    }
                }
                if (dr["verified"].ToString() != "")
                {
                    if (dr["verified"].ToString() == "1")
                    {
                        _verified = true;
                    }
                    else
                    {
                        _verified = false;
                    }
                }
                if (dr["allow_all_act_msg"].ToString() != "")
                {
                    if (dr["allow_all_act_msg"].ToString() == "1")
                    {
                        _allow_all_act_msg = true;
                    }
                    else
                    {
                        _allow_all_act_msg = false;
                    }
                }
                if (dr["geo_enabled"].ToString() != "")
                {
                    if (dr["geo_enabled"].ToString() == "1")
                    {
                        _geo_enabled = true;
                    }
                    else
                    {
                        _geo_enabled = false;
                    }
                }
                _iteration = Convert.ToInt32(dr["iteration"]);
                _update_time = dr["update_time"].ToString();
                //-------------20120222加--------------
                _idstr = dr["idstr"].ToString();
                _weihao = dr["weihao"].ToString();
                if (dr["bi_followers_count"].ToString() != "")
                {
                    _bi_followers_count = Convert.ToInt32(dr["bi_followers_count"]);
                }
                if (dr["online_status"].ToString() != "")
                {
                    _online_status = Convert.ToInt32(dr["online_status"]);
                }
                if (dr["verified_type"].ToString() != "")
                {
                    _verified_type = Convert.ToInt32(dr["verified_type"]);
                }
                _verified_reason = dr["verified_reason"].ToString();
                if (dr["allow_all_comment"].ToString() != "")
                {
                    if (dr["allow_all_comment"].ToString() == "1")
                    {
                        _allow_all_comment = true;
                    }
                    else
                    {
                        _allow_all_comment = false;
                    }
                }
                _avatar_large = dr["avatar_large"].ToString();
                _lang = dr["lang"].ToString();
                //--------------------------------------
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 同时根据用户ID和用户昵称得到一个对象实体
        /// </summary>
        public bool GetModel(long lUid, string strScreenName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSql = "select  top 1 * FROM users where user_id=" + lUid.ToString() + " and screen_name='" + strScreenName + "'";

            DataRow dr = db.GetDataRow(strSql);
            if (dr != null)
            {
                _user_id = Convert.ToInt64(dr["user_id"]);
                _screen_name = dr["screen_name"].ToString();
                _name = dr["name"].ToString();
                _province = dr["province"].ToString();
                _city = dr["city"].ToString();
                _location = dr["location"].ToString();
                _description = dr["description"].ToString();
                _url = dr["url"].ToString();
                _profile_image_url = dr["profile_image_url"].ToString();
                _domain = dr["domain"].ToString();
                _gender = dr["gender"].ToString();
                if (dr["followers_count"].ToString() != "")
                {
                    _followers_count = Convert.ToInt32(dr["followers_count"]);
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32(dr["friends_count"]);
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32(dr["statuses_count"]);
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32(dr["favourites_count"]);
                }
                _created_at = dr["created_at"].ToString();
                if (dr["following"].ToString() != "")
                {
                    if (dr["following"].ToString() == "1")
                    {
                        _following = true;
                    }
                    else
                    {
                        _following = false;
                    }
                }
                if (dr["verified"].ToString() != "")
                {
                    if (dr["verified"].ToString() == "1")
                    {
                        _verified = true;
                    }
                    else
                    {
                        _verified = false;
                    }
                }
                if (dr["allow_all_act_msg"].ToString() != "")
                {
                    if (dr["allow_all_act_msg"].ToString() == "1")
                    {
                        _allow_all_act_msg = true;
                    }
                    else
                    {
                        _allow_all_act_msg = false;
                    }
                }
                if (dr["geo_enabled"].ToString() != "")
                {
                    if (dr["geo_enabled"].ToString() == "1")
                    {
                        _geo_enabled = true;
                    }
                    else
                    {
                        _geo_enabled = false;
                    }
                }
                _iteration = Convert.ToInt32(dr["iteration"]);
                _update_time = dr["update_time"].ToString();
                //-------------20120222加--------------
                _idstr = dr["idstr"].ToString();
                _weihao = dr["weihao"].ToString();
                if (dr["bi_followers_count"].ToString() != "")
                {
                    _bi_followers_count = Convert.ToInt32(dr["bi_followers_count"]);
                }
                if (dr["online_status"].ToString() != "")
                {
                    _online_status = Convert.ToInt32(dr["online_status"]);
                }
                if (dr["verified_type"].ToString() != "")
                {
                    _verified_type = Convert.ToInt32(dr["verified_type"]);
                }
                _verified_reason = dr["verified_reason"].ToString();
                if (dr["allow_all_comment"].ToString() != "")
                {
                    if (dr["allow_all_comment"].ToString() == "1")
                    {
                        _allow_all_comment = true;
                    }
                    else
                    {
                        _allow_all_comment = false;
                    }
                }
                _avatar_large = dr["avatar_large"].ToString();
                _lang = dr["lang"].ToString();
                //--------------------------------------
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 获得用户表中已爬取所有UserID
        /// 返回DataTable，以便于调用者观察进度——本不应该将DataTable暴露给上层的，无奈
        /// </summary>
        static public DataTable GetCrawedUserIDTable(int nTop)
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select top "+nTop.ToString()+" user_id from users order by update_time";
            DataSet ds = db.GetDataSet(strSQL);
            if (ds == null) return null;
            else return ds.Tables[0];
        }

        #endregion  成员方法
    }
}

