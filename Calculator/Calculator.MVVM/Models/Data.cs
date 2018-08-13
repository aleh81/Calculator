using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Calculator.MVVM.Models
{
    public class Data
    {
        private string arithmeticExpression;

        public string ArithmeticExpression
        {
            get
            {
                return arithmeticExpression; 
            }
            set
            {
                arithmeticExpression = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if(PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
