using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Calculator.MVVM.WPF.Models;
using Calculator.MVVM.WPF.Commands;
using System.Collections.ObjectModel;

namespace Calculator.MVVM.WPF.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Phone selectedPhone;
        private Data targetData;

        public ObservableCollection<Phone> Phones { get; set; }

        //private RelayCommand addCommand;

        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return addCommand ??
        //            (addCommand = new RelayCommand(obj =>
        //            {
        //                var phone = new Phone();
        //                Phones.Insert(0, phone);
        //                SelectedPhone = phone;
        //            }));
        //    }
        //}

        public Data TargetData
        {
            get
            {
                return targetData;
            }
            set
            {
                targetData = value;
                OnPropertyChanged("TargetData");
            }
        }

        public Phone SelectedPhone
        {
            get
            {
                return selectedPhone;
            }
            set
            {
                selectedPhone = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public ApplicationViewModel()
        {
            Phones = new ObservableCollection<Phone>
            {
                new Phone { Title="iPhone 7", Company="Apple", Price=56000 },
                new Phone {Title="Galaxy S7 Edge", Company="Samsung", Price =60000 },
                new Phone {Title="Elite x3", Company="HP", Price=56000 },
                new Phone {Title="Mi5S", Company="Xiaomi", Price=35000 }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
