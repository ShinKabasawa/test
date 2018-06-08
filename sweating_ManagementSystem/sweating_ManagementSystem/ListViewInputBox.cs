using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sweating_ManagementSystem
{
    public class ListViewInputBox : TextBox
    {
        public class InputEventArgs : EventArgs
        {
            public string Path = "";
            public string Newname = "";
        }

        public delegate void InputEventHandler(object sender, InputEventArgs e);

        // イベントデリゲートの宣言 
        public event InputEventHandler FinishInput;

        private InputEventArgs args = new InputEventArgs();

        private bool finished = false;

        private ListViewItem listviewitem;

        private int Index;

        private ListView ListView;

        /// <summary>
        /// </summary>
        /// <param name="parent">対象となるListViewコントロール</param>
        /// <param name="item">編集対象アイテム</param>
        /// <param name="subItem_index">編集する対象の列</param>
        public ListViewInputBox(ListView parent, ListViewItem item, int subItem_index)
        {
            args.Path = item.SubItems[subItem_index].Text;
            args.Newname = item.SubItems[subItem_index].Text;

            int left = 0;
            
            for(int i = 0; i < subItem_index; i++){
                left += parent.Columns[i].Width;
            }

            listviewitem = item;
            Index = subItem_index;
            ListView = parent;

            int width = item.SubItems[subItem_index].Bounds.Width;
            int height = item.SubItems[subItem_index].Bounds.Height;

            if (subItem_index == 0)
            {
                width = 100;
            }

            this.Parent = parent;
            this.Size = new Size(width, height);
            this.Left = left;
            this.Top = item.Position.Y - 1;
            this.Text = item.SubItems[subItem_index].Text;
            this.LostFocus += new EventHandler(textbox_LostFocus);
            this.ImeMode = ImeMode.NoControl;
            this.Multiline = false;
            this.KeyDown += new KeyEventHandler(textbox_Keydown);
            this.Focus();

        }

        void Finish(string new_name)
        {
            //Enterで入力を鑑賞した場合はKeyDownが呼ばれた後に
            //さらにLostFocusが呼ばれる為、2かFinishが呼ばれる
            if (!finished)
            {
                //textbox.Hide()すると同時にLostFocusする為、
                //finished=trueを先に呼び出しておかないと
                //このブロックが２回呼ばれてしまう
                finished = true;
                this.Hide();
                args.Newname = new_name;
                FinishInput(this, args);
            }
        }

        void textbox_Keydown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Finish(this.Text);

                bool exits = false;

                for (int i = 0; i < 40; i++)
                {
                    //同じ名称がないか確認
                    if (this.Text == ListView.Items[i].SubItems[Index].Text)
                    {
                        exits = true;
                    }
                }

                if (exits)
                {   
                    //同じ名称が存在する場合、リスト内の名称を変更しない
                    MessageBox.Show("既に存在します。");
                }
                else
                {
                    listviewitem.SubItems[Index].Text = this.Text;
                }
                
            }
            else 
                if (e.KeyCode == Keys.Escape)
            {
                Finish(args.Newname);
                listviewitem.SubItems[Index].Text = this.Text;

            }
            else if (e.KeyCode == Keys.Tab)
            {
                
                    ListViewInputBox input = new ListViewInputBox(ListView, listviewitem, Index);
                    input.FinishInput += new ListViewInputBox.InputEventHandler(input_finishInput);
            }
        }

        void textbox_LostFocus(object sender, EventArgs e)
        {
            Finish(this.Text);
        }

        void input_finishInput(object sender, ListViewInputBox.InputEventArgs e)
        {
            Console.WriteLine(e.Path);
            Console.WriteLine(e.Newname);
        }
    }
}
