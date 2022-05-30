using System;
using System.Diagnostics;
// using System.Numerics;
using System.Windows.Forms;
using ParallelPipesIntervals.Core;

namespace ParallelPipesIntervals
{
    static class Program
    {
        public static MainForm mainForm;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new MainForm();
            Application.Run(mainForm);
        }
    }
}
