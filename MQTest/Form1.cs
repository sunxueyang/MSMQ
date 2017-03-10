using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MsgModel m = receiveComplexMsg<MsgModel>();
            MessSHow(m);
        }
        private void sendSimpleMsg()
        {
            //实例化MessageQueue,并指向现有的一个名称为VideoQueue队列  
            MessageQueue MQ = new MessageQueue(@".\private$\MsgQueue");
            //MQ.Send("消息测试", "测试消息");  
            System.Messaging.Message message = new System.Messaging.Message();
            message.Label = "消息lable";
            message.Body = "消息body";
            MQ.Send(message);

            MessSHow("成功发送消息，" + DateTime.Now + "<br/>");
        }
        private void receiveSimpleMsg()
        {
            MessageQueue MQ = new MessageQueue(@".\private$\MsgQueue");
            //调用MessageQueue的Receive方法接收消息  
            if (MQ.GetAllMessages().Length > 0)
            {
                System.Messaging.Message message = MQ.Receive(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    //message.Formatter = new System.Messaging.XmlMessageFormatter(new string[] { "Message.Bussiness.VideoPath,Message" });//消息类型转换  
                    message.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(string) });
                    MessSHow(string.Format("接收消息成功,lable:{0},body:{1},{2}<br/>", message.Label, message.Body.ToString(), DateTime.Now));
                }
            }
            else
            {
                MessSHow("没有消息了！<br/>");
            }
        }
        private void sendComplexMsg(string id, string content)
        {
            //实例化MessageQueue,并指向现有的一个名称为VideoQueue队列  
            MessageQueue MQ = new MessageQueue(@".\private$\MsgQueue");
            //MQ.Send("消息测试", "测试消息");  
            System.Messaging.Message message = new System.Messaging.Message();
            message.Label = "复杂消息lable";
            message.Body = new MsgModel(id, content);
            MQ.Send(message);

            MessSHow("成功发送消息，" + DateTime.Now + "<br/>");
        }
        private void receiveComplexMsg()
        {
            MessageQueue MQ = new MessageQueue(@".\private$\MsgQueue");
            //调用MessageQueue的Receive方法接收消息  
            if (MQ.GetAllMessages().Length > 0)
            {
                System.Messaging.Message message = MQ.Receive(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    message.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(MsgModel) });//消息类型转换  
                    MsgModel msg = (MsgModel)message.Body;
                    MessSHow(string.Format("接收消息成功,lable:{0},body:{1},{2}<br/>", message.Label, msg, DateTime.Now));
                }
            }
            else
            {
                MessSHow("没有消息了！<br/>");
            }
        }
        private T receiveComplexMsg<T>()
        {
            MessageQueue MQ = new MessageQueue(@".\private$\MsgQueue");
            if (!MessageQueue.Exists(@".\private$\MsgQueue"))
            {
                CreateNewQueue("MsgQueue");
            }
            //调用MessageQueue的Receive方法接收消息  
            if (MQ.GetAllMessages().Length > 0)
            {
                System.Messaging.Message message = MQ.Receive(TimeSpan.FromSeconds(5));
                
                if (message != null)
                {
                    message.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(T) });//消息类型转换  
                    T msg = (T)message.Body;
                    return msg;
                }
            }
            else
            {
                sendComplexMsg(new Random().Next(100, 999).ToString(), DateTime.Now.ToString());
            }

            return default(T);
        }

        /// <summary>  
        /// 创建消息队列  
        /// </summary>  
        /// <param name="name">消息队列名称</param>  
        /// <returns></returns>  
        public void CreateNewQueue(string name)
        {
            if (!System.Messaging.MessageQueue.Exists(".\\private$\\" + name))//检查是否已经存在同名的消息队列  
            {

                System.Messaging.MessageQueue mq = System.Messaging.MessageQueue.Create(".\\private$\\" + name);
                mq.Label = "private$\\" + name;
                MessSHow("创建成功！<br/>");
            }
            else
            {
                //System.Messaging.MessageQueue.Delete(".\\private$\\" + name);//删除一个消息队列  
                MessSHow("已经存在<br/>");
            }
        }

        public void MessSHow(MsgModel m)
        {
            txtRes.Text += m?.ToString();
        }
        public void MessSHow(string content)
        {
            txtRes.Text += $"\r\n{content}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendComplexMsg(new Random().Next(100, 999).ToString(), textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MsgModel m = receiveComplexMsg<MsgModel>();
            MessSHow(m);
        }
    }
    [Serializable]
    public class MsgModel
    {
        public string id { get; set; }
        public string Name { get; set; }
        public MsgModel() { }
        public MsgModel(string _id, string _Name)
        {
            id = _id;
            Name = _Name;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(Name)) return "";
            return string.Format("id--{0},Name--{1}", id, Name);
        }
    }
}
