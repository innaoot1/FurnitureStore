
namespace FurnitureStore
{
    partial class ClientsInsert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientsInsert));
            this.textBoxFIO = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.maskedTextBoxPhone = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // textBoxFIO
            // 
            this.textBoxFIO.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxFIO.Location = new System.Drawing.Point(174, 18);
            this.textBoxFIO.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFIO.MaxLength = 100;
            this.textBoxFIO.Name = "textBoxFIO";
            this.textBoxFIO.Size = new System.Drawing.Size(437, 39);
            this.textBoxFIO.TabIndex = 37;
            this.textBoxFIO.TextChanged += new System.EventHandler(this.textBoxFIO_TextChanged);
            this.textBoxFIO.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFIO_KeyPress);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label8.Location = new System.Drawing.Point(7, 84);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 32);
            this.label8.TabIndex = 36;
            this.label8.Text = "Телефон:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label2.Location = new System.Drawing.Point(7, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 32);
            this.label2.TabIndex = 34;
            this.label2.Text = "ФИО:";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.MistyRose;
            this.buttonBack.FlatAppearance.BorderSize = 0;
            this.buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBack.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonBack.Location = new System.Drawing.Point(13, 153);
            this.buttonBack.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(120, 65);
            this.buttonBack.TabIndex = 41;
            this.buttonBack.Text = "Назад";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonWrite
            // 
            this.buttonWrite.BackColor = System.Drawing.Color.MistyRose;
            this.buttonWrite.FlatAppearance.BorderSize = 0;
            this.buttonWrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWrite.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonWrite.Location = new System.Drawing.Point(447, 153);
            this.buttonWrite.Margin = new System.Windows.Forms.Padding(4);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(185, 65);
            this.buttonWrite.TabIndex = 40;
            this.buttonWrite.Text = "Сохранить";
            this.buttonWrite.UseVisualStyleBackColor = false;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // maskedTextBoxPhone
            // 
            this.maskedTextBoxPhone.BackColor = System.Drawing.SystemColors.Window;
            this.maskedTextBoxPhone.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.maskedTextBoxPhone.ForeColor = System.Drawing.Color.Black;
            this.maskedTextBoxPhone.Location = new System.Drawing.Point(174, 84);
            this.maskedTextBoxPhone.Margin = new System.Windows.Forms.Padding(4);
            this.maskedTextBoxPhone.Mask = "+7 (000) 000-00-00";
            this.maskedTextBoxPhone.Name = "maskedTextBoxPhone";
            this.maskedTextBoxPhone.Size = new System.Drawing.Size(437, 39);
            this.maskedTextBoxPhone.TabIndex = 54;
            this.maskedTextBoxPhone.Click += new System.EventHandler(this.maskedTextBoxPhone_Click);
            this.maskedTextBoxPhone.Enter += new System.EventHandler(this.maskedTextBoxPhone_Enter);
            // 
            // ClientsInsert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(645, 231);
            this.ControlBox = false;
            this.Controls.Add(this.maskedTextBoxPhone);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.textBoxFIO);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "ClientsInsert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Клиент";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxFIO;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxPhone;
    }
}