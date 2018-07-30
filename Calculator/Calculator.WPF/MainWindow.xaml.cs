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
        private string operation = null;
        private string leftOperand = null;
        private string rightOperand = null;

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
            var value = (string)((Button)e.OriginalSource).Content;

            TextBlock.Text += value;

            bool result = int.TryParse(value, out var num);

            if (result == true)
            {
                if (operation == "")
                {
                    leftOperand += value;
                }
                else
                {
                    rightOperand += value;
                }
            }
            else
            {
                if (value == "=")
                {
                    Update_RightOperation();

                    TextBlock.Text += rightOperand;
                    operation = "";
                }
                else if (value == "C")
                {
                    Clear();
                }
            }
        }

        private void Update_RightOperation()
        {
            int.TryParse(leftOperand, out var leftNum);
            int.TryParse(rightOperand, out var rightNum);

            switch (operation)
            {
                case "+":
                    {
                        rightOperand = (leftNum + rightNum).ToString();
                       
                    }
                    break;
                case "-":
                    rightOperand = (leftNum - rightNum).ToString();
                    break;
                case "*":
                    rightOperand = (leftNum * rightNum).ToString();
                    break;
                case "/":
                    rightOperand = (leftNum / rightNum).ToString();
                    break;
            }
        }

        private void Clear()
        {
            leftOperand = "";
            rightOperand = "";
            operation = "";
            TextBlock.Text = "";
        }

        private static void Display(TextBlock tb, string rightOperand)
        {
            tb.Text = rightOperand;
        }
    }
}
