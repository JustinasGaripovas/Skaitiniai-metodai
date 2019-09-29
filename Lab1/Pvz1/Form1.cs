using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace Pvz1
{
    public partial class Form1 : Form
    {
        List<Timer> Timerlist = new List<Timer>();

        const double precisionPoint = 1e-6;

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        // ---------------------------------------------- PUSIAUKIRTOS METODAS ----------------------------------------------

        private double F(double x)
        {
            return (double)
                (-1.29*Math.Pow(x, 4))+
                (+5.08*Math.Pow(x, 3))+
                (-2.76*Math.Pow(x, 2))+
                (-6.31*Math.Pow(x, 1))+
                + 4.10;
        }

        private double Fcustom(double m)
        {
            return (double)(50 * Math.Pow(Math.E, (-0.15 * 3) / m)) + 
                (((9.8 * m) / 0.15) * 
                (Math.Pow(Math.E, (-0.15 * 3) / m) - 1)) - 14;
        }

        private double FD(double x)
        {
            return (double)
               (-5.16 * Math.Pow(x, 3)) +
               (+15.24 * Math.Pow(x, 2)) +
               (-5.52 * Math.Pow(x, 1)) +
               -6.31;
        }

        private double G(double x)
        {
            return (double)(x * Math.Pow(Math.Cos(x), 2)) - (Math.Pow(x / 2, 2));
        }

        private double GD(double x)
        {
            return (double) Math.Pow(Math.Cos(x), 2) - (x * Math.Sin(x*2)) - x/2;
        }

        public delegate double CurrentFucntion(double x);
        public delegate double CurrentFucntionD(double x);
        Series FNC, XMID, CHORDS, NEWTON, ITTERATION;

        private void SeriesInitializer()
        {
            CHORDS = chart1.Series.Add("CHORDS");
            CHORDS.MarkerStyle = MarkerStyle.Circle;
            CHORDS.MarkerSize = 8;
            CHORDS.ChartType = SeriesChartType.Point;
            CHORDS.ChartType = SeriesChartType.Line;

            NEWTON = chart1.Series.Add("NEWTON");
            NEWTON.MarkerStyle = MarkerStyle.Circle;
            NEWTON.MarkerSize = 8;
            NEWTON.ChartType = SeriesChartType.Point;
            NEWTON.ChartType = SeriesChartType.Line;

            ITTERATION = chart1.Series.Add("ITTERATION");
            ITTERATION.MarkerStyle = MarkerStyle.Circle;
            ITTERATION.MarkerSize = 8;
            ITTERATION.ChartType = SeriesChartType.Point;
            ITTERATION.ChartType = SeriesChartType.Line;

            XMID = chart1.Series.Add("XMID");
            XMID.MarkerStyle = MarkerStyle.Circle;
            XMID.ChartType = SeriesChartType.Point;
            XMID.ChartType = SeriesChartType.Line;
            XMID.MarkerSize = 8;

            FNC = chart1.Series.Add("funkcija");
            FNC.ChartType = SeriesChartType.Line;
            FNC.MarkerSize = 8;

        }

        float x1, x2, xstep;
        private void FormInitializer(float x1, float x2, CurrentFucntion function, string functionName)
        {
            ClearForm(); // išvalomi programos duomenys
            PreparareForm(-5, 5, -10, 10);
            SeriesInitializer();

            this.x1 = x1; // izoliacijos intervalo pradžia
            this.x2 = x2; // izoliacijos intervalo galas

            xstep = 0.1F;

            richTextBox1.AppendText("Iteracija         x            F(x)        x1          x2          F(x1)         F(x2)       \n");

            double x = -50;
            for (int i = 0; i < 5000; i++)
            {
                FNC.Points.AddXY(x, function(x));
                x = x + (2 * Math.PI) / 50;

                //Console.WriteLine("x->{0}",x);
                Console.WriteLine("F(x)->{0}", function(x));

            }

            FNC.BorderWidth = 2;
        }

        public void ChordMethod(double x1, double x2, CurrentFucntion function)
        {
            int iii = 0;
            int N = 1000;

            double k = Math.Abs(function(x1) / function(x2));
            double xmid = (x1 + (k * x2)) / (1 + k);

            while (Math.Abs(function(xmid)) > precisionPoint && iii <= N)
            {
                if (DifferentSign(function(x1), function(xmid), function))
                {
                    x2 = xmid;
                }
                else
                {
                    x1 = xmid;
                }

                k = Math.Abs(function(x1) / function(x2));
                xmid = (x1 + k * x2) / (1 + k);

                iii = iii + 1;
            }

            richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                                    iii, xmid, function(xmid), x1, x2, function(x1), function(x2)));

            CHORDS.Points.AddXY(xmid, 0);
        }

        public void NewtonMethod(double x1, double x2, CurrentFucntion function, CurrentFucntionD functionD)
        {
            int iii = 0;
            int N = 1000;

            double answer = x1;
            double step = function(answer) / functionD(x1);

            while (Math.Abs(function(answer)) > precisionPoint && iii <= N)
            {
                step = step = function(answer) / functionD(x1);
                answer -= step;

                iii++;
            }

            richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7}\n",
                                   iii, answer, function(answer)));

            NEWTON.Points.AddXY(answer, 0);
        }

        bool found = false;
        int itterations = 0;
        public void DecreasingIntervalMethod(double xstart, double xend, double step, CurrentFucntion function)
        {
            if (found)
                return;

            if ( Math.Abs(Math.Abs(function(xstart+step)) - (Math.Abs(function(xstart)))) > precisionPoint)
            { 
                for (double i = xstart; i < xend; i += step)
                {
                    itterations++;

                    if (DifferentSign(i, i + step, function))
                    {
                        DecreasingIntervalMethod(i, i + step, step / 5, function);
                    }
                }
            }
            else
            {
                found = true;

                if (found)
                {
                    richTextBox1.AppendText(String.Format(" {0,6:d}   {1,12:f7}  {2,12:f7} {3,12:f7} {4,12:f7} {5,12:f7} {6,12:f7}\n",
                    itterations, xstart + (step / 2), function(xstart + (step / 2)), xstart, xend, function(xstart), function(xend)));

                    ITTERATION.Points.AddXY(xstart, 0);
                    ITTERATION.Points.AddXY(xstart + step, 0);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, F, "F(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, F))
                {
                    ChordMethod(i, i + xstep, F);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, F, "F(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, F))
                {
                    NewtonMethod(i, i + xstep,F,FD);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, F, "F(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, F))
                {
                    DecreasingIntervalMethod(i, i + xstep, 0.05, F);
                    found = false; itterations = 0;
                }
            }
        }


        // ---------------------------------------------- TEKSTINE FUNKCIJOS ----------------------------------------------

        private void button5_Click(object sender, EventArgs e)
        {
            FormInitializer(-10F, 10F, Fcustom, "F(m)");
            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, Fcustom))
                {
                    ChordMethod(i, i + xstep, Fcustom);
                }
            }
        }

        // ---------------------------------------------- PARAMETRINĖS FUNKCIJOS ----------------------------------------------

        private void button8_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, G, "G(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, G))
                {
                    ChordMethod(i, i + xstep, G);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, G, "G(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, G))
                {
                    NewtonMethod(i, i + xstep, G, GD);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FormInitializer(-3.89F, 3.89F, G, "G(x)");

            for (float i = x1; i < x2; i += xstep)
            {
                if (DifferentSign(i, i + xstep, G))
                {
                    DecreasingIntervalMethod(i, i + xstep, 0.05, G);
                    found = false; itterations = 0;
                }
            }
        }

        // ---------------------------------------------- TIESINĖ ALGEBRA ----------------------------------------------

        /// <summary>
        /// Tiesine algebra (naudojama MathNet)
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            ClearForm();

            double[,] x = { { 1, 2, 3 }, { 3, 4, 5 }, { 6, 5, 8 } };
            // iš masyvo sugeneruoja matricą, is matricos išskiria eilutę - suformuoja vektorių
            Matrix<double> m = Matrix<double>.Build.DenseOfArray(x);
            Vector<double> v = m.Row(1);
            richTextBox1.AppendText("\nMatrica m:\n");
            richTextBox1.AppendText(m.ToString());

            richTextBox1.AppendText("\nVektorius v:\n");
            richTextBox1.AppendText(v.ToString());

            richTextBox1.AppendText("\ntranspose(m):\n");
            richTextBox1.AppendText(m.Transpose().ToString());

            Matrix<double> vm = v.ToRowMatrix();
            richTextBox1.AppendText("\nvm = v' - toRowMatrix()\n");
            richTextBox1.AppendText(vm.ToString());

            Vector<double> v1 = m * v;
            richTextBox1.AppendText("\nv1 = m * v\n");
            richTextBox1.AppendText(v1.ToString());
            richTextBox1.AppendText("\nmin(v1)\n");
            richTextBox1.AppendText(v1.Min().ToString());

            Matrix<double> m1 = m.Inverse();
            richTextBox1.AppendText("\ninverse(m)\n");
            richTextBox1.AppendText(m1.ToString());

            richTextBox1.AppendText("\ndet(m)\n");
            richTextBox1.AppendText(m.Determinant().ToString());

            // you must add reference to assembly system.Numerics
            Evd<double> eigenv = m.Evd();
            richTextBox1.AppendText("\neigenvalues(m)\n");
            richTextBox1.AppendText(eigenv.EigenValues.ToString());
            
            LU<double> LUanswer = m.LU();
            richTextBox1.AppendText("\nMatricos M LU skaida\n");
            richTextBox1.AppendText("\nMatrica L:\n");
            richTextBox1.AppendText(LUanswer.L.ToString());
            richTextBox1.AppendText("\nMatrica U:\n");
            richTextBox1.AppendText(LUanswer.U.ToString());
            
            QR<double> QRanswer = m.QR();
            richTextBox1.AppendText("\nMatricos M QR skaida\n");
            richTextBox1.AppendText("\nMatrica Q:\n");
            richTextBox1.AppendText(QRanswer.Q.ToString());
            richTextBox1.AppendText("\nMatrica R:\n");
            richTextBox1.AppendText(QRanswer.R.ToString());

            Vector<double> v3 = m.Solve(v);
            richTextBox1.AppendText("\nm*v3 = v sprendziama QR metodu\n");
            richTextBox1.AppendText(v3.ToString());
            richTextBox1.AppendText("Patikrinimas\n");
            richTextBox1.AppendText((m * v3 - v).ToString());
        }


        // ---------------------------------------------- KITI METODAI ----------------------------------------------

        /// <summary>
        /// Nustotoma ar f(num1) ir f(num2) ženklai yra skirtingi
        /// </summary>
        private bool DifferentSign(double num1, double num2, CurrentFucntion function)
        {
            return Math.Sign((double)function(num1)) != Math.Sign((double)function(num2));
        }

        /// <summary>
        /// Uždaroma programa
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        /// <summary>
        /// Išvalomas grafikas ir consolė
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        

        public void ClearForm()
        {
            richTextBox1.Clear(); // isvalomas richTextBox1
            // sustabdomi timeriai jei tokiu yra
            foreach (var timer in Timerlist)
            {
                timer.Stop();
            }

            // isvalomos visos nubreztos kreives
            chart1.Series.Clear();
        }
    }
}
