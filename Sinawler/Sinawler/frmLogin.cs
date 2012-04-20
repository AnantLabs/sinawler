using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Open.Sina2SDK;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Sinawler
{
    public partial class frmLogin : Form
    {
        private SinaSerive _apiUserRelation = GlobalPool.GetAPI(SysArgFor.USER_RELATION).API;
        private SinaSerive _apiUserInfo = GlobalPool.GetAPI(SysArgFor.USER_INFO).API;
        private SinaSerive _apiUserTag = GlobalPool.GetAPI(SysArgFor.USER_TAG).API;
        private SinaSerive _apiStatus = GlobalPool.GetAPI(SysArgFor.STATUS).API;
        private SinaSerive _apiComment = GlobalPool.GetAPI(SysArgFor.COMMENT).API;

        private WebBrowser wbUserInfo = new WebBrowser();
        private WebBrowser wbUserTag = new WebBrowser();
        private WebBrowser wbStatus = new WebBrowser();
        private WebBrowser wbComment = new WebBrowser();
        private WebBrowser[] wbBrowsers = new WebBrowser[5];

        private bool blnDirectAuthUserRelation = false;
        private bool blnDirectAuthUserInfo = false;
        private bool blnDirectAuthUserTag = false;
        private bool blnDirectAuthStatus = false;
        private bool blnDirectAuthComment = false;

        private int nCountOfReady = 0;
        private int nCountOfLoginOK = 0;

        private bool blnMessageBox = true;

        public frmLogin()
        {
            InitializeComponent();
            wbBrowsers[0] = wbUserRelation;
            wbBrowsers[1] = wbUserInfo;
            wbBrowsers[2] = wbUserTag;
            wbBrowsers[3] = wbStatus;
            wbBrowsers[4] = wbComment;

            wbBrowsers[0].DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbUserRelation_DocumentCompleted);
            wbBrowsers[1].DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbUserInfo_DocumentCompleted);
            wbBrowsers[2].DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbUserTag_DocumentCompleted);
            wbBrowsers[3].DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbStatus_DocumentCompleted);
            wbBrowsers[4].DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wbComment_DocumentCompleted);
        }

        private void AutoLogin(string strUserID, string strPWD)
        {
            int i = 0;
            foreach (WebBrowser wb in wbBrowsers)
            {
                wb.Document.GetElementById("userId").SetAttribute("value", strUserID);
                wb.Document.GetElementById("passwd").SetAttribute("value", strPWD);
                HtmlElement e=wb.Document.GetElementById("ssologinFlag");
                if(e!=null) e.SetAttribute("checked", "true");
                HtmlElementCollection elements = wb.Document.GetElementsByTagName("a");
                foreach (HtmlElement element in elements)
                {
                    if (element.OuterHtml.Contains("WB_btn_oauth formbtn_01"))    //授权按钮
                    {
                        switch (i)
                        {
                            case 1:
                                blnDirectAuthUserInfo = true;
                                break;
                            case 2:
                                blnDirectAuthUserTag = true;
                                break;
                            case 3:
                                blnDirectAuthStatus = true;
                                break;
                            case 4:
                                blnDirectAuthComment = true;
                                break;
                            default:
                                blnDirectAuthUserRelation = true;
                                break;
                        }
                        element.InvokeMember("click");
                    }
                    //if (element.InnerHtml.ToLower() == "<em>登录并授权</em>")
                    //{
                    //    element.InvokeMember("click");
                    //}
                }
                i++;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            txtUserID.Enabled = false;
            txtPWD.Enabled = false;
            btnLogin.Text = "Login...";
            btnLogin.Enabled = false;
            btnCancel.Enabled = false;
            string strUserID = txtUserID.Text;
            string strPWD = txtPWD.Text;
            blnMessageBox = true;
            AutoLogin(strUserID, strPWD);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            btnLogin.Text = "Initializing...";
            //the two strings below must to be "" to generate right signature
            try
            {
                Sinawler.Properties.Settings settings = new Sinawler.Properties.Settings();

                _apiUserRelation.App_Key = settings.appKeyForUserRelation;
                _apiUserRelation.App_Secret = settings.appSecretForUserRelation;
                GlobalPool.MinSleepMsForUserRelation = settings.MinSleepMsForUserRelation;

                _apiUserInfo.App_Key = settings.appKeyForUserInfo;
                _apiUserInfo.App_Secret = settings.appSecretForUserInfo;
                GlobalPool.MinSleepMsForUserInfo = settings.MinSleepMsForUserInfo;

                _apiUserTag.App_Key = settings.appKeyForUserTag;
                _apiUserTag.App_Secret = settings.appSecretForUserTag;
                GlobalPool.MinSleepMsForUserTag = settings.MinSleepMsForUserTag;

                _apiStatus.App_Key = settings.appKeyForStatus;
                _apiStatus.App_Secret = settings.appSecretForStatus;
                GlobalPool.MinSleepMsForStatus = settings.MinSleepMsForStatus;

                _apiComment.App_Key = settings.appKeyForComment;
                _apiComment.App_Secret = settings.appSecretForComment;
                GlobalPool.MinSleepMsForComment = settings.MinSleepMsForComment;
            }
            catch
            {
                MessageBox.Show("Authentication failed! Please check appKey and appSecret values in .config file.");
                Application.Exit();
            }
            try
            {                
                wbUserInfo.Url = new Uri(_apiUserInfo.GetAuthorizationCodeURL());
                wbUserTag.Url = new Uri(_apiUserTag.GetAuthorizationCodeURL());
                wbStatus.Url = new Uri(_apiStatus.GetAuthorizationCodeURL());
                wbComment.Url = new Uri(_apiComment.GetAuthorizationCodeURL());
                wbUserRelation.Url = new Uri(_apiUserRelation.GetAuthorizationCodeURL());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        #region 监视ajax登录返回结果
        private void WebUserRelationOnChange(Object sender, EventArgs e)
        {
            HtmlElement div = wbUserRelation.Document.GetElementById("cs");
            if (div == null) return;
            if (div.InnerText.Contains("登录名或密码错误") && div.GetAttribute("display")=="")
            {
                if (blnMessageBox)
                {
                    MessageBox.Show("Login failed. Please try again.", "Sinawler");
                    blnMessageBox = false;
                }
                txtUserID.Enabled = true;
                txtPWD.Enabled = true;
                btnLogin.Enabled = true;
                btnCancel.Enabled = true;
                btnLogin.Text = "Login";
                return;
            }
        }
        #endregion

        private void wbUserRelation_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {            
            if (wbUserRelation.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserRelation.Url.ToString())
            {
                string url=wbUserRelation.Url.ToString();
                if (url.Contains("code="))
                {
                    string code = url.Substring(url.IndexOf("code=") + 5);
                    _apiUserRelation.GetAccessTokenByAuthorizationCode(code);
                }
                if (wbUserRelation.Url.ToString().Contains("https://api.weibo.com/oauth2/authorize?client_id="))
                {
                    nCountOfReady++;
                    if (nCountOfReady == 5)
                    {
                        txtUserID.Enabled = true;
                        txtPWD.Enabled = true;
                        btnLogin.Enabled = true;
                        btnCancel.Enabled = true;

                        btnLogin.Text = "Login";
                        txtUserID.Focus();
                    }
                    HtmlElement target = wbUserRelation.Document.GetElementById("cs");
                    if (target != null)
                    {
                        target.AttachEventHandler("onpropertychange", new EventHandler(WebUserRelationOnChange));
                    }
                    return;
                }
                nCountOfLoginOK++;
                if (blnDirectAuthUserRelation && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
            }
        }

        private void wbUserInfo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbUserInfo.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserInfo.Url.ToString())
            {
                string url = wbUserInfo.Url.ToString();
                if (url.Contains("code="))
                {
                    string code = url.Substring(url.IndexOf("code=") + 5);
                    _apiUserInfo.GetAccessTokenByAuthorizationCode(code);
                }
                if (wbUserInfo.Url.ToString().Contains("https://api.weibo.com/oauth2/authorize?client_id="))
                {
                    nCountOfReady++;
                    if (nCountOfReady == 5)
                    {
                        txtUserID.Enabled = true;
                        txtPWD.Enabled = true;
                        btnLogin.Enabled = true;
                        btnCancel.Enabled = true;

                        btnLogin.Text = "Login";
                        txtUserID.Focus();
                    }
                    return;
                }
                nCountOfLoginOK++;
                if (blnDirectAuthUserInfo && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
            }
        }

        private void wbUserTag_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbUserTag.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserTag.Url.ToString())
            {
                string url = wbUserTag.Url.ToString();
                if (url.Contains("code="))
                {
                    string code = url.Substring(url.IndexOf("code=") + 5);
                    _apiUserTag.GetAccessTokenByAuthorizationCode(code);
                }
                if (wbUserTag.Url.ToString().Contains("https://api.weibo.com/oauth2/authorize?client_id="))
                {
                    nCountOfReady++;
                    if (nCountOfReady == 5)
                    {
                        txtUserID.Enabled = true;
                        txtPWD.Enabled = true;
                        btnLogin.Enabled = true;
                        btnCancel.Enabled = true;

                        btnLogin.Text = "Login";
                        txtUserID.Focus();
                    }
                    return;
                }
                nCountOfLoginOK++;
                if (blnDirectAuthUserTag && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
            }
        }

        private void wbStatus_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbStatus.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbStatus.Url.ToString())
            {
                string url = wbStatus.Url.ToString();
                if (url.Contains("code="))
                {
                    string code = url.Substring(url.IndexOf("code=") + 5);
                    _apiStatus.GetAccessTokenByAuthorizationCode(code);
                }
                if (wbStatus.Url.ToString().Contains("https://api.weibo.com/oauth2/authorize?client_id="))
                {
                    nCountOfReady++;
                    if (nCountOfReady == 5)
                    {
                        txtUserID.Enabled = true;
                        txtPWD.Enabled = true;
                        btnLogin.Enabled = true;
                        btnCancel.Enabled = true;

                        btnLogin.Text = "Login";
                        txtUserID.Focus();
                    }
                    return;
                }
                nCountOfLoginOK++;
                if (blnDirectAuthStatus && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
            }
        }

        private void wbComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbComment.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbComment.Url.ToString())
            {
                string url = wbComment.Url.ToString();
                if (url.Contains("code="))
                {
                    string code = url.Substring(url.IndexOf("code=") + 5);
                    _apiComment.GetAccessTokenByAuthorizationCode(code);
                }
                if (wbComment.Url.ToString().Contains("https://api.weibo.com/oauth2/authorize?client_id="))
                {
                    nCountOfReady++;
                    if (nCountOfReady == 5)
                    {
                        txtUserID.Enabled = true;
                        txtPWD.Enabled = true;
                        btnLogin.Enabled = true;
                        btnCancel.Enabled = true;

                        btnLogin.Text = "Login";
                        txtUserID.Focus();
                    }
                    return;
                }
                nCountOfLoginOK++;
                if (blnDirectAuthComment && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
            }
        }
    }
}
