using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Calculator.MVVM.Services
{
    public static class AppService
    {
        public static void CloseApp()
        {
            var targetProcess = Process.GetCurrentProcess();
            targetProcess.CloseMainWindow();
            targetProcess.Close();

            Environment.Exit(0);
        }
    }
}
