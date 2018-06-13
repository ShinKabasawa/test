using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sweating_ManagementSystem
{
    public partial class ServerURL_input : Form
    {
        private bool save_flg;

        public ServerURL_input()
        {
            InitializeComponent();

            //ユーザーがサイズ変更できないようにする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //フォームが最大化されないようにする
            this.MaximizeBox = false;

            //フォームが最小化されないようにする
            this.MinimizeBox = false;

            //半角英数字のみ
            textBox1.ImeMode = ImeMode.Alpha;

        }

        private void SeverURL_input_Load(object sender, EventArgs e)
        {
            //ロード処理
            this.textBox1.Text = Properties.Settings.Default.Server_URL;
            save_flg = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;

                return createParams;
            }
        }

        /// <summary>
        /// 登録ボタンクリック時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //保存処理
            Properties.Settings.Default.Server_URL = this.textBox1.Text;
            Properties.Settings.Default.Save();

            MessageBox.Show("保存しました", "保存");

            save_flg = true;

            this.Close();

        }

        /// <summary>
        /// フォームを閉じるときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerURL_input_fromClosing(object sender, FormClosingEventArgs e)
        {
            if (!save_flg)
            {
                DialogResult dr = MessageBox.Show("入力した接続先URLを保存せずに閉じてもよろしいですか", "確認", MessageBoxButtons.YesNo);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }   
            }
        }
    }
}
