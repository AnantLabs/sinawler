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
    public partial class frmLogin : Form
    {
        private SinaApiService _api;
        WebBrowser wbBrowser = new WebBrowser();  //用于爬取web页面的浏览器对象

        public frmLogin(SinaApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        //登录页面已加载，登录
        private void LoginPageLoaded ( Object sender, WebBrowserDocumentCompletedEventArgs e )
        {
            if (wbBrowser.ReadyState != WebBrowserReadyState.Complete || e.Url.ToString() != wbBrowser.Url.ToString()) return;
            wbBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler( ProfilePageLoaded );
            wbBrowser.Document.All["loginname"].SetAttribute( "value", _api.UserName );
            wbBrowser.Document.All["password"].SetAttribute( "value", _api.PassWord );
            wbBrowser.Document.All["login_submit_btn"].InvokeMember( "Click" );
        }

        private void ProfilePageLoaded ( Object sender, WebBrowserDocumentCompletedEventArgs e )
        {
            if (wbBrowser.ReadyState != WebBrowserReadyState.Complete) return;
            _api.SetCookies(wbBrowser.Document.Cookie);
            wbBrowser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler( ProfilePageLoaded );
            wbBrowser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler( LoginPageLoaded );
            this.DialogResult = DialogResult.OK;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            txtUserID.Enabled = false;
            txtPWD.Enabled = false;
            btnLogin.Text = "登录中...";
            btnLogin.Enabled = false;
            btnCancel.Enabled = false;
            string strUserID = txtUserID.Text;
            string strPWD = txtPWD.Text;
            if (!_api.oAuthDesktop(strUserID, strPWD))
            {
                MessageBox.Show("登录失败。请重试。","新浪微博爬虫");
                btnLogin.Enabled = true;
                btnCancel.Enabled = true;
                return;
            }
            else
            {
                wbBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler( LoginPageLoaded );
                wbBrowser.Navigate( "http://t.sina.com.cn/login.php" );
            }
        }
    }
}
