﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator.WPF.Services
{
    public static class Parser
    {
        public static string Count(string expression)
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

            if(arrNumbers.Length < 2)
            {
                return null;
            }

            var arrOperators = Regex.Split(expression, signedPattern);

            var operatorsStr = arrOperators.Where(s => s != "").ToList();

            if(operatorsStr.Count() < 1)
            {
                return null;
            }

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

            return fnum.ToString();
        }
    }
}