using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using CP_ParallelPipesForm.Core;

namespace CP_ParallelPipesForm
{
    public partial class ResultForm : Form
    {
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T) formatter.Deserialize(ms);
            }
        }

        CP cp;

        public ResultForm(CP cp) : this()
        {
            if (cp.name != "")
            {
                Text = "Результаты вычислений для исследования " + cp.name;
            }

            this.cp = cp;
            dateTimeLabel.Text = "Дата: " + cp.dateTime.ToString();
            CalcTimeText.Text =
                "Загружено из файла"; //string.Format("Время вычисления: {0} мин. {1} сек. {2} мс.", calcTime.Minutes, calcTime.Seconds, calcTime.Milliseconds);

            InitComboBoxes();
        }

        public ResultForm(string name, CP cp, TimeSpan calcTime) : this()
        {
            if (cp.name != "")
            {
                Text = "Результаты вычислений для исследования " + cp.name;
            }

            this.cp = DeepCopy<CP>(cp);

            dateTimeLabel.Text = "Дата: " + cp.dateTime.ToString();
            CalcTimeText.Text = string.Format("Время вычисления: {0} мин. {1} сек. {2} мс.", calcTime.Minutes,
                calcTime.Seconds, calcTime.Milliseconds);

            InitComboBoxes();
        }

        public ResultForm()
        {
            InitializeComponent();
            UprButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Защитный потенциал,Upr, В", "Upr", 
                    cp.name, comboBoxUpr, cp.getUtpr, cp.getUtprInterval);
            };
            UtgButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Потенциал в грунте, Utg, В", "Utg", cp.name, comboBoxUtg, cp.getUtg);
            };
            UtmButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Потенциал в металле, Utm, В", "Utm", cp.name, comboBoxUtm, cp.getUtm);
            };
            ItgButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Ток, втекающий через боковую поверхность, Itg, А/м2", cp.name, "Itg", comboBoxItg,
                    cp.getItg, cp.getItgInterval);
            };
            ItxButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Продольный ток между соседними ФИ, Itx, А/м2", cp.name, "Itx", comboBoxItx,
                    cp.getItx);
            };
            JtxButtonGraph.Click += (object sender, EventArgs e) =>
            {
                graphicBtn_Click("Плотность тока \"грунт-труба\", Jtx, А/м2", cp.name, "Jtx", comboBoxItx, cp.getJt, cp.getJtInterval);
            };
            SaveUtgButton.Click += (object sender, EventArgs e) => { saveResultsToFile(cp.getUtg, "Ut{0}g "); };
            SaveUtmButton.Click += (object sender, EventArgs e) => { saveResultsToFile(cp.getUtm, "Ut{0}m "); };
            SaveItgButton.Click += (object sender, EventArgs e) => { saveResultsToFile(cp.getItg, "It{0}g "); };
            SaveItxButton.Click += (object sender, EventArgs e) => { saveResultsToFile(cp.getItx, "It{0}x "); };
            SaveUprButton.Click += (object sender, EventArgs e) => { saveResultsToFile(cp.getUtpr, "Ut{0}pr"); };
        }

        void InitComboBoxes()
        {
            string[] boxesNames = {"comboBoxUpr", "comboBoxUtg", "comboBoxUtm", "comboBoxItg", "comboBoxItx"};
            for (int i = 0; i < cp.Pipes.Count; i++)
            {
                for (int j = 0; j < boxesNames.Length; j++)
                {
                    ComboBox box = (ComboBox) this.GetType()
                        .GetField(boxesNames[j], BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                    box.Items.Add("Все");
                    box.Items.Add(cp.Pipes[i].name);
                    box.SelectedIndex = 0;
                }
            }
        }

        private void graphicBtn_Click(string graphName, string name, string issledName, ComboBox comboBox,
            Func<int, double[][]> points, Func<int, (double[] x, Interval[] y)> pointsInterval = null, bool useIntervals = true)
        {
            GraphicForm gf = new GraphicForm(graphName, name, issledName);
            gf.Text = "График: " + graphName;

            if (comboBox.SelectedIndex == 0)
            {
                for (int i = 0; i < cp.Pipes.Count; i++)
                {
                    if (useIntervals && pointsInterval != null)
                    {
                        gf.ShowChart(cp.Pipes[i].name, pointsInterval(i).x, pointsInterval(i).y, i);
                    }
                    else
                    {
                        gf.ShowChart(cp.Pipes[i].name, points(i)[0], points(i)[1], i);
                    }
                }
            }
            else
            {
                double[][] pointss = points(comboBox.SelectedIndex - 1);
                gf.ShowChart(cp.Pipes[comboBox.SelectedIndex - 1].name, pointss[0],
                    pointss[1], comboBox.SelectedIndex - 1);
            }

            gf.Show();
        }

        private void saveResultsToFile(Func<int, double[][]> points, string pointsName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Text file|*.txt|All files|*.*";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.FileName = (string.Format(pointsName, "") + cp.name).Replace(" ", "");

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string data = "";
                List<double[][]> allPoints = new List<double[][]>();

                for (int i = 0; i < cp.Pipes.Count; i++)
                {
                    double[][] pointss = points(i);
                    allPoints.Add(pointss);
                }

                for (int i = 0; i < allPoints[0][1].Length; i++)
                {
                    if (i == 0)
                    {
                        data += string.Format("{0,23}", "X");
                        for (int j = 0; j < cp.Pipes.Count; j++)
                        {
                            data += string.Format("{0,23}", cp.Pipes[j].name); // string.Format(pointsName, j + 1)
                        }

                        data += "\r\n";
                    }

                    for (int j = 0; j < cp.Pipes.Count; j++)
                    {
                        if (j == 0)
                        {
                            data += string.Format("{0,23}", allPoints[j][0][i].ToString("E8"));
                        }

                        data += string.Format("{0,23}", allPoints[j][1][i].ToString("E8"));
                    }

                    data += "\r\n";
                }

                File.WriteAllText(saveFileDialog.FileName, data);
            }
        }

        void ShowOrigParamsButtonClick(object s, EventArgs e)
        {
            Form otherParamsForm = new Form();

            otherParamsForm.Text = "Исходные и прочие параметры " + cp.name;
            //otherParamsForm.AutoSize = true;

            //otherParamsForm. = new System.Drawing.Size(500, 1000);
            otherParamsForm.Size = new System.Drawing.Size(420, 600);

            string[] otherFields = new string[]
            {
                "Сила тока, А",
                "Глубина от уровня земли до центр. точки, м",
                "Уд сопр грунта, Ом*м",
                "Длина труб, м",
                "Число фиктивных источников",
                "-----Результаты вычислений-----",
                "Удельная электропроводность грунта"
            };

            string[] otherValuesFields = new string[]
            {
                cp.anod.I0.ToString(),
                cp.anod.Za.ToString(),
                cp.ro_g.ToString(),
                cp.L.ToString(),
                cp.Nfi.ToString(),
                "",
                cp.Sigma_g.ToString()
            };

            string[] pipeFields = new string[]
            {
                "Глубина от уровня земли до верхней т.Т, м.",
                "Расстояние до анода по горизонтали, м",
                "Удельное сопротивление, Ом*м",
                "Внешний диаметр, м",
                "Толщина стенки, м",
                "Сопротивление изоляции, Ом*м2",
                "-----Результаты вычислений-----",
                "Площадь сеч металла",
                "Продольное сопротивление трубы, Ом/м",
                "Сопротивление изоляции трубы, Ом*м",
                "Удельная электропроводность металла трубы",
                "Площади боковых поверхностей",
                "Сопротивление по нормали КОЭ трубы"
            };

            string[][] fields = new string[2][];
            fields[0] = new string[otherFields.Length + cp.Pipes.Count * (pipeFields.Length + 1)];
            fields[1] = new string[otherFields.Length + cp.Pipes.Count * (pipeFields.Length + 1)];

            for (int i = 0; i < otherFields.Length; i++)
            {
                fields[0][i] += otherFields[i];
                fields[1][i] += otherValuesFields[i];
            }

            for (int i = 0; i < cp.Pipes.Count; i++)
            {
                fields[0][otherFields.Length + (pipeFields.Length + 1) * i] =
                    "-----------------------------" + cp.Pipes[i].name.ToUpper();
                string[] pipeValuesFields = new string[]
                {
                    cp.Pipes[i].Ht.ToString(),
                    cp.Pipes[i].Lta.ToString(),
                    cp.Pipes[i].ro_t.ToString(),
                    cp.Pipes[i].Dt2.ToString(),
                    cp.Pipes[i].Det.ToString(),
                    cp.Pipes[i].Ct.ToString(),
                    "",
                    cp.Pipes[i].SechT.ToString("E3"),
                    cp.Pipes[i].RproT.ToString("E3"),
                    cp.Pipes[i].Rct.ToString("E3"),
                    cp.Pipes[i].Sigma_t.ToString("E3"),
                    cp.Pipes[i].St.ToString("E3"),
                    (cp.Pipes[i].Ct / cp.Pipes[i].St).ToString("E3"),
                };

                for (int j = 0; j < pipeFields.Length; j++)
                {
                    fields[0][otherFields.Length + (pipeFields.Length + 1) * i + j + 1] = pipeFields[j];
                    fields[1][otherFields.Length + (pipeFields.Length + 1) * i + j + 1] = pipeValuesFields[j];
                }
            }

            for (int i = 0; i < fields[0].Length; i++)
            {
                Label lb = new Label();
                lb.Location = new System.Drawing.Point(25, 25 + i * 25);
                lb.Text = fields[0][i];
                lb.Size = new System.Drawing.Size(250, 25);
                otherParamsForm.Controls.Add(lb);

                Label lbValue = new Label();
                lbValue.Location = new System.Drawing.Point(275, 25 + i * 25);
                lbValue.Size = new System.Drawing.Size(100, 25);
                lbValue.Text = string.Format("{0,20}", fields[1][i]);
                otherParamsForm.Controls.Add(lbValue);
            }

            otherParamsForm.AutoScroll = true;
            otherParamsForm.Show();
        }


        public static void LoadResultFromFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Cathodic Protection File|*.cpf";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stream stream = ofd.OpenFile();
                BinaryFormatter bf = new BinaryFormatter();

                CP cp = bf.Deserialize(stream) as CP;
                stream.Close();

                ResultForm rf = new ResultForm(cp);
                rf.Show();
            }
        }

        private void SaveAllResultButtonClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Cathodic Protection File|*.cpf";
            saveFileDialog.FileName = cp.name;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, cp);
                stream.Close();
            }
        }

        private void SaveOrigButton_Click(object sender, EventArgs e)
        {
        }
    }
}