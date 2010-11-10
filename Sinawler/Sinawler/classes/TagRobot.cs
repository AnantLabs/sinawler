using System;
using System.Collections.Generic;
using System.Text;
using Sina.Api;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Sinawler.Model;
using System.Data;

namespace Sinawler
{
    class TagRobot : RobotBase
    {
        private UserQueue queueUserForTagRobot;        //TAG������ʹ�õ��û���������

        //���캯������Ҫ������Ӧ������΢��API��������
        public TagRobot ( SinaApiService oAPI, UserQueue qUserForTagRobot )
            : base( oAPI )
        {
            strLogFile = Application.StartupPath + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_tag.log";
            queueUserForTagRobot = qUserForTagRobot;
        }

        /// <summary>
        /// ��ָ����UserIDΪ��㿪ʼ����
        /// </summary>
        /// <param name="lUid"></param>
        public void Start ( long lStartUserID )
        {
            if (lStartUserID == 0) return;
            //����ʼUserID���
            queueUserForTagRobot.Enqueue( lStartUserID );
            lCurrentID = lStartUserID;
            //�Զ�������ѭ�����У�ֱ���в�����ͣ��ֹͣ
            while (true)
            {
                if (blnAsyncCancelled) return;
                while (blnSuspending)
                {
                    if (blnAsyncCancelled) return;
                    Thread.Sleep( 50 );
                }

                //����ͷȡ��
                lCurrentID = queueUserForTagRobot.RollQueue();

                #region Ԥ����
                if (lCurrentID == lStartUserID)  //˵������һ��ѭ������
                {
                    if (blnAsyncCancelled) return;
                    while (blnSuspending)
                    {
                        if (blnAsyncCancelled) return;
                        Thread.Sleep( 50 );
                    }

                    //��־
                    Log( "��ʼ����֮ǰ���ӵ�������..." );

                    Tag.NewIterate();
                    UserTag.NewIterate();
                }
                #endregion
                //#region �û���ǩ��Ϣ
                //if (blnAsyncCancelled) return;
                //while (blnSuspending)
                //{
                //    if (blnAsyncCancelled) return;
                //    Thread.Sleep( 50 );
                //}

                ////��־
                //Log( "��ȡ�û�" + lCurrentID.ToString() + "�ı�ǩ..." );
                //LinkedList<Tag> lstTag = crawler.GetTagsByWeb( lCurrentID );
                ////��־
                //Log( "����" + lstTag.Count.ToString() + "����ǩ��" );

                //while (lstTag.Count > 0)
                //{
                //    if (blnAsyncCancelled) return;
                //    while (blnSuspending)
                //    {
                //        if (blnAsyncCancelled) return;
                //        Thread.Sleep( 50 );
                //    }
                //    Tag tag = lstTag.First.Value;
                //    if (!Tag.Exists( tag.tag_id ))
                //    {
                //        //��־
                //        Log( "����ǩ" + tag.tag_id.ToString() + "�������ݿ�..." );
                //        tag.Add();
                //    }
                //    else
                //        //��־
                //        Log( "��ǩ" + tag.tag_id.ToString() + "�Ѵ��ڡ�" );

                //    if (!UserTag.Exists( lCurrentID, tag.tag_id ))
                //    {
                //        //��־
                //        Log( "��¼�û�" + lCurrentID.ToString() + "ӵ�б�ǩ" + tag.tag_id.ToString() + "..." );
                //        UserTag user_tag = new UserTag();
                //        user_tag.user_id = lCurrentID;
                //        user_tag.tag_id = tag.tag_id;
                //        user_tag.Add();
                //    }
                //    else
                //        //��־
                //        Log( "�û�" + lCurrentID.ToString() + "ӵ�б�ǩ" + tag.tag_id.ToString() + "�Ѵ��ڡ�" );

                //    lstTag.RemoveFirst();
                //}
                //#endregion
                //��־
                Log( "�û�" + lCurrentID.ToString() + "�ı�ǩ��������ȡ��ϡ�" );
                //��־
                Log( "����������Ϊ" + crawler.SleepTime.ToString() + "���롣��Сʱʣ��" + crawler.ResetTimeInSeconds.ToString() + "�룬ʣ���������Ϊ" + crawler.RemainingHits.ToString() + "��" );
            }
        }

        public override void Initialize ()
        {
            //��ʼ����Ӧ����
            blnAsyncCancelled = false;
            blnSuspending = false;
            crawler.StopCrawling = false;
            queueUserForTagRobot.Initialize();
        }
    }
}
