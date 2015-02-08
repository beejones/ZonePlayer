using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZonePlayer
{
    /// <summary>
    /// Implementation of <see cref="CanNotPlayItemException.cs"/>
    /// </summary>    
    public class CanNotPlayItemException : Exception
    {
        public CanNotPlayItemException(string message, Uri item) : base(string.Format("{0} - {1}", message, item.ToString()))
        {
        }
    }
}
