using System;
using System.Windows.Forms;

namespace CP_ParallelPipesForm
{
    public partial class PipeForm : Form
    {
        Pipe pipe;
        Action saveClickCallBack;
        public PipeForm(Pipe pipe, Action saveClickCallBack = null) : this()
        {
            this.pipe = pipe;
            PipeNameText.Text = pipe.name;
            HtText.Text = pipe.Ht.ToString();
            ro_tText.Text = pipe.ro_t.ToString();
            Dt2Text.Text = pipe.Dt2.ToString();
            DetText.Text = pipe.Det.ToString();
            CtText.Text = pipe.Ct.ToString();
            LtaText.Text = pipe.Lta.ToString();

            this.saveClickCallBack = saveClickCallBack;
            SaveButton.Click += SaveButton_Click;
        }
        public PipeForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                pipe.name = PipeNameText.Text;

                double result;

                if (double.TryParse(HtText.Text, out result))
                    pipe.Ht = result;
                else
                {
                    HtText.Text += "!!!";
                    throw new Exception();
                }

                if (double.TryParse(ro_tText.Text, out result))
                    pipe.ro_t = result;
                else
                {
                    ro_tText.Text += "!!!";
                    throw new Exception();
                }
                if (double.TryParse(Dt2Text.Text, out result))
                    pipe.Dt2 = result;
                else
                {
                    Dt2Text.Text += "!!!";
                    throw new Exception();
                }
                if (double.TryParse(DetText.Text, out result))
                    pipe.Det = result;
                else
                {
                    DetText.Text += "!!!";
                    throw new Exception();
                }
                if (double.TryParse(CtText.Text, out result))
                    pipe.Ct = result;
                else
                {
                    CtText.Text += "!!!";
                    throw new Exception();
                }
                if (double.TryParse(LtaText.Text, out result))
                    pipe.Lta = result;
                else
                {
                    LtaText.Text += "!!!";
                    throw new Exception();
                }

                //pipe.ro_t = double.Parse(ro_tText.Text);
                //pipe.Dt2 = double.Parse(Dt2Text.Text);
                //pipe.Det = double.Parse(DetText.Text);
                //pipe.Ct = double.Parse(CtText.Text);
                //pipe.Lta = double.Parse(LtaText.Text);

                saveClickCallBack();

                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Некорректные данные!");
            }

        }
    }
}
