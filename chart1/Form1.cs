using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace chart1
{
    [Guid("7DD14230-1252-4E0D-B900-6E48FB60633E")]
    public interface IUseCsharpChart
    {
        void UseTimer();    // 外部接口以实现c++调用timer的运行
        void StopTimer();
    }
    public partial class Form1 : Form, IUseCsharpChart   // 继承接口
    {

        public Queue<double> dataQueue = new Queue<double>(100);   // 显示100个点
        public Queue<string> timeQueue = new Queue<string>(100);

        private int num = 1;//每次删除增加几个点

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = SeriesChartType.Point;
            chart1.Series[0].IsValueShownAsLabel = true;    // 设置是否在Chart中显示坐标点值
            this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.chart1.ChartAreas[0].AxisY.Maximum = 100;
            // this.chart1.ChartAreas[0].AxisX.Interval = 5;
            chart1.ChartAreas[0].Name = "绘图";
            chart1.ChartAreas[0].AxisX.Title = "当前时间(H:M:S)";   // x轴名称
            chart1.Series[0].XValueType = ChartValueType.Time;  // X坐标轴数据格式
            chart1.Series[0].Name = "数据1"; //设置数据名称(text)
            chart1.BorderlineWidth = 2;
            //chart1.Width = 800;
            //chart1.Height = 400;
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            this.timer1.Start();
            chart1.Series[0].Points.Clear();
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // DateTime.Now.ToLongTimeString().ToString();  // 格式20:13:52
            UpdateQueueValue();
            this.chart1.Series[0].Points.Clear();   // 每次更新之前画的图
            for (int i = 0; i < dataQueue.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY(timeQueue.ElementAt(i), dataQueue.ElementAt(i));
            }
        }

        private void UpdateQueueValue()
        {

            if (dataQueue.Count > 100)   // 表示队列有100个点
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    dataQueue.Dequeue();
                    timeQueue.Dequeue();
                }
            }

            Random r = new Random();
            for (int i = 0; i < num; i++)
            {
                dataQueue.Enqueue(r.Next(0, 100));
                timeQueue.Enqueue(DateTime.Now.ToLongTimeString().ToString());
            }

            string bmpName = "0" + ".jpeg";
            // Save image
            chart1.SaveImage(bmpName, ChartImageFormat.Jpeg);
        }

        public void UseTimer()  // 给外部引用
        {
            timer1.Enabled = true;
            this.timer1.Start();
            chart1.Series[0].Points.Clear();
        }

        public void StopTimer()
        {
            this.timer1.Stop();
            Environment.Exit(0);
        }

    }

}
