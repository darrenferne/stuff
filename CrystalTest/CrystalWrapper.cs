using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class CrystalWrapper
    {
        private bool _batchMode;
        private string _destinationFileName;
        private ReportFormat _destinationFormat;
        private ReportDestination _destinationType;
        private DisplayFlag _displayFlags;
        private string _DSN;
        private bool _isValid;
        private bool _loggedOn;
        private string _mailCCList;
        private string _mailMessage;
        private string _mailSubject;
        private string _mailToList;
        private bool _printCollate;
        private int _printCopies;
        private bool _printDuplex;
        private string _printerName;
        private PrintOrientation _printOrientation;
        private string _password;
        private string _reportTemplate;
        private string _reportTitle;
        private string _selectionFormula;
        private SelectionFormulaType _selectionFormulaType;
        private string _userId;
        private ReportParameters _parameters;

        public CrystalWrapper()
        {
            _parameters = new ReportParameters();
        }

        public bool BatchMode
        {
            get { return _batchMode; }
            set { _batchMode = value; }
        }
        
        public string DestinationFileName
        {
            get { return _destinationFileName; }
            set { _destinationFileName = value; }
        }
        
        public ReportFormat DestinationFormat
        {
            get { return _destinationFormat; }
            set { _destinationFormat = value; }
        }
        
        public ReportDestination DestinationType
        {
            get { return _destinationType; }
            set { _destinationType = value; }
        }
        
        public DisplayFlag DisplayFlags
        {
            get { return _displayFlags; }
            set { _displayFlags = value; }
        }

        public string DSN
        {
            get { return _DSN; }
            set { _DSN = value; }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }

        public bool LoggedOn
        {
            get { return _loggedOn; }
            set { _loggedOn = value; }
        }

        public string MailCCList
        {
            get { return _mailCCList; }
            set { _mailCCList = value; }
        }

        public string MailMessage
        {
            get { return _mailMessage; }
            set { _mailMessage = value; }
        }

        public string MailSubject
        {
            get { return _mailSubject; }
            set { _mailSubject = value; }
        }

        public string MailToList
        {
            get { return _mailToList; }
            set { _mailToList = value; }
        }

        public ReportParameters Parameters
        {
            get { return _parameters; }
        }

        public bool PrintCollate
        {
            get { return _printCollate; }
            set { _printCollate = value; }
        }

        public int PrintCopies
        {
            get { return _printCopies; }
            set { _printCopies = value; }
        }

        public bool PrintDuplex
        {
            get { return _printDuplex; }
            set { _printDuplex = value; }
        }

        public string PrinterName
        {
            get { return _printerName; }
            set { _printerName = value; }
        }

        public PrintOrientation PrintOrientation
        {
            get { return _printOrientation; }
            set { _printOrientation = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string ReportTemplate
        {
            get { return _reportTemplate; }
            set { _reportTemplate = value; }
        }

        public string ReportTitle
        {
            get { return _reportTitle; }
            set { _reportTitle = value; }
        }

        public string SelectionFormula
        {
            get { return _selectionFormula; }
            set { _selectionFormula = value; }
        }

        public SelectionFormulaType SelectionFormulaType
        {
            get { return _selectionFormulaType; }
            set { _selectionFormulaType = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private ReportDocument OpenReport(string template)
        {
            if (string.IsNullOrEmpty(template))
                throw new CrystalWrapperException("Report template required");

            ReportDocument report = new ReportDocument();
            string filePath = Path.GetFullPath(template);

            report.Load(filePath);

            return report;
        }

        private void SetReportConnections(ReportDocument report)
        {
            //if (logonInfo == null)
            //{
            //    logonInfo = new TableLogOnInfo();
            //    logonInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("DSN", _DSN));
            //    logonInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("User ID", _userId));
            //    logonInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("Password", _password));
            //    logonInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("UseDSNProperties", true));
            //    logonInfo.ConnectionInfo.AllowCustomConnection = true;
            //}
            
           
            foreach (Table table in report.Database.Tables)
            {
                if (table.LogOnInfo.ConnectionInfo.LogonProperties.ContainsKey("DSN"))
                {
                    TableLogOnInfo logOnInfo = table.LogOnInfo.Clone() as TableLogOnInfo;

                    logOnInfo.ConnectionInfo.AllowCustomConnection = true;
                    logOnInfo.ConnectionInfo.ServerName = string.Empty;
                    logOnInfo.ConnectionInfo.DatabaseName = string.Empty;
                    logOnInfo.ConnectionInfo.UserID = string.Empty;
                    logOnInfo.ConnectionInfo.Password = string.Empty;
                    logOnInfo.ConnectionInfo.LogonProperties.Clear();
                    logOnInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("DSN", _DSN));
                    logOnInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("User ID", _userId));
                    logOnInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("Password", _password));
                    logOnInfo.ConnectionInfo.LogonProperties.Add(new NameValuePair2("UseDSNProperties", true));
                    
                    table.ApplyLogOnInfo(logOnInfo);
                }
            }

            if (!report.IsSubreport)
            {
                foreach (ReportDocument subReport in report.Subreports)
                {
                    SetReportConnections(subReport);
                }
            }
        }

        private bool ValidateSelectionFormula(ReportDocument report)
        {
            return true;
        }

        private void ToggleParameterPrompting(ReportDocument report, bool toggle)
        {
            if (!report.IsSubreport)
            {
                if (report.ParameterFields.Count > 0)
                {
                    foreach (ReportDocument subReport in report.Subreports)
                        ToggleParameterPrompting(subReport, toggle);
                }
            }
        }

        private void SetReportParameters(ReportDocument report)
        {
            foreach (ParameterField crParameter in report.ParameterFields)
            {
                if (crParameter.ReportParameterType == ParameterType.ReportParameter)
                {
                    ReportParameter trParameter = _parameters.FirstOrDefault(p => string.Compare(p.Name, crParameter.Name, true) == 0);
                    if (trParameter != null)
                    {
                        bool ok = false;
                        switch (crParameter.ParameterValueType)
                        {
                            case ParameterValueKind.StringParameter:
                                ok = trParameter.ParameterType == ReportParameterType.String;
                                break;
                            case ParameterValueKind.NumberParameter:
                            case ParameterValueKind.CurrencyParameter:
                                ok = trParameter.ParameterType == ReportParameterType.Number;
                                break;
                            case ParameterValueKind.DateParameter:
                            case ParameterValueKind.DateTimeParameter:
                            case ParameterValueKind.TimeParameter:
                                ok = trParameter.ParameterType == ReportParameterType.Date;
                                break;
                            case ParameterValueKind.BooleanParameter:
                                ok = trParameter.ParameterType == ReportParameterType.Boolean;
                                break;
                        }

                        if (ok)
                        {
                            if (trParameter.ValueCount > 1 && crParameter.EnableAllowMultipleValue)
                            {
                                for(int i = 0; i < trParameter.ValueCount; i++)
                                    crParameter.CurrentValues.Add(trParameter.GetValue(i));
                            }
                            else
                            {
                                crParameter.CurrentValues.Add(trParameter.GetValue(0));
                            }
                        }
                    }
                }
            }
        }

        private void SetSelectionFormula(ReportDocument report)
        {
            if (_selectionFormulaType != SelectionFormulaType.None && !string.IsNullOrEmpty(_selectionFormula))
            {
                if (_selectionFormulaType == SelectionFormulaType.Crystal)
                {
                    report.RecordSelectionFormula = _selectionFormula;
                }
                else
                {
                    //TODO
                }
            }
        }

        private ReportDocument SetupAndValidateReport()
        {
            if (_destinationType == ReportDestination.Email)
            {
                if (string.IsNullOrEmpty(_mailToList) && string.IsNullOrEmpty(_mailCCList))
                    throw new CrystalWrapperException("No Mail recipients specified");
            }
            else if (_destinationType == ReportDestination.Disk)
            {
                if (string.IsNullOrEmpty(_destinationFileName))
                    throw new CrystalWrapperException("No destination filename specified");

                if (_destinationFormat == ReportFormat.None)
                    throw new CrystalWrapperException("No destination format specified");
            }

            ReportDocument report = OpenReport(_reportTemplate);

            if (_batchMode)
            {
                ToggleParameterPrompting(report, false);
            }

            SetReportConnections(report);

            if (ValidateSelectionFormula(report))
            {
                SetSelectionFormula(report);

                if (!_batchMode)
                    ToggleParameterPrompting(report, true);

                SetReportParameters(report);
            }

            return report;
        }

        private string GetSQLQuery(ReportDocument report)
        {
            //report.ReportClientDocument.Database.Tables[0]

            CrystalDecisions.ReportAppServer.DataDefModel.ISCRGroupPath gp = new CrystalDecisions.ReportAppServer.DataDefModel.GroupPath();

            string reserved;
            var s = report.ReportClientDocument.RowsetController.GetSQLStatement(gp, out reserved);

            return string.Empty;
        }

        public void ExportReport(ReportDocument report)
        {
            ExportOptions options = new ExportOptions();

            options.ExportDestinationType = (ExportDestinationType)_destinationType;
            options.ExportFormatType = (ExportFormatType)_destinationFormat;

            if (_destinationType == ReportDestination.Disk)
            {
                DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();
                diskOptions.DiskFileName = _destinationFileName;

                options.ExportDestinationOptions = diskOptions;
                
            }
            else if (_destinationType == ReportDestination.Disk)
            {
                MicrosoftMailDestinationOptions mailOptions = new MicrosoftMailDestinationOptions();
                mailOptions.MailSubject = _mailSubject;
                mailOptions.MailMessage = _mailMessage;
                mailOptions.MailToList = _mailToList;
                mailOptions.MailCCList = _mailCCList;

                options.ExportDestinationOptions = mailOptions;
            }

            report.Export(options);
            
        }

        public void ExportReport(bool preview, bool print)
        {
            ReportDocument report = SetupAndValidateReport();

            if (preview)
                ViewReport(report);

            if (print)
                PrintReport(report);

            ExportReport(report);
        }

        public string[] GetCounterpartyList(string reportField, bool applyGrouping, bool forConfirmation)
        {
            return null;
        }

        public string GetDefaultExtension(ReportFormat format)
        {
            switch (format)
            {
                case ReportFormat.XML:
                    return "XML";
                case ReportFormat.Word:
                    return "DOC";
                case ReportFormat.TSV:
                    return "TSV";
                case ReportFormat.Text:
                    return "TXT";
                case ReportFormat.RTF:
                    return "RTF";
                case ReportFormat.RPT:
                    return "RPT";
                case ReportFormat.PDF:
                    return "PDF";
                case ReportFormat.HTML32:
                case ReportFormat.HTML40:
                    return "HTML";
                case ReportFormat.Excel:
                    return "XLS";
                case ReportFormat.CSV:
                    return "CSV";
                default:
                    return string.Empty;
            }
        }

        public string[] GetDetailFields(string template)
        {
            return null;
        }

        public ReportField[] GetReportFields(string template)
        {
            return null;
        }

        public ReportField[] GetReportTableFields(string template, string table)
        {
            return null;
        }

        public string[] GetReportTables(string template)
        {
            return null;
        }

        public void Logon()
        { }

        public void PrintReport(ReportDocument report)
        {
            report.PrintOptions.PrinterName = _printerName;
            report.PrintOptions.PaperOrientation = (PaperOrientation)_printOrientation;
            if (_printDuplex)
                report.PrintOptions.PrinterDuplex = PrinterDuplex.Default;

            report.PrintToPrinter(_printCopies, _printCollate, 0, 0);
        }

        public void PrintReport(bool preview)
        {
            ReportDocument report = SetupAndValidateReport();

            if (preview)
                ViewReport(report);

            PrintReport(report);
        }

        public bool ValidateReportTemplate(string template)
        {
            return false;
        }

        public bool ValidateSelectionFormula(string template, SelectionFormulaType formulaType, string formula)
        {
            return false;
        }

        public void ViewReport(ReportDocument report)
        {
            ViewerForm viewer = new ViewerForm();
            
            viewer.ReportSource = report;
            viewer.ShowDialog();
        }

        public void ViewReport()
        {
            ReportDocument report = SetupAndValidateReport();

            var s = GetSQLQuery(report);

            ViewReport(report);
        }
    }
}
