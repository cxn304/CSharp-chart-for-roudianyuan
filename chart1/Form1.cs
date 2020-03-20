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
using System.IO;

namespace chart1
{
    [Guid("7DD14230-1252-4E0D-B900-6E48FB60633E")]
    public interface IUseCsharpChart
    {
        void UseTimer();    // 外部接口以实现c++调用timer的运行
        void StopTimer();
        void CsvAndPlot();
    }
    public partial class Form1 : Form, IUseCsharpChart   // 继承接口
    {

        public Queue<double> xQueue = new Queue<double>(100);   // 显示100个点
        public Queue<double> yQueue = new Queue<double>(100);
        public Queue<string> typeQueue = new Queue<string>(100);
        public Queue<string> areasQueue = new Queue<string>(100);
        public Queue<string> contlengthQueue = new Queue<string>(100);
        public Queue<string> radiusQueue = new Queue<string>(100);
        public Queue<string> pixelValueQueue = new Queue<string>(100);


        private int num = 1;//每次删除增加几个点

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = SeriesChartType.Point;
            chart1.Series[0].IsValueShownAsLabel = true;    // 设置是否在Chart中显示坐标点值
            this.chart1.ChartAreas[0].AxisX.Minimum = 0;
            // this.chart1.ChartAreas[0].AxisY.Maximum = 100;
            this.chart1.ChartAreas[0].AxisX.Interval = 5;
            chart1.ChartAreas[0].Name = "绘图";
            // chart1.ChartAreas[0].AxisX.Title = "当前时间(H:M:S)";   // x轴名称
            // chart1.Series[0].XValueType = ChartValueType.Time;  // X坐标轴数据格式
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
            for (int i = 0; i < xQueue.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY(yQueue.ElementAt(i), xQueue.ElementAt(i));
            }
        }

        private void UpdateQueueValue()
        {

            if (xQueue.Count > 100)   // 表示队列有100个点
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    xQueue.Dequeue();
                    yQueue.Dequeue();
                }
            }

            Random r = new Random();
            for (int i = 0; i < num; i++)
            {
                xQueue.Enqueue(r.Next(0, 100));
                // yQueue.Enqueue(DateTime.Now.ToLongTimeString().ToString());
            }

            string bmpName = "../x64/Debug/0.jpeg";
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

        public void CsvAndPlot()
        {
            csv2dt("../x64/Debug/roudian.csv", 0);
        }

        public void csv2dt(string filePath, int n)  // 读取csv文件并用chart画图和储存
        {
            StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8, false);
            int m = 0;
            reader.Peek();
            while (reader.Peek() > 0)
            {
                string str = reader.ReadLine(); // 一下读取一行
                if (m >= n + 1) // n表示表头行数，输出是第零行
                {
                    
                    string[] split = str.Split(',');    // 对每行进行分割
                    xQueue.Enqueue(double.Parse(split[0]));
                    yQueue.Enqueue(double.Parse(split[1]));
                    typeQueue.Enqueue(split[2]);
                    areasQueue.Enqueue(split[3]);
                    contlengthQueue.Enqueue(split[4]);
                    radiusQueue.Enqueue(split[5]);
                    pixelValueQueue.Enqueue(split[6]);
                }
                m += 1;
            }
            this.chart1.Series[0].Points.Clear();   // 每次更新之前画的图
            this.chart1.ChartAreas[0].AxisX.Interval = xQueue.Count/2;
            chart1.Width = 50 * xQueue.Count;
            for (int i = 0; i < xQueue.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY(xQueue.ElementAt(i), yQueue.ElementAt(i));
            }
            string bmpName = "../x64/Debug/0.jpeg";
            // Save image
            chart1.SaveImage(bmpName, ChartImageFormat.Jpeg);
            reader.Close();     // 要释放打开的文件
            //string filelength = @"../x64/Debug/roudian.csv";
            //File.Delete(filelength);
        }
    }

}
