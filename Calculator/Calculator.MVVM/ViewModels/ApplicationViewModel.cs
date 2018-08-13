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

namespace Calculator.MVVM.ViewModels
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public ICommand MyCommand { get; set; }
        public ICommand ButtonPressCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _expression;
        private string _nubersum;


        private void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }      

        public int Expression
        {
            get => _expression;
            set { _expression = value; OnPropertyChanged("Expression"); }
        }

        public string NumberSum
        {
            get => _nubersum;
            set { _nubersum = value; OnPropertyChanged("NumberSum"); }
        }


        public ApplicationViewModel()
        {
            MyCommand = new RelayCommand(Execute, Canexecute);
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
            NumberSum = expression;
        }
    }
}

//https://www.c-sharpcorner.com/article/overview-of-multi-binding-in-mvvm-wpf/
