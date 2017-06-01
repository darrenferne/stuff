namespace WindowsFormsApplication1
{
    partial class TestForm
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
            this.txtReportTemplate = new System.Windows.Forms.TextBox();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.dlgBrowse = new System.Windows.Forms.OpenFileDialog();
            this.cmdRunAll = new System.Windows.Forms.Button();
            this.chkIncludeSubFolders = new System.Windows.Forms.CheckBox();
            this.cmdRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReportTemplate
            // 
            this.txtReportTemplate.Location = new System.Drawing.Point(12, 12);
            this.txtReportTemplate.Name = "txtReportTemplate";
            this.txtReportTemplate.Size = new System.Drawing.Size(414, 20);
            this.txtReportTemplate.TabIndex = 0;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdBrowse.Location = new System.Drawing.Point(432, 10);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(27, 23);
            this.cmdBrowse.TabIndex = 1;
            this.cmdBrowse.Text = "...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // cmdRunAll
            // 
            this.cmdRunAll.Location = new System.Drawing.Point(384, 65);
            this.cmdRunAll.Name = "cmdRunAll";
            this.cmdRunAll.Size = new System.Drawing.Size(75, 23);
            this.cmdRunAll.TabIndex = 2;
            this.cmdRunAll.Text = "Run All";
            this.cmdRunAll.UseVisualStyleBackColor = true;
            this.cmdRunAll.Click += new System.EventHandler(this.cmdRunAll_Click);
            // 
            // chkIncludeSubFolders
            // 
            this.chkIncludeSubFolders.AutoSize = true;
            this.chkIncludeSubFolders.Location = new System.Drawing.Point(258, 71);
            this.chkIncludeSubFolders.Name = "chkIncludeSubFolders";
            this.chkIncludeSubFolders.Size = new System.Drawing.Size(120, 17);
            this.chkIncludeSubFolders.TabIndex = 3;
            this.chkIncludeSubFolders.Text = "Include Sub Folders";
            this.chkIncludeSubFolders.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.cmdRun.Location = new System.Drawing.Point(384, 39);
            this.cmdRun.Name = "button1";
            this.cmdRun.Size = new System.Drawing.Size(75, 23);
            this.cmdRun.TabIndex = 4;
            this.cmdRun.Text = "Run";
            this.cmdRun.UseVisualStyleBackColor = true;
            this.cmdRun.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 96);
            this.Controls.Add(this.cmdRun);
            this.Controls.Add(this.chkIncludeSubFolders);
            this.Controls.Add(this.cmdRunAll);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.txtReportTemplate);
            this.Name = "TestForm";
            this.Text = "Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReportTemplate;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.OpenFileDialog dlgBrowse;
        private System.Windows.Forms.Button cmdRunAll;
        private System.Windows.Forms.CheckBox chkIncludeSubFolders;
        private System.Windows.Forms.Button cmdRun;
    }
}