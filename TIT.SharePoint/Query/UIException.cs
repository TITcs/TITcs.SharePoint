using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace TITcs.SharePoint.Query
{
    [Serializable]
    public class UIException : ApplicationException
    {
        public string LogicClass { get; set; }
        public string ControlClass { get; set; }
        public string ModelClass { get; set; }
        private Exception _exception = null;
        public UIException()
        {

        }
        private readonly Queue<SharePointExceptionInternal> _queueExceptions = new Queue<SharePointExceptionInternal>();
        public UIException(Exception exception, Dictionary<string, object> variables)
        {
            //var sharePointExceptionInternal = new SharePointExceptionInternal();
            //sharePointExceptionInternal.CurrentException = exception;
            //sharePointExceptionInternal.Variables = variables;
            //_queueExceptions.Enqueue(sharePointExceptionInternal);
            MessageULS();
        }
        public UIException(Dictionary<string, object> variables)
        {
            //var sharePointExceptionInternal = new SharePointExceptionInternal();
            //sharePointExceptionInternal.Variables = variables;
            //_queueExceptions.Enqueue(sharePointExceptionInternal);
            MessageULS();
        }
        public UIException(Exception exception)
            : base(exception.Message)
        {
            _exception = exception;
            //var sharePointExceptionInternal = new SharePointExceptionInternal();
            //sharePointExceptionInternal.CurrentException = exception;
            //_queueExceptions.Enqueue(sharePointExceptionInternal);
            MessageULS();
        }

        public UIException(string message)
            : this(message, null)
        {
            MessageULS();
        }

        public UIException(string message, params object[] args)
        {
            //var sharePointExceptionInternal = new SharePointExceptionInternal();
            //sharePointExceptionInternal.CurrentException = new Exception(string.Format(message, args));
            //_queueExceptions.Enqueue(sharePointExceptionInternal);
            MessageULS();

        }

        private void MessageULS()
        {
            string stackTrace = string.Empty;
            string source = string.Empty;
            if (_exception != null)
            {
                stackTrace = _exception.StackTrace;
                source = _exception.Source;

            }
            var message = string.Format("Logic:{0}, Control:{1}, Model:{2},Message:{3},StackTrace:{4},Source:{5}",
                LogicClass, ControlClass,
                ModelClass, Message, stackTrace, source);


            if (SPDiagnosticsService.Local != null)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                    new SPDiagnosticsCategory("T-IT Framework", TraceSeverity.Monitorable, EventSeverity.Error),
                    TraceSeverity.Monitorable,
                    message,
                    new object[] {"T-IT DATA"});
            }

        }

        class SharePointExceptionInternal
        {

            public Exception CurrentException { get; set; }
            public Dictionary<string, object> Variables { get; set; }

            internal SharePointExceptionInternal()
            {
                Variables = new Dictionary<string, object>();
            }

        }

        public void Render(HtmlTextWriter writer)
        {
            try
            {
                var message = string.Format("Logic:{0}, Control:{1}, Model:{2},Message:{3}", LogicClass, ControlClass,
                                ModelClass,Message);

                if (SPDiagnosticsService.Local != null)
                {
                    SPDiagnosticsService.Local.WriteTrace(0,
                        new SPDiagnosticsCategory("T-IT Framework", TraceSeverity.Monitorable, EventSeverity.Error),
                        TraceSeverity.Monitorable,
                        message,
                        new object[] {"T-IT DATA"});
                }

                writer.Write(message);
            }
            catch (Exception)
            {
                if (SPDiagnosticsService.Local != null)
                {
                    SPDiagnosticsService.Local.WriteTrace(0,
                        new SPDiagnosticsCategory("T-IT Framework", TraceSeverity.Monitorable, EventSeverity.Error),
                        TraceSeverity.Monitorable,
                        "Error ao escrever log",
                        new object[] {"T-IT DATA"});
                }
            }

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}