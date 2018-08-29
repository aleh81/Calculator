using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Calculator.MVVM.Services;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace Calculator.MVVM.ViewModels
{
    public class ButtonViewModel 
    {
        public string Value { get; set; }
    }
}
