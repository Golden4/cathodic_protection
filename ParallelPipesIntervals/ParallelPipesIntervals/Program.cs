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
            // var vec = new Vector3<IntervalDouble>(new IntervalDouble(1, 2), new IntervalDouble(5, 10), new IntervalDouble(45, 60));
            // var vec1 = new Vector3<IntervalDouble>(new IntervalDouble(5, 2), new IntervalDouble(50, 60), new IntervalDouble() );
            // var sss = vec - vec1;
            // var ss1 = vec * vec1;
            // var ss2 = vec / vec1;
            // var ss3 = vec * vec1;
            // var sassa = Vector3<IntervalDouble>.Sqrt(vec);
        }
    }
}
