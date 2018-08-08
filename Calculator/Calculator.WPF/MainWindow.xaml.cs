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
        string leftop = ""; 
        string operation = ""; 
        string rightop = ""; 

        private static Mutex _instance;
        private const string _appName = "Calculator";

        public MainWindow()
        {
            bool tryCreateNewApp;

            _instance = new Mutex(true, _appName, out tryCreateNewApp);

            if (tryCreateNewApp)
            {
                InitializeComponent();

                foreach (UIElement el in Root.Children)
                {
                    if (el is Button)
                    {
                        ((Button)el).Click += Button_Click;
                    }
                }
            }
            else
            {
                MessageBox.Show($"Приложение {_appName} уже запущено");

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
            int num;

            bool result = Int32.TryParse(s, out num);

            if (result == true)
            {
                if (operation == "")
                {
                    leftop += s;
                }
                else
                {
                    rightop += s;
                }
            }
            else
            {
                if (s == "=")
                {
                    Update_RightOp();
                    TextBlock.Text += rightop;
                    operation = "";
                }
                else if (s == "C")
                {
                    leftop = "";
                    rightop = "";
                    operation = "";
                    TextBlock.Text = "";
                }
                else
                {
                    if (rightop != "")
                    {
                        Update_RightOp();
                        leftop = rightop;
                        rightop = "";
                    }
                    operation = s;
                }
            }
        }

        private void Update_RightOp()
        {
            int num1 = Int32.Parse(leftop);
            int num2 = Int32.Parse(rightop);

            switch (operation)
            {
                case "+":
                    rightop = (num1 + num2).ToString();
                    break;
                case "-":
                    rightop = (num1 - num2).ToString();
                    break;
                case "*":
                    rightop = (num1 * num2).ToString();
                    break;
                case "/":
                    rightop = (num1 / num2).ToString();
                    break;
            }
        }

        private void Clear()
        {

        }

        private static void Display(TextBlock tb, string rightOperand)
        {
            tb.Text = rightOperand;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
