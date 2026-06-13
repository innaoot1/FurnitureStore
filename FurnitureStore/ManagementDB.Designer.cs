
namespace FurnitureStore
{
    partial class ManagementDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagementDB));
            this.comboBoxImport = new System.Windows.Forms.ComboBox();
            this.labelCategory = new System.Windows.Forms.Label();
            this.buttonImportFile = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonBackup = new System.Windows.Forms.Button();
            this.buttonStructure = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxExport = new System.Windows.Forms.ComboBox();
            this.buttonExportFile = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxImport
            // 
            this.comboBoxImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxImport.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.comboBoxImport.FormattingEnabled = true;
            this.comboBoxImport.Location = new System.Drawing.Point(19, 45);
            this.comboBoxImport.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxImport.Name = "comboBoxImport";
            this.comboBoxImport.Size = new System.Drawing.Size(378, 40);
            this.comboBoxImport.TabIndex = 1;
            // 
            // labelCategory
            // 
            this.labelCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCategory.AutoSize = true;
            this.labelCategory.BackColor = System.Drawing.Color.Transparent;
            this.labelCategory.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.labelCategory.Location = new System.Drawing.Point(13, 9);
            this.labelCategory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(329, 32);
            this.labelCategory.TabIndex = 48;
            this.labelCategory.Text = "Таблица для импорта:";
            // 
            // buttonImportFile
            // 
            this.buttonImportFile.BackColor = System.Drawing.Color.MistyRose;
            this.buttonImportFile.FlatAppearance.BorderSize = 0;
            this.buttonImportFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImportFile.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonImportFile.Location = new System.Drawing.Point(19, 93);
            this.buttonImportFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonImportFile.Name = "buttonImportFile";
            this.buttonImportFile.Size = new System.Drawing.Size(378, 56);
            this.buttonImportFile.TabIndex = 2;
            this.buttonImportFile.Text = "Файл для импорта";
            this.buttonImportFile.UseVisualStyleBackColor = false;
            this.buttonImportFile.Click += new System.EventHandler(this.buttonImportFile_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.BackColor = System.Drawing.Color.MistyRose;
            this.buttonBack.FlatAppearance.BorderSize = 0;
            this.buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBack.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonBack.Location = new System.Drawing.Point(13, 355);
            this.buttonBack.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(120, 82);
            this.buttonBack.TabIndex = 9;
            this.buttonBack.Text = "Назад";
            this.buttonBack.UseVisualStyleBackColor = false;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonBackup
            // 
            this.buttonBackup.BackColor = System.Drawing.Color.MistyRose;
            this.buttonBackup.FlatAppearance.BorderSize = 0;
            this.buttonBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBackup.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonBackup.Location = new System.Drawing.Point(338, 355);
            this.buttonBackup.Margin = new System.Windows.Forms.Padding(4);
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(204, 82);
            this.buttonBackup.TabIndex = 8;
            this.buttonBackup.Text = "Резервное копирование";
            this.buttonBackup.UseVisualStyleBackColor = false;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // buttonStructure
            // 
            this.buttonStructure.BackColor = System.Drawing.Color.MistyRose;
            this.buttonStructure.FlatAppearance.BorderSize = 0;
            this.buttonStructure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStructure.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonStructure.Location = new System.Drawing.Point(550, 265);
            this.buttonStructure.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStructure.Name = "buttonStructure";
            this.buttonStructure.Size = new System.Drawing.Size(237, 82);
            this.buttonStructure.TabIndex = 7;
            this.buttonStructure.Text = "Восстановить структуру БД";
            this.buttonStructure.UseVisualStyleBackColor = false;
            this.buttonStructure.Click += new System.EventHandler(this.buttonStructure_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.label1.Location = new System.Drawing.Point(403, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(338, 32);
            this.label1.TabIndex = 48;
            this.label1.Text = "Таблица для экспорта:";
            // 
            // comboBoxExport
            // 
            this.comboBoxExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExport.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.comboBoxExport.FormattingEnabled = true;
            this.comboBoxExport.Location = new System.Drawing.Point(409, 45);
            this.comboBoxExport.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxExport.Name = "comboBoxExport";
            this.comboBoxExport.Size = new System.Drawing.Size(378, 40);
            this.comboBoxExport.TabIndex = 4;
            // 
            // buttonExportFile
            // 
            this.buttonExportFile.BackColor = System.Drawing.Color.MistyRose;
            this.buttonExportFile.FlatAppearance.BorderSize = 0;
            this.buttonExportFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportFile.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonExportFile.Location = new System.Drawing.Point(409, 93);
            this.buttonExportFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExportFile.Name = "buttonExportFile";
            this.buttonExportFile.Size = new System.Drawing.Size(378, 56);
            this.buttonExportFile.TabIndex = 5;
            this.buttonExportFile.Text = "Файл для экспорта";
            this.buttonExportFile.UseVisualStyleBackColor = false;
            this.buttonExportFile.Click += new System.EventHandler(this.buttonExportFile_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.BackColor = System.Drawing.Color.MistyRose;
            this.buttonImport.FlatAppearance.BorderSize = 0;
            this.buttonImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImport.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonImport.Location = new System.Drawing.Point(19, 157);
            this.buttonImport.Margin = new System.Windows.Forms.Padding(4);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(378, 56);
            this.buttonImport.TabIndex = 3;
            this.buttonImport.Text = "Импортировать данные";
            this.buttonImport.UseVisualStyleBackColor = false;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.BackColor = System.Drawing.Color.MistyRose;
            this.buttonExport.FlatAppearance.BorderSize = 0;
            this.buttonExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExport.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonExport.Location = new System.Drawing.Point(409, 157);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(378, 56);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Text = "Экспортировать данные";
            this.buttonExport.UseVisualStyleBackColor = false;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonRestore
            // 
            this.buttonRestore.BackColor = System.Drawing.Color.MistyRose;
            this.buttonRestore.FlatAppearance.BorderSize = 0;
            this.buttonRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRestore.Font = new System.Drawing.Font("Verdana", 15.75F);
            this.buttonRestore.Location = new System.Drawing.Point(550, 355);
            this.buttonRestore.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(237, 82);
            this.buttonRestore.TabIndex = 7;
            this.buttonRestore.Text = "Восстановить базу данных";
            this.buttonRestore.UseVisualStyleBackColor = false;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // ManagementDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBoxExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxImport);
            this.Controls.Add(this.labelCategory);
            this.Controls.Add(this.buttonExportFile);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonImportFile);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonBackup);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.buttonStructure);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagementDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Управление БД";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxImport;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.Button buttonImportFile;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonBackup;
        private System.Windows.Forms.Button buttonStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxExport;
        private System.Windows.Forms.Button buttonExportFile;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonRestore;
    }
}