using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sweating_ManagementSystem
{
    public partial class Form1 : Form


    {
        public Form1()
        {
            InitializeComponent();

            //ユーザーがサイズ変更できないようにする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //フォームが最大化されないようにする
            this.MaximizeBox = false;

            //フォームが最小化されないようにする
            this.MinimizeBox = false;

            button1.Text = "接続";
            button2.Text = "設定";

        }

        private int count = 0;
        private Form2 form2;

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }


            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            form2 = null;

            string[] text = GetDeviceNames();

            for (int i = 0; i < text.Length; i++)
            {

                if (text[i].Substring(0,15).Equals("USB Serial Port"))
                {
                    //接続機器名が「USB Serial Port」のCOMポートの数値を取得
                    serialPort1.PortName = text[i].Substring(17).Replace(")","");
                    break;
                }
            }
           
        }

        /// <summary>
        /// 接続ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string csvpath   = "";
            bool writeHeader = true;

            try {
                //選択したCOMポートで接続を行う 
                string portName = comboBox1.SelectedItem.ToString();
                serialPort1.BaudRate  = 9600;
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits  = StopBits.One;
                serialPort1.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialPort1.Handshake = Handshake.None;
                //serialPort1.PortName = portName;
                serialPort1.Open();
            }
            catch
            {
            }

            ///////////////////////////////////////////////////////////////
            //SweatingWeb.API_Device device;                             //
            //device = new SweatingWeb.API_Device()                      //
            //{                                                          //
            //    guidance = new List<SweatingWeb.API_Device.Guidance>(){//
            //        new SweatingWeb.API_Device.Guidance(){             //
            //            card_no = "00",                                //
            //            serial_no = "00",                              //
            //            setting_no = '1',                              //
            //        }                                                  //
            //    }                                                      //
            //};                                                         //
            ///////////////////////////////////////////////////////////////

            ////////////////////
            //String bd = ""; //
            //String fn = ""; //
            //String ln = ""; //
            //String fnk = "";//
            //String lnk = "";//
            //String mem = "";//
            //String pss = "";//
            //String sex = "";//
            ////////////////////

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
                //次の5秒間分のデータをZIPに圧縮し、送信する準備を行う
                MessageBox.Show("ステータスはOKです"+st);
            }
            else
            {
                //送信失敗時の処理を記述
                //サーバーに送信出来なかった取得データを保存
                MessageBox.Show("ステータスはNGです"+ st);
            }

            //行の追加
            DataTable dt = new DataTable("DevDataTable");
            DataRow dRow = dt.NewRow();


            ///////////////////////////////////////////////////////////
            //確認用                                                 //
            //列の追加                                               //
            //MessageBox MessageBox;                                 //
            //MessageBox.Show("ボタンがクリックされました。");       //
            //dt.Columns.Add("id", Type.GetType("System.Int32"));    //
            //dt.Columns.Add("title");                               //
            //dt.Columns.Add("content");                             //
            //dt.Columns.Add("date",Type.GetType("System.DateTime"));//
            ///////////////////////////////////////////////////////////


            dt.Columns.Add("SetId", Type.GetType("System.Int32"));          //セットID
            dt.Columns.Add("DevNo", Type.GetType("System.Int32"));          //機器No
            dt.Columns.Add("No", Type.GetType("System.Int32"));             //連番
            dt.Columns.Add("RR_interval", Type.GetType("System.Int32"));    //RR間隔
            dt.Columns.Add("AccX", Type.GetType("System.Int32"));           //加速度X
            dt.Columns.Add("AccY", Type.GetType("System.Int32"));           //加速度Y
            dt.Columns.Add("AccZ", Type.GetType("System.Int32"));           //加速度Z
            dt.Columns.Add("Sweating", Type.GetType("System.Int32"));       //発汗
            dt.Columns.Add("Temperature", Type.GetType("System.Int32"));    //温度
            dt.Columns.Add("Humidity", Type.GetType("System.Int32"));       //湿度
            //dt.Columns.Add("Battery_status", Type.GetType("Sytem.Int32"));  //バッテリー状態
            dt.Columns.Add("Reserve1", Type.GetType("System.Int32"));       //予備1～23
            //dt.Columns.Add("Reserve2", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve3", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve4", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve5", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve6", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve7", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve8", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve9", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve10", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve11", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve12", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve13", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve14", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve15", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve16", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve17", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve18", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve19", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve20", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve21", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve22", Type.GetType("System.Int32"));
            //dt.Columns.Add("Reserve23", Type.GetType("System.Int32"));
            

            /////////////////////////////////////////////
            //確認用                                   //
            //DataSet dSet = new DataSet();            //
            //1行目                                    //
            //DataRowを使った方法                      //
            //dRow["id"] = 1;                          //
            //dRow["title"] = "first";                 //
            //dRow["content"] = "1行目です";           //
            //dRow["date"] = new DateTime();           //
            //dSet.Tables["DevRcvData"].Rows.Add(dRow);//
            /////////////////////////////////////////////


            ///////////////////// データ取得時の処理 Start /////////////////////
            //一クラス分（約40人）
            for (int i = 0; i < 40; i++)
            {
                //dt.Rows.Clear();
                //5秒分取得し、DatTableに格納
                for (int j = 0; j < 5; j++)
                {
                    dRow = dt.NewRow();
                    dRow["SetId"]           = 1;        //セットID
                    dRow["DevNo"]           = 1001;     //機器No
                    dRow["No"]              = 1;        //連番
                    dRow["RR_interval"]     = 120;      //RR間隔
                    dRow["AccX"]            = 120.5;    //加速度X
                    dRow["AccY"]            = 20.2;     //加速度Y
                    dRow["AccZ"]            = 24.3;     //加速Z
                    dRow["Sweating"]        = 28;       //発汗
                    dRow["Temperature"]     = 25.6;     //温度
                    dRow["Humidity"]        = 35;       //湿度
                    //dRow["Battery_status"]  = 1;        //バッテリー状態
                    dRow["Reserve1"]        = 100;      //予備1～23
                }
                //DatTableに追加
                dt.ImportRow(dRow);
            
                

                //WebAPIで取得したデータをサーバーに送信

            }
            ///////////////////// データ取得時の処理 End ///////////////////////


            ////////////////////////////////////////////////////////////
            //確認用                                                  //
            //DataTableに追加                                         //
            //dt.Rows.Add(dRow);                                      //
            //別の方法1                                               //
            //dt.Rows.Add(dRow);                                      //
            //2行目                                                   //
            //Rows.Addメソッドを使った方法                            //
            //dt.Rows.Add("2", "second", "2行目です", new DateTime());//
            ////////////////////////////////////////////////////////////

            //////////////////// 機器からのデータ取得完了時の処理 Start ////////////////////
            DataRow[] foundTable;
            DataTable dt2 = new DataTable("Result");
            dt2.Columns.Add("SetId", Type.GetType("System.Int32"));          //セットID
            dt2.Columns.Add("DevNo", Type.GetType("System.Int32"));          //機器No
            dt2.Columns.Add("No", Type.GetType("System.Int32"));             //連番
            dt2.Columns.Add("RR_interval", Type.GetType("System.Int32"));    //RR間隔
            dt2.Columns.Add("AccX", Type.GetType("System.Int32"));           //加速度X
            dt2.Columns.Add("AccY", Type.GetType("System.Int32"));           //加速度Y
            dt2.Columns.Add("AccZ", Type.GetType("System.Int32"));           //加速度Z
            dt2.Columns.Add("Sweating", Type.GetType("System.Int32"));       //発汗
            dt2.Columns.Add("Temperature", Type.GetType("System.Int32"));    //温度
            dt2.Columns.Add("Humidity", Type.GetType("System.Int32"));       //湿度
            //dt2.Columns.Add("Battery_status", Type.GetType("Sytem.Int32"));  //バッテリー状態
            //dt2.Columns.Add("Reserve1", Type.GetType("System.Int32"));       //予備1～23
            //dt2.Columns.Add("Reserve2", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve3", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve4", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve5", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve6", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve7", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve8", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve9", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve10", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve11", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve12", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve13", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve14", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve15", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve16", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve17", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve18", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve19", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve20", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve21", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve22", Type.GetType("System.Int32"));
            //dt2.Columns.Add("Reserve23", Type.GetType("System.Int32"));

            //一クラス分（約40人）の各人のデータを抽出し、CSVファイルを作成
            for (int x = 0; x < 40; x++)
            {

                //dt2.Rows.Clear();
                //Select条件文
                String expression = "SetId = 1 and DevNo = 1001";

                //Sort条件文
                String sortorder = "No ASC";

                //条件文にあったデータを取得
                foundTable = dt.Select(expression, sortorder);

                for (int y = 0; y < foundTable.Length; y++)
                {
                    foundTable[y] = dt2.NewRow();
                    //取得した結果を結果テーブルに追加
                    //dt2.Rows.Add(foundTable[y]);
                    dt2.ImportRow(foundTable[y]);

                    //dt2.NewRow();
                }

                //String setId = dt2.Rows[0]["SetId"].ToString();
                //String setId = dt2.Rows[0].ToString();
                String today = new DateTime().ToString();

                String setId = "00101010";

                //csvpath = "C:\\mouthdb"+x+".csv";

                csvpath = "C:\\SweatingApp";
                csvpath = csvpath + "\\DevData.csv";
                //指定したディレクトリが存在するか確認
                DirectoryUtils.SafeCreateDirectory(csvpath);

                //抽出したデータをCSVに出力する
                //dt2         = 各人の機器から取得したデータのテーブル
                //csvpath     = 格納先/"DevData + セットId_日付.csv"
                //writeHeader = CSVにヘッダーを書き込むか書き込まないかの判定フラグ
                //CsvOutput.ConvertDataTableToCsv(dt, csvpath, writeHeader);
            }
            //////////////////// 機器からのデータ取得完了時の処理 End //////////////////////

        }

        delegate void SetTextCallback(string text);
        /// <summary>
        /// データ受信後の処理
        /// </summary>
        /// <param name="text"></param>
        private void Response_(string text)
        {
            if (textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Response_);
                BeginInvoke(d, new object[] { text });

            }
            else
            {
                textBox1.AppendText(text + "\n");
            }
        }

        /// <summary>
        /// 設定ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();


            //二重起動防止
            if (this.form2 == null || this.form2.IsDisposed)
            {
                // null、又は、破棄されていた場合、Form2を表示する
                this.form2 = new Form2();
                this.form2.Show();

            }
            
        }

        /// <summary>
        /// シリアル接続でデータ受信した時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                count++;
                string str = serialPort1.ReadExisting();
                Response_(str);
            }
            else
            {
                return;
            }

            if (count == 5)
            {
                //5秒分の機器からの取得データをクラウドへ送信

                count = 0;
            }


        }

        /// <summary>
        /// フォームを閉じる時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        /// <summary>
        /// シリアル接続する機器名を取得するメソッド
        /// </summary>
        /// <returns></returns>
        private string[] GetDeviceNames()
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

        private void serialPort1_PinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

    }
}

