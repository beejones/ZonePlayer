using Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZonePlayerInterface
{
    /// <summary>
    /// Implementation of <see cref="ZonePlayerInterfaceHelpers"/> to provide helpers for building interfaces
    /// </summary>    
    public class ZonePlayerInterfaceHelpers
    {
        /// <summary>
        /// Decode the command passed on the command line interface
        /// </summary>
        /// <param name="argCommand"></param>
        /// <returns>The command converted into <see cref="Commands"/>. Null if commabd is not recognized</returns>
        public static Commands? GetCommand(string argCommand)
        {
            string command = Checks.IsNullOrWhiteSpace("argCommand", argCommand).ToLower();
            foreach (var c in GetCommands())
            {
                if (c.ToLower().CompareTo(command) == 0)
                {
                    Commands cmd;
                    if (Enum.TryParse(c, out cmd))
                    {
                        return cmd;
                    }

                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Return list of all commands
        /// </summary>
        /// <returns>List of all commands</returns>
        public static List<string> GetCommands()
        {
            List<string> commands = Enum.GetValues(typeof(Commands)).Cast<Commands>().Select(v => v.ToString()).ToList();
            return commands;            
        }
    }
}
