using System;
using System.Windows.Forms;
using ParallelPipesIntervals.Core;

namespace ParallelPipesIntervals
{
    public partial class IntervalForm : Form
    {
        private double[] xG;
        private Interval[] yG;
        public IntervalForm(CP cp)
        {
            InitializeComponent();
            double startInt = double.Parse(StartInterval.Text);
            double endInt = double.Parse(EndInterval.Text);
            var x = new double[4];
            var y = new Interval[4];
            x[0] = 0;
            x[1] = 10000;
            x[2] = 20000;
            x[3] = cp.L;
            y[0] = new Interval(startInt, endInt);
            y[1] = new Interval(500, 720);
            y[2] = new Interval(300, 350);
            y[3] = new Interval(startInt, endInt);
            xG = new double[cp.Nfi];
            yG = new Interval[cp.Nfi];
            for (int i = 0; i < cp.Nfi; i++)
            {
                xG[i] = i * cp.L / (cp.Nfi - 1);
                yG[i] = Interpolation.LinearInterpolation(xG[i], x, y); // new IntervalDouble(startInt, endInt);
            }
        }

        private void GraphButton_Click(object sender, EventArgs e)
        {
            GraphicForm graphicForm = new GraphicForm("Распрдение", "", "");
            graphicForm.ShowChart("Распп", xG, yG);
            graphicForm.Show();
        }
    }
}