using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Sina.Api
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
    
    //id: 用户UID
    //screen_name: 微博昵称
    //name: 友好显示名称，如Tim Yang(此特性暂不支持)
    //province:省份编码（参考省份编码表）
    //city: 城市编码（参考城市编码表）
    //location：地址
    //description: 个人描叙
    //url: 用户博客地址
    //profile_image_url: 自定义图像
    //domain: 用户个性化域名
    //gender: 性别,m--男，f--女,n--未知
    //followers_count: 粉丝数
    //friends_count: 关注数
    //statuses_count: 微博数
    //favourites_count: 收藏数
    //created_at: 创建时间
    //following: 是否已关注(此特性暂不支持)
    //verified: 加V标示
    //allow_all_act_msg: 未知
    //geo_enabled: 未知（允许地理信息？）

	public class User
	{
        private Database db = new Database();

		public User()
		{}
		#region Model
		private long _uid;
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
		private DateTime _created_at;
		private bool _following;
		private bool _verified;
		private bool _allow_all_act_msg;
		private bool _geo_enabled;
		/// <summary>
		/// 用户UID（XML中为id）
		/// </summary>
		public long uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 微博昵称
		/// </summary>
		public string screen_name
		{
			set{ _screen_name=value;}
			get{return _screen_name;}
		}
		/// <summary>
		/// 友好显示名称，如Bill Gates(此特性暂不支持) 
		/// </summary>
		public string name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 省份编码（参考省份编码表）
		/// </summary>
		public string province
		{
			set{ _province=value;}
			get{return _province;}
		}
		/// <summary>
		/// 城市编码（参考城市编码表）
		/// </summary>
		public string city
		{
			set{ _city=value;}
			get{return _city;}
		}
		/// <summary>
		/// 地址
		/// </summary>
		public string location
		{
			set{ _location=value;}
			get{return _location;}
		}
		/// <summary>
		/// 个人描述
		/// </summary>
		public string description
		{
			set{ _description=value;}
			get{return _description;}
		}
		/// <summary>
		/// 用户博客地址
		/// </summary>
		public string url
		{
			set{ _url=value;}
			get{return _url;}
		}
		/// <summary>
		/// 自定义图像
		/// </summary>
		public string profile_image_url
		{
			set{ _profile_image_url=value;}
			get{return _profile_image_url;}
		}
		/// <summary>
		/// 用户个性化URL
		/// </summary>
		public string domain
		{
			set{ _domain=value;}
			get{return _domain;}
		}
		/// <summary>
		/// 性别,m--男，f--女,n--未知
		/// </summary>
		public string gender
		{
			set{ _gender=value;}
			get{return _gender;}
		}
		/// <summary>
		/// 粉丝数
		/// </summary>
		public int followers_count
		{
			set{ _followers_count=value;}
			get{return _followers_count;}
		}
		/// <summary>
		/// 关注数
		/// </summary>
		public int friends_count
		{
			set{ _friends_count=value;}
			get{return _friends_count;}
		}
		/// <summary>
		/// 微博数
		/// </summary>
		public int statuses_count
		{
			set{ _statuses_count=value;}
			get{return _statuses_count;}
		}
		/// <summary>
		/// 收藏数
		/// </summary>
		public int favourites_count
		{
			set{ _favourites_count=value;}
			get{return _favourites_count;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime created_at
		{
			set{ _created_at=value;}
			get{return _created_at;}
		}
		/// <summary>
		/// 是否已关注(此特性暂不支持)
		/// </summary>
		public bool following
		{
			set{ _following=value;}
			get{return _following;}
		}
		/// <summary>
		/// 加V标示，是否微博认证用户
		/// </summary>
		public bool verified
		{
			set{ _verified=value;}
			get{return _verified;}
		}
		/// <summary>
		/// 未知
		/// </summary>
		public bool allow_all_act_msg
		{
			set{ _allow_all_act_msg=value;}
			get{return _allow_all_act_msg;}
		}
		/// <summary>
		/// 未知（允许地理信息？）
		/// </summary>
		public bool geo_enabled
		{
			set{ _geo_enabled=value;}
			get{return _geo_enabled;}
		}
		#endregion Model
        
		#region  成员方法

		/// <summary>
		/// 根据指定的Uid得到一个对象实体
		/// </summary>
		public User(long lUid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select uid,screen_name,name,province,city,location,description,url,profile_image_url,domain,gender,followers_count,friends_count,statuses_count,favourites_count,created_at,following,verified,allow_all_act_msg,geo_enabled ");
            //strSql.Append(" FROM users ");
            //strSql.Append(" where uid=@uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    uid=ds.Tables[0].Rows[0]["uid"].ToString();
            //    screen_name=ds.Tables[0].Rows[0]["screen_name"].ToString();
            //    name=ds.Tables[0].Rows[0]["name"].ToString();
            //    province=ds.Tables[0].Rows[0]["province"].ToString();
            //    city=ds.Tables[0].Rows[0]["city"].ToString();
            //    location=ds.Tables[0].Rows[0]["location"].ToString();
            //    description=ds.Tables[0].Rows[0]["description"].ToString();
            //    url=ds.Tables[0].Rows[0]["url"].ToString();
            //    profile_image_url=ds.Tables[0].Rows[0]["profile_image_url"].ToString();
            //    domain=ds.Tables[0].Rows[0]["domain"].ToString();
            //    gender=ds.Tables[0].Rows[0]["gender"].ToString();
            //    if(ds.Tables[0].Rows[0]["followers_count"].ToString()!="")
            //    {
            //        followers_count=int.Parse(ds.Tables[0].Rows[0]["followers_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["friends_count"].ToString()!="")
            //    {
            //        friends_count=int.Parse(ds.Tables[0].Rows[0]["friends_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["statuses_count"].ToString()!="")
            //    {
            //        statuses_count=int.Parse(ds.Tables[0].Rows[0]["statuses_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["favourites_count"].ToString()!="")
            //    {
            //        favourites_count=int.Parse(ds.Tables[0].Rows[0]["favourites_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
            //    {
            //        created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["following"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["following"].ToString()=="1")||(ds.Tables[0].Rows[0]["following"].ToString().ToLower()=="true"))
            //        {
            //            following=true;
            //        }
            //        else
            //        {
            //            following=false;
            //        }
            //    }

            //    if(ds.Tables[0].Rows[0]["verified"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["verified"].ToString()=="1")||(ds.Tables[0].Rows[0]["verified"].ToString().ToLower()=="true"))
            //        {
            //            verified=true;
            //        }
            //        else
            //        {
            //            verified=false;
            //        }
            //    }

            //    if(ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString()=="1")||(ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString().ToLower()=="true"))
            //        {
            //            allow_all_act_msg=true;
            //        }
            //        else
            //        {
            //            allow_all_act_msg=false;
            //        }
            //    }

            //    if(ds.Tables[0].Rows[0]["geo_enabled"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["geo_enabled"].ToString()=="1")||(ds.Tables[0].Rows[0]["geo_enabled"].ToString().ToLower()=="true"))
            //        {
            //            geo_enabled=true;
            //        }
            //        else
            //        {
            //            geo_enabled=false;
            //        }
            //    }

            //}
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from users");
			strSql.Append(" where uid=@uid ");

			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;

			return true;
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into users(");
			strSql.Append("uid,screen_name,name,province,city,location,description,url,profile_image_url,domain,gender,followers_count,friends_count,statuses_count,favourites_count,created_at,following,verified,allow_all_act_msg,geo_enabled)");
			strSql.Append(" values (");
			strSql.Append("@uid,@screen_name,@name,@province,@city,@location,@description,@url,@profile_image_url,@domain,@gender,@followers_count,@friends_count,@statuses_count,@favourites_count,@created_at,@following,@verified,@allow_all_act_msg,@geo_enabled)");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@screen_name", SqlDbType.VarChar,50),
					new SqlParameter("@name", SqlDbType.VarChar,50),
					new SqlParameter("@province", SqlDbType.Char,2),
					new SqlParameter("@city", SqlDbType.VarChar,2),
					new SqlParameter("@location", SqlDbType.VarChar,200),
					new SqlParameter("@description", SqlDbType.VarChar,200),
					new SqlParameter("@url", SqlDbType.VarChar,50),
					new SqlParameter("@profile_image_url", SqlDbType.VarChar,100),
					new SqlParameter("@domain", SqlDbType.VarChar,20),
					new SqlParameter("@gender", SqlDbType.Char,1),
					new SqlParameter("@followers_count", SqlDbType.Int,4),
					new SqlParameter("@friends_count", SqlDbType.Int,4),
					new SqlParameter("@statuses_count", SqlDbType.Int,4),
					new SqlParameter("@favourites_count", SqlDbType.Int,4),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@following", SqlDbType.Bit,1),
					new SqlParameter("@verified", SqlDbType.Bit,1),
					new SqlParameter("@allow_all_act_msg", SqlDbType.Bit,1),
					new SqlParameter("@geo_enabled", SqlDbType.Bit,1)};
            Hashtable htValues = new Hashtable();
            htValues.Add("uid", uid);
            htValues.Add("screen_name","'"+screen_name.Replace( "'", "''" )+"'");
            htValues.Add("name", "'" + name.Replace("'", "''") + "'");
            htValues.Add("province","'"+province+"'");
            htValues.Add("city","'"+city+"'");
            htValues.Add("location", "'" + location.Replace("'", "''") + "'");
            htValues.Add("description", "'" + description.Replace("'", "''") + "'");
            htValues.Add("url", "'" + url.Replace("'", "''") + "'");
            htValues.Add("profile_image_url", "'" + profile_image_url.Replace("'", "''") + "'");
            htValues.Add("domain", "'" + domain.Replace("'", "''") + "'");
            htValues.Add("gender","'"+gender+"'");
            htValues.Add("followers_count",followers_count);
            htValues.Add("friends_count", friends_count);
            htValues.Add("statuses_count",statuses_count);
            htValues.Add("favourites_count",favourites_count);
            htValues.Add("created_at",created_at);
			parameters[16].Value = following;
			parameters[17].Value = verified;
			parameters[18].Value = allow_all_act_msg;
			parameters[19].Value = geo_enabled;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public void Update()
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update users set ");
			strSql.Append("screen_name=@screen_name,");
			strSql.Append("name=@name,");
			strSql.Append("province=@province,");
			strSql.Append("city=@city,");
			strSql.Append("location=@location,");
			strSql.Append("description=@description,");
			strSql.Append("url=@url,");
			strSql.Append("profile_image_url=@profile_image_url,");
			strSql.Append("domain=@domain,");
			strSql.Append("gender=@gender,");
			strSql.Append("followers_count=@followers_count,");
			strSql.Append("friends_count=@friends_count,");
			strSql.Append("statuses_count=@statuses_count,");
			strSql.Append("favourites_count=@favourites_count,");
			strSql.Append("created_at=@created_at,");
			strSql.Append("following=@following,");
			strSql.Append("verified=@verified,");
			strSql.Append("allow_all_act_msg=@allow_all_act_msg,");
			strSql.Append("geo_enabled=@geo_enabled");
			strSql.Append(" where uid=@uid ");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt,8),
					new SqlParameter("@screen_name", SqlDbType.VarChar,50),
					new SqlParameter("@name", SqlDbType.VarChar,50),
					new SqlParameter("@province", SqlDbType.Char,2),
					new SqlParameter("@city", SqlDbType.VarChar,2),
					new SqlParameter("@location", SqlDbType.VarChar,200),
					new SqlParameter("@description", SqlDbType.VarChar,200),
					new SqlParameter("@url", SqlDbType.VarChar,50),
					new SqlParameter("@profile_image_url", SqlDbType.VarChar,100),
					new SqlParameter("@domain", SqlDbType.VarChar,20),
					new SqlParameter("@gender", SqlDbType.Char,1),
					new SqlParameter("@followers_count", SqlDbType.Int,4),
					new SqlParameter("@friends_count", SqlDbType.Int,4),
					new SqlParameter("@statuses_count", SqlDbType.Int,4),
					new SqlParameter("@favourites_count", SqlDbType.Int,4),
					new SqlParameter("@created_at", SqlDbType.DateTime),
					new SqlParameter("@following", SqlDbType.Bit,1),
					new SqlParameter("@verified", SqlDbType.Bit,1),
					new SqlParameter("@allow_all_act_msg", SqlDbType.Bit,1),
					new SqlParameter("@geo_enabled", SqlDbType.Bit,1)};
			parameters[0].Value = uid;
			parameters[1].Value = screen_name;
			parameters[2].Value = name;
			parameters[3].Value = province;
			parameters[4].Value = city;
			parameters[5].Value = location;
			parameters[6].Value = description;
			parameters[7].Value = url;
			parameters[8].Value = profile_image_url;
			parameters[9].Value = domain;
			parameters[10].Value = gender;
			parameters[11].Value = followers_count;
			parameters[12].Value = friends_count;
			parameters[13].Value = statuses_count;
			parameters[14].Value = favourites_count;
			parameters[15].Value = created_at;
			parameters[16].Value = following;
			parameters[17].Value = verified;
			parameters[18].Value = allow_all_act_msg;
			parameters[19].Value = geo_enabled;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public void Delete(long uid)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from users ");
			strSql.Append(" where uid=@uid ");
			SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.BigInt)};
			parameters[0].Value = uid;

			//DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public void GetModel(long uid)
		{
            //StringBuilder strSql=new StringBuilder();
            //strSql.Append("select  top 1 uid,screen_name,name,province,city,location,description,url,profile_image_url,domain,gender,followers_count,friends_count,statuses_count,favourites_count,created_at,following,verified,allow_all_act_msg,geo_enabled ");
            //strSql.Append(" FROM users ");
            //strSql.Append(" where uid=@uid ");
            //SqlParameter[] parameters = {
            //        new SqlParameter("@uid", SqlDbType.BigInt)};
            //parameters[0].Value = uid;

            //DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    if(ds.Tables[0].Rows[0]["uid"].ToString()!="")
            //    {
            //        model.uid=long.Parse(ds.Tables[0].Rows[0]["uid"].ToString());
            //    }
            //    model.screen_name=ds.Tables[0].Rows[0]["screen_name"].ToString();
            //    model.name=ds.Tables[0].Rows[0]["name"].ToString();
            //    model.province=ds.Tables[0].Rows[0]["province"].ToString();
            //    model.city=ds.Tables[0].Rows[0]["city"].ToString();
            //    model.location=ds.Tables[0].Rows[0]["location"].ToString();
            //    model.description=ds.Tables[0].Rows[0]["description"].ToString();
            //    model.url=ds.Tables[0].Rows[0]["url"].ToString();
            //    model.profile_image_url=ds.Tables[0].Rows[0]["profile_image_url"].ToString();
            //    model.domain=ds.Tables[0].Rows[0]["domain"].ToString();
            //    model.gender=ds.Tables[0].Rows[0]["gender"].ToString();
            //    if(ds.Tables[0].Rows[0]["followers_count"].ToString()!="")
            //    {
            //        model.followers_count=int.Parse(ds.Tables[0].Rows[0]["followers_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["friends_count"].ToString()!="")
            //    {
            //        model.friends_count=int.Parse(ds.Tables[0].Rows[0]["friends_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["statuses_count"].ToString()!="")
            //    {
            //        model.statuses_count=int.Parse(ds.Tables[0].Rows[0]["statuses_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["favourites_count"].ToString()!="")
            //    {
            //        model.favourites_count=int.Parse(ds.Tables[0].Rows[0]["favourites_count"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["created_at"].ToString()!="")
            //    {
            //        model.created_at=DateTime.Parse(ds.Tables[0].Rows[0]["created_at"].ToString());
            //    }
            //    if(ds.Tables[0].Rows[0]["following"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["following"].ToString()=="1")||(ds.Tables[0].Rows[0]["following"].ToString().ToLower()=="true"))
            //        {
            //            model.following=true;
            //        }
            //        else
            //        {
            //            model.following=false;
            //        }
            //    }
            //    if(ds.Tables[0].Rows[0]["verified"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["verified"].ToString()=="1")||(ds.Tables[0].Rows[0]["verified"].ToString().ToLower()=="true"))
            //        {
            //            model.verified=true;
            //        }
            //        else
            //        {
            //            model.verified=false;
            //        }
            //    }
            //    if(ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString()=="1")||(ds.Tables[0].Rows[0]["allow_all_act_msg"].ToString().ToLower()=="true"))
            //        {
            //            model.allow_all_act_msg=true;
            //        }
            //        else
            //        {
            //            model.allow_all_act_msg=false;
            //        }
            //    }
            //    if(ds.Tables[0].Rows[0]["geo_enabled"].ToString()!="")
            //    {
            //        if((ds.Tables[0].Rows[0]["geo_enabled"].ToString()=="1")||(ds.Tables[0].Rows[0]["geo_enabled"].ToString().ToLower()=="true"))
            //        {
            //            model.geo_enabled=true;
            //        }
            //        else
            //        {
            //            model.geo_enabled=false;
            //        }
            //    }
            //}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select * ");
			strSql.Append(" FROM users ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return null;
		}

		#endregion  成员方法
	}
}

