using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using CP_ParallelPipesForm.Core;

namespace CP_ParallelPipesForm
{
    public partial class GraphicForm : Form
    {
        public string name;
        public string issledName;

        public static List<Color> colors = ChartColorPallets.BrightPastel;

        public GraphicForm(string graphName,string name,string issledName)
        {
            this.name = name;
            this.issledName = issledName;
            InitializeComponent();
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisX.IsStartedFromZero = false;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;
            //chart1.ChartAreas[0].AxisY.IsMarginVisible = false;
            //chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1.ChartAreas[0].AxisX.Title = "X, м";
            chart1.ChartAreas[0].AxisY.Title = graphName;
        }

        public void ShowChart(string chartName,int index, double[] x, double[] y)
        {
            try
            {
                Series newSeries = new Series(chartName);
                newSeries.Points.Clear();
                newSeries.ChartType = SeriesChartType.Line;
                newSeries.MarkerStyle = MarkerStyle.Circle;
                chart1.Series.Add(newSeries);
                newSeries.Color = colors[index];

                for (int i = 0; i < x.Length; i++)
                {
                    chart1.Series[chart1.Series.Count - 1].Points.AddXY(x[i], y[i]);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowChart(string chartName, double[] x, double[] y)
        {
            try
            {
                Series newSeries = new Series(chartName);
                newSeries.Points.Clear();
                newSeries.ChartType = SeriesChartType.Line;
                newSeries.MarkerStyle = MarkerStyle.Circle;
                chart1.Series.Add(newSeries);

                for (int i = 0; i < x.Length; i++)
                {
                    chart1.Series[chart1.Series.Count - 1].Points.AddXY(x[i], y[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowChart(string chartName, double[] x, Interval[] y)
        {
            var y1 = new double[y.Length];
            var y2 = new double[y.Length];
            var yMid = new double[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                y1[i] = y[i].x1;
                y2[i] = y[i].x2;
                yMid[i] = y[i].Mid();
            }
            ShowChart(chartName + "Нач", x, y1);
            ShowChart(chartName + "Ср", x, yMid);
            ShowChart(chartName + "Кон", x, y2);
        }

        private void SaveGraphButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            string filter = "";

            for (int i = 0; i < 3; i++)
            {
                filter += string.Format("{0}|*.{1}", (ChartImageFormat)i, ((ChartImageFormat)i).ToString().ToLower());

                if(i != 2)
                {
                    filter += "|";
                }
            }

            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.FileName = "График"+name+issledName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                chart1.SaveImage(saveFileDialog.FileName, (ChartImageFormat)saveFileDialog.FilterIndex);
            }
        }

        private void CompireGraphButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text file|*.txt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(openFileDialog.FileName);

                List<List<string>> allValues = new List<List<string>>();

                for (int i = 0; i < lines.Length; i++)
                {
                    List<string> values = new List<string>();
                    string value = "";

                    for (int j = 0; j < lines[i].Length; j++)
                    {
                        if (j % 23 == 22)
                        {
                            value += lines[i][j];
                            values.Add(value);
                            value = "";
                        } else
                        {
                            if (value == "" && lines[i][j] == ' ')
                            {
                               
                            } else
                            {
                                value += lines[i][j];
                            }
                        }

                        if(lines[i].Length-1 == j)
                        {

                            allValues.Add(values);
                        }
                    }
                }

                string[] names = new string[allValues[0].Count];
                double[] xValues = new double[allValues.Count-1];
                double[][] yValues = new double[allValues[1].Count][];

                for (int i = 0; i < allValues.Count; i++)
                {
                    for (int j = 0; j < allValues[i].Count; j++)
                    {
                        if (i == 0) {
                            names[j] = allValues[0][j];
                            yValues[j] = new double[allValues.Count-1];
                        } else {
                            try
                            {
                                if (j == 0)
                                {
                                    
                                    xValues[i-1] = double.Parse(allValues[i][j]);
                                }
                                else
                                {
                                    yValues[j - 1][i-1] = double.Parse(allValues[i][j]);
                                }
                            } catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return;
                            }
                        }
                    }
                }

                for (int i = 1; i < allValues[1].Count; i++)
                {
                    ShowChart(names[i], xValues, yValues[i-1]);
                }
            }
        }
    }

    public static class ChartColorPallets
    {

        public static List<Color> Bright
            => new List<Color>() {
                "#008000".FromHex(),
                "#0000FF".FromHex(),
                "#800080".FromHex(),
                "#00FF00".FromHex(),
                "#FF00FF".FromHex(),
                "#008080".FromHex(),
                "#FFFF00".FromHex(),
                "#808080".FromHex(),
                "#00FFFF".FromHex(),
                "#000080".FromHex(),
                "#800000".FromHex(),
                "#FF0000".FromHex(),
                "#808000".FromHex(),
                "#C0C0C0".FromHex(),
                "#FF6347".FromHex(),
                "#FFE4B5".FromHex()
        };
        public static List<Color> GreyScale
            => new List<Color>() {
                "#C8C8C8".FromHex(),
                "#BDBDBD".FromHex(),
                "#B2B2B2".FromHex(),
                "#A7A7A7".FromHex(),
                "#9C9C9C".FromHex(),
                "#919191".FromHex(),
                "#868686".FromHex(),
                "#7B7B7B".FromHex(),
                "#707070".FromHex(),
                "#656565".FromHex(),
                "#5A5A5A".FromHex(),
                "#4F4F4F".FromHex(),
                "#444444".FromHex(),
                "#393939".FromHex(),
                "#2E2E2E".FromHex(),
                "#232323".FromHex()
        };
        public static List<Color> Excel
            => new List<Color>() {
                "#9999FF".FromHex(),
                "#993366".FromHex(),
                "#FFFFCC".FromHex(),
                "#CCFFFF".FromHex(),
                "#660066".FromHex(),
                "#FF8080".FromHex(),
                "#0066CC".FromHex(),
                "#CCCCFF".FromHex(),
                "#000080".FromHex(),
                "#FF00FF".FromHex(),
                "#FFFF00".FromHex(),
                "#00FFFF".FromHex(),
                "#800080".FromHex(),
                "#800000".FromHex(),
                "#008080".FromHex(),
                "#0000FF".FromHex()
        };
        public static List<Color> Light
            => new List<Color>() {
                "#E6E6FA".FromHex(),
                "#FFF0F5".FromHex(),
                "#FFDAB9".FromHex(),
                "#FFFACD".FromHex(),
                "#FFE4E1".FromHex(),
                "#F0FFF0".FromHex(),
                "#F0F8FF".FromHex(),
                "#F5F5F5".FromHex(),
                "#FAEBD7".FromHex(),
                "#E0FFFF".FromHex()
        };
        public static List<Color> Pastel
            => new List<Color>() {
                "#87CEEB".FromHex(),
                "#32CD32".FromHex(),
                "#BA55D3".FromHex(),
                "#F08080".FromHex(),
                "#4682B4".FromHex(),
                "#9ACD32".FromHex(),
                "#40E0D0".FromHex(),
                "#FF69B4".FromHex(),
                "#F0E68C".FromHex(),
                "#D2B48C".FromHex(),
                "#8FBC8B".FromHex(),
                "#6495ED".FromHex(),
                "#DDA0DD".FromHex(),
                "#5F9EA0".FromHex(),
                "#FFDAB9".FromHex(),
                "#FFA07A".FromHex()
        };
        public static List<Color> EarthTones
            => new List<Color>() {
                "#FF8000".FromHex(),
                "#B8860B".FromHex(),
                "#C04000".FromHex(),
                "#6B8E23".FromHex(),
                "#CD853F".FromHex(),
                "#C0C000".FromHex(),
                "#228B22".FromHex(),
                "#D2691E".FromHex(),
                "#808000".FromHex(),
                "#20B2AA".FromHex(),
                "#F4A460".FromHex(),
                "#00C000".FromHex(),
                "#8FBC8B".FromHex(),
                "#B22222".FromHex(),
                "#8B4513".FromHex(),
                "#C00000".FromHex()
        };
        public static List<Color> SemiTransparent
            => new List<Color>() {
                "#FF0000".FromHex(),
                "#00FF00".FromHex(),
                "#0000FF".FromHex(),
                "#FFFF00".FromHex(),
                "#00FFFF".FromHex(),
                "#FF00FF".FromHex(),
                "#AA7814".FromHex(),
                "#FF0000".FromHex(),
                "#00FF00".FromHex(),
                "#0000FF".FromHex(),
                "#FFFF00".FromHex(),
                "#00FFFF".FromHex(),
                "#FF00FF".FromHex(),
                "#AA7814".FromHex(),
                "#647832".FromHex(),
                "#285A96".FromHex()
        };
        public static List<Color> Berry
            => new List<Color>() {
                "#8A2BE2".FromHex(),
                "#BA55D3".FromHex(),
                "#4169E1".FromHex(),
                "#C71585".FromHex(),
                "#0000FF".FromHex(),
                "#8A2BE2".FromHex(),
                "#DA70D6".FromHex(),
                "#7B68EE".FromHex(),
                "#C000C0".FromHex(),
                "#0000CD".FromHex(),
                "#800080".FromHex()
        };
        public static List<Color> Chocolate
            => new List<Color>() {
                "#A0522D".FromHex(),
                "#D2691E".FromHex(),
                "#8B0000".FromHex(),
                "#CD853F".FromHex(),
                "#A52A2A".FromHex(),
                "#F4A460".FromHex(),
                "#8B4513".FromHex(),
                "#C04000".FromHex(),
                "#B22222".FromHex(),
                "#B65C3A".FromHex()
        };
        public static List<Color> Fire
            => new List<Color>() {
                "#FFD700".FromHex(),
                "#FF0000".FromHex(),
                "#FF1493".FromHex(),
                "#DC143C".FromHex(),
                "#FF8C00".FromHex(),
                "#FF00FF".FromHex(),
                "#FFFF00".FromHex(),
                "#FF4500".FromHex(),
                "#C71585".FromHex(),
                "#DDE221".FromHex()
        };
        public static List<Color> SeaGreen
            => new List<Color>() {
                "#2E8B57".FromHex(),
                "#66CDAA".FromHex(),
                "#4682B4".FromHex(),
                "#008B8B".FromHex(),
                "#5F9EA0".FromHex(),
                "#3CB371".FromHex(),
                "#48D1CC".FromHex(),
                "#B0C4DE".FromHex(),
                "#8FBC8B".FromHex(),
                "#87CEEB".FromHex()
        };
        public static List<Color> BrightPastel
            => new List<Color>() {
                "#418CF0".FromHex(),
                "#FCB441".FromHex(),
                "#E0400A".FromHex(),
                "#056492".FromHex(),
                "#BFBFBF".FromHex(),
                "#1A3B69".FromHex(),
                "#FFE382".FromHex(),
                "#129CDD".FromHex(),
                "#CA6B4B".FromHex(),
                "#005CDB".FromHex(),
                "#F3D288".FromHex(),
                "#506381".FromHex(),
                "#F1B9A8".FromHex(),
                "#E0830A".FromHex(),
                "#7893BE".FromHex()
        };
        private static Color FromHex(this string hex) => ColorTranslator.FromHtml(hex);
    }
}
