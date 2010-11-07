using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sinawler.Model;

namespace Sinawler
{
    class UserQueue : QueueBase
    {
        private EnumPreLoadQueue iPreLoadQueue = EnumPreLoadQueue.NO_PRELOAD;       //�Ƿ�����ݿ���Ԥ�����û����С�Ĭ��Ϊ����
        //ͷ�ڵ㱻��ȡ�Ĵ�����Ӧ�ɹ��û������˺�΢�������˶�ȡ��ÿ����ȡһ��ֵ��1������Ϊ2��������ȡ����ʱ���û������˺�΢�������˸�һ�Σ���ͷ�ڵ������β��������������

        public EnumPreLoadQueue PreLoadQueue
        {
            get { return iPreLoadQueue; }
            set { iPreLoadQueue = value; }
        }

        public UserQueue (QueueBufferFor who)
            : base()
        {
            if(who==QueueBufferFor.COMMENT) //Ĭ��ΪUSER_INFO
                lstWaitingIDInDB = new QueueBuffer( QueueBufferFor.USER_INFO );
            else
                lstWaitingIDInDB = new QueueBuffer(who);
        }
    }
}
