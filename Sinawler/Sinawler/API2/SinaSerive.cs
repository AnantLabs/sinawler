using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using log4net;
using Newtonsoft.Json;
using Sinawler;
using Sinawler.Model;

namespace Open.Sina2SDK
{
    public class SinaSerive:OAuthBase,ISerive
    {
        //ILog logger = LogManager.GetLogger(typeof(SinaSerive));
        private JsonError error = new JsonError();
        private string json;

        public string JsonResult
        {get{return json;}}

        #region 构造函数
        public SinaSerive(string app_key, string app_secret, string redirect_uri)
            : base(app_key, app_secret, redirect_uri)
        { }

        public SinaSerive()
            : base()
        { }
        #endregion

        #region 微博
        #region 读取接口
        #region 获取最新的公共微博
        /// <summary>
        /// 获取最新的公共微博
        /// </summary>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <returns></returns>
        public Status[] Statuses_Public_Timeline(int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"page",page??1},
                {"count",count??50}
            };
            return HttpGet<Status[]>("statuses/public_timeline.json", dictionary);
        }
        #endregion

        #region 获取当前登录用户及其所关注用户的最新微博
        /// <summary>
        /// 获取当前登录用户及其所关注用户的最新微博
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="feature">过滤类型ID,默认为All。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Friends_Timeline(long? since_id, long? max_id, int? page, int? count, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"feature",(int)(feature??Feature.All)}
            };
            return HttpGet<JsonStatus[]>("statuses/friends_timeline.json", dictionary);
        }
        #endregion

        #region 获取当前登录用户及其所关注用户的最新微博
        /// <summary>
        /// 获取当前登录用户及其所关注用户的最新微博
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="feature">过滤类型ID，默认为All。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Home_Timeline(long? since_id, long? max_id, int? page, int? count, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"feature",(int)(feature??Feature.All)}
            };
            return HttpGet<JsonStatus[]>("statuses/home_timeline.json", dictionary); 
        }
        #endregion        

        #region 获取当前登录用户及其所关注用户的最新微博的ID
        /// <summary>
        /// 获取当前登录用户及其所关注用户的最新微博的ID
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="feature">过滤类型ID，默认为All。</param>
        /// <returns></returns>
        public string Statuses_Friends_Timeline_Ids(long? since_id, long? max_id, int? page, int? count, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"feature",(int)(feature??Feature.All)}
            };
            return HttpGet("statuses/friends_timeline/ids.json", dictionary);
        }
        #endregion

        #region 获取某个用户最新发表的微博列表
        /// <summary>
        /// 获取某个用户最新发表的微博列表
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为200。</param>
        /// <param name="trim_user">返回值中user信息开关，0：返回完整的user信息、1：user字段仅返回user_id，默认为0。</param>
        /// <param name="feature">过滤类型ID，默认为All。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_User_Timeline(long uid, long? since_id, long? max_id, int? page, int? count, int? trim_user, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"uid",uid},
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??200},
                {"trim_user",trim_user??0},
                {"feature",(int)(feature??Feature.All)}
            };
            return HttpGet<JsonStatus[]>("statuses/user_timeline.json", dictionary); 
        }
        #endregion

        #region 获取用户发布的微博的ID
        /// <summary>
        /// 获取用户发布的微博的ID
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="trim_user">返回值中user信息开关，0：返回完整的user信息、1：user字段仅返回user_id，默认为0。</param>
        /// <param name="feature">过滤类型ID，默认为All。</param>
        /// <returns></returns>
        public string Statuses_User_Timeline_Ids(int uid, long? since_id, long? max_id, int? page, int? count, int? trim_user, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"uid",uid},
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"trim_user",trim_user??0},
                {"feature",(int)(feature??Feature.All)}
            };
            return HttpGet("statuses/user_timeline/ids.json", dictionary); 
        }
        #endregion

        #region 获取指定微博的转发微博列表
        /// <summary>
        /// 获取指定微博的转发微博列表
        /// </summary>
        /// <param name="id">需要查询的微博ID。</param>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为200。</param>
        /// <param name="filter_by_author">作者筛选类型，默认为All。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Repost_Timeline(long id, long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"id",id},
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??200},
                {"feature",(int)(filter_by_author??FilterByAuthor.All)}
            };
            return HttpGet<JsonStatus[]>("statuses/repost_timeline.json", dictionary); 
        }
        #endregion

        #region 获取指定微博的转发微博的ID
        /// <summary>
        /// 获取指定微博的转发微博的ID
        /// </summary>
        /// <param name="id">需要查询的微博ID。</param>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="filter_by_author">作者筛选类型，默认为All。</param>
        /// <returns></returns>
        public string Statuses_Repost_Timeline_Ids(long id, long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"id",id},
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"feature",(int)(filter_by_author??FilterByAuthor.All)}
            };
            return HttpGet("statuses/repost_timeline/ids.json", dictionary);
        }
        #endregion

        #region 获取当前用户最新转发的微博列表
        /// <summary>
        /// 获取当前用户最新转发的微博列表
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Repost_By_Me(long? since_id, long? max_id, int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50}
            };
            return HttpGet<JsonStatus[]>("statuses/repost_by_me.json", dictionary); 
        }
        #endregion

        #region 获取@当前用户的最新微博
        /// <summary>
        /// 获取最新的提到登录用户的微博列表，即@我的微博
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <param name="filter_by_source">来源筛选类型，0：全部、1：来自微博、2：来自微群，默认为0。</param>
        /// <param name="filter_by_type">原创筛选类型，0：全部微博、1：原创的微博，默认为0。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Mentions(long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author, FilterBySource? filter_by_source, FilterByType? filter_by_type)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"filter_by_author",(int)(filter_by_author??FilterByAuthor.All)},
                {"filter_by_source",(int)(filter_by_source??FilterBySource.All)},
                {"filter_by_type",(int)(filter_by_type??FilterByType.All)}
            };
            return HttpGet<JsonStatus[]>("statuses/mentions.json", dictionary);
        }
        #endregion

        #region 获取@当前用户的最新微博的ID
        /// <summary>
        /// 获取@当前用户的最新微博的ID
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <param name="filter_by_source">来源筛选类型，0：全部、1：来自微博、2：来自微群，默认为0。</param>
        /// <param name="filter_by_type">原创筛选类型，0：全部微博、1：原创的微博，默认为0。</param>
        /// <returns></returns>
        public string Statuses_Mentions_Ids(long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author, FilterBySource? filter_by_source, FilterByType? filter_by_type)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"filter_by_author",(int)(filter_by_author??FilterByAuthor.All)},
                {"filter_by_source",(int)(filter_by_source??FilterBySource.All)},
                {"filter_by_type",(int)(filter_by_type??FilterByType.All)}
            };
            return HttpGet("statuses/mentions/ids.json", dictionary);
        }
        #endregion

        #region 获取双向关注用户的最新微博
        /// <summary>
        /// 获取双向关注用户的最新微博
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="feature">过滤类型ID，0：全部、1：原创、2：图片、3：视频、4：音乐，默认为0。</param>
        /// <returns></returns>
        public JsonStatus[] Statuses_Bilateral_Timeline(long? since_id, long? max_id, int? page, int? count, Feature? feature)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"feature",(int)(feature??Feature.All)},
            };
            return HttpGet<JsonStatus[]>("statuses/bilateral_timeline.json", dictionary);
        }
        #endregion

        #region 根据ID获取单条微博信息
        /// <summary>
        /// 根据微博ID获取单条微博内容
        /// </summary>
        /// <param name="id">需要获取的微博ID。</param>
        /// <returns></returns>
        public JsonStatus Statuses_Show(long id)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"id",id}
            };
            return HttpGet<JsonStatus>("statuses/show.json", dictionary);
        }
        #endregion

        #region 通过id获取mid
        /// <summary>
        /// 通过微博（评论、私信）ID获取其MID
        /// </summary>
        /// <param name="id">需要查询的微博（评论、私信）ID，批量模式下，用半角逗号分隔，最多不超过20个。</param>
        /// <param name="type">获取类型，1：微博、2：评论、3：私信，默认为1。</param>
        /// <param name="is_batch">是否使用批量模式，0：否、1：是，默认为0。</param>
        /// <returns></returns>
        public string Statuses_Querymid(string id, int? type, int? is_batch)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"id",id},
                {"type",type??1},
                {"is_batch",is_batch??0}
            };
            return HttpGet("statuses/querymid.json", dictionary);
        }
        #endregion

        #region 通过mid获取id
        /// <summary>
        /// 通过微博（评论、私信）MID获取其ID
        /// </summary>
        /// <param name="mid">需要查询的微博（评论、私信）MID，批量模式下，用半角逗号分隔，最多不超过20个。</param>
        /// <param name="type">获取类型，1：微博、2：评论、3：私信，默认为1。</param>
        /// <param name="is_batch">是否使用批量模式，0：否、1：是，默认为0。</param>
        /// <param name="inbox">仅对私信有效，当MID类型为私信时用此参数，0：发件箱、1：收件箱，默认为0 。</param>
        /// <param name="isBase62">MID是否是base62编码，0：否、1：是，默认为0。</param>
        /// <returns></returns>
        public string Statuses_Queryid(string mid, int? type, int? is_batch, int? inbox, int? isBase62)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"mid",mid},
                {"type",type??1},
                {"is_batch",is_batch??0},
                {"inbox",inbox??0},
                {"isBase62",isBase62??0}
            };
            return HttpGet("statuses/queryid.json", dictionary);
        }
        #endregion

        #region 按天返回热门转发榜
        /// <summary>
        /// 按天返回热门微博转发榜的微博列表
        /// </summary>
        /// <param name="count">返回的记录条数，最大不超过50，默认为20。</param>
        /// <returns></returns>
        public IList<Status> Statuses_Hot_Repost_Daily(int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"count",count??20}
            };
            return HttpGet<IList<Status>>("statuses/hot/repost_daily.json", dictionary);
        }
        #endregion

        #region 按周返回热门转发榜
        /// <summary>
        /// 按周返回热门微博转发榜的微博列表
        /// </summary>
        /// <param name="count">返回的记录条数，最大不超过50，默认为20。</param>
        /// <returns></returns>
        public IList<Status> Statuses_Hot_Repost_Weekly(int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"count",count??20}
            };
            return HttpGet<IList<Status>>("statuses/hot/repost_weekly.json", dictionary);
        }
        #endregion

        #region 按天返回热门评论榜
        /// <summary>
        /// 按天返回热门微博评论榜的微博列表
        /// </summary>
        /// <param name="count">返回的记录条数，最大不超过50，默认为20。</param>
        /// <returns></returns>
        public IList<Status> Statuses_Hot_Comments_Daily(int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"count",count??20}
            };
            return HttpGet<IList<Status>>("statuses/hot/comments_daily.json", dictionary);
        }
        #endregion

        #region 按周返回热门评论榜
        /// <summary>
        /// 按周返回热门微博评论榜的微博列表
        /// </summary>
        /// <param name="count">返回的记录条数，最大不超过50，默认为20。</param>
        /// <returns></returns>
        public IList<Status> Statuses_Hot_Comments_Weekly(int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"count",count??20}
            };
            return HttpGet<IList<Status>>("statuses/hot/comments_weekly.json", dictionary);
        }
        #endregion

        #region 批量获取指定微博的转发数评论数
        /// <summary>
        /// 批量获取指定微博的转发数评论数
        /// </summary>
        /// <param name="ids">需要获取数据的微博ID，多个之间用逗号分隔，最多不超过100个。</param>
        /// <returns></returns>
        public string Statuses_Count(string ids)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"ids",ids}
            };
            return HttpGet("statuses/count.json", dictionary);
        }
        #endregion
        #endregion

        #region 写入接口
        #region 转发一条微博信息
        /// <summary>
        /// 转发一条微博
        /// </summary>
        /// <param name="id">要转发的微博ID。</param>
        /// <param name="status">添加的转发文本，必须做URLencode，内容不超过140个汉字，不填则默认为“转发微博”。</param>
        /// <param name="is_comment">是否在转发的同时发表评论，0：否、1：评论给当前微博、2：评论给原微博、3：都评论，默认为0 。</param>
        /// <returns></returns>
        public Status Statuses_Repost(long id, string status, int? is_comment)
        {
            status = status.Length > 140 ? status.Substring(0, 140) : status;
            var dictionary = new Dictionary<object, object>
            {              
                {"id",id},
                {"status",Uri.EscapeDataString(status)},
                {"is_comment",is_comment??0}
            };
            return HttpPost<Status>("statuses/repost.json", dictionary);
        }
        #endregion

        #region 删除微博信息
        /// <summary>
        /// 根据微博ID删除指定微博
        /// </summary>
        /// <param name="id">需要删除的微博ID。</param>
        /// <returns></returns>
        public Status Statuses_Destroy(long id)
        {
            var dictionary = new Dictionary<object, object>
            {              
                {"id",id}
            };
            return HttpPost<Status>("statuses/destroy.json", dictionary);
        }
        #endregion

        #region 发布一条微博信息
        /// <summary>
        /// 发布一条微博信息
        /// </summary>
        /// <param name="status">要发布的微博文本内容，必须做URLencode，内容不超过140个汉字。</param>
        /// <returns></returns>
        public string Statuses_Update(string status)
        {
            status = status.Length > 140 ? status.Substring(0, 140) : status;
            var dictionary = new Dictionary<object, object>
            {                
                {"status",Uri.EscapeDataString(status)}
            };
            return HttpPost("statuses/update.json", dictionary);
        }
        #endregion

        #region 上传图片并发布一条微博
        /// <summary>
        /// 上传图片并发布一条新微博
        /// </summary>
        /// <param name="status">要发布的微博文本内容，必须做URLencode，内容不超过140个汉字。</param>
        /// <param name="pic">要上传的图片，仅支持JPEG、GIF、PNG格式，图片大小小于5M。</param>
        /// <returns></returns>
        public Status Statuses_Upload(string status, byte[] pic)
        {
            status = status.Length > 140 ? status.Substring(0, 140) : status;
            var dictionary = new Dictionary<object, object>
            {                
                {"status",Uri.EscapeDataString(status)},
            };
            return HttpPost<Status>("statuses/upload.json", dictionary, pic);

        }
        #endregion

        #region 发布一条微博同时指定上传的图片或图片url(高级接口)
        /// <summary>
        /// 指定一个图片URL地址抓取后上传并同时发布一条新微博
        /// </summary>
        /// <param name="status">要发布的微博文本内容，必须做URLencode，内容不超过140个汉字。</param>
        /// <param name="url">图片的URL地址，必须以http开头。</param>
        /// <returns></returns>
        public Status Statuses_Upload_Url_Text(string status, string url)
        {
            status = status.Length > 140 ? status.Substring(0, 140) : status;
            var dictionary = new Dictionary<object, object>
            {                
                {"status",Uri.EscapeDataString(status)},
                {"url",url}
            };
            return HttpPost<Status>("statuses/upload_url_text.json", dictionary);
        }
        #endregion

        //#region 获取官方表情
        ///// <summary>
        ///// 获取微博官方表情的详细信息
        ///// </summary>
        ///// <param name="type">表情类别，face：普通表情、ani：魔法表情、cartoon：动漫表情，默认为face。</param>
        ///// <param name="language">语言类别，cnname：简体、twname：繁体，默认为cnname。</param>
        ///// <returns></returns>
        //public IList<Emotions> Emotions(EmotionsType? type, EmotionsLanguage? language)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {                
        //        {"type",type??EmotionsType.face},
        //        {"language",language??EmotionsLanguage.cnname}
        //    };
        //    return HttpGet<IList<Emotions>>("emotions.json", dictionary);
        //}
        //#endregion
        #endregion
        #endregion

        #region 评论
        #region 读取接口
        #region 获取某条微博的评论列表
        /// <summary>
        /// 根据微博ID返回某条微博的评论列表
        /// </summary>
        /// <param name="id">需要查询的微博ID。</param>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为200。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <returns></returns>
        public JsonComment[] Comments_Show(long id, long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"id",id},
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??200},
                {"filter_by_author",(int)(filter_by_author??FilterByAuthor.All)}
            };
            return HttpGet<JsonComment[]>("comments/show.json", dictionary);
        }
        #endregion

        #region 我发出的评论列表
        /// <summary>
        /// 获取当前登录用户所发出的评论列表
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为200。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <returns></returns>
        public JsonComment[] Comments_Show(long? since_id, long? max_id, int? page, int? count, FilterBySource? filter_by_source)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??200},
                {"filter_by_source",(int)(filter_by_source??FilterBySource.All)}
            };
            return HttpGet<JsonComment[]>("comments/by_me.json", dictionary);
        }
        #endregion

        #region 我收到的评论列表
        /// <summary>
        /// 获取当前登录用户所接收到的评论列表
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <param name="filter_by_source">来源筛选类型，0：全部、1：来自微博的评论、2：来自微群的评论，默认为0。</param>
        /// <returns></returns>
        public JsonComment[] Comments_To_Me(long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author, FilterBySource? filter_by_source)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"filter_by_author",(int)(filter_by_author??FilterByAuthor.All)},
                {"filter_by_source",(int)(filter_by_source??FilterBySource.All)}
            };
            return HttpGet<JsonComment[]>("comments/to_me.json", dictionary);
        }
        #endregion

        #region 获取用户发送及收到的评论列表
        /// <summary>
        /// 获取当前登录用户的最新评论包括接收到的与发出的
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <returns></returns>
        public JsonComment[] Comments_Timeline(long? since_id, long? max_id, int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50}
            };
            return HttpGet<JsonComment[]>("comments/timeline.json", dictionary);
        }
        #endregion

        #region 获取@到我的评论
        /// <summary>
        /// 获取最新的提到当前登录用户的评论，即@我的评论
        /// </summary>
        /// <param name="since_id">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。</param>
        /// <param name="max_id">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="filter_by_author">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <param name="filter_by_source">来源筛选类型，0：全部、1：来自微博的评论、2：来自微群的评论，默认为0。</param>
        /// <returns></returns>
        public JsonComment[] Comments_Mentions(long? since_id, long? max_id, int? page, int? count, FilterByAuthor? filter_by_author, FilterBySource? filter_by_source)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"since_id",since_id??0},
                {"max_id",max_id??0},
                {"page",page??1},
                {"count",count??50},
                {"filter_by_author",(int)(filter_by_author??FilterByAuthor.All)},
                {"filter_by_source",(int)(filter_by_source??FilterBySource.All)}
            };
            return HttpGet<JsonComment[]>("comments/mentions.json", dictionary);
        }
        #endregion

        #region 批量获取评论内容
        /// <summary>
        /// 根据评论ID批量返回评论信息
        /// </summary>
        /// <param name="cids">需要查询的批量评论ID，用半角逗号分隔，最大50。</param>
        /// <returns></returns>
        public IList<Comment> Comments_Show_Batch(string cids)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"cids",cids}
            };
            return HttpGet<IList<Comment>>("comments/show_batch.json", dictionary);
        }
        #endregion
        #endregion

        #region 写入接口
        #region 评论一条微博
        /// <summary>
        /// 对一条微博进行评论
        /// </summary>
        /// <param name="id">需要评论的微博ID。</param>
        /// <param name="comment">评论内容，必须做URLencode，内容不超过140个汉字。</param>
        /// <param name="comment_ori">当评论转发微博时，是否评论给原微博，0：否、1：是，默认为0。</param>
        /// <returns></returns>
        public Comment Comments_Create(long id, string comment, int? comment_ori)
        {
            comment = comment.Length > 140 ? comment.Substring(0, 140) : comment;
            var dictionary = new Dictionary<object, object>
            {                
                {"id",id},
                {"comment",Uri.EscapeDataString(comment)},
                {"comment_ori",comment_ori??0}
            };
            return HttpPost<Comment>("comments/create.json", dictionary);
        }
        #endregion

        #region 删除一条评论
        /// <summary>
        /// 删除一条评论
        /// </summary>
        /// <param name="cid">要删除的评论ID，只能删除登录用户自己发布的评论。</param>
        /// <returns></returns>
        public Comment Comments_Destroy(long cid)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"cid",cid}
            };
            return HttpPost<Comment>("comments/destroy.json", dictionary);
        }
        #endregion

        #region 批量删除评论
        /// <summary>
        /// 根据评论ID批量删除评论
        /// </summary>
        /// <param name="ids">需要删除的评论ID，用半角逗号隔开，最多20个。</param>
        /// <returns></returns>
        public IList<Comment> Comments_Destroy_Batch(string ids)
        {            
            var dictionary = new Dictionary<object, object>
            {                
                {"ids",ids}
            };
            return HttpPost<IList<Comment>>("comments/destroy_batch.json", dictionary);
        }
        #endregion

        #region 回复一条微博
        /// <summary>
        /// 回复一条评论
        /// </summary>
        /// <param name="id">需要评论的微博ID。</param>
        /// <param name="cid">需要回复的评论ID。</param>
        /// <param name="comment">回复评论内容，必须做URLencode，内容不超过140个汉字。</param>
        /// <param name="without_mention">回复中是否自动加入“回复@用户名”，0：是、1：否，默认为0。</param>
        /// <param name="comment_ori">当评论转发微博时，是否评论给原微博，0：否、1：是，默认为0。</param>
        /// <returns></returns>
        public Comment Comments_Reply(long id, long cid, string comment, int? without_mention, int? comment_ori)
        {
            comment = comment.Length > 140 ? comment.Substring(0, 140) : comment;
            var dictionary = new Dictionary<object, object>
            {                
                {"id",id},
                {"cid",cid},
                {"comment",Uri.EscapeDataString(comment)},
                {"without_mention",without_mention??0},
                {"comment_ori",comment_ori??0}
            };
            return HttpPost<Comment>("comments/reply.json", dictionary);
        }
        #endregion
        #endregion
        #endregion

        #region 用户
        #region 读取接口
        #region 根据用户ID获取用户信息
        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="uid">需要查询的用户ID。</param>
        /// <returns></returns>
        public User Users_Show(long uid)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid}
            };
            return HttpGet<User>("users/show.json", dictionary);
        }
        #endregion

        #region 根据用户昵称获取用户信息
        /// <summary>
        /// 根据用户昵称获取用户信息
        /// </summary>
        /// <param name="domain">需要查询的用户昵称。</param>
        /// <returns></returns>
        public User Users_Show(string screen_name)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"screen_name",screen_name}
            };
            return HttpGet<User>("users/show.json", dictionary);
        }
        #endregion

        #region 同时根据用户ID和昵称获取用户信息
        /// <summary>
        /// 同时根据用户ID和昵称获取用户信息
        /// </summary>
        /// <param name="uid">需要查询的用户ID。</param>
        /// <param name="screen_name">需要查询的用户昵称。</param>
        /// <returns></returns>
        public User Users_Show(long uid, string screen_name)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"screen_name",screen_name}
            };
            return HttpGet<User>("users/show.json", dictionary);
        }
        #endregion

        #region 通过个性域名获取用户信息
        /// <summary>
        /// 通过个性化域名获取用户资料以及用户最新的一条微博
        /// </summary>
        /// <param name="domain">需要查询的个性化域名。</param>
        /// <returns></returns>
        public User Users_Domain_Show(string domain)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"domain",domain}
            };
            return HttpGet<User>("users/domain_show.json", dictionary);
        }
        #endregion

        #region 批量获取用户的粉丝数、关注数、微博数
        /// <summary>
        /// 批量获取用户的粉丝数、关注数、微博数
        /// </summary>
        /// <param name="uids">需要获取数据的用户UID，多个之间用逗号分隔，最多不超过100个。</param>
        /// <returns></returns>
        public string Users_Counts(string uids)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uids",uids}
            };
            return HttpGet("users/counts.json", dictionary);
        }
        #endregion
        #endregion
        #endregion

        #region 关系
        #region 关注读取接口
        #region 获取用户的关注列表
        /// <summary>
        /// 获取用户的关注列表
        /// </summary>
        /// <param name="uid">需要查询的用户UID。</param>
        /// <param name="count">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <param name="cursor">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。</param>
        /// <returns></returns>
        public JsonIDs Friendships_Friends(long uid, int? count, int? cursor)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??5000},
                {"cursor",cursor??-1}
            };
            return HttpGet<JsonIDs>("friendships/friends.json", dictionary);
        }
        #endregion

        #region 获取共同关注人列表
        /// <summary>
        /// 获取两个用户之间的共同关注人列表
        /// </summary>
        /// <param name="uid">需要获取共同关注关系的用户UID。</param>
        /// <param name="suid">需要获取共同关注关系的用户UID，默认为当前登录用户。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <returns></returns>
        public User[] Friendships_Friends_In_Common(long uid, long? suid, int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"suid",suid},                
                {"cursor",page??1},
                {"count",count??50}
            };
            return HttpGet<User[]>("friendships/friends/in_common.json", dictionary);
        }
        #endregion

        #region 获取双向关注列表
        /// <summary>
        /// 获取用户的双向关注列表，即互粉列表
        /// </summary>
        /// <param name="uid">需要获取共同关注关系的用户UID。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="sort">排序类型，0：按关注时间最近排序，默认为0。</param>
        /// <returns></returns>
        public User[] Friendships_Friends_Bilateral(long uid, int? page, int? count, int? sort)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},             
                {"cursor",page??1},
                {"count",count??50},
                {"sort",sort??0}
            };
            return HttpGet<User[]>("friendships/friends/bilateral.json", dictionary);
        }
        #endregion

        #region 获取双向关注UID列表
        /// <summary>
        /// 获取用户双向关注的用户ID列表，即互粉UID列表
        /// </summary>
        /// <param name="uid">需要获取共同关注关系的用户UID。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <param name="sort">排序类型，0：按关注时间最近排序，默认为0。</param>
        /// <returns></returns>
        public string Friendships_Friends_Bilateral_Ids(long uid, int? page, int? count, int? sort)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},             
                {"cursor",page??1},
                {"count",count??50},
                {"sort",sort??0}
            };
            return HttpGet("friendships/friends/bilateral/ids.json", dictionary);
        }
        #endregion

        #region 获取用户关注对象UID列表
        /// <summary>
        /// 获取用户关注的用户UID列表
        /// </summary>
        /// <param name="uid">需要查询的用户UID。</param>
        /// <param name="count">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <param name="cursor">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。</param>
        /// <returns></returns>
        public JsonIDs Friendships_Friends_Ids(long uid, int? count, int? cursor)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??5000},
                {"cursor",cursor??-1}
            };
            return HttpGet<JsonIDs>("friendships/friends/ids.json", dictionary);
        }
        #endregion
        #endregion

        #region 粉丝读取接口
        #region 获取用户粉丝列表
        /// <summary>
        /// 获取用户的粉丝列表
        /// </summary>
        /// <param name="uid">需要查询的用户UID。</param>
        /// <param name="count">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <param name="cursor">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。</param>
        /// <returns></returns>
        public User[] Friendships_Followers(long uid, int? count, int? cursor)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??50},
                {"cursor",cursor??0}
            };
            return HttpGet<User[]>("friendships/followers.json", dictionary);
        }
        #endregion

        #region 获取用户粉丝UID列表
        /// <summary>
        /// 获取用户粉丝UID列表
        /// </summary>
        /// <param name="uid">需要查询的用户UID。</param>
        /// <param name="count">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <param name="cursor">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。</param>
        /// <returns></returns>
        public JsonIDs Friendships_Followers_Ids(long uid, int? count, int? cursor)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??5000},
                {"cursor",cursor??-1}
            };
            return HttpGet<JsonIDs>("friendships/followers/ids.json", dictionary);
        }
        #endregion

        #region 获取用户优质粉丝列表
        /// <summary>
        /// 获取用户的活跃粉丝列表
        /// </summary>
        /// <param name="uid">需要查询的用户UID。</param>
        /// <param name="count">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <returns></returns>
        public IList<User> Friendships_Followers_Active(long uid, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??50}
            };
            return HttpGet<IList<User>>("friendships/followers/active.json", dictionary);
        }
        #endregion
        #endregion

        #region 关系链读取接口
        #region 获取我的关注人中关注了指定用户的人
        /// <summary>
        /// 获取当前登录用户的关注人中又关注了指定用户的用户列表
        /// </summary>
        /// <param name="uid">指定的关注目标用户UID。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为50。</param>
        /// <returns></returns>
        public User[] Friendships_Friends_Chain_Followers(long uid, int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"count",count??50},
                {"page",page??1}
            };
            return HttpGet<User[]>("friendships/friends_chain/followers.json", dictionary);
        }
        #endregion
        #endregion

        #region 关系状态读取接口
        #region 获取两个用户关系的详细情况
        /// <summary>
        /// 获取两个用户之间的详细关注关系情况
        /// </summary>
        /// <param name="source_id">源用户的UID。</param>
        /// <param name="target_id">目标用户的UID。</param>
        /// <returns></returns>
        public RelationShip Friendships_Show(long source_id, long target_id)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"source_id",source_id},
                {"target_id",target_id}
            };
            return HttpGet<RelationShip>("friendships/show.json", dictionary);
        }
        #endregion
        #endregion

        #region 写入接口
        #region 关注某用户
        /// <summary>
        /// 关注一个用户
        /// </summary>
        /// <param name="uid">需要关注的用户ID。</param>
        /// <returns></returns>
        public User Friendships_Create(long uid)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid}
            };
            return HttpPost<User>("friendships/create.json", dictionary);
        }
        #endregion

        #region 取消关注某用户
        /// <summary>
        /// 取消关注一个用户
        /// </summary>
        /// <param name="uid">需要关注的用户ID。</param>
        /// <returns></returns>
        public User Friendships_Destroy(long uid)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid}
            };
            return HttpPost<User>("friendships/destroy.json", dictionary);
        }
        #endregion

        #region 更新关注人备注(高级接口)
        /// <summary>
        /// 更新当前登录用户所关注的某个好友的备注信息
        /// </summary>
        /// <param name="uid">需要修改备注信息的用户UID。</param>
        /// <param name="remark">备注信息，需要URLencode。</param>
        /// <returns></returns>
        public User Friendships_Remark_Update(long uid, string remark)
        {
            var dictionary = new Dictionary<object, object>
            {                
                {"uid",uid},
                {"remark",Uri.EscapeDataString(remark)}
            };
            return HttpPost<User>("friendships/remark/update.json", dictionary);
        }
        #endregion
        #endregion
        #endregion

        #region 账户
        #region 读取接口
        #region 获取隐私设置信息
        /// <summary>
        /// 获取当前登录用户的隐私设置
        /// </summary>
        /// <returns></returns>
        public string Account_Get_Privacy()
        {
            var dictionary = new Dictionary<object, object>();
            return HttpGet("account/get_privacy.json", dictionary);
        }
        #endregion

        #region 获取所有学校列表
        /// <summary>
        /// 获取所有的学校列表
        /// </summary>
        /// <param name="province">省份范围，省份ID。</param>
        /// <param name="city">城市范围，城市ID。</param>
        /// <param name="area">区域范围，区ID。</param>
        /// <param name="type">学校类型，1：大学、2：高中、3：中专技校、4：初中、5：小学，默认为1。</param>
        /// <param name="keyword">学校名称关键字。</param>
        /// <param name="count">返回的记录条数，默认为10。</param>
        /// <returns></returns>
        public string Account_Profile_School_List(int? province, int? city, int? area, int? type, string keyword, int? count)
        {
            var dictionary = new Dictionary<object, object> 
            {
                {"province",province},
                {"city",city},
                {"area",area},
                {"type",type??1},
                {"keyword",keyword},
                {"count",count??10}
            };
            return HttpGet("account/profile/school_list.json", dictionary);
        }
        #endregion

        #region 获取当前用户API访问频率限制
        /// <summary>
        /// 获取当前登录用户的API访问频率限制情况
        /// </summary>
        /// <returns></returns>
        public JsonRateLimit Account_Rate_Limit_Status()
        {
            var dictionary = new Dictionary<object, object>();
            return HttpGet<JsonRateLimit>("account/rate_limit_status.json", dictionary); 
        }
        #endregion

        #region OAuth授权之后获取用户UID
        /// <summary>
        /// OAuth授权之后，获取授权用户的UID
        /// </summary>
        /// <returns></returns>
        public long Account_Get_Uid()
        {
            var dictionary = new Dictionary<object, object>();
            JsonAccountUID uid= HttpGet<JsonAccountUID>("account/get_uid.json", dictionary);
            if (uid != null) return uid.uid;
            else return -1;
        }
        #endregion
        #endregion

        #region 写入接口
        #region 退出登录
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public User Account_End_Session()
        {
            var dictionary = new Dictionary<object, object>();
            return HttpGet<User>("account/end_session.json", dictionary);
        }
        #endregion
        #endregion
        #endregion

        //#region 收藏
        //#region 读取接口
        //#region 获取当前登录用户的收藏列表
        ///// <summary>
        ///// 获取当前登录用户的收藏列表
        ///// </summary>
        ///// <param name="page">返回结果的页码，默认为1。</param>
        ///// <param name="count">单页返回的记录条数，默认为50。</param>
        ///// <returns></returns>
        //public Favorites Favorites(int? page,int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"page",page??1},
        //        {"count",count??50}
        //    };
        //    return HttpGet<Favorites>("favorites.json", dictionary);
        //}
        //#endregion

        //#region 获取当前用户的收藏列表的ID
        ///// <summary>
        ///// 获取当前用户的收藏列表的ID
        ///// </summary>
        ///// <param name="page">返回结果的页码，默认为1。</param>
        ///// <param name="count">单页返回的记录条数，默认为50。</param>
        ///// <returns></returns>
        //public string Favorites_Ids(int? page, int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"page",page??1},
        //        {"count",count??50}
        //    };
        //    return HttpGet("favorites/ids.json", dictionary);
        //}
        //#endregion

        //#region 获取单条收藏信息
        ///// <summary>
        ///// 根据收藏ID获取指定的收藏信息
        ///// </summary>
        ///// <param name="id">需要查询的收藏ID。</param>
        ///// <returns></returns>
        //public Favorite Favorites_Show(long id)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",id}
        //    };
        //    return HttpGet<Favorite>("favorites/show.json", dictionary);
        //}
        //#endregion

        //#region 获取当前用户某个标签下的收藏列表
        ///// <summary>
        ///// 根据标签获取当前登录用户该标签下的收藏列表
        ///// </summary>
        ///// <param name="tid">需要查询的标签ID。</param>
        ///// <param name="page">单页返回的记录条数，默认为50。</param>
        ///// <param name="count">返回结果的页码，默认为1。</param>
        ///// <returns></returns>
        //public Favorites Favorites_By_Tags(long tid, int? page, int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"tid",tid},
        //        {"page",page??1},
        //        {"count",count??50}
        //    };
        //    return HttpGet<Favorites>("favorites/by_tags.json", dictionary);
        //}
        //#endregion

        //#region 当前登录用户的收藏标签列表
        ///// <summary>
        ///// 获取当前登录用户的收藏标签列表
        ///// </summary>
        ///// <param name="page">返回结果的页码，默认为1。</param>
        ///// <param name="count">单页返回的记录条数，默认为50。</param>
        ///// <returns></returns>
        //public string Favorites_Tags(int? page, int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"page",page??1},
        //        {"count",count??50}
        //    };
        //    return HttpGet("favorites/tags.json", dictionary);
        //}
        //#endregion

        //#region 获取当前用户某个标签下的收藏列表的ID
        ///// <summary>
        ///// 根据标签获取当前登录用户该标签下的收藏列表
        ///// </summary>
        ///// <param name="tid">需要查询的标签ID。</param>
        ///// <param name="page">单页返回的记录条数，默认为50。</param>
        ///// <param name="count">返回结果的页码，默认为1。</param>
        ///// <returns></returns>
        //public string Favorites_By_Tags_Ids(long tid, int? page, int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"tid",tid},
        //        {"page",page??1},
        //        {"count",count??50}
        //    };
        //    return HttpGet("favorites/by_tags/ids.json", dictionary);
        //}
        //#endregion
        //#endregion

        //#region 写入接口
        //#region 添加收藏
        ///// <summary>
        ///// 添加一条微博到收藏里
        ///// </summary>
        ///// <param name="id">要收藏的微博ID。</param>
        ///// <returns></returns>
        //public Favorite Favorites_Create(long id)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",id}
        //    };
        //    return HttpPost<Favorite>("favorites/create.json", dictionary);
        //}
        //#endregion

        //#region 删除收藏
        ///// <summary>
        ///// 取消收藏一条微博
        ///// </summary>
        ///// <param name="id">要取消收藏的微博ID。</param>
        ///// <returns></returns>
        //public Favorite Favorites_Destroy(long id)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",id}
        //    };
        //    return HttpPost<Favorite>("favorites/destroy.json", dictionary);
        //}
        //#endregion

        //#region 批量删除收藏
        ///// <summary>
        ///// 根据收藏ID批量取消收藏
        ///// </summary>
        ///// <param name="ids">要取消收藏的收藏ID，用半角逗号分隔，最多不超过10个。</param>
        ///// <returns></returns>
        //public string Favorites_Destroy_Batch(string ids)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"ids",ids}
        //    };
        //    return HttpPost("favorites/destroy_batch.json", dictionary);
        //}
        //#endregion

        //#region 更新收藏标签
        ///// <summary>
        ///// 更新一条收藏的收藏标签(参数tags为空即为删除标签)
        ///// </summary>
        ///// <param name="id">需要更新的收藏ID。</param>
        ///// <param name="tags">需要更新的标签内容，必须做URLencode，用半角逗号分隔，最多不超过2条。</param>
        ///// <returns></returns>
        //public Favorite Favorites_Tags_Update(long id, string tags)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",id},
        //        {"tags",Uri.EscapeDataString(tags)}
        //    };
        //    return HttpPost<Favorite>("favorites/tags/update.json", dictionary);
        //}
        //#endregion

        //#region 更新当前用户所有收藏下的指定标签
        ///// <summary>
        ///// 更新当前登录用户所有收藏下的指定标签
        ///// </summary>
        ///// <param name="tid">需要更新的标签ID。</param>
        ///// <param name="tag">需要更新的标签内容，必须做URLencode。</param>
        ///// <returns></returns>
        //public string Favorites_Tags_Update_Batch(long tid, string tag)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",tid},
        //        {"tags",Uri.EscapeDataString(tag)}
        //    };
        //    return HttpPost("favorites/tags/update_batch.json", dictionary);
        //}
        //#endregion

        //#region 删除当前用户所有收藏下的指定标签
        ///// <summary>
        ///// 删除当前登录用户所有收藏下的指定标签
        ///// </summary>
        ///// <param name="tid">需要删除的标签ID。</param>
        ///// <returns></returns>
        //public string Favorites_Tags_Destroy_Batch(long tid)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"id",tid}
        //    };
        //    return HttpPost("favorites/tags/destroy_batch.json", dictionary);
        //}
        //#endregion
        //#endregion
        //#endregion

        //#region 话题
        //#region 读取接口
        //#region 获取某人话题
        ///// <summary>
        ///// 获取某人的话题列表
        ///// </summary>
        ///// <param name="uid">需要获取话题的用户的UID。</param>
        ///// <param name="page">返回结果的页码，默认为1。</param>
        ///// <param name="count">单页返回的记录条数，默认为10。</param>
        ///// <returns></returns>
        //public string Trends(long uid,int? page,int? count)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"uid",uid},
        //        {"page",page??1},
        //        {"count",count??10}
        //    };
        //    return HttpGet("trends.json", dictionary);
        //}
        //#endregion

        //#region 是否关注某话题
        ///// <summary>
        ///// 判断当前用户是否关注某话题
        ///// </summary>
        ///// <param name="trend_name">话题关键字，必须做URLencode。</param>
        ///// <returns></returns>
        //public string Trends_Is_Follow(string trend_name)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"trend_name",Uri.EscapeDataString(trend_name)},
        //    };
        //    return HttpGet("trends/is_follow.json", dictionary);
        //}
        //#endregion

        //#region 返回最近一小时内的热门话题
        ///// <summary>
        ///// 返回最近一小时内的热门话题
        ///// </summary>
        ///// <returns></returns>
        //public string Trends_Hourly()
        //{
        //    var dictionary = new Dictionary<object, object>();
        //    return HttpGet("trends/hourly.json", dictionary);
        //}
        //#endregion

        //#region 返回最近一天内的热门话题
        ///// <summary>
        ///// 返回最近一天内的热门话题
        ///// </summary>
        ///// <returns></returns>
        //public string Trends_Daily()
        //{
        //    var dictionary = new Dictionary<object, object>();
        //    return HttpGet("trends/daily.json", dictionary);
        //}
        //#endregion

        //#region 返回最近一周内的热门话题
        ///// <summary>
        ///// 返回最近一周内的热门话题
        ///// </summary>
        ///// <returns></returns>
        //public string Trends_Weekly()
        //{
        //    var dictionary = new Dictionary<object, object>();
        //    return HttpGet("trends/weekly.json", dictionary);
        //}
        //#endregion
        //#endregion

        //#region 写入接口
        //#region 关注某话题
        ///// <summary>
        ///// 关注某话题
        ///// </summary>
        ///// <param name="trend_name">要关注的话题关键词。</param>
        ///// <returns></returns>
        //public string Trends_Follow(string trend_name)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"trend_name",trend_name}
        //    };
        //    return HttpPost("trends/follow.json", dictionary);
        //}
        //#endregion

        //#region 取消关注的某一个话题
        ///// <summary>
        ///// 取消对某话题的关注
        ///// </summary>
        ///// <param name="trend_id">要取消关注的话题ID。</param>
        ///// <returns></returns>
        //public string Trends_Destroy(long trend_id)
        //{
        //    var dictionary = new Dictionary<object, object>
        //    {
        //        {"trend_id",trend_id}
        //    };
        //    return HttpPost("trends/destroy.json", dictionary);
        //}
        //#endregion
        //#endregion
        //#endregion

        #region 标签
        #region 读取接口
        #region 返回指定用户的标签列表
        /// <summary>
        /// 返回指定用户的标签列表
        /// </summary>
        /// <param name="uid">要获取的标签列表所属的用户ID。</param>
        /// <param name="page">返回结果的页码，默认为1。</param>
        /// <param name="count">单页返回的记录条数，默认为20。</param>
        /// <returns></returns>
        public JsonTag[] Tags(long uid, int? page, int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"uid",uid},
                {"page",page??1},
                {"count",count??20}
            };
            return HttpGet<JsonTag[]>("tags.json", dictionary);
        }
        #endregion

        #region 批量获取用户标签
        /// <summary>
        /// 批量获取用户的标签列表
        /// </summary>
        /// <param name="uids">要获取标签的用户ID。最大20，逗号分隔。</param>
        /// <returns></returns>
        public string Tags_Tags_Batch(string uids)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"uids",uids}
            };
            return HttpGet("tags/tags_batch.json", dictionary);
        }
        #endregion

        #region 返回系统推荐的标签列表
        /// <summary>
        /// 获取系统推荐的标签列表
        /// </summary>
        /// <param name="count">返回记录数，默认10，最大10。</param>
        /// <returns></returns>
        public string Tags_Suggestions(int? count)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"count",count??10}
            };
            return HttpGet("tags/suggestions.json", dictionary);
        }
        #endregion
        #endregion

        #region 写入接口
        #region 添加用户标签
        /// <summary>
        /// 为当前登录用户添加新的用户标签
        /// </summary>
        /// <param name="tags">要创建的一组标签，用半角逗号隔开，每个标签的长度不可超过7个汉字，14个半角字符。</param>
        /// <returns></returns>
        public string Tags_Create(string tags)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"tags",tags}
            };
            return HttpPost("tags/create.json", dictionary);
        }
        #endregion

        #region 删除用户标签
        /// <summary>
        /// 删除一个用户标签
        /// </summary>
        /// <param name="tag_id">要删除的标签ID。</param>
        /// <returns></returns>
        public string Tags_Destroy(long tag_id)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"tag_id",tag_id}
            };
            return HttpPost("tags/destroy.json", dictionary);
        }
        #endregion

        #region 批量删除用户标签
        /// <summary>
        /// 批量删除一组标签
        /// </summary>
        /// <param name="ids">要删除的一组标签ID，以半角逗号隔开，一次最多提交10个ID。</param>
        /// <returns></returns>
        public string Tags_Destroy_Batch(string ids)
        {
            var dictionary = new Dictionary<object, object>
            {
                {"ids",ids}
            };
            return HttpPost("tags/destroy_batch.json", dictionary);
        }
        #endregion
        #endregion
        #endregion

        #region Must
        #region Post
        public T HttpPost<T>(string partUrl, IDictionary<object, object> dictionary) where T : class
        {
            return HttpPost<T>(partUrl, dictionary, null);
        }

        public T HttpPost<T>(string partUrl, IDictionary<object, object> dictionary,byte[] file) where T : class
        {
            dictionary.Add("access_token", base.Token.access_token);

            var url = string.Format(base.baseUrl,partUrl);// ToFormat(partUrl);            
            var query = PubHelper.ToQueryString(dictionary);
            //logger.Error(url);
            //logger.Error(query);
            if (file != null)
            {
                json = base.HttpMethod.HttpPost(url, dictionary, file);
            }
            else
            {
                json = base.HttpMethod.HttpPost(url, query);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public string HttpPost(string partUrl, IDictionary<object, object> dictionary)
        {
            dictionary.Add("access_token", base.Token.access_token);

            var url = string.Format(base.baseUrl, partUrl);// ToFormat(partUrl);            
            var query = PubHelper.ToQueryString(dictionary);
            //logger.Error(url);
            //logger.Error(query);
            return base.HttpMethod.HttpPost(url, query);
        }
        #endregion

        #region Get
        public T HttpGet<T>(string partUrl, IDictionary<object, object> dictionary) where T : class
        {
            dictionary.Add("access_token", base.Token.access_token);

            var url = string.Format(base.baseUrl,partUrl);
            var query = PubHelper.ToQueryString(dictionary);
            //logger.Error(url + "?" + query);
            try
            {
                json = base.HttpMethod.HttpGet(url + "?" + query);
                if (typeof(T).Name == "JsonTag[]")
                {
                    json = PubHelper.ParseNewTags(json);
                }
            }
            catch (Exception ex)
            {
                if (partUrl == "users/show.json")
                {
                    json = ex.Message;
                    return null;
                }
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string HttpGet(string partUrl, IDictionary<object, object> dictionary)
        {
            dictionary.Add("access_token", base.Token.access_token);

            var url = string.Format(base.baseUrl, partUrl);
            var query = PubHelper.ToQueryString(dictionary);
            //logger.Error(url + "?" + query);
            return base.HttpMethod.HttpGet(url + "?" + query);
        }
        #endregion
        #endregion
    }
}
