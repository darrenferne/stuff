﻿using Brady.Common.CrystalWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestViewer
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            dlgBrowse.Title = "Select a Report Template.";
            dlgBrowse.InitialDirectory = ConfigurationManager.AppSettings.Get("INITIAL_DIR");
            dlgBrowse.Filter = "Report Templates (*.rpt)|*.rpt";
            dlgBrowse.FileName = this.txtReportTemplate.Text;
            dlgBrowse.ShowDialog(this);
            this.txtReportTemplate.Text = dlgBrowse.FileName;
        }

        private void RunReport(string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                try
                {
                    CrystalWrapper wrapper = new CrystalWrapper();

                    wrapper.DSN = ConfigurationManager.AppSettings.Get("DSN");
                    wrapper.UserId = ConfigurationManager.AppSettings.Get("USERNAME");
                    wrapper.Password = ConfigurationManager.AppSettings.Get("PASSWORD");
                    wrapper.ReportTemplate = template;
                    wrapper.ViewReport();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in tempalate: {template}: {ex.Message}");
                };
            }
        }

        private void RunAll(string folder, bool includeSubFolders)
        {
            foreach (var template in Directory.GetFiles(folder, "*.rpt"))
            {
                RunReport(template);
            }

            foreach (var subFolder in Directory.GetDirectories(folder))
            {
                RunAll(subFolder, includeSubFolders);
            }
        }

        private void cmdRun_Click(object sender, EventArgs e)
        {
            string template = this.txtReportTemplate.Text;
            RunReport(template);
        }

        private void cmdRunAll_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtReportTemplate.Text))
            {
                string folder = Path.GetDirectoryName(this.txtReportTemplate.Text);
                bool includeSubFolders = this.chkIncludeSubFolders.Checked;

                RunAll(folder, includeSubFolders);
            }
        }
    }
}
