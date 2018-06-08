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

namespace sweating_ManagementSystem
{
    public partial class NEWS_form : Form
    {
        public NEWS_form()
        {
            InitializeComponent();
        }

        private void NEWS_form_Load(object sender, EventArgs e)
        {

            //ユーザーがサイズ変更できないようにする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //最大化出来ないようにする
            this.MaximizeBox = false;

            //最小化出来ないようにする
            this.MinimizeBox = false;

            //バルーン表示
            ShowNewsBalloon("常駐アプリ", "クレードル内に未送信データが存在する可能性があります。\n必ずクレードル内の未転送データを受信してください", 30000);
        }

        /// <summary>
        /// 未転送データお知らせバルーン表示
        /// </summary>
        /// <param name="Title">タイトル</param>
        /// <param name="Text">メッセージ内容</param>
        /// <param name="time">表示時間（ms)</param>
        public void ShowNewsBalloon(string Title, string Text, int time)
        {
            Titlelabel.Text = Title;
            Textlabel.Text = Text;

            int ScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int Screenheigth = Screen.PrimaryScreen.WorkingArea.Height;
            int AppWidth = this.Width;
            int AppHeight = this.Height;

            int AppLeftXPos = ScreenWidth - AppWidth;
            int AppLeftYPos = Screenheigth;

            Rectangle tempRect = new Rectangle(AppLeftXPos, AppLeftYPos, AppWidth, AppHeight);
            this.DesktopBounds = tempRect;

            this.Visible = true;
            this.Update();

            for (int i = 0; i < AppHeight; i += 3)
            {
                AppLeftXPos = ScreenWidth - AppWidth;
                AppLeftYPos = Screenheigth - i;

                tempRect = new Rectangle(AppLeftXPos, AppLeftYPos, AppWidth, AppHeight);
                this.DesktopBounds = tempRect;

                System.Threading.Thread.Sleep(10);

                this.Update();
            }
        }

        /// <summary>
        /// 「今すぐ受信」クリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void now_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("この後シリアル接続確認を行う");
            this.Close();
        }

        /// <summary>
        /// 「後で受信」クリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void later_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("バルーン表示したまま");
        }

    }
}
