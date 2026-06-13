namespace FurnitureStore
{
    partial class Autorizathion
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Autorizathion));
            this.imagePanel = new System.Windows.Forms.Panel();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPasswd = new System.Windows.Forms.TextBox();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.pictureBoxCaptcha = new System.Windows.Forms.PictureBox();
            this.labelCaptcha = new System.Windows.Forms.Label();
            this.textBoxCaptcha = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).BeginInit();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.BackColor = System.Drawing.Color.Transparent;
            this.imagePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imagePanel.BackgroundImage")));
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.imagePanel.Font = new System.Drawing.Font("Verdana", 14F);
            this.imagePanel.Location = new System.Drawing.Point(94, 49);
            this.imagePanel.Margin = new System.Windows.Forms.Padding(4);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(267, 185);
            this.imagePanel.TabIndex = 13;
            // 
            // buttonEnter
            // 
            this.buttonEnter.BackColor = System.Drawing.Color.MistyRose;
            this.buttonEnter.FlatAppearance.BorderSize = 0;
            this.buttonEnter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnter.Font = new System.Drawing.Font("Verdana", 14F);
            this.buttonEnter.Location = new System.Drawing.Point(170, 473);
            this.buttonEnter.Margin = new System.Windows.Forms.Padding(4);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(126, 62);
            this.buttonEnter.TabIndex = 5;
            this.buttonEnter.Text = "Войти";
            this.buttonEnter.UseVisualStyleBackColor = false;
            this.buttonEnter.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 14F);
            this.label2.Location = new System.Drawing.Point(16, 336);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 29);
            this.label2.TabIndex = 9;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 14F);
            this.label1.Location = new System.Drawing.Point(16, 263);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 29);
            this.label1.TabIndex = 10;
            this.label1.Text = "Логин:";
            // 
            // textBoxPasswd
            // 
            this.textBoxPasswd.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxPasswd.Location = new System.Drawing.Point(21, 366);
            this.textBoxPasswd.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPasswd.MaxLength = 50;
            this.textBoxPasswd.Name = "textBoxPasswd";
            this.textBoxPasswd.Size = new System.Drawing.Size(398, 36);
            this.textBoxPasswd.TabIndex = 2;
            this.textBoxPasswd.UseSystemPasswordChar = true;
            this.textBoxPasswd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPasswd_KeyPress);
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxLogin.Location = new System.Drawing.Point(21, 296);
            this.textBoxLogin.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxLogin.MaxLength = 50;
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(398, 36);
            this.textBoxLogin.TabIndex = 1;
            this.textBoxLogin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLogin_KeyPress);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Verdana", 14F);
            this.checkBox1.Location = new System.Drawing.Point(427, 376);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonSettings
            // 
            this.buttonSettings.BackColor = System.Drawing.Color.Transparent;
            this.buttonSettings.BackgroundImage = global::FurnitureStore.Properties.Resources.settings;
            this.buttonSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSettings.FlatAppearance.BorderSize = 0;
            this.buttonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSettings.Font = new System.Drawing.Font("Verdana", 14F);
            this.buttonSettings.Location = new System.Drawing.Point(0, 0);
            this.buttonSettings.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(40, 40);
            this.buttonSettings.TabIndex = 6;
            this.buttonSettings.UseVisualStyleBackColor = false;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // pictureBoxCaptcha
            // 
            this.pictureBoxCaptcha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCaptcha.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCaptcha.Location = new System.Drawing.Point(21, 410);
            this.pictureBoxCaptcha.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxCaptcha.Name = "pictureBoxCaptcha";
            this.pictureBoxCaptcha.Size = new System.Drawing.Size(398, 81);
            this.pictureBoxCaptcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCaptcha.TabIndex = 14;
            this.pictureBoxCaptcha.TabStop = false;
            this.pictureBoxCaptcha.Visible = false;
            // 
            // labelCaptcha
            // 
            this.labelCaptcha.AutoSize = true;
            this.labelCaptcha.BackColor = System.Drawing.Color.Transparent;
            this.labelCaptcha.Font = new System.Drawing.Font("Verdana", 14F);
            this.labelCaptcha.Location = new System.Drawing.Point(16, 506);
            this.labelCaptcha.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCaptcha.Name = "labelCaptcha";
            this.labelCaptcha.Size = new System.Drawing.Size(121, 29);
            this.labelCaptcha.TabIndex = 9;
            this.labelCaptcha.Text = "Captcha:";
            this.labelCaptcha.Visible = false;
            // 
            // textBoxCaptcha
            // 
            this.textBoxCaptcha.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxCaptcha.Font = new System.Drawing.Font("Verdana", 14F);
            this.textBoxCaptcha.Location = new System.Drawing.Point(21, 539);
            this.textBoxCaptcha.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCaptcha.MaxLength = 5;
            this.textBoxCaptcha.Name = "textBoxCaptcha";
            this.textBoxCaptcha.Size = new System.Drawing.Size(398, 36);
            this.textBoxCaptcha.TabIndex = 4;
            this.textBoxCaptcha.Visible = false;
            this.textBoxCaptcha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCaptcha_KeyPress);
            // 
            // Autorizathion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::FurnitureStore.Properties.Resources.mainAutorization;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(450, 554);
            this.Controls.Add(this.pictureBoxCaptcha);
            this.Controls.Add(this.imagePanel);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.labelCaptcha);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPasswd);
            this.Controls.Add(this.textBoxCaptcha);
            this.Controls.Add(this.textBoxLogin);
            this.Controls.Add(this.checkBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Autorizathion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            this.Load += new System.EventHandler(this.Autorizathion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCaptcha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPasswd;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.PictureBox pictureBoxCaptcha;
        private System.Windows.Forms.Label labelCaptcha;
        private System.Windows.Forms.TextBox textBoxCaptcha;
    }
}

