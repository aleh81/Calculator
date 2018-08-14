using System.Windows;
using System.Windows.Input;
using Calculator.MVVM.Services;
using System.Threading;

namespace Calculator.MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Mutex _instance;
        private const string AppName = "Calculator";

        public MainWindow()
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
