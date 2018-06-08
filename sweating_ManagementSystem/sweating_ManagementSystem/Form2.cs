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
    public partial class Form2 : Form
    {
        
        private ColumnHeader columnName;        //使用者氏名
        private ColumnHeader columnSetId;       //セットID
        private ColumnHeader columnDevNo;       //機器No
        private ColumnHeader columnCustomerNo;  //顧客No 
        


        public Form2()
        {
            //ユーザーがサイズ変更できないようにする
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            
            //最大化出来ないようにする
            this.MaximizeBox = false;

            //最小化出来ないようにする
            this.MinimizeBox = false;

            InitializeComponent();

            //ListViewコントロールを初期化
            InitializeListView();

            //ListView内のデータを更新
            RefreshListView();
            
            label1.Text = "使用者設定";
            button1.Text = "登録";
            
        }

        /// <summary>
        /// アイテムのラベルの編集が開始された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            //インデックスが0のアイテムは編集できないようにする
            if (e.Item == 0)
            {
                e.CancelEdit = true;
            }
        }


        /// <summary>
        /// アイテムのラベルが編集された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            //ラベルが変更されたか判定
            if (e.Label != null)
            {
                ListView lv = (ListView)sender;

                //同名のアイテムがあるか判定
                foreach (ListViewItem lvi in lv.Items)
                {
                    //同名のアイテムがあるときは編集をキャンセル
                    if (lvi.Index != e.Item && lvi.Text == e.Label)
                    {
                        MessageBox.Show("同名のアイテムが既に存在します。");
                        e.CancelEdit = true;
                        return;
                    }
                    
                }
            }
        }


        /// <summary>
        /// ListViewでキーが離れた時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            ListView lv = (ListView)sender;
            //F2キーが離されたは、フォーカスのあるアイテムの編集を開始する
            if (e.KeyCode == Keys.F2 && lv.FocusedItem != null && lv.LabelEdit)
            {
                lv.FocusedItem.BeginEdit();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //アプリ終了
            //Application.Exit();

        }

        /// <summary>
        /// 登録ボタンがクリックされた時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {   
            //この画面を閉じる
            //this.Close();

            string s1 = "";
            string s2 = "";
            string s3 = "";

            for (int i = 1; i <= listView1.Items.Count; i++)
            {
                //s1 = listView1.Items[i].Text;
                //s2 = listView1.SelectedItems[i].SubItems[1].Text;
                //s3 = listView1.SelectedItems[i].SubItems[2].Text;
            }

            MessageBox.Show(listView1.Items.Count + "行です");

            RefreshListView();

            this.Close();

        }

        /// <summary>
        /// ListViewのコントロールの初期化
        /// </summary>
        private void InitializeListView()
        {
            //ListViewコントロールのプロパティを設定
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Sorting = SortOrder.Ascending;
            listView1.View = View.Details;

            // 列（コラム）ヘッダの作成
            columnName = new ColumnHeader();
            columnSetId = new ColumnHeader();
            columnDevNo = new ColumnHeader();
            columnCustomerNo = new ColumnHeader();

            columnCustomerNo.Text = "施設No";
            columnCustomerNo.Width = 100;
            columnName.Text = "セットID";
            columnName.Width = 100;
            columnSetId.Text = "機器No";
            columnSetId.Width = 60;
            columnDevNo.Text = "氏名";
            columnDevNo.Width = 100;

            ColumnHeader[] colHeaderRegValue = {this.columnCustomerNo, this.columnName, this.columnSetId, this.columnDevNo };
            listView1.Columns.AddRange(colHeaderRegValue);

        }

        /// <summary>
        /// ListViewのコントロールのデータ更新
        /// </summary>
        private void RefreshListView()
        {
            //ListViewコントロールのデータｗすべて消去
            listView1.Items.Clear();

            string[] item1 = { "00000001", "0001", "1001", "赤" };
            
            //確認用
            for (int i = 0; i < 40; i++)
            {
                
                listView1.Items.Add(new ListViewItem(item1));

            }
        }


        void input_finishInput(object sender, ListViewInputBox.InputEventArgs e)
        {
            Console.WriteLine(e.Path);
            Console.WriteLine(e.Newname);
        }

        /// <summary>
        /// ダブルクリックされた時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);

            Console.WriteLine(e.X);
            Console.WriteLine(e.Y);

            int index = 0;

            //施設Noをダブルクリックした時の処理
            if (e.X > 0 && e.X < 101)
            {
                index = 0;

                if (info.SubItem != null && e.Button == MouseButtons.Left)
                {
                    ListViewInputBox input = new ListViewInputBox(listView1, info.Item, index);
                    input.FinishInput += new ListViewInputBox.InputEventHandler(input_finishInput);
                    input.Show();

                }
            }

            //セットIDをダブルクリックした時の処理
            else if (e.X > 101 && e.X < 201)
            {
                index = 1;

                if (info.SubItem != null && e.Button == MouseButtons.Left)
                {
                    ListViewInputBox input = new ListViewInputBox(listView1, info.Item, index);
                    input.FinishInput += new ListViewInputBox.InputEventHandler(input_finishInput);
                    input.Show();

                }
            }

            //機器Noをダブルクリックした時の処理
            else if (e.X > 200 && e.X < 261)
            {

                index = 2;

                if (info.SubItem != null && e.Button == MouseButtons.Left)
                {
                    ListViewInputBox input = new ListViewInputBox(listView1, info.Item, index);
                    input.FinishInput += new ListViewInputBox.InputEventHandler(input_finishInput);
                    input.Show();

                }
            }

            //氏名をダブルクリックした時の処理
            else if (e.X > 260 && e.X < 361)
            {
                
                index = 3;

                if (info.SubItem != null && e.Button == MouseButtons.Left)
                {
                    ListViewInputBox input = new ListViewInputBox(listView1, info.Item, index);
                    input.FinishInput += new ListViewInputBox.InputEventHandler(input_finishInput);
                    input.Show();

                }
            }
        }

        //カラムの幅が変更されるときの処理
        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }

        //×ボタンがクリックされた時の処理
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
