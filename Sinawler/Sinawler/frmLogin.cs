using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sina.Api;
using System.Configuration;

namespace Sinawler
{
    public partial class frmLogin : Form
    {
        private SinaApiService _apiUserRelation = GlobalPool.GetAPI(SysArgFor.USER_RELATION).API;
        private SinaApiService _apiUserInfo = GlobalPool.GetAPI(SysArgFor.USER_INFO).API;
        private SinaApiService _apiUserTag = GlobalPool.GetAPI(SysArgFor.USER_TAG).API;
        private SinaApiService _apiStatus = GlobalPool.GetAPI(SysArgFor.STATUS).API;
        private SinaApiService _apiComment = GlobalPool.GetAPI(SysArgFor.COMMENT).API;

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
                HtmlElementCollection elements = wb.Document.GetElementsByTagName("a");
                foreach (HtmlElement element in elements)
                {
                    if (element.InnerHtml != null)
                    {
                        if (element.InnerHtml.ToLower() == "<em>授权</em>")
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
                        if (element.InnerHtml.ToLower() == "<em>登录并授权</em>")
                        {
                            element.InvokeMember("click");
                        }
                    }
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

            AutoLogin(strUserID, strPWD);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            btnLogin.Text = "Initializing...";
            //the two strings below must to be "" to generate right signature
            _apiUserRelation.Token = "";
            _apiUserRelation.TokenSecret = "";
            _apiUserInfo.Token = "";
            _apiUserInfo.TokenSecret = "";
            _apiUserTag.Token = "";
            _apiUserTag.TokenSecret = "";
            _apiStatus.Token = "";
            _apiStatus.TokenSecret = "";
            _apiComment.Token = "";
            _apiComment.TokenSecret = "";
            try
            {
                Sinawler.Properties.Settings settings = new Sinawler.Properties.Settings();

                _apiUserRelation.appKey = settings.appKeyForUserRelation;
                _apiUserRelation.appSecret = settings.appSecretForUserRelation;
                GlobalPool.MinSleepMsForUserRelation = settings.MinSleepMsForUserRelation;

                _apiUserInfo.appKey = settings.appKeyForUserInfo;
                _apiUserInfo.appSecret = settings.appSecretForUserInfo;
                GlobalPool.MinSleepMsForUserInfo = settings.MinSleepMsForUserInfo;

                _apiUserTag.appKey = settings.appKeyForUserTag;
                _apiUserTag.appSecret = settings.appSecretForUserTag;
                GlobalPool.MinSleepMsForUserTag = settings.MinSleepMsForUserTag;

                _apiStatus.appKey = settings.appKeyForStatus;
                _apiStatus.appSecret = settings.appSecretForStatus;
                GlobalPool.MinSleepMsForStatus = settings.MinSleepMsForStatus;

                _apiComment.appKey = settings.appKeyForComment;
                _apiComment.appSecret = settings.appSecretForComment;
                GlobalPool.MinSleepMsForComment = settings.MinSleepMsForComment;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Authentication failed! Please check appKey and appSecret values in .config file.");
                Application.Exit();
            }
            try
            {
                wbUserRelation.Url = new Uri(_apiUserRelation.AuthorizationGet());
                wbUserInfo.Url = new Uri(_apiUserInfo.AuthorizationGet());
                wbUserTag.Url = new Uri(_apiUserTag.AuthorizationGet());
                wbStatus.Url = new Uri(_apiStatus.AuthorizationGet());
                wbComment.Url = new Uri(_apiComment.AuthorizationGet());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void wbUserRelation_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbUserRelation.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserRelation.Url.ToString())
            {
                if (wbUserRelation.Url.ToString().Contains("http://api.t.sina.com.cn/oauth/authorize?oauth_token="))
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
                string strHTML = wbUserRelation.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("User Relation thread login failed. Please try again.", "Sinawler");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "Login";
                    return;
                }
                if (blnDirectAuthUserRelation && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                string strPin = _apiUserRelation.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _apiUserRelation.Verifier = strPin;
                    _apiUserRelation.AccessTokenGet();
                    nCountOfLoginOK++;
                    if (nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void wbUserInfo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbUserInfo.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserInfo.Url.ToString())
            {
                if (wbUserInfo.Url.ToString().Contains("http://api.t.sina.com.cn/oauth/authorize?oauth_token="))
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
                string strHTML = wbUserInfo.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("User Information thread login failed. Please try again.", "Sinawler");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "Login";
                    return;
                }
                if (blnDirectAuthUserInfo && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                string strPin = _apiUserInfo.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _apiUserInfo.Verifier = strPin;
                    _apiUserInfo.AccessTokenGet();

                    nCountOfLoginOK++;

                    if (nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void wbUserTag_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbUserTag.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbUserTag.Url.ToString())
            {
                if (wbUserTag.Url.ToString().Contains("http://api.t.sina.com.cn/oauth/authorize?oauth_token="))
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
                string strHTML = wbUserTag.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("User Tag thread login failed. Please try again.", "Sinawler");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "Login";
                    return;
                }
                if (blnDirectAuthUserTag && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                string strPin = _apiUserTag.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _apiUserTag.Verifier = strPin;
                    _apiUserTag.AccessTokenGet();

                    nCountOfLoginOK++;

                    if (nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void wbStatus_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbStatus.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbStatus.Url.ToString())
            {
                if (wbStatus.Url.ToString().Contains("http://api.t.sina.com.cn/oauth/authorize?oauth_token="))
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
                string strHTML = wbStatus.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("Status thread login failed. Please try again.", "Sinawler");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "Login";
                    return;
                }
                if (blnDirectAuthStatus && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                string strPin = _apiStatus.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _apiStatus.Verifier = strPin;
                    _apiStatus.AccessTokenGet();

                    nCountOfLoginOK++;

                    if (nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                }
            }
        }

        private void wbComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (wbComment.ReadyState == WebBrowserReadyState.Complete && e.Url.ToString() == wbComment.Url.ToString())
            {
                if (wbComment.Url.ToString().Contains("http://api.t.sina.com.cn/oauth/authorize?oauth_token="))
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
                string strHTML = wbComment.DocumentText;
                if (strHTML.Contains("登录名或密码错误"))
                {
                    MessageBox.Show("Comment thread login failed. Please try again.", "Sinawler");
                    txtUserID.Enabled = true;
                    txtPWD.Enabled = true;
                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                    btnLogin.Text = "Login";
                    return;
                }
                if (blnDirectAuthComment && nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                string strPin = _apiComment.ParseHtml(strHTML);
                if (strPin != "")
                {
                    _apiComment.Verifier = strPin;
                    _apiComment.AccessTokenGet();

                    nCountOfLoginOK++;

                    if (nCountOfLoginOK == 5) this.DialogResult = DialogResult.OK;
                }
            }
        }
    }
}
