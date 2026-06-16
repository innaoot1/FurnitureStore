namespace FurnitureStore
{
    partial class SupplierInsert
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupplierInsert));
            this.textBoxSupplierName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.maskedTextBoxPhone = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxINN = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxContactPerson = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxSupplierName
            // 
            this.textBoxSupplierName.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxSupplierName.Location = new System.Drawing.Point(19, 45);
            this.textBoxSupplierName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxSupplierName.MaxLength = 100;
            this.textBoxSupplierName.Name = "textBoxSupplierName";
            this.textBoxSupplierName.Size = new System.Drawing.Size(356, 39);
            this.textBoxSupplierName.TabIndex = 1;
            this.textBoxSupplierName.TextChanged += new System.EventHandler(this.textBoxSupplierName_TextChanged);
            this.textBoxSupplierName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSupplierName_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label2.Location = new System.Drawing.Point(13, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 32);
            this.label2.TabIndex = 34;
            this.label2.Text = "Поставщик:";
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.MistyRose;
            this.buttonBack.FlatAppearance.BorderSize = 0;
            this.buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBack.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonBack.Location = new System.Drawing.Point(13, 253);
            this.buttonBack.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(120, 65);
            this.buttonBack.TabIndex = 8;
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
            this.buttonWrite.Location = new System.Drawing.Point(567, 253);
            this.buttonWrite.Margin = new System.Windows.Forms.Padding(4);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(185, 65);
            this.buttonWrite.TabIndex = 7;
            this.buttonWrite.Text = "Сохранить";
            this.buttonWrite.UseVisualStyleBackColor = false;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // maskedTextBoxPhone
            // 
            this.maskedTextBoxPhone.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.maskedTextBoxPhone.Location = new System.Drawing.Point(19, 203);
            this.maskedTextBoxPhone.Margin = new System.Windows.Forms.Padding(4);
            this.maskedTextBoxPhone.Mask = "+7 (000) 000-00-00";
            this.maskedTextBoxPhone.Name = "maskedTextBoxPhone";
            this.maskedTextBoxPhone.Size = new System.Drawing.Size(356, 39);
            this.maskedTextBoxPhone.TabIndex = 3;
            this.maskedTextBoxPhone.Click += new System.EventHandler(this.maskedTextBoxPhone_Click);
            this.maskedTextBoxPhone.Enter += new System.EventHandler(this.maskedTextBoxPhone_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label4.Location = new System.Drawing.Point(383, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 32);
            this.label4.TabIndex = 36;
            this.label4.Text = "Адрес:";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxAddress.Location = new System.Drawing.Point(389, 45);
            this.textBoxAddress.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxAddress.MaxLength = 10000;
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(356, 39);
            this.textBoxAddress.TabIndex = 4;
            this.textBoxAddress.TextChanged += new System.EventHandler(this.textBoxAddress_TextChanged);
            this.textBoxAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAddress_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label5.Location = new System.Drawing.Point(383, 88);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 32);
            this.label5.TabIndex = 36;
            this.label5.Text = "ИНН:";
            // 
            // textBoxINN
            // 
            this.textBoxINN.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxINN.Location = new System.Drawing.Point(389, 124);
            this.textBoxINN.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxINN.MaxLength = 12;
            this.textBoxINN.Name = "textBoxINN";
            this.textBoxINN.Size = new System.Drawing.Size(356, 39);
            this.textBoxINN.TabIndex = 5;
            this.textBoxINN.TextChanged += new System.EventHandler(this.textBoxINN_TextChanged);
            this.textBoxINN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxINN_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label6.Location = new System.Drawing.Point(383, 167);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 32);
            this.label6.TabIndex = 36;
            this.label6.Text = "Описание:";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxDescription.Location = new System.Drawing.Point(389, 203);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(356, 39);
            this.textBoxDescription.TabIndex = 6;
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            this.textBoxDescription.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxDescription_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label1.Location = new System.Drawing.Point(13, 88);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 32);
            this.label1.TabIndex = 36;
            this.label1.Text = "Конт. лицо:";
            // 
            // textBoxContactPerson
            // 
            this.textBoxContactPerson.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.textBoxContactPerson.Location = new System.Drawing.Point(19, 124);
            this.textBoxContactPerson.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxContactPerson.MaxLength = 100;
            this.textBoxContactPerson.Name = "textBoxContactPerson";
            this.textBoxContactPerson.Size = new System.Drawing.Size(356, 39);
            this.textBoxContactPerson.TabIndex = 2;
            this.textBoxContactPerson.TextChanged += new System.EventHandler(this.textBoxContactPerson_TextChanged);
            this.textBoxContactPerson.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxContactPerson_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label3.Location = new System.Drawing.Point(13, 167);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 32);
            this.label3.TabIndex = 34;
            this.label3.Text = "Телефон:";
            // 
            // SupplierInsert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(765, 331);
            this.Controls.Add(this.maskedTextBoxPhone);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxINN);
            this.Controls.Add(this.textBoxAddress);
            this.Controls.Add(this.textBoxContactPerson);
            this.Controls.Add(this.textBoxSupplierName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SupplierInsert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Поставщик";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox textBoxSupplierName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxPhone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxContactPerson;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxINN;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDescription;
    }
}