using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Open.Sina2SDK
{
    #region 用户信息
    public class Users
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public IList<User> users { get; set; }

        /// <summary>
        /// 前游标
        /// </summary>
        public int previous_cursor { get; set; }

        /// <summary>
        /// 后游标
        /// </summary>
        public long next_cursor { get; set; }

        /// <summary>
        /// 返回数
        /// </summary>
        public int total_number { get; set; }
    }

    public class User
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string screen_name { get; set; }

        /// <summary>
        /// 友好显示名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 用户所在地区ID
        /// </summary>
        public int province { get; set; }

        /// <summary>
        /// 用户所在城市ID
        /// </summary>
        public int city { get; set; }

        /// <summary>
        /// 用户所在地
        /// </summary>
        public string location { get; set; }

        /// <summary>
        /// 用户描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 用户博客地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 用户头像地址
        /// </summary>
        public string profile_image_url { get; set; }

        /// <summary>
        /// 用户的个性化域名
        /// </summary>
        public string domain { get; set; }

        /// <summary>
        /// 性别，m：男、f：女、n：未知
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 粉丝数
        /// </summary>
        public int followers_count { get; set; }

        /// <summary>
        /// 关注数
        /// </summary>
        public int friends_count { get; set; }

        /// <summary>
        /// 微博数
        /// </summary>
        public int statuses_count { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int favourites_count { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 当前登录用户是否已关注该用户
        /// </summary>
        public bool following { get; set; }

        /// <summary>
        /// 是否允许所有人给我发私信
        /// </summary>
        public bool allow_all_act_msg { get; set; }

        /// <summary>
        /// 是否允许带有地理信息
        /// </summary>
        public bool geo_enabled { get; set; }

        /// <summary>
        /// 是否是微博认证用户，即带V用户
        /// </summary>
        public bool verified { get; set; }

        /// <summary>
        /// 是否允许所有人对我的微博进行评论
        /// </summary>
        public bool allow_all_comment { get; set; }

        /// <summary>
        /// 用户大头像地址
        /// </summary>
        public string avatar_large { get; set; }

        /// <summary>
        /// 认证原因
        /// </summary>
        public string verified_reason { get; set; }

        /// <summary>
        /// 该用户是否关注当前登录用户
        /// </summary>
        public bool follow_me { get; set; }

        /// <summary>
        /// 用户的在线状态，0：不在线、1：在线
        /// </summary>
        public int online_status { get; set; }

        /// <summary>
        /// 用户的互粉数
        /// </summary>
        public int bi_followers_count { get; set; }

        /// <summary>
        /// 用户的最近一条微博信息字段
        /// </summary>
        public Status status { get; set; }
    }
    #endregion

    #region 微博信息
    public class Statuses
    {
        /// <summary>
        /// 微博列表
        /// </summary>
        public IList<Status> statuses { get; set; }

        /// <summary>
        /// 前游标
        /// </summary>
        public int previous_cursor { get; set; }

        /// <summary>
        /// 后游标
        /// </summary>
        public long next_cursor { get; set; }

        /// <summary>
        /// 返回数
        /// </summary>
        public int total_number { get; set; }
    }

    public class Status
    {
        /// <summary>
        /// 字符串型的微博ID
        /// </summary>
        public string idstr { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 微博ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 微博信息内容
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 微博来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 是否已收藏
        /// </summary>
        public bool favorited { get; set; }

        /// <summary>
        /// 是否被截断
        /// </summary>
        public bool truncated { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public string in_reply_to_status_id { get; set; }

        /// <summary>
        /// 回复人UID
        /// </summary>
        public string in_reply_to_user_id { get; set; }

        /// <summary>
        /// 回复人昵称
        /// </summary>
        public string in_reply_to_screen_name { get; set; }

        /// <summary>
        /// 微博MID
        /// </summary>
        public long mid { get; set; }

        /// <summary>
        /// 中等尺寸图片地址
        /// </summary>
        public string bmiddle_pic { get; set; }

        /// <summary>
        /// 原始图片地址
        /// </summary>
        public string original_pic { get; set; }

        /// <summary>
        /// 缩略图片地址
        /// </summary>
        public string thumbnail_pic { get; set; }

        /// <summary>
        /// 转发数
        /// </summary>
        public int reposts_count { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int comments_count { get; set; }

        /// <summary>
        /// 微博附加注释信息
        /// </summary>
        public Array annotations { get; set; }

        /// <summary>
        /// 微博作者的用户信息字段
        /// </summary>
        public User user { get; set; }
    }
    #endregion

    #region 转发信息
    public class Reposts
    {
        /// <summary>
        /// 转发列表
        /// </summary>
        public IList<Repost> reposts { get; set; }

        /// <summary>
        /// 前游标
        /// </summary>
        public int previous_cursor { get; set; }

        /// <summary>
        /// 后游标
        /// </summary>
        public long next_cursor { get; set; }

        /// <summary>
        /// 返回数
        /// </summary>
        public int total_number { get; set; }
    }

    public class Repost
    {
        /// <summary>
        /// 字符串型的微博ID
        /// </summary>
        public string idstr { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 微博ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 微博信息内容
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 微博来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 是否已收藏
        /// </summary>
        public bool favorited { get; set; }

        /// <summary>
        /// 是否被截断
        /// </summary>
        public bool truncated { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public string in_reply_to_status_id { get; set; }

        /// <summary>
        /// 回复人UID
        /// </summary>
        public string in_reply_to_user_id { get; set; }

        /// <summary>
        /// 回复人昵称
        /// </summary>
        public string in_reply_to_screen_name { get; set; }

        /// <summary>
        /// 微博MID
        /// </summary>
        public long mid { get; set; }

        /// <summary>
        /// 中等尺寸图片地址
        /// </summary>
        public string bmiddle_pic { get; set; }

        /// <summary>
        /// 原始图片地址
        /// </summary>
        public string original_pic { get; set; }

        /// <summary>
        /// 缩略图片地址
        /// </summary>
        public string thumbnail_pic { get; set; }

        /// <summary>
        /// 转发数
        /// </summary>
        public int reposts_count { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int comments_count { get; set; }

        /// <summary>
        /// 微博附加注释信息
        /// </summary>
        public Array annotations { get; set; }

        /// <summary>
        /// 微博作者的用户信息字段
        /// </summary>
        public User user { get; set; }

        /// <summary>
        /// 转发的微博信息字段
        /// </summary>
        public Status retweeted_status { get; set; }
    }
    #endregion

    #region 评论信息
    public class Comments
    {
        /// <summary>
        /// 评论列表
        /// </summary>
        public IList<Comment> comments { get; set; }

        /// <summary>
        /// 前游标
        /// </summary>
        public int previous_cursor { get; set; }

        /// <summary>
        /// 后游标
        /// </summary>
        public long next_cursor { get; set; }

        /// <summary>
        /// 返回数
        /// </summary>
        public int total_number { get; set; }
    }

    public class Comment
    {
        /// <summary>
        /// 评论创建时间
        /// </summary>
        public string created_at { get; set; }

        /// <summary>
        /// 评论的ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 评论的内容
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 评论的来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 评论的MID
        /// </summary>
        public long mid { get; set; }

        /// <summary>
        /// 评论作者的用户信息字段
        /// </summary>
        public User user { get; set; }

        /// <summary>
        /// 评论的微博信息字段
        /// </summary>
        public Status status { get; set; }

        /// <summary>
        /// 回复的评论信息字段
        /// </summary>
        public Comment reply_comment { get; set; }
    }
    #endregion

    #region 表情信息
    public class Emotions
    {
        /// <summary>
        /// 分类名
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 是否官方
        /// </summary>
        public bool common { get; set; }

        /// <summary>
        /// 是否热门
        /// </summary>
        public bool hot { get; set; }

        /// <summary>
        /// icon
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 含义
        /// </summary>
        public string phrase { get; set; }

        /// <summary>
        /// 表情id
        /// </summary>
        public string picid { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
    #endregion

    #region 关系信息
    public class RelationShip
    {
        public RelationInfo source { get; set; }
        public RelationInfo target { get; set; }
    }

    public class RelationInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Int64 id { get; set; }
        /// <summary>
        /// 微博昵称
        /// </summary>
        public String screen_name { get; set; }
        /// <summary>
        /// 关注
        /// </summary>
        public Boolean following { get; set; }
        /// <summary>
        /// 被关注
        /// </summary>
        public Boolean followed_by { get; set; }
        /// <summary>
        /// 启用通知
        /// </summary>
        public Boolean notifications_enabled { get; set; }
    }
    #endregion

    #region 收藏信息
    public class Favorites
    {
        /// <summary>
        /// 收藏列表
        /// </summary>
        public IList<Favorite> favorites { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int total_number { get; set; }
    }

    public class Favorite
    {
        /// <summary>
        /// 微博信息
        /// </summary>
        public Status status { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        public string favorited_time { get; set; }
    }
    #endregion
}
