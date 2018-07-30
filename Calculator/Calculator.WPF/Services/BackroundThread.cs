using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Calculator.WPF.Services
{
    public class BackroundThread
    {
        TextBlock _textBlock;

        public delegate void StateHandler( string result);

        public event StateHandler Display;

        public BackroundThread(TextBlock textBlock)
        {
            _textBlock = textBlock;
        }

        public void SetResult(string res)
        {
            if(Display != null)
            {
                Display(_text);
            }
        }
    }
}
