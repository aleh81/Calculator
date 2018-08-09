using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.WPF.Services
{
    public class DataChangedEventArgs : EventArgs
    {
        public string Value { get; set; }

        public DataChangedEventArgs(string value)
        {
            Value = value;
        }
    }
}
