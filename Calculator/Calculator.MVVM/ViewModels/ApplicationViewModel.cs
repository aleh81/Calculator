﻿using System;
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

namespace Calculator.MVVM.ViewModels
{
    //hello world
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private bool eventOccured = false;
        private string _expression;
        private string _nubersum;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ButtonPressCommand { get; set; }

        public ApplicationViewModel()
        {
            ButtonPressCommand = new RelayCommand(ExpressionExecute, ExpressionCanExecute);

            DoAsyncWork();
        }

        private void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;

                if (value != "")
                {
                    var lastSign = value[value.Length - 1];

                    if (value.Contains("Off"))
                    {
                        AppService.CloseApp();
                    }
                    else if (value.Contains("C"))
                    {
                        NumberSum = "";
                        OnPropertyChanged("NumberSum");
                        Expression = "";
                    }
                    else if (value.Contains("->"))
                    {
                        var targetValue = Expression;
                        targetValue.Remove(targetValue.Length - 1);
                        Expression = targetValue;
                    }
                    if (!(lastSign == '+' || lastSign == '-' || lastSign == '*' || lastSign == '/'))
                    {
                        eventOccured = true;
                    }

                    OnPropertyChanged("Expression");
                }
            }
        }

        public string NumberSum
        {
            get => _nubersum;
            set { _nubersum = value; OnPropertyChanged("NumberSum"); }
        }

        private bool ExpressionCanExecute(object parametr)
        {
            var value = (string)parametr;

            return true;
        }

        private void ExpressionExecute(object parametr)
        {
            string expression = (string)parametr;
            Expression += expression;
        }

        private void DoAsyncWork()
        {
            bool state = true;

            new Thread(new ThreadStart(() =>
            {
                while (state)
                {
                    Thread.Sleep(50);

                    if (eventOccured)
                    {
                        ToCount(Expression);
                    }

                    eventOccured = false;
                }

            })).Start();
        }

        private void ToCount(string expression)
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

            NumberSum = fnum.ToString();
        }
    }

}

