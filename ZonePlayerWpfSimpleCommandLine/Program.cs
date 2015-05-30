using Diagnostics;
//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZonePlayerInterface;

namespace ZonePlayerWpfSimpleCommandLine
{
    /// <summary>
    /// Implementation of <see cref="Program"/> for the providing a remote interface as a command line interface to zone player
    /// Usage:
    /// ZonePlayerWpfSimpleCommandLine <command> <zonename> [argument]
    ///     play Zone1 : Start playing zone1
    ///     stop Zone1 : Stop playing Zone1
    ///     next Zone1 : Play next item in playlist for Zone1
    ///     volget Zone1 : Get the current volume of Zone1
    ///     volset Zone1 10 : Set the current volume of Zone1 to 10
    ///     volup Zone1 : Increase the current volume of Zone1 with one unit
    ///     voldown Zone1 : Decrease the current volume of Zone1 with one unit
    /// </summary>    

    class Program
    {
        /// <summary>
        /// Main entry program
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            // Get command from commandline
            Commands? command = ZonePlayerInterfaceHelpers.GetCommand(args[0]);
            if (command == null)
            {
                string message = string.Format("Command '{0}' not recognized", args[0]);
                ShowMessage(message);
                ShowUsage();
                return;
            }

            // Connect to interface
            Uri endpoint = null;
            if (!Uri.TryCreate(Properties.Settings.Default.ZonePlayerInterface, UriKind.Absolute, out endpoint))
            {
                string message = string.Format("Invalid uri'{0}' in configuration", Properties.Settings.Default.ZonePlayerInterface);
                ShowMessage(message);
                return;
            }

            IZonePlayerService client = Connect<IZonePlayerService>.GetClient(endpoint);
            try
            {
                Task<string> task = client.Remote(command.ToString(), args[1], args.Length > 2 ? args[2] : null);
                string result = task.Result;
            }
            catch 
            {
                string message = string.Format("Can't connect to ZonePlayerWpf. Make sure the App is running");
                ShowMessage(message);
                return;
            }
        }

        /// <summary>
        /// Show message on console
        /// </summary>
        /// <param name="message"></param>
        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Show help how to use the command line interface
        /// </summary>
        private static void ShowUsage()
        {
            string usage = @"    
               Usage:
               ZonePlayerWpfSimpleCommandLine <command> <zonename> [argument]
                   play Zone1 : Start playing Zone1
                   stop Zone1 : Stop playing Zone1
                   next Zone1 : Play next item in playlist for Zone1
                   volget Zone1 : Get the current volume of Zone1
                   volset Zone1 10 : Set the current volume of Zone1 to 10
                   volup Zone1 : Increase the current volume of Zone1 with one unit
                   voldown Zone1 : Decrease the current volume of Zone1 with one unit
               ";

           ShowMessage(usage);
        }
    }
}
