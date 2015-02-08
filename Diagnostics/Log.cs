using System;
//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System.Diagnostics;
using System.Globalization;

namespace Diagnostics
{
    /// <summary>
    /// Implementation of <see cref="Log"/> for logging into the windows event system
    /// </summary>
    public sealed class Log
    {
        /// <summary>
        /// Name of the log
        /// </summary>
        public static string LogName = Logger;

        /// <summary>
        /// Name for logger
        /// </summary>
        private const string Logger = "Beejones";

        /// <summary>
        /// Log an item to the windows event logger
        /// </summary>
        /// <param name="level">Indicates error, warning, etc.</param>
        /// <param name="message">Message to log</param>
        /// <param name="pars">Additional parameters for message. Optional</param>
        public static void Item(EventLogEntryType level, string message, params object[] pars)
        {
            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = LogName;

            // Write an informational entry to the event log.    
            myLog.WriteEntry(string.Format(CultureInfo.CurrentCulture, message, pars), EventLogEntryType.Warning);
            Console.WriteLine(string.Format("{0}: {1}", level, message), pars);
        }
    }
}
