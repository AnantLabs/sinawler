using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sina.Api;

namespace Sinawler
{
    public partial class frmBrowser : Form
    {
        public frmBrowser()
        {
            InitializeComponent();
        }

        private SinaApiService api = GlobalPool.API;
        ///默认请求之前等待3.6秒钟，此值根据每小时100次的限制算得（每次3.6秒），但鉴于日志操作也有等待时间，故此值应能保证请求次数不超限
        ///后经测试，此值还可缩小。2010-10-11定为最小值2000，可调整
        ///2010-10-12定为不设下限
        ///2011-02-23设定为下限500ms
        private int iSleep = 3000;
        private bool blnStopCrawling = false;   //是否停止爬行
        private int iTryTimes = 10;             //the times that try to get XML

        private string strWebContent = "";  //web页面的HTML内容
        private int iCurStatusPage = 1;     //微博页面的页号，初始为1
        private int iMaxStatusPage = 1;     //微博页面的最大页号，初始为1
        private bool blnPageGot = false;    //已获取页面内容的标记

        private long lUid = 0;

        public int SleepTime
        {
            set { iSleep = value; }
        }

        public bool StopCrawling
        {
            set { blnStopCrawling = value; }
        }

        public long UserID
        {
            set { lUid = value; }
        }

        private void frmBrowser_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(iSleep);
            LinkedList<long> ids = GlobalPool.StatusIDsListByWeb;

            wbForStatusRobot.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(StatusPageLoaded);
            wbForStatusRobot.Navigate("http://weibo.com/profile.php?uid=" + lUid.ToString() + "&page=1");
            while (!blnPageGot) { System.Threading.Thread.Sleep(50); }

            //此时已获取第一页内容，循环，直到最大页
            for (iCurStatusPage = 2; iCurStatusPage <= iMaxStatusPage; iCurStatusPage++)
            {
                blnPageGot = false;
                wbForStatusRobot.Navigate("http://weibo.com/profile.php?uid=" + lUid.ToString() + "&page=" + iCurStatusPage.ToString());
                while (!blnPageGot) { System.Threading.Thread.Sleep(50); }
            }

            //循环结束，已获取所有页面的粉丝。下面解析页面内容，提取微博内容
            int index1 = strWebContent.IndexOf("mid=\"") + 5;
            int index2 = strWebContent.IndexOf("\"", index1);
            while (index1 != -1)
            {
                string mid = strWebContent.Substring(index1, index2 - index1);  //get mid
                string strResult = api.id_by_mid(mid);  //get status_id by mid
                string strTmp = strResult.Split(':')[1];  //such as "123456"]
                long status_id = Convert.ToInt64(strTmp.Substring(1, strTmp.Length - 3));
                if (status_id != -1 && !ids.Contains(status_id)) ids.AddLast(status_id);

                index1 = strWebContent.IndexOf("mid=\"", index2 + 1) + 5;
                index2 = strWebContent.IndexOf("\"", index1);
            }

            this.Dispose();
        }

        private void StatusPageLoaded(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            if (wb.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wb.Url.ToString())
            {
                strWebContent += wb.Document.GetElementById("feed_list").InnerHtml;

                if (iCurStatusPage == 1)    //如果是第一页，则获取总页数
                {
                    HtmlElementCollection hrefs = wb.Document.Body.GetElementsByTagName("a");
                    foreach (HtmlElement href in hrefs)
                    {
                        string strHref = href.GetAttribute("href");
                        if (strHref.Contains("http://weibo.com/profile.php?uid="))
                        {
                            int i = Convert.ToInt32(strHref.Substring(strHref.IndexOf("page=") + 5));  //find the page number
                            if (i > iMaxStatusPage) iMaxStatusPage = i;
                        }
                    }
                }

                blnPageGot = true;
            }
        }
    }
}
