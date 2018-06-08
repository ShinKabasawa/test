using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace sweating_ManagementSystem
{
    static class Program
    {



        /// <summary>
        /// アプリケーションのメインエントリポイント
        /// </summary>
        [STAThread]
        static void Main()
        {


            /// 多重起動禁止判定 Start ///
            //Mutex名を決める（必ずアプリケーション固有の文字列に変更すること
            string muteName = "SweatingApp";

            bool createdNew;

            System.Threading.Mutex mutex = new System.Threading.Mutex(true, muteName, out createdNew);

            //ミューテックスの初期所有権が付与されたか調べる
            if (createdNew == false)
            {
                //されていなかった場合、既に起動していると判断して終了
                MessageBox.Show("既にこのアプリ起動されています。", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                mutex.Close();
                return;
            }
            /// 多重起動禁止判定 End ///


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            try
            {
                //常駐アプリを作成する場合
                Resident resident = new Resident();
                Application.Run();

            }
            finally
            {
                //ミューテックスを解放する。
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }
    }

    class Resident : Form
    {

        private Thread thread;          //WebAPI通信用スレッド
        public  SerialPort serialport;  //シリアルポート
        private int count = 0;
        private Form2 form2;
        private Form1 form1;
        private NEWS_form news_form;
        private readonly string csvpath = "C:\\SweatingApp";
        public Thread serial_check;
        private ServerURL_input serverUrl_input;
        private NotifyIcon notifyicon;
        private System.Windows.Forms.Timer SerialCheck_timer;
        private System.Windows.Forms.Timer SendDevData_Timer;
        private string URL;

        public Resident()
        {
            this.ShowInTaskbar = false;
            this.setCompornent();

            form1 = null;
            form2 = null;
            serverUrl_input = null;


            //施設情報確認
            URL = Properties.Settings.Default.Server_URL;

            if (URL == null || URL.Equals(""))
            {
                if (serverUrl_input == null || serverUrl_input.IsDisposed)
                {
                    //null、又は、破棄されていた場合、Form1を表示
                    serverUrl_input = new ServerURL_input();
                    serverUrl_input.Show();
                    return;
                }
            }


            //接続確認



            connect();

            //COMポートを検索
            //string[] ports = SerialPort.GetPortNames();

            //string[] DevName = GetDeviceNames();

            //serialport = new SerialPort();


            //int size = 0;

            ////nullチェック
            //if (DevName != null)
            //{
            //    size = DevName.Length;   
            //}

            //for (int i = 0; i < size; i++)
            //{
            //    if (DevName[i].Substring(0, 15).Equals("USB Serial Port"))
            //    {
            //        //接続機器名が「USB Serial Port」のCOMポートの数値を取得
            //        serialport.PortName = DevName[i].Substring(17).Replace(")", "");
            //        break;
            //    }
            //}


            ////検索したCOMポートでシリアル接続
            //try
            //{
            //    //USBシリアル変換器を検索した結果のCOMポート名を
            //    //使用しシリアル接続を行う
            //    serialport.BaudRate = 9600;
            //    serialport.Parity = Parity.None;
            //    serialport.DataBits = 8;
            //    serialport.StopBits = StopBits.One;
            //    serialport.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
            //    serialport.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
            //    serialport.Handshake = Handshake.None;
            //    serialport.Open();
            //}
            //catch (Exception e)
            //{

            //}

            ////シリアル接続確認
            //if (!serialport.IsOpen)
            //{
            //    MessageBox.Show("シリアル接続出来ませんでした。");
            //}


            ////USBシリアル変換器の変化を監視
            //serial_check = new Thread(SerialCheck);
            //serial_check.Start();

            ////アプリ起動時にタイマーセット
            //timer = new System.Windows.Forms.Timer();
            //timer.Tick += new EventHandler(Periodic_execution);
            //timer.Interval = 1000;
            //timer.Enabled = true;
            

        }

        /// <summary>
        /// シリアル接続を行う
        /// </summary>
        public void connect()
        {
            //COMポートを検索
            string[] ports = SerialPort.GetPortNames();

            string[] DevName = GetDeviceNames();

            serialport = new SerialPort();


            int size = 0;

            //nullチェック
            if (DevName != null)
            {
                size = DevName.Length;
            }

            for (int i = 0; i < size; i++)
            {
                if (DevName[i].Substring(0, 15).Equals("USB Serial Port"))
                {
                    //接続機器名が「USB Serial Port」のCOMポートの数値を取得
                    serialport.PortName = DevName[i].Substring(17).Replace(")", "");
                    break;
                }
            }


            //検索したCOMポートでシリアル接続
            try
            {
                //USBシリアル変換器を検索した結果のCOMポート名を
                //使用しシリアル接続を行う
                serialport.BaudRate = 9600;
                serialport.Parity = Parity.None;
                serialport.DataBits = 8;
                serialport.StopBits = StopBits.One;
                serialport.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                serialport.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialport.Handshake = Handshake.None;
                serialport.Open();
            }
            catch (Exception e)
            {

            }

            //シリアル接続確認
            if (!serialport.IsOpen)
            {
                MessageBox.Show("シリアル接続出来ませんでした。");
            }


            //USBシリアル変換器の変化を監視
            //serial_check = new Thread(SerialCheck);
            //serial_check.Start();

            //アプリ起動時にタイマーセット
            SerialCheck_timer = new System.Windows.Forms.Timer();
            SerialCheck_timer.Tick += new EventHandler(SerialCheck);
            SerialCheck_timer.Interval = 500;
            SerialCheck_timer.Enabled = true;


            //1秒周期で機器から取得したデータをサーバーへ送信
            SendDevData_Timer = new System.Windows.Forms.Timer();
            SendDevData_Timer.Tick += new EventHandler(Periodic_execution);
            SendDevData_Timer.Interval = 1000;
            SendDevData_Timer.Enabled = true;
        }

        /// <summary>
        /// タイマーによる定期実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Periodic_execution(object sender, EventArgs e)
        {
            //データ送信スレッド開始
            thread = new Thread(SendData2Cloud);
            thread.Start();

            //thread.Join();
            //thread.Abort();

        }

        /// <summary>
        /// USBシリアル変換器接続時：シリアル接続を行う
        /// USBシリアル変換器抜かれた時：シリアル接続を切断する
        /// </summary>
        private void SerialCheck(object sender,EventArgs e)
        {

            string[] name = GetDeviceNames();
            int CountDevice = 0;
            if (name != null)
            {
                CountDevice = name.Length;
            }
             
            int CountDeviceBefour = CountDevice;
            bool serial_flg = false;
            string port_name = null;

            //while (true)
            //{

                name = GetDeviceNames();

                //nullチェック
                if (name != null)
                {
                    CountDevice = name.Length;
                }
                else
                {
                    CountDevice = 0;
                    serial_flg = false;
                }
                
                
                //if (CountDevice != CountDeviceBefour)
                //{

                    for (int i = 0; i < CountDevice; i++)
                    {
                        if (name[i].Substring(0, 15).Equals("USB Serial Port"))
                        {
                            //接続機器名が「USB Serial Port」のCOMポートの数値を取得
                            port_name = name[i].Substring(17).Replace(")", "");
                            serial_flg = true;
                            break;
                        }
                        else
                        {
                            serial_flg = false;
                        }
                    }

                    if (serial_flg)
                    {
                        if (!serialport.IsOpen)
                        {
                            try
                            {                                
                                serialport.PortName = port_name;
                                serialport.Open();
                                MessageBox.Show("接続された");
   
                            }
                            catch (Exception)
                            {
                                serial_flg = false;        
                                throw;
                            }
                        }
                    }
                    else
                    {
                        //serialport.Close();
                        DialogResult dr = new DialogResult();

                        if (dr == null)
                        {
                         
                            dr = MessageBox.Show("クレードルとの通信が切断されました。\nクレードル内に未送信データが存在する可能性があります。\n今すぐ未転送データを受信しますか", "確認", MessageBoxButtons.YesNo);
   
                        }
                        if (dr == System.Windows.Forms.DialogResult.Yes)
                        {
                            MessageBox.Show("Yesがクリックされました");
                        }
                        else
                        {
                            SerialCheck_timer.Stop();
                            if (news_form == null)
                            {
                             
                                news_form = new NEWS_form();
                                news_form.ShowVersionUp("常駐アプリ", "クレードル内に未送信データが存在する可能性があります。\n必ずクレードル内の未転送データを受信してください", 100000);   
   
                            }
                            //break;

                        }   

                    }
                //}

                //CountDeviceBefour = CountDevice;
            //}
            //news_form.ShowVersionUp("常駐アプリ", "クレードル内に未送信データが存在する可能性があります。\n必ずクレードル内の未転送データを受信してください", 30000);   
        }

        /// <summary>
        /// シリアル通信：データ受信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialport.IsOpen)
            {
                //受信した回数をカウント
                count++;

                string str = serialport.ReadExisting();

                Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(serialport.ReadExisting());

                Response_(str);

                //Byte[] bytes = System.Text.Encoding.ASCII.GetBytes(serialport.ReadExisting());

                //Response_(str);

                //for (int i = 0; i < bytes.Length; i++)
                //{
                //    str += Convert.ToString(bytes[i], 16);
                //}
                //ディレクトリが存在するか確認
                //DirectoryUtils.SafeCreateDirectory(csvpath);

                //受信したデータをテキストファイルに出力
                string txtpath = "C:\\SweatingApp\\serialData" + count + ".txt";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(txtpath, false,
                      System.Text.Encoding.GetEncoding("UTF-8")))
                {
                    file.Write(str);
                }

                //取得したデータを出力するパス
                String csvpath_new = csvpath + "\\DevData" + count + ".csv";

                //DataTable dt = new DataTable("DevDataTable");
                //DataRow dRow = dt.NewRow();
                //dt.Columns.Add("SetId", Type.GetType("System.Int32"));          //セットID
                //dt.Columns.Add("DevNo", Type.GetType("System.Int32"));          //機器No
                //dt.Columns.Add("No", Type.GetType("System.Int32"));             //連番
                //dt.Columns.Add("RR_interval", Type.GetType("System.Int32"));    //RR間隔
                //dt.Columns.Add("AccX", Type.GetType("System.Int32"));           //加速度X
                //dt.Columns.Add("AccY", Type.GetType("System.Int32"));           //加速度Y
                //dt.Columns.Add("AccZ", Type.GetType("System.Int32"));           //加速度Z
                //dt.Columns.Add("Sweating", Type.GetType("System.Int32"));       //発汗
                //dt.Columns.Add("Temperature", Type.GetType("System.Int32"));    //温度
                //dt.Columns.Add("Humidity", Type.GetType("System.Int32"));       //湿度
                //dt.Columns.Add("Reserve1", Type.GetType("System.Int32"));

                //for (int i = 0; i < 40; i++)
                //{
                //dt.Rows.Clear();
                //5秒分取得し、DatTableに格納
                //    for (int j = 0; j < 5; j++)
                //    {
                //        dRow = dt.NewRow();
                //        dRow["SetId"] = 1;              //セットID
                //        dRow["DevNo"] = 1001;           //機器No
                //        dRow["No"] = 1;                 //連番
                //        dRow["RR_interval"] = 120;      //RR間隔
                //        dRow["AccX"] = 120.5;           //加速度X
                //        dRow["AccY"] = 20.2;            //加速度Y
                //        dRow["AccZ"] = 24.3;            //加速Z
                //        dRow["Sweating"] = 28;          //発汗
                //        dRow["Temperature"] = 25.6;     //温度
                //        dRow["Humidity"] = 35;          //湿度
                //        dRow["Reserve1"] = 100;         //予備1～23
                //    }
                //DatTableに追加
                //    dt.ImportRow(dRow);
                //}

                //取得したデータをCSVに出力
                //CsvOutput.ConvertDataTableToCsv(dt, csvpath_new, true);

            }
            else
            {
                //シリアル接続されていない場合、再度接続
                serialport.Open();
            }

            //送信済みのCSVファイルを削除
            //for (int i = 1; i <= 5; i++)
            //{
            //    //CSVファイル削除
            //    System.IO.File.Delete(csvpath + "\\DevData" + i + ".csv");
            //    //txtファイル削除
            //    System.IO.File.Delete(csvpath + "\\serialData" + i + ".txt");
            //    System.IO.File.Delete(csvpath + "\\serialData_copy" + i + ".txt");
            //}
            //}
        }


        /// <summary>
        /// 取得した機器データをサーバーへ送信
        /// </summary>
        private void SendData2Cloud()
        {

            //ディレクトリ内のファイル読み込む


            //JSONの作成


            SweatingWeb.API_USER_Auth_Info user_info;
            user_info = new SweatingWeb.API_USER_Auth_Info
            {
                user_info = new List<SweatingWeb.API_USER_Auth_Info.UserInfo>(){
                    new SweatingWeb.API_USER_Auth_Info.UserInfo() {
                    birthday = "201800411",
                    card_no = "00101010",
                    first_name = "kabasawa",
                    first_name_kana = "kabasawa",
                    last_name = "shin",
                    last_name_kana = "shin",
                    memo = "test",
                    pass = "01234",
                    sex = '1'
                   }
               }
            };

            //サーバー間通信結果格納変数
            String st;


            //PCアプリとサーバー間疎通確認（サーバーに到達データ到達を確認）
            st = SweatingWeb.API_Request(SweatingWeb.Request.UserAuthRequest, user_info);


            MessageBox MessageBox;


            if (st.Equals("OK"))
            {
                //送信成功時の処理を記述
                MessageBox.Show("ステータスはOKです" + st);
            }
            else
            {
                //送信失敗時の処理を記述
                MessageBox.Show("ステータスはNGです" + st);
            }
        }

        delegate void SetTextCallback(string text);
        private void Response_(string text)
        {
            //受信したデータをメッセージボックスで表示
            MessageBox.Show(text);
        }


        /// <summary>
        /// コンテキストメニューの「終了」がクリックされた時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Clisk(object sender, EventArgs e)
        {
            //アプリ終了時にシリアルポートクローズ
            if (serialport != null && serialport.IsOpen)
            {
                serialport.Close();
            }

            //シリアル確認スレッド中止
            if (serial_check != null)
            {
                serial_check.Abort();
            }

            //タイマー停止
            SerialCheck_timer.Stop();

            //データ送信スレッド中止
            if (thread != null)
            {
                thread.Abort();
            }

            //アプリ終了
            Application.Exit();

        }

        /// <summary>
        /// コンテキストメニューの「設定」がクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Setting(object sender, EventArgs e)
        {
            //二重起動防止
            if (serverUrl_input == null || serverUrl_input.IsDisposed)
            {   
              
                //null、又は、破棄されていた場合、Form1を表示
                serverUrl_input = new ServerURL_input();
                serverUrl_input.Show();
            }
        }

        private void Click_Form2(object sender, EventArgs e)
        {
            //二重起動防止
            if (form2 == null || form2.IsDisposed)
            {
                // null、又は、破棄されていた場合、Form2を表示する
                form2 = new Form2();
                form2.Show();
            }
        }


        /// <summary>
        /// 常駐アプリのアイコン、コンテキストメニューの設定
        /// </summary>
        private void setCompornent()
        {
            NotifyIcon icon = new NotifyIcon();
            icon.Icon = new Icon("icon1.ico");
            icon.Visible = true;
            icon.Text = "常駐アプリテスト";
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuitem = new ToolStripMenuItem();
            menuitem.Text = "終了";
            menuitem.Click += new EventHandler(Close_Clisk);
            menu.Items.Add(menuitem);
            ToolStripMenuItem menuitem2 = new ToolStripMenuItem();
            menuitem2.Text = "設定";
            menuitem2.Click += new EventHandler(Click_Setting);
            menu.Items.Add(menuitem2);
            icon.ContextMenuStrip = menu;
        }


        /// <summary>
        /// シリアルポートの接続先機器名を取得
        /// </summary>
        /// <returns>接続先機器名配列</returns>
        public static string[] GetDeviceNames()
        {
            var deviceNameList = new System.Collections.ArrayList();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    deviceNameList.Add(name);
                }
            }

            //戻り値作成
            if (deviceNameList.Count > 0)
            {
                string[] deviceNames = new string[deviceNameList.Count];
                int index = 0;
                foreach (var name in deviceNameList)
                {
                    deviceNames[index++] = name.ToString();
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }
    }
}
