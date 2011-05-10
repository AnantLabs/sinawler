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
        private SinaApiService _api=GlobalPool.API;
        private bool blnDirectAuth = false;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void AutoLogin(string strUserID, string strPWD)
        {
            wbBrowser.Document.GetElementById("userId").SetAttribute("value",strUserID);
            wbBrowser.Document.GetElementById("passwd").SetAttribute("value", strPWD);
            HtmlElementCollection elements=wbBrowser.Document.GetElementsByTagName("a");
            foreach (HtmlElement element in elements)
            {
                if (element.InnerHtml != null)
                {
                    if (element.InnerHtml.ToLower() == "<em>授权</em>")
                    {
                        blnDirectAuth = true;
                        element.InvokeMember("click");
                    }
                    if (element.InnerHtml.ToLower() == "<em>登录并授权</em>")
                    {
                        element.InvokeMember("click");
                    }
                    
                }   
            }
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

            AutoLogin(strUserID, strPWD);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            btnLogin.Text = "初始化中...";
            //the two strings below must to be "" to generate right signature
            _api.Token = "";
            _api.TokenSecret = "";

            wbBrowser.Url=new Uri(_api.AuthorizationGet());
        }

        private void wbBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbBrowser.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbBrowser.Url.ToString())
            {
                if (wbBrowser.Url.ToString().Contains("http://api.weibo.com/oauth/authorize?oauth_token="))
                {
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;

                    btnLogin.Text = "登录";
                    txtUserID.Focus();
                    return;
                }
                string strHTML = wbBrowser.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("登录失败。请重试。", "新浪微博爬虫");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "登录";
                    return;
                }
                if (blnDirectAuth) this.DialogResult = DialogResult.OK;
                string strPin = _api.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _api.Verifier = strPin;
                    _api.AccessTokenGet();
                    this.DialogResult = DialogResult.OK;
                }
            }
        }
    }
}
