using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sinawler.Model
{
	/// <summary>
	/// ��user�������еķ��������������ݿ⽻�������ж�ȡ��������д������
	/// </summary>
    
    //<id>88888888</id> 
    //<screen_name>��Ϳ�����</screen_name> 
    //<name>��Ϳ�����</name> 
    //<province>50</province> 
    //<city>1000</city> 
    //<location>����</location> 
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
    
    //id: �û�UserID
    //screen_name: ΢���ǳ�
    //name: �Ѻ���ʾ���ƣ���Tim Yang(�������ݲ�֧��)
    //province:ʡ�ݱ��루�ο�ʡ�ݱ����
    //city: ���б��루�ο����б����
    //location����ַ
    //description: ��������
    //url: �û����͵�ַ
    //profile_image_url: �Զ���ͼ��
    //domain: �û����Ի�����
    //gender: �Ա�,m--�У�f--Ů,n--δ֪
    //followers_count: ��˿��
    //friends_count: ��ע��
    //statuses_count: ΢����
    //favourites_count: �ղ���
    //created_at: ����ʱ��
    //following: �Ƿ��ѹ�ע(�������ݲ�֧��)
    //verified: ��V��ʾ
    //allow_all_act_msg: δ֪
    //geo_enabled: δ֪�����������Ϣ����
    //iteration: ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������

	public class User
	{
        public User()
        {  }

		#region Model
		private long _user_id=0;
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
		/// <summary>
		/// �û�UserID��XML��Ϊid��
		/// </summary>
		public long user_id
		{
			set{ _user_id=value;}
			get{return _user_id;}
		}
		/// <summary>
		/// ΢���ǳ�
		/// </summary>
		public string screen_name
		{
			set{ _screen_name=value;}
			get{return _screen_name;}
		}
		/// <summary>
		/// �Ѻ���ʾ���ƣ���Bill Gates(�������ݲ�֧��) 
		/// </summary>
		public string name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// ʡ�ݱ��루�ο�ʡ�ݱ����
		/// </summary>
		public string province
		{
			set{ _province=value;}
			get{return _province;}
		}
		/// <summary>
		/// ���б��루�ο����б����
		/// </summary>
		public string city
		{
			set{ _city=value;}
			get{return _city;}
		}
		/// <summary>
		/// ��ַ
		/// </summary>
		public string location
		{
			set{ _location=value;}
			get{return _location;}
		}
		/// <summary>
		/// ��������
		/// </summary>
		public string description
		{
			set{ _description=value;}
			get{return _description;}
		}
		/// <summary>
		/// �û����͵�ַ
		/// </summary>
		public string url
		{
			set{ _url=value;}
			get{return _url;}
		}
		/// <summary>
		/// �Զ���ͼ��
		/// </summary>
		public string profile_image_url
		{
			set{ _profile_image_url=value;}
			get{return _profile_image_url;}
		}
		/// <summary>
		/// �û����Ի�URL
		/// </summary>
		public string domain
		{
			set{ _domain=value;}
			get{return _domain;}
		}
		/// <summary>
		/// �Ա�,m--�У�f--Ů,n--δ֪
		/// </summary>
		public string gender
		{
			set{ _gender=value;}
			get{return _gender;}
		}
		/// <summary>
		/// ��˿��
		/// </summary>
		public int followers_count
		{
			set{ _followers_count=value;}
			get{return _followers_count;}
		}
		/// <summary>
		/// ��ע��
		/// </summary>
		public int friends_count
		{
			set{ _friends_count=value;}
			get{return _friends_count;}
		}
		/// <summary>
		/// ΢����
		/// </summary>
		public int statuses_count
		{
			set{ _statuses_count=value;}
			get{return _statuses_count;}
		}
		/// <summary>
		/// �ղ���
		/// </summary>
		public int favourites_count
		{
			set{ _favourites_count=value;}
			get{return _favourites_count;}
		}
		/// <summary>
		/// ����ʱ�䣬��ʽΪyyyy-mm-dd hh:mm:ss
		/// </summary>
		public string created_at
		{
			set{ _created_at=value;}
			get{return _created_at;}
		}
		/// <summary>
		/// �Ƿ��ѹ�ע(�������ݲ�֧��)
		/// </summary>
		public bool following
		{
			set{ _following=value;}
			get{return _following;}
		}
		/// <summary>
		/// ��V��ʾ���Ƿ�΢����֤�û�
		/// </summary>
		public bool verified
		{
			set{ _verified=value;}
			get{return _verified;}
		}
		/// <summary>
		/// δ֪
		/// </summary>
		public bool allow_all_act_msg
		{
			set{ _allow_all_act_msg=value;}
			get{return _allow_all_act_msg;}
		}
		/// <summary>
		/// δ֪�����������Ϣ����
		/// </summary>
		public bool geo_enabled
		{
			set{ _geo_enabled=value;}
			get{return _geo_enabled;}
		}
        /// <summary>
        /// ����������Ĭ��Ϊ0��ÿ����һ�Σ��ͼ�1����Ϊ0��Ϊ���������
        /// </summary>
        public int iteration
        {
            get { return _iteration; }
        }
        /// <summary>
        /// ��¼����ʱ��
        /// </summary>
        public string update_time
        {
            set { _update_time = value; }
            get { return _update_time; }
        }
		#endregion Model
        
		#region  ��Ա����

		/// <summary>
		/// ����ָ����Uid�õ�һ������ʵ��
		/// </summary>
		public User(long lUid)
		{
            this.GetModel( lUid );
		}

		/// <summary>
		/// �Ƿ���ڸü�¼
		/// </summary>
		static public bool Exists(long lUid)
		{
            Database db = DatabaseFactory.CreateDatabase();
            int count = db.CountByExecuteSQLSelect( "select count(user_id) from users where user_id=" + lUid.ToString() );
            return count > 0;
		}

        /// <summary>
        /// �������ݿ����������ݵĵ�������
        /// </summary>
        static public void NewIterate ()
        {
            Database db = DatabaseFactory.CreateDatabase();
            db.CountByExecuteSQL("update users set iteration=iteration+1");
        }

		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add()
		{
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "user_id", _user_id );
                htValues.Add( "screen_name", "'" + _screen_name.Replace( "'", "''" ) + "'" );
                htValues.Add( "name", "'" + _name.Replace( "'", "''" ) + "'" );
                htValues.Add( "province", "'" + _province + "'" );
                htValues.Add( "city", "'" + _city + "'" );
                htValues.Add( "location", "'" + _location.Replace( "'", "''" ) + "'" );
                htValues.Add( "description", "'" + _description.Replace( "'", "''" ) + "'" );
                htValues.Add( "url", "'" + _url.Replace( "'", "''" ) + "'" );
                htValues.Add( "profile_image_url", "'" + _profile_image_url.Replace( "'", "''" ) + "'" );
                htValues.Add( "domain", "'" + _domain.Replace( "'", "''" ) + "'" );
                htValues.Add( "gender", "'" + _gender + "'" );
                htValues.Add( "followers_count", _followers_count );
                htValues.Add( "friends_count", _friends_count );
                htValues.Add( "statuses_count", _statuses_count );
                htValues.Add( "favourites_count", _favourites_count );
                htValues.Add( "created_at", "'" + _created_at + "'" );
                if (_following)
                    htValues.Add( "following", 1 );
                else
                    htValues.Add( "following", 0 );
                if (_verified)
                    htValues.Add( "verified", 1 );
                else
                    htValues.Add( "verified", 0 );
                if (_allow_all_act_msg)
                    htValues.Add( "allow_all_act_msg", 1 );
                else
                    htValues.Add( "allow_all_act_msg", 0 );
                if (_geo_enabled)
                    htValues.Add( "geo_enabled", 1 );
                else
                    htValues.Add( "geo_enabled", 0 );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Insert( "users", htValues );
            }
            catch
            { return; }
		}

        /// <summary>
        /// ��������
        /// </summary>
        public void Update()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                Hashtable htValues = new Hashtable();
                _update_time = "'" + DateTime.Now.ToString( "u" ).Replace( "Z", "" ) + "'";
                htValues.Add( "screen_name", "'" + _screen_name.Replace( "'", "''" ) + "'" );
                htValues.Add( "name", "'" + _name.Replace( "'", "''" ) + "'" );
                htValues.Add( "province", "'" + _province + "'" );
                htValues.Add( "city", "'" + _city + "'" );
                htValues.Add( "location", "'" + _location.Replace( "'", "''" ) + "'" );
                htValues.Add( "description", "'" + _description.Replace( "'", "''" ) + "'" );
                htValues.Add( "url", "'" + _url.Replace( "'", "''" ) + "'" );
                htValues.Add( "profile_image_url", "'" + _profile_image_url.Replace( "'", "''" ) + "'" );
                htValues.Add( "domain", "'" + _domain.Replace( "'", "''" ) + "'" );
                htValues.Add( "gender", "'" + _gender + "'" );
                htValues.Add( "followers_count", _followers_count );
                htValues.Add( "friends_count", _friends_count );
                htValues.Add( "statuses_count", _statuses_count );
                htValues.Add( "favourites_count", _favourites_count );
                htValues.Add( "created_at", "'" + _created_at + "'" );
                if (_following)
                    htValues.Add( "following", 1 );
                else
                    htValues.Add( "following", 0 );
                if (_verified)
                    htValues.Add( "verified", 1 );
                else
                    htValues.Add( "verified", 0 );
                if (_allow_all_act_msg)
                    htValues.Add( "allow_all_act_msg", 1 );
                else
                    htValues.Add( "allow_all_act_msg", 0 );
                if (_geo_enabled)
                    htValues.Add( "geo_enabled", 1 );
                else
                    htValues.Add( "geo_enabled", 0 );
                htValues.Add( "iteration", 0 );
                htValues.Add( "update_time", _update_time );

                db.Update( "users", htValues,"user_id="+_user_id.ToString() );
            }
            catch
            { return; }
        }

		/// <summary>
		/// ����UserID�õ�һ������ʵ��
		/// </summary>
		public bool GetModel(long lUid)
		{
            Database db = DatabaseFactory.CreateDatabase();
            StringBuilder strSql = new StringBuilder();
            strSql.Append( "select  top 1 * FROM users " );
            strSql.Append( " where user_id="+lUid.ToString() );

            DataRow dr=db.GetDataRow( strSql.ToString() );
            if (dr != null)
            {
                _user_id = Convert.ToInt64( dr["user_id"] );
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
                    _followers_count = Convert.ToInt32( dr["followers_count"] );
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32( dr["friends_count"] );
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32( dr["statuses_count"] );
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32( dr["favourites_count"] );
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
                _iteration = Convert.ToInt32( dr["iteration"] );
                _update_time = dr["update_time"].ToString();
                return true;
            }
            else
                return false;
		}

        /// <summary>
        /// �����û��ǳƵõ�һ������ʵ��
        /// </summary>
        public bool GetModel ( string strScreenName )
        {
            Database db = DatabaseFactory.CreateDatabase();
            StringBuilder strSql = new StringBuilder();
            strSql.Append( "select  top 1 * FROM users " );
            strSql.Append( " where screen_name='" + strScreenName+"'" );

            DataRow dr = db.GetDataRow( strSql.ToString() );
            if (dr != null)
            {
                _user_id = Convert.ToInt64( dr["user_id"] );
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
                    _followers_count = Convert.ToInt32( dr["followers_count"] );
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32( dr["friends_count"] );
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32( dr["statuses_count"] );
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32( dr["favourites_count"] );
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
                _iteration = Convert.ToInt32( dr["iteration"] );
                _update_time = dr["update_time"].ToString();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// ͬʱ�����û�ID���û��ǳƵõ�һ������ʵ��
        /// </summary>
        public bool GetModel ( long lUid,string strScreenName )
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSql = "select  top 1 * FROM users where user_id="+lUid.ToString()+" and screen_name='" + strScreenName + "'";

            DataRow dr = db.GetDataRow( strSql );
            if (dr != null)
            {
                _user_id = Convert.ToInt64( dr["user_id"] );
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
                    _followers_count = Convert.ToInt32( dr["followers_count"] );
                }
                if (dr["friends_count"].ToString() != "")
                {
                    _friends_count = Convert.ToInt32( dr["friends_count"] );
                }
                if (dr["statuses_count"].ToString() != "")
                {
                    _statuses_count = Convert.ToInt32( dr["statuses_count"] );
                }
                if (dr["favourites_count"].ToString() != "")
                {
                    _favourites_count = Convert.ToInt32( dr["favourites_count"] );
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
                _iteration = Convert.ToInt32( dr["iteration"] );
                _update_time = dr["update_time"].ToString();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// ����û���������ȡ����UserID
        /// ����DataTable���Ա��ڵ����߹۲���ȡ�������Ӧ�ý�DataTable��¶���ϲ�ģ�����
        /// </summary>
        static public DataTable GetCrawedUserIDTable()
        {
            Database db = DatabaseFactory.CreateDatabase();
            string strSQL = "select user_id from users order by update_time";
            DataSet ds = db.GetDataSet(strSQL);
            if (ds == null) return null;
            else return ds.Tables[0];
        }

		#endregion  ��Ա����
	}
}

