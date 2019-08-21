namespace WindowsFormsApplication1
{
    partial class ViewerForm
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
            this.ReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.cmdContinue = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReportViewer
            // 
            this.ReportViewer.ActiveViewIndex = -1;
            this.ReportViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.ReportViewer.DisplayBackgroundEdge = false;
            this.ReportViewer.DisplayStatusBar = false;
            this.ReportViewer.Location = new System.Drawing.Point(0, 0);
            this.ReportViewer.Name = "ReportViewer";
            this.ReportViewer.Padding = new System.Windows.Forms.Padding(5);
            this.ReportViewer.ShowCloseButton = false;
            this.ReportViewer.Size = new System.Drawing.Size(697, 344);
            this.ReportViewer.TabIndex = 0;
            this.ReportViewer.ToolPanelWidth = 215;
            // 
            // cmdContinue
            // 
            this.cmdContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdContinue.Location = new System.Drawing.Point(529, 354);
            this.cmdContinue.Name = "cmdContinue";
            this.cmdContinue.Size = new System.Drawing.Size(75, 23);
            this.cmdContinue.TabIndex = 1;
            this.cmdContinue.Text = "Continue";
            this.cmdContinue.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(610, 354);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // ViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 386);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdContinue);
            this.Controls.Add(this.ReportViewer);
            this.Name = "ViewerForm";
            this.Text = "Report Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private CrystalDecisions.Windows.Forms.CrystalReportViewer ReportViewer;
        private System.Windows.Forms.Button cmdContinue;
        private System.Windows.Forms.Button cmdCancel;
    }
}

