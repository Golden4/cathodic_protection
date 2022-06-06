using System;
using System.ComponentModel;
using System.Windows.Forms;
using CP_ParallelPipesForm.Core;

namespace CP_ParallelPipesForm
{
    public partial class SetIntervalForm : Form
    {
        private double[] x;
        private Interval[] y;
        private string name;
        private int colorIndex;
        
        public event EventHandler<ResultEventArgs> onResult;

        public class ResultEventArgs : EventArgs
        {
            public double[] x;
            public Interval[] y;
        }

        public SetIntervalForm(double[] x, Interval[] y, string name, int colorIndex)
        {
            InitializeComponent();
            InitGridView(x, y);
            this.name = name;
            this.colorIndex = colorIndex;
        }

        public SetIntervalForm(double[] x, double[] y)
        {
            InitializeComponent();
        }

        private void InitGridView(double[] x, Interval[] y)
        {
            var columns = new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn()
                {
                    Name = "x",
                    Width = 50,
                    ValueType = typeof(double),
                    SortMode = DataGridViewColumnSortMode.Programmatic
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "y1",
                    Width = 50,
                    ValueType = typeof(double),
                    SortMode = DataGridViewColumnSortMode.NotSortable
                },
                new DataGridViewTextBoxColumn()
                {
                    Name = "y2",
                    Width = 50,
                    ValueType = typeof(double),
                    SortMode = DataGridViewColumnSortMode.NotSortable
                },
                // new DataGridViewButtonColumn()
                // {
                //     Name = "Действие",
                //     Width = 70,
                //     Text = "Удалить",
                //     UseColumnTextForButtonValue = true
                // }
            };
            dataGridView1.Columns.AddRange(columns);
            dataGridView1.AllowUserToDeleteRows = true;
            AddDataToGridView(x, y);
            TryUpdateDataFromGrid();
        }

        private void AddDataToGridView(double[] x, Interval[] y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                dataGridView1.Rows.Add(x[i], y[i].x1, y[i].x2);
            }
        }

        protected bool TryUpdateDataFromGrid()
        {
            int rowsCount = dataGridView1.Rows.Count - 1;
            x = new double[rowsCount];
            y = new Interval[rowsCount];
            try
            {
                for (int rows = 0; rows < rowsCount; rows++)
                {
                    var cells = dataGridView1.Rows[rows].Cells;
                    if (rows == 0 || rows == rowsCount)
                    {
                        dataGridView1.Rows[rows].Cells[0].ReadOnly = true;
                    }

                    x[rows] = double.Parse(cells[0].Value.ToString());
                    y[rows] = new Interval(double.Parse(cells[1].Value.ToString()),
                        double.Parse(cells[2].Value.ToString()));
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Некорректные данные в таблице");
                return false;
            }
        }

        private void GraphButton_Click(object sender, EventArgs e)
        {
            if (TryUpdateDataFromGrid())
            {
                GraphicForm graphicForm = new GraphicForm("Распределение сопротивления изоляции, 0м*м2", "", "");
                graphicForm.ShowChart(name, x, y, colorIndex, false);
                graphicForm.Show();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (TryUpdateDataFromGrid())
            {
                onResult?.Invoke(this, new ResultEventArgs() {x = x, y = y});
                Close();
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }

            foreach (DataGridViewCell cell in dataGridView1.Rows[e.RowIndex].Cells)
            {
                cell.ErrorText = null;
                if (cell.Value == null || cell.Value.ToString() == "")
                {
                    cell.ErrorText = "Ячейки обязательны для заполнения";
                }
                else
                {
                    string value = e.ColumnIndex == cell.ColumnIndex
                        ? e.FormattedValue.ToString()
                        : cell.Value.ToString();
                    if (!double.TryParse(value,
                        out double newInteger))
                    {
                        // cell.Value = newInteger;
                        cell.ErrorText = "Поле обязательно для заполнения";
                    }
                }
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            string text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText;
            if (e.ColumnIndex == 0 && String.IsNullOrEmpty(text))
            {
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            }
        }
    }
}