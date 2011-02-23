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

        public frmLogin(SinaApiService api)
        {
            InitializeComponent();
            _api = api;
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
            if (!_api.oAuthDesktop( strUserID, strPWD ))
            {
                MessageBox.Show( "登录失败。请重试。", "新浪微博爬虫" );
                txtUserID.Enabled = true;
                txtPWD.Enabled = true;
                btnLogin.Enabled = true;
                btnCancel.Enabled = true;
                btnLogin.Text = "登录";
                return;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
