using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket _socket;

        //照片存放路径
        string path = "E:\\XYJ\\IOT\\text\\text1\\WpfApp1\\WpfApp1\\img";
        public MainWindow()
        {
            InitializeComponent();
           
            Connection();
        }

        private void init()
        {
            //大于30   打开风扇
            MessageBox.Show(getAndset.Temp.ToString(),"");
            if (getAndset.Temp > 30)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {

                    //更新照片
                    System.Windows.Controls.Image imgping = new System.Windows.Controls.Image();
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(path + "\\b.png");
                    bi.EndInit();
                    img_Fan.Source = bi;

                    getAndset.Fan1 = true;
                    label_Fan.Content = "开";
                });
            }
            else if (getAndset.Temp < 10)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    //更新照片
                    System.Windows.Controls.Image imgping = new System.Windows.Controls.Image();
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(path + "\\a.png");
                    bi.EndInit();
                    img_Light.Source = bi;

                    getAndset.Light_bulb1 = true;
                    label_Light.Content = "开";
                });
            }
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    System.Windows.Controls.Image imgping = new System.Windows.Controls.Image();
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(path + "\\c.png");
                    bi.EndInit();

                    img_Fan.Source = bi;
                    img_Light.Source = bi;

                    getAndset.Fan1 = false;
                    getAndset.Light_bulb1 = false;
                    label_Fan.Content = "关";
                    label_Light.Content = "关";
                });
            }
        }

        /// <summary>
        /// 连接事件
        /// </summary>
        private void Connection()
        {
            //摘要
            //创建Socket对象
            /*
             * 
             * 第一个参数:IPV4 ---> InterNetwork
             *           IPV6 ---> InterNetwork6
             * 第二个参数：
             *      支持可靠、 双向、 基于连接的字节流，而无需复制数据，不保留边界。 
             *       一个 Socket 这种类型的通信与单个对等方并在可以开始通信之前需要远程主机的连接
             * 第三个参数:
             *       传输控制协议。
            */
            Socket conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;

            _socket = conn;

            //连接服务器，绑定IP、端口
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(info.IP),info.PORT);

            try 
            { 
                conn.Connect(iPEndPoint);
                MessageBox.Show("登录成功","提示") ;
            } 
            catch (Exception e) 
            {
                MessageBox.Show("请重新连接","警告");
                return;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveServerMsg),conn);

        }

        /// <summary>
        /// 不断接受客户端信息  子程序线程方法
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveServerMsg(object state)
        {
            var proxSocket = state as Socket;

            //创建缓存内存，存储接收的信息 ,不能放到while中，这块内存可以循环利用
            byte[] data = new byte[1020*1024];
            while (true)
            {
                int len;
                try
                {
                    //接受信息，返回字节长度
                    len = proxSocket.Receive(data, 0, data.Length, SocketFlags.None);
                }
                catch (Exception e)
                {
                    try
                    {
                        //关闭Socket
                        ServierExite(String.Format("服务器：{0}非正常退出。", proxSocket.RemoteEndPoint.ToString()), proxSocket);
                    }
                    catch (Exception ex)
                    {

                    }
                    return;//让方法结束，终结当前客户端数据的异步线程，方法退出，即线程结束
                }


                if (len <= 0)
                {
                    // 7、关闭Socket
                    //小于0表示正常退出
                    try
                    {
                        //关闭Socket
                        ServierExite(String.Format("服务器：{0}非正常退出。", proxSocket.RemoteEndPoint.ToString()), proxSocket);
                    }
                    catch (Exception ex1)
                    {

                    }
                    return;//让方法结束，终结当前客户端数据的异步线程，方法退出，即线程结束
                }

                string msgstr = Encoding.Default.GetString(data,0,len);

                AppendTxtLogText(msgstr);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        private void ServierExite(string v, Socket _socket)
        {
            AppendTxtLogText(v);

            try
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close(100);
                }
            }
            catch (Exception e)
            { 
            
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        private void AppendTxtLogText(string v)
        {
            Rootobject rb = JsonConvert.DeserializeObject<Rootobject>(v);
            Console.WriteLine(rb.address.Humi);

            getAndset.Temp = Convert.ToDouble(rb.address.Temp);
            getAndset.Humi = Convert.ToDouble(rb.address.Humi);

            //UI线程
            App.Current.Dispatcher.Invoke((Action)delegate ()
           {
               label_Humi_value.Content = getAndset.Humi;
               label_Temp_value.Content = getAndset.Temp;
           });
           init();
        }

        /// <summary>
        /// 串口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ServierExite(null,_socket);
        }

        private void label_Temp_value_Loaded(object sender, RoutedEventArgs e)
        {
            label_Temp_value.Content = getAndset.Temp;
        }

        private void label_Humi_value_Loaded(object sender, RoutedEventArgs e)
        {
            label_Humi_value.Content = getAndset.Humi;
        }

        private void btn_Add_Value_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                getAndset.Box_Max_Humi = Convert.ToDouble(Text_Max_Temp.Text);
                getAndset.Box_Mix_Temp = Convert.ToDouble(Text_Mix_Temp.Text);

                if (getAndset.Box_Mix_Temp > getAndset.Box_Max_Humi)
                {
                    MessageBox.Show("最小值不能大于最大值","警告");
                    return;
                }
                MessageBox.Show("已修改","提示");
            }
            catch (FormatException )
            {
                MessageBox.Show("格式无效或复合格式字符串的格式不标准", "提示");
                return;
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("无效的强制转换或显式转换！", "提示");
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("强制转换或转换运算导致溢出！", "提示");
                return;
            }
            
        }
    }
}
