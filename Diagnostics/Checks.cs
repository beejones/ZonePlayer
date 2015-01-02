//---------------------------------------------------------------
// The MIT License. Beejones 
//---------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Globalization;

namespace Diagnostics
{
    /// <summary>
    /// Implementation of <see cref="Checks"/> for checking values of variables and throw exceptions
    /// </summary>
    public sealed class Checks
    {
        /// <summary>
        /// Check a varialbe for null
        /// </summary>
        /// <param name="varName">Name of the variable under test</param>
        /// <param name="var">Variable of type T</param>
        public static T NotNull<T>(string varName, T var)
        {
            if (var == null)
            {
                throw new ArgumentNullException(varName);
            }

            return var;
        }

        /// <summary>
        /// Check a string for being null or whitespace
        /// </summary>
        /// <param name="varName">Name of the variable under test</param>
        /// <param name="var">Variable of type T</param>
        public static string IsNullOrWhiteSpace(string varName, string var)
        {
            if (string.IsNullOrWhiteSpace(var))
            {
                throw new ArgumentNullException(varName);
            }

            return var;
        }
    }
}
