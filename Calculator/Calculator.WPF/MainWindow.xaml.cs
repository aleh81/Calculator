using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Calculator.WPF.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Calculator.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _leftop = ""; 
        string _operation = ""; 
        string _rightop = ""; 

        private static Mutex _instance;
        private const string AppName = "Calculator";

        public MainWindow()
        {
            _instance = new Mutex(true, AppName, out var tryCreateNewApp);

            if (tryCreateNewApp)
            {
                InitializeComponent();

                foreach (UIElement el in Root.Children)
                {
                    if (el is Button button)
                    {
                        button.Click += Button_Click;
                    }
                }
            }
            else
            {
                MessageBox.Show($"Приложение {AppName} уже запущено");

                var targetProcess = Process.GetCurrentProcess();

                targetProcess.CloseMainWindow();
                targetProcess.Close();

                Environment.Exit(0);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string s = (string)((Button)e.OriginalSource).Content;

            TextBlock.Text += s;

            var result = double.TryParse(s, out var num);

            if (result)
            {
                if (_operation == "")
                {
                    _leftop += s;
                }
                else
                {
                    _rightop += s;
                }
            }
            else
            {
                switch (s)
                {
                    case "=":
                        Update_RightOp();
                        TextBlock.Text = "";
                        TextBlock.Text += _rightop;
                        _operation = "";
                        break;
                    case "C":
                        _leftop = "";
                        _rightop = "";
                        _operation = "";
                        TextBlock.Text = "";
                        break;
                    default:
                        if (_rightop != "")
                        {
                            Update_RightOp();
                            _leftop = _rightop;
                            _rightop = "";
                        }
                        _operation = s;
                        break;
                }
            }
        }

        private void Update_RightOp()
        {
            double.TryParse(_leftop, out var num1);
            double.TryParse(_rightop, out var num2);

            switch (_operation)
            {
                case "+":
                    _rightop = (num1 + num2).ToString();
                    break;
                case "-":
                    _rightop = (num1 - num2).ToString();
                    break;
                case "*":
                    _rightop = (num1 * num2).ToString();
                    break;
                case "/":
                    _rightop = (num1 / num2).ToString();
                    break;
            }
        }

        private void Clear()
        {

        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
