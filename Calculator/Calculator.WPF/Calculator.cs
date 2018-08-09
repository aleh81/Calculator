using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator.WPF.Services
{
    public static class Calculator
    {
        public static List<string> Count(string expression)
        {
            var pattern = @"([-+*/])";
            List<string> strList = new List<string>();
            
            foreach(Match m in Regex.Matches(expression, pattern))
            {
                strList.Add(m.Groups[1].Value);
            }

            
            return strList;
        }
    }
}
