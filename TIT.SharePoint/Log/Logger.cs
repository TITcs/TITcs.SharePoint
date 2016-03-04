using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace TITcs.SharePoint.Log
{
    public class Logger
    {
        private static void WriteLog(LoggerCategory categoryName, string source, string errorMessage)
        {
            try
            {
                _Logger.WriteLog(categoryName, source, errorMessage);
            }
            catch (Exception e)
            {
                
            }
        }

        public static void Information(string source, string errorMessage)
        {
            WriteLog(LoggerCategory.Information, source, errorMessage);
        }

        public static void Information(string source, string errorMessage, params object[] parameters)
        {
            WriteLog(LoggerCategory.Information, source, string.Format(errorMessage, parameters));
        }

        public static void Unexpected(string source, string errorMessage)
        {
            WriteLog(LoggerCategory.Unexpected, source, errorMessage);
        }
    }

    public enum LoggerCategory
    {
        Unexpected,
        High,
        Medium,
        Information
    }

    public class _Logger : SPDiagnosticsServiceBase
    {
        public static string DiagnosticAreaName = "TIT Framework";
        private static _Logger _current;

        public static _Logger Current
        {
            get { return _current ?? (_current = new _Logger()); }
        }

        public _Logger()
            : base("TIT Logging", SPFarm.Local)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory("Unexpected", TraceSeverity.Unexpected, EventSeverity.Error),
                    new SPDiagnosticsCategory("High", TraceSeverity.High, EventSeverity.Warning),
                    new SPDiagnosticsCategory("Medium", TraceSeverity.Medium, EventSeverity.Information),
                    new SPDiagnosticsCategory("Information", TraceSeverity.Verbose, EventSeverity.Information)
                })
            };

            return areas;
        }

        public static void WriteLog(LoggerCategory categoryName, string source, string errorMessage)
        {
            SPDiagnosticsCategory category = Current.Areas[DiagnosticAreaName].Categories[categoryName.ToString()];
            Current.WriteTrace(0, category, category.TraceSeverity, string.Concat(source, ": ", errorMessage));
        }

        public static void Information(string source, string errorMessage)
        {
            WriteLog(LoggerCategory.Information, source, errorMessage);
        }

        public static void Information(string source, string errorMessage, params object[] parameters)
        {
            WriteLog(LoggerCategory.Information, source, string.Format(errorMessage, parameters));
        }

        public static void Unexpected(string source, string errorMessage)
        {
            WriteLog(LoggerCategory.Unexpected, source, errorMessage);
        }
    }
}
