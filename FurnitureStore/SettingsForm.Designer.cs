
namespace FurnitureStore
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.textBoxUid = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 14F);
            this.label2.Location = new System.Drawing.Point(8, 315);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 29);
            this.label2.TabIndex = 14;
            this.label2.Text = "Пользователь:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 14F);
            this.label1.Location = new System.Drawing.Point(8, 242);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 29);
            this.label1.TabIndex = 15;
            this.label1.Text = "Сервер:";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxPwd.Location = new System.Drawing.Point(13, 421);
            this.textBoxPwd.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPwd.MaxLength = 50;
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(398, 36);
            this.textBoxPwd.TabIndex = 12;
            this.textBoxPwd.UseSystemPasswordChar = true;
            this.textBoxPwd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPwd_KeyPress);
            // 
            // textBoxUid
            // 
            this.textBoxUid.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxUid.Location = new System.Drawing.Point(13, 348);
            this.textBoxUid.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxUid.MaxLength = 50;
            this.textBoxUid.Name = "textBoxUid";
            this.textBoxUid.Size = new System.Drawing.Size(398, 36);
            this.textBoxUid.TabIndex = 11;
            this.textBoxUid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUid_KeyPress);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Verdana", 14F);
            this.checkBox1.Location = new System.Drawing.Point(419, 431);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBoxHost
            // 
            this.textBoxHost.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxHost.Location = new System.Drawing.Point(13, 275);
            this.textBoxHost.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHost.MaxLength = 50;
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(398, 36);
            this.textBoxHost.TabIndex = 11;
            this.textBoxHost.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxHost_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 14F);
            this.label3.Location = new System.Drawing.Point(8, 388);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 29);
            this.label3.TabIndex = 14;
            this.label3.Text = "Пароль:";
            // 
            // buttonEnter
            // 
            this.buttonEnter.BackColor = System.Drawing.Color.MistyRose;
            this.buttonEnter.FlatAppearance.BorderSize = 0;
            this.buttonEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnter.Font = new System.Drawing.Font("Verdana", 14F);
            this.buttonEnter.Location = new System.Drawing.Point(276, 475);
            this.buttonEnter.Margin = new System.Windows.Forms.Padding(4);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(161, 62);
            this.buttonEnter.TabIndex = 16;
            this.buttonEnter.Text = "Сохранить";
            this.buttonEnter.UseVisualStyleBackColor = false;
            this.buttonEnter.Click += new System.EventHandler(this.buttonEnter_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackColor = System.Drawing.Color.MistyRose;
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Font = new System.Drawing.Font("Verdana", 14F);
            this.buttonExit.Location = new System.Drawing.Point(13, 475);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(132, 62);
            this.buttonExit.TabIndex = 17;
            this.buttonExit.Text = "Назад";
            this.buttonExit.UseVisualStyleBackColor = false;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // imagePanel
            // 
            this.imagePanel.BackColor = System.Drawing.Color.Transparent;
            this.imagePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imagePanel.BackgroundImage")));
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imagePanel.Font = new System.Drawing.Font("Verdana", 14F);
            this.imagePanel.Location = new System.Drawing.Point(106, 24);
            this.imagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(238, 187);
            this.imagePanel.TabIndex = 18;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::FurnitureStore.Properties.Resources.mainAutorization;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(450, 550);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPwd);
            this.Controls.Add(this.textBoxHost);
            this.Controls.Add(this.textBoxUid);
            this.Controls.Add(this.checkBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка подключения";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.TextBox textBoxUid;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Panel imagePanel;
    }
}