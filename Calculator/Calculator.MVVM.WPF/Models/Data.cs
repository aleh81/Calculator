﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.MVVM.WPF.Models
{
    public class Data : INotifyPropertyChanged
    {
        private string expression;

        public string Expression
        {
            get
            {
                return expression;
            }
            set
            {
                expression = value;
                OnPropertyChanged("Expressioin");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
