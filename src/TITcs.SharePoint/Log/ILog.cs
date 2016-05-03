
using System;

namespace TITcs.SharePoint.Log
{
    public interface ILog
    {
        void Error(string source, string message);
        void Error(string source, Exception ex);
        void Error(Exception ex);
        void Info(string source, string message);
        void Info(string source);
    }
}
