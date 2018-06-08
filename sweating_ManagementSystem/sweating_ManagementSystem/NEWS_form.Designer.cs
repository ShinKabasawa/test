namespace sweating_ManagementSystem
{
    partial class NEWS_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NEWS_form));
            this.Titlelabel = new System.Windows.Forms.Label();
            this.Textlabel = new System.Windows.Forms.Label();
            this.now_btn = new System.Windows.Forms.Button();
            this.later_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Titlelabel
            // 
            this.Titlelabel.AutoSize = true;
            this.Titlelabel.Location = new System.Drawing.Point(12, 35);
            this.Titlelabel.Name = "Titlelabel";
            this.Titlelabel.Size = new System.Drawing.Size(28, 12);
            this.Titlelabel.TabIndex = 0;
            this.Titlelabel.Text = "Title";
            // 
            // Textlabel
            // 
            this.Textlabel.AutoSize = true;
            this.Textlabel.Location = new System.Drawing.Point(12, 82);
            this.Textlabel.Name = "Textlabel";
            this.Textlabel.Size = new System.Drawing.Size(28, 12);
            this.Textlabel.TabIndex = 1;
            this.Textlabel.Text = "Text";
            // 
            // now_btn
            // 
            this.now_btn.Location = new System.Drawing.Point(44, 131);
            this.now_btn.Name = "now_btn";
            this.now_btn.Size = new System.Drawing.Size(75, 23);
            this.now_btn.TabIndex = 2;
            this.now_btn.Text = "今すぐ受信";
            this.now_btn.UseVisualStyleBackColor = true;
            this.now_btn.Click += new System.EventHandler(this.now_btn_Click);
            // 
            // later_btn
            // 
            this.later_btn.Location = new System.Drawing.Point(203, 131);
            this.later_btn.Name = "later_btn";
            this.later_btn.Size = new System.Drawing.Size(75, 23);
            this.later_btn.TabIndex = 3;
            this.later_btn.Text = "後で受信";
            this.later_btn.UseVisualStyleBackColor = true;
            this.later_btn.Click += new System.EventHandler(this.later_btn_Click);
            // 
            // NEWS_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(323, 180);
            this.Controls.Add(this.later_btn);
            this.Controls.Add(this.now_btn);
            this.Controls.Add(this.Textlabel);
            this.Controls.Add(this.Titlelabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NEWS_form";
            this.Text = "未転送データ確認";
            this.Load += new System.EventHandler(this.NEWS_form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Titlelabel;
        private System.Windows.Forms.Label Textlabel;
        private System.Windows.Forms.Button now_btn;
        private System.Windows.Forms.Button later_btn;
    }
}