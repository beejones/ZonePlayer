using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote
{
    public class CommandListItem
    {
        public CommandListItem(string command)
        {
            this._name = command;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        private string _name;
    }
}
