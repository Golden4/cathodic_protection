using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;

namespace CP_ParallelPipesForm
{
    public partial class MainForm : Form
    {
        public string version = "0.6";
        public MainForm()
        {
            InitializeComponent();
            VersionLabel.Text = "Версия программы: " + version;

            I0Text.Text = cp.anod.I0.ToString();
            ZaText.Text = cp.anod.Za.ToString();
            LText.Text = cp.L.ToString();
            NfiText.Text = cp.Nfi.ToString();
            ro_gText.Text = cp.ro_g.ToString();
        }

        public CP cp = new CP();

        bool SetFormValues()
        {
            try
            {
                cp.name = NameVar.Text;
                cp.anod.I0 = double.Parse(I0Text.Text);
                cp.anod.Za = double.Parse(ZaText.Text);
                cp.L = int.Parse(LText.Text);
                cp.Nfi = int.Parse(NfiText.Text);
                cp.ro_g = double.Parse(ro_gText.Text);
                return true;
            }
            catch (Exception )
            {
                MessageBox.Show("Некорректные данные!");
            }

            return false;
        }

        private void CalcButton_Click(object sender, EventArgs e)
        {
            if(cp.Pipes.Count == 0)
            {
                MessageBox.Show("Добавьте трубы!");
                return;
            }

            if (backgroundWorker1.IsBusy)
            {
                MessageBox.Show("Дождитесь окончания рассчета!");
                return;
            }

            if (SetFormValues())
            {
                CalcMessage.Visible = true;
                CalcMessage.Text = "Вычисляю, пожалуйста, подождите...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        void EditPipe(int index)
        {
            PipeForm pf = new PipeForm(cp.Pipes[index], ()=> { EditPipeFieldName(cp.Pipes[index].name, index); });
            pf.ShowDialog();
        }

        void DeletePipe(int index)
        {

            cp.Pipes.RemoveAt(index);

            for (int i = 0; i < pipeFields[index].Length; i++)
            {
                PipesGropup.Controls.Remove(pipeFields[index][i]);
            }

            pipeFields.RemoveAt(index);

            for (int i = 0; i < pipeFields.Count; i++)
            {
                for (int j = 0; j < pipeFields[i].Length; j++)
                {
                    pipeFields[i][j].Location = new Point(pipeFields[i][j].Location.X, 15 + i * distance);
                }
            }

            pipesCount--;
        }

        List<Control[]> pipeFields = new List<Control[]>();

        int distance = 40;
        int pipesCount = 0;
        int maxPipes = 7;

        void EditPipeFieldName(string name, int index)
        {
            pipeFields[index][2].Text = name;
        }
       public void AddPipesFiels(string pipeName = "")
        {
            Button editButton = new Button();
            Button deleteButton = new Button();
            Label namePipe = new Label();

            editButton.Location = new Point(188, 15 + pipesCount * distance);
            editButton.Name = "editButton";
            editButton.Size = new Size(111, 34);
            editButton.TabIndex = 11;
            editButton.Text = "Изменить параметры";
            editButton.UseVisualStyleBackColor = true;

            editButton.Click += (object sender1, EventArgs e1) =>
            {
                int index = 0;

                for (int i = 0; i < pipeFields.Count; i++)
                {
                    if (editButton == pipeFields[i][0])
                    {
                        index = i;
                    }
                }

                EditPipe(index);
            };

            deleteButton.Location = new Point(305, 15 + pipesCount * distance);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(110, 34);
            deleteButton.TabIndex = 13;
            deleteButton.Text = "Удалить";
            deleteButton.UseVisualStyleBackColor = true;

            namePipe.AutoSize = true;
            namePipe.Location = new Point(20, 15 + pipesCount * distance);
            namePipe.Name = "namePipe";
            namePipe.Size = new Size(46, 13);
            namePipe.TabIndex = 14;

            if(pipeName!= "")
                namePipe.Text = pipeName;
            else
                namePipe.Text = "Труба " + (pipesCount + 1);

            int pipeIndex = pipesCount;

            deleteButton.Click += (object sender1, EventArgs e1) =>
            {
                int index = 0;

                for (int i = 0; i < pipeFields.Count; i++)
                {
                    if (deleteButton == pipeFields[i][1])
                    {
                        index = i;
                    }
                }

                DeletePipe(index);
            };

            PipesGropup.Controls.Add(editButton);
            PipesGropup.Controls.Add(deleteButton);
            PipesGropup.Controls.Add(namePipe);

            Control[] elems = new Control[3];
            elems[0] = editButton;
            elems[1] = deleteButton;
            elems[2] = namePipe;
            pipeFields.Add(elems);

            pipesCount++;
        }

        private void AddPipeButton_Click(object sender, EventArgs e)
        {
            if (pipesCount >= maxPipes)
            {
                MessageBox.Show("Максимальное число труб!");
                return;
            }

            Pipe pipe = new Pipe();
            pipe.name = "Труба " + (pipesCount + 1);

            PipeForm pf = new PipeForm(pipe, ()=> {
                cp.Pipes.Add(pipe);
                AddPipesFiels(pipe.name);
            });
            pf.ShowDialog();
        }
        
        Stopwatch stopWatch;
        private void DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();//начинаем отчет времени

            cp.Solve();

            stopWatch.Stop();//останавливаем отсчет
        }

        private void WorkerProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            CalcMessage.Text = "Вычисляю, пожалуйста, подождите..." + e.ProgressPercentage.ToString() + "%";
        }

        private void RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                CalcMessage.Text = "Отменен!";
            }
            else if (e.Error != null)
            {
                CalcMessage.Text = "Ошибка: " + e.Error.Message;
            }
            else
            {
                TimeSpan ts = stopWatch.Elapsed;

                ResultForm rs = new ResultForm(NameVar.Text, cp, ts);

                rs.Show();

                CalcMessage.Visible = false;
            }

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        private void AboutMenuItemClick(object sender, EventArgs e)
        {
            string text = "  Программа для расчета электрических полей катодной защиты для параллельно расположенных и подключенных к станции катодной защиты труб с одним точечным анодом";
            text += "\r\n--------------------------------------------------------------------------------------";
            text += "\r\n - Болотнов А.М. Автор алгоритмов для расчета электрических полей катодной защиты";
            text += "\r\n - Алсынбаев Ф.С. Создатель программы и алгоритма для случая паралельных труб";
            text += "\r\n--------------------------------------------------------------------------------------";
            text += "\r\nСпециально для ВКР. УФА - 2020";
            text += "\r\n--------------------------------------------------------------------------------------";
            text += "\r\nВерсия программы: " + version;
            MessageBox.Show(text, "О программе");
        }

        private void LoadResultsMenuItemClick(object sender, EventArgs e)
        {
            ResultForm.LoadResultFromFile();

        }
    }
}
