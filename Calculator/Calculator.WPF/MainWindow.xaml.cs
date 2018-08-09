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

        Thread thread;
        Parser parser;

        private static Mutex _instance;
        private const string AppName = "Calculator";

        public MainWindow()
        {
            _instance = new Mutex(true, AppName, out var tryCreateNewApp);

            parser = new Parser();
            parser.CounterChanged += CounterEventHandler;

            thread = new Thread(new ParameterizedThreadStart(parser.Count));
            thread.Start("1+2");

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
                    case "C":
                        _leftop = "";
                        _rightop = "";
                        _operation = "";
                        TextBlock.Text = "";
                        ResultTextBlock.Text = "";
                        break;
                    case "Off":
                       // MessageBox.Show("Close");
                        var targetProcess = Process.GetCurrentProcess();

                        targetProcess.CloseMainWindow();
                        targetProcess.Close();

                        Environment.Exit(0);
                        break;
                }
            }
        }



        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs arg)
        {
            //ResultTextBlock.Text = "";
            //var text = TextBlock.Text;
            //var parser = new Parser();

            //var task = new Task<string>(() => parser.Count(text));

            //task.Start();

            //task.Wait();

            //var value = task.Result;

            //if (value != null)
            //{
            //    ResultTextBlock.Text = value;
            //}
        }

        

        private void CounterEventHandler(object sender, CounterChangedEventArgs e)
        {
            ResultTextBlock.Text = e.Value;
        }
    }
}
