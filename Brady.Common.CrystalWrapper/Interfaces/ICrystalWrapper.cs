using CrystalDecisions.CrystalReports.Engine;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    [Guid("4314EC1D-A530-4610-B6EC-D74BC0DDAA96")]
    public interface ICrystalWrapper
    {
        bool BatchMode { get; set; }
        string DestinationFileName { get; set; }
        ReportFormat DestinationFormat { get; set; }
        ReportDestination DestinationType { get; set; }
        DisplayFlag DisplayFlags { get; set; }
        string DSN { get; set; }
        bool IsValid { get; set; }
        bool LoggedOn { get; set; }
        string MailCCList { get; set; }
        string MailMessage { get; set; }
        string MailSubject { get; set; }
        string MailToList { get; set; }
        ReportParameters Parameters { get; }
        string Password { get; set; }
        bool PrintCollate { get; set; }
        int PrintCopies { get; set; }
        bool PrintDuplex { get; set; }
        string PrinterName { get; set; }
        PrintOrientation PrintOrientation { get; set; }
        string ReportTemplate { get; set; }
        string ReportTitle { get; set; }
        string SelectionFormula { get; set; }
        SelectionFormulaType SelectionFormulaType { get; set; }
        string UserId { get; set; }

        void ExportReport(bool preview, bool print);
        void ExportReport(ReportDocument report);
        string[] GetCounterpartyList(string reportField, bool applyGrouping, bool forConfirmation);
        string GetDefaultExtension(ReportFormat format);
        string[] GetDetailFields(string template);
        ReportField[] GetReportFields(string template);
        ReportField[] GetReportTableFields(string template, string table);
        string[] GetReportTables(string template);
        void Logon();
        void PrintReport(bool preview);
        void PrintReport(ReportDocument report);
        bool ValidateReportTemplate(string template);
        bool ValidateSelectionFormula(string template, SelectionFormulaType formulaType, string formula);
        void ViewReport();
        void ViewReport(ReportDocument report);
    }
}