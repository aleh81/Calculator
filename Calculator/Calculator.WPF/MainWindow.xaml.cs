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
using System.Text.RegularExpressions;

namespace Calculator.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler<CounterChangedEventArgs> CounterChanged;

        bool eventOccured = false;
        public string mainValue;

        private static Mutex _instance;
        private const string AppName = "Calculator";

        public MainWindow()
        {
            _instance = new Mutex(true, AppName, out var tryCreateNewApp);

            if (tryCreateNewApp)
            {
                InitializeComponent();

                Thread.CurrentThread.Name = "MainThread";

                DoAsyncWork();

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
                MessageBox.Show($"Application {AppName} is already running");

                CloseApp();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = (string)((Button)e.OriginalSource).Content;

            TextBlock.Text += s;
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs arg)
        {
            var value = TextBlock.Text;
            if (value != "")
            {
                var lastSign = value[value.Length - 1];

                if (value.Contains("Off"))
                {
                    CloseApp();
                }
                else if (value.Contains("C"))
                {
                    TextBlock.Text = "";
                    ResultTextBlock.Text = "";
                }
                if (!(lastSign == '+' || lastSign == '-' || lastSign == '*' || lastSign == '/'))
                {
                    mainValue = TextBlock.Text;

                    eventOccured = true;
                }
            }
        }

        public void DoAsyncWork()
        {
            bool state = true;

            new Thread(new ThreadStart(() =>
            {
                CounterChanged += CounterEventHandler;

                while (state)
                {
                    Thread.Sleep(100);

                    if (eventOccured)
                    {
                        ToCount(mainValue);
                    }

                    eventOccured = false;
                }

            })).Start();
        }

        private void OnCounterChanged(string value)
        {
            CounterChanged?.Invoke(this, new CounterChangedEventArgs(value));
        }

        private void CounterEventHandler(object sender, CounterChangedEventArgs e)
        {
            var val = e.Value;

            this.Dispatcher.Invoke((ThreadStart)delegate { ResultTextBlock.Text = val; });
        }

        private void CloseApp()
        {
            var targetProcess = Process.GetCurrentProcess();
            targetProcess.CloseMainWindow();
            targetProcess.Close();

            Environment.Exit(0);
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

            //Event
            OnCounterChanged(fnum.ToString());
        }
    }
}
