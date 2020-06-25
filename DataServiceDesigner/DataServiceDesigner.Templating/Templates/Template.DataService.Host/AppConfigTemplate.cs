﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace DataServiceDesigner.Templating.Templates.Template.DataService.Host
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\git\stuff\DataServiceDesigner\DataServiceDesigner.Templating\Templates\Template.DataService.Host\AppConfigTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class AppConfigTemplate : AppConfigTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <configSections>
    <section name=""log4net"" type=""log4net.Config.Log4NetConfigurationSectionHandler,log4net"" />
    <section name=""dataServiceConfiguration"" type=""BWF.DataServices.Core.Configuration.ConnectionSettingsSection, BWF.DataServices.Core"" />
  </configSections>
  <startup>
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.6.1"" />
  </startup>
  <connectionStrings>
    <clear />
");
            
            #line 19 "C:\git\stuff\DataServiceDesigner\DataServiceDesigner.Templating\Templates\Template.DataService.Host\AppConfigTemplate.tt"
 foreach(var dataService in DataServiceSolution.DataServices) {
            
            #line default
            #line hidden
            this.Write("    <add name=\"reporting\" connectionString=\"Data Source=localhost\\sqlexpress;Init" +
                    "ial Catalog=limits;Integrated Security=True;Pooling=False\" />\r\n");
            
            #line 21 "C:\git\stuff\DataServiceDesigner\DataServiceDesigner.Templating\Templates\Template.DataService.Host\AppConfigTemplate.tt"
} 
            
            #line default
            #line hidden
            this.Write(@"  </connectionStrings>
  <dataServiceConfiguration>
    <ConnectionSettings>
      <dataService name=""reporting"" connectionString=""reporting"" type=""SQLServer"" />
    </ConnectionSettings>
  </dataServiceConfiguration>
  <appSettings>
    <add key=""HostUrl"" value=""https://localhost:");
            
            #line 29 "C:\git\stuff\DataServiceDesigner\DataServiceDesigner.Templating\Templates\Template.DataService.Host\AppConfigTemplate.tt"
DataServiceSolution.DefaultPort
            
            #line default
            #line hidden
            this.Write("\" />\r\n    <add key=\"RedisConnection\" value=\"localhost:6379\" />\r\n  </appSettings>\r" +
                    "\n  <log4net debug=\"false\">\r\n    <logger name=\"BWF\" additivity=\"false\">\r\n      <l" +
                    "evel value=\"FATAL\" />\r\n    </logger>\r\n    <logger name=\"NHibernate\" additivity=\"" +
                    "false\">\r\n      <level value=\"DEBUG\" />\r\n    </logger>\r\n    <logger name=\"Topshel" +
                    "f\">\r\n      <level value=\"WARN\" />\r\n    </logger>\r\n    <logger name=\"OsDetector\">" +
                    "\r\n      <level value=\"WARN\" />\r\n    </logger>\r\n    <appender name=\"ColoredConsol" +
                    "eAppender\" type=\"log4net.Appender.ColoredConsoleAppender\">\r\n      <filter type=\"" +
                    "log4net.Filter.LevelRangeFilter\">\r\n        <levelMin value=\"DEBUG\" />\r\n        <" +
                    "levelMax value=\"FATAL\" />\r\n      </filter>\r\n      <mapping>\r\n        <level valu" +
                    "e=\"ERROR\" />\r\n        <foreColor value=\"White\" />\r\n        <backColor value=\"Red" +
                    ", HighIntensity\" />\r\n      </mapping>\r\n      <mapping>\r\n        <level value=\"DE" +
                    "BUG\" />\r\n        <foreColor value=\"Green\" />\r\n      </mapping>\r\n      <mapping>\r" +
                    "\n        <level value=\"INFO\" />\r\n        <foreColor value=\"Yellow\" />\r\n      </m" +
                    "apping>\r\n      <mapping>\r\n        <level value=\"WARN\" />\r\n        <foreColor val" +
                    "ue=\"CYAN\" />\r\n      </mapping>\r\n      <mapping>\r\n        <level value=\"FATAL\" />" +
                    "\r\n        <foreColor value=\"White\" />\r\n        <backColor value=\"Red, HighIntens" +
                    "ity\" />\r\n      </mapping>\r\n      <layout type=\"log4net.Layout.PatternLayout\">\r\n " +
                    "       <conversionPattern value=\"%date [%thread] %-5level %logger [%property{NDC" +
                    "}] - %message%newline\" />\r\n      </layout>\r\n    </appender>\r\n    <appender name=" +
                    "\"RollingFile\" type=\"log4net.Appender.RollingFileAppender\">\r\n      <file value=\"L" +
                    "ogs\\DataServiceHost.log\" />\r\n      <appendToFile value=\"true\" />\r\n      <rolling" +
                    "Style value=\"Composite\" />\r\n      <maxSizeRollBackups value=\"-1\" />\r\n      <maxi" +
                    "mumFileSize value=\"100MB\" />\r\n      <datePattern value=\"-dd-MMM-yyyy\" />\r\n      " +
                    "<staticLogFileName value=\"true\" />\r\n      <preserveLogFileNameExtension value=\"t" +
                    "rue\" />\r\n      <filter type=\"log4net.Filter.LevelRangeFilter\">\r\n        <levelMi" +
                    "n value=\"DEBUG\" />\r\n        <levelMax value=\"FATAL\" />\r\n      </filter>\r\n      <" +
                    "lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />\r\n      <layout " +
                    "type=\"log4net.Layout.PatternLayout\">\r\n        <conversionPattern value=\"%date %-" +
                    "5level - %message%newline\" />\r\n      </layout>\r\n    </appender>\r\n    <root>\r\n   " +
                    "   <level value=\"INFO\" />\r\n      <appender-ref ref=\"RollingFile\" />\r\n      <appe" +
                    "nder-ref ref=\"ColoredConsoleAppender\" />\r\n    </root>\r\n  </log4net>\r\n  <runtime>" +
                    "\r\n  </runtime>\r\n</configuration>");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\git\stuff\DataServiceDesigner\DataServiceDesigner.Templating\Templates\Template.DataService.Host\AppConfigTemplate.tt"

private global::DataServiceDesigner.Domain.DataServiceSolution _DataServiceSolutionField;

/// <summary>
/// Access the DataServiceSolution parameter of the template.
/// </summary>
private global::DataServiceDesigner.Domain.DataServiceSolution DataServiceSolution
{
    get
    {
        return this._DataServiceSolutionField;
    }
}


/// <summary>
/// Initialize the template
/// </summary>
public virtual void Initialize()
{
    if ((this.Errors.HasErrors == false))
    {
bool DataServiceSolutionValueAcquired = false;
if (this.Session.ContainsKey("DataServiceSolution"))
{
    this._DataServiceSolutionField = ((global::DataServiceDesigner.Domain.DataServiceSolution)(this.Session["DataServiceSolution"]));
    DataServiceSolutionValueAcquired = true;
}
if ((DataServiceSolutionValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("DataServiceSolution");
    if ((data != null))
    {
        this._DataServiceSolutionField = ((global::DataServiceDesigner.Domain.DataServiceSolution)(data));
    }
}


    }
}


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class AppConfigTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
