using System.Windows;
using System.Threading;
using System.Windows.Input;
using Calculator.MVVM.Services;

namespace Calculator.MVVM.Views
{
    public partial class CalculatorView : Window
    {
        private static Mutex _instance;
        private const string AppName = "Calculator";

        public CalculatorView()
        {
            _instance = new Mutex(true, AppName, out var tryCreateNewApp);

            if (tryCreateNewApp)
            {
                InitializeComponent();
            }
            else
            {
                MessageBox.Show($"Application {AppName} is already running");

                AppService.CloseApp();
            }
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
