// File Name:     Program.cs
// By:            Saidi Tarik
// Date:          16, 09, 2022

using ConnectFour.Forms;
using System;
using System.Windows.Forms;

namespace ConnectFour
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StarForm());
        }
    }
}