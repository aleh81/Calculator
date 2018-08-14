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

namespace Calculator.MVVM.ViewModels
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public ICommand MyCommand { get; set; }
        public ICommand ButtonPressCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _expression;
        private string _nubersum;

        public ApplicationViewModel()
        {
            MyCommand = new RelayCommand(Execute, Canexecute);
            ButtonPressCommand = new RelayCommand(Execute2, CanExecute2);
        }

        private void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }      

        public string Expression
        {
            get => _expression;
            set { _expression = value; OnPropertyChanged("Expression"); }
        }

        public string NumberSum
        {
            get => _nubersum;
            set { _nubersum = value; OnPropertyChanged("NumberSum"); }
        }

        private bool Canexecute(object parameter)
        {
            if (Expression != null)
            {
                return true;
            }
            else { return false; }
        }

        private bool CanExecute2(object parametr)
        {
            return true;
        }

        private void Execute(object parameter)
        {
            NumberSum = _expression.ToString();
        }

        private void Execute2(object parametr)
        {
            string expression = (string) parametr;
            Expression += expression;
           // NumberSum = expression;
        }

        private string ToCount(string expression)
        {
            var pattern = @"[-+*/]";
            var signedPattern = @"\d+";
            double fnum;

            List<string> strList = new List<string>();

            string[] elements = Regex.Split(expression, pattern);

            var arrNumbers = new double[elements.Length];

            for (var i = 0; i < elements.Length; i++)
            {
                double.TryParse(elements[i], out arrNumbers[i]);
            }

            var arrOperators = Regex.Split(expression, signedPattern);

            var operatorsStr = arrOperators.Where(s => s != "").ToList();

            int b = 0;

            fnum = arrNumbers[0];

            while (b < operatorsStr.Count)
            {
                if (operatorsStr[b] == "+")
                {
                    fnum = fnum + arrNumbers[b + 1];
                    b++;
                }
                else if (operatorsStr[b] == "-")
                {
                    fnum = fnum - arrNumbers[b + 1];
                    b++;
                }
                else if (operatorsStr[b] == "*")
                {
                    fnum = fnum * arrNumbers[b + 1];
                    b++;
                }
                else if (operatorsStr[b] == "/")
                {
                    fnum = fnum / arrNumbers[b + 1];
                    b++;
                }
            }

            return fnum.ToString();
        }
    }
}

//https://www.c-sharpcorner.com/article/overview-of-multi-binding-in-mvvm-wpf/
