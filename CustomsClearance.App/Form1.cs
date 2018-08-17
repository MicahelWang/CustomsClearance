using CustomsClearance.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomsClearance.App
{
    public partial class Form1 : Form
    {
        private readonly NetworkInterceptor _networkInterceptor;
        private readonly Messager<HttpMessage> _messager;
        private IList<HttpMessage> _httpMessages;
        public Form1()
        {
            InitializeComponent();
            _httpMessages = new List<HttpMessage>();
            _messager = Messagers.Default[MessageNames.HttpMessage] as Messager<HttpMessage>;
            _ListenUrl = "https://swapp.singlewindow.cn/vplatformserver/swProxy/decserver/sw/dec/merge/addDecHead";
            _networkInterceptor = NetworkInterceptor.Instance;
            Initialize();


        }
        private void Initialize()
        {
            this.listView1.Columns.Add("序号", 40, HorizontalAlignment.Left);
            this.listView1.Columns.Add("时间", 60, HorizontalAlignment.Left);
            this.listView1.Columns.Add("内容", 300, HorizontalAlignment.Left);


            this.listView1.View = System.Windows.Forms.View.Details;  //这命令比较重要，否则不能显示
            var timer = new System.Windows.Forms.Timer() { Interval = 200 };
            timer.Tick += (o, args) =>
            {
                foreach (var message in _httpMessages)
                {
                    var item = new ListViewItem
                    {
                        Text = (this.listView1.Items.Count + 1).ToString()
                    };
                    item.SubItems.Add(message.OnCreated.ToString("T"));
                    item.SubItems.Add(message.Body);
                    listView1.Items.Insert(0, item);
                }

                _httpMessages.Clear();
            };
            timer.Start();
            _messager.ReciveHandler = (message) =>
            {
                _httpMessages.Add(message);
            };

            this.txtListenUrl.Text = _ListenUrl;
            SetStatus();
            SetEvent();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetStatus()
        {
            if (_networkInterceptor.isRunning)
            {
                this.lblStatus.Text = "运行中";
                this.lblStatus.ForeColor = Color.Blue;
            }
            else
            {
                this.lblStatus.Text = "已停止";
                this.lblStatus.ForeColor = Color.Red;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
                this.notifyIcon1.Text = "文件监视器";//最小化到托盘时，鼠标点击时显示的文本
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            _networkInterceptor.Start();
            SetStatus();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _networkInterceptor.Stop();
            SetStatus();
        }

        private string _ListenUrl;

        private void BtnListenUrl_Click(object sender, EventArgs e)
        {
            var url = this.txtListenUrl.Text;
            if (!string.IsNullOrEmpty(url))
            {
                _ListenUrl = url;

            }
            SetEvent();
        }

        public string _RequestUrl;

        private void SetEvent()
        {
            if (!string.IsNullOrEmpty(_ListenUrl))
            {
                var filter = new BaseFilterEvent() { Url = _ListenUrl };
                _networkInterceptor.AddEvent(filter);
            }
        }

        private void btnDestory_Click(object sender, EventArgs e)
        {
            _networkInterceptor.Destroy(); SetStatus();
        }
        ~Form1()
        {
            if (_networkInterceptor != null)
            {
                _networkInterceptor.Stop();
                _networkInterceptor.Destroy();
            }
        }
    }
}
