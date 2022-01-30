using System;
using System.Collections.Generic;
using ParallelPipesIntervals.Core;

namespace ParallelPipesIntervals
{
    [Serializable]
    //ТРУБА
    public class Pipe
    {
        public string name;

        public double
            Ht = 1, // глубина от уровня земли до верхней т.Т, м
            ro_t = 2.45e-7, // Уд сопр стали, Ом*м
            Dt2 = 1.22, // Внешний диаметр Т, м: 0.3..1.5
            Det = 0.022, // Толщина стенки Т, м: 0.01..0.025
            Lta = 200; // расстояние до анода

        public double Rt2 { get; private set; } // Внешн радиус Т
        public double Rt0 { get; private set; } // Внутр радиус Т
        public double SechT { get; private set; } // Площадь сеч металла Т
        public double RproT { get; private set; } // продольное сопротивление трубы, Ом/м
        public double RproT1 { get; private set; } // продольное сопротивление трубы на единицу длины, Ом*м
        public double Sigma_t { get; private set; } // Удельная электропроводность металла трубы
        public double Lfi { get; private set; } // длина одной части ФИ трубы
        public double St { get; private set; } // площади боковых поверхностей
        public double Zt { get; private set; } //входное сопротивление трубопровода, Ом
        public double Its; // Ток, втекающий в точку дренажа 
        public Vector3<double>[] FIs; // координаты ФИ

        // ======================== ИЗОЛЯЦИЯ ТРУБЫ ========================
        public double Ct = 300000; // уд. сопротивление изоляции трубы, Ом*м2
        public double Rct { get; private set; } // сопротивление изоляции трубы, Ом*м
        public double Rct1 { get; private set; } // сопротивление изоляции на единицу длины, Ом*м

        public Pipe()
        {
        }

        public void Init(int L, int Nfi)
        {
            Lfi = L / Nfi;
            Rt2 = 0.5 * Dt2;
            Rt0 = Rt2 - Det;
            SechT = Math.PI * (Rt2 * Rt2 - Rt0 * Rt0);
            RproT = ro_t / SechT;

            St = Math.PI * 2 * Rt2 * Lfi;
            Sigma_t = 1d / ro_t;
            Rct = Ct / St;
            Rct1 = Ct / (Math.PI * Dt2);
            RproT1 = ro_t / (Math.PI * (Dt2 - Det) * Det);

            Zt = Math.Sqrt(Rct1 * RproT1) / 2d;

            FIs = new Vector3<double>[Nfi];

            for (int i = 0; i < Nfi; i++)
            {
                FIs[i] = new Vector3<double>((i * Lfi + Lfi / 2), Lta, (Ht + Rt2));
            }
        }

        public double getDistanceBetweenFIs(int fiIndex1, int fiIndex2) // расстояние м/у фиктивными источниками
        {
            return Vector3<double>.Distance(FIs[fiIndex1], FIs[fiIndex2]);
        }

        public double
            getDistanceBetweenPoint(int fiIndex, Vector3<double> point) // расстояние м/у фиктивным источником и точкой
        {
            return Vector3<double>.Distance(FIs[fiIndex], point);
        }
    }

    [System.Serializable]
    //Анод
    public class Anod
    {
        public double
            I0 = 0.3, // сила тока анода
            Za = 1; // Глубина точечного анода

        public Vector3<double> pos { get; private set; }

        public Anod()
        {
        }

        public void Init(int L)
        {
            pos = new Vector3<double>(L / 2, 0, Za);
        }
    }

    [System.Serializable]
    public class CP
    {
        public string name;
        public DateTime dateTime;

        public int L = 24000; // длина труб
        public int Nfi = 100; // кол-во ФИ трубы

        public List<Pipe> Pipes = new List<Pipe>(); // трубы
        public Anod anod = new Anod(); // анод
        public double Z0 { get; private set; } // общее входное сопротивление, Ом

        // ======================== ГРУНТ =================================
        public double ro_g = 500; // Уд сопр грунта, Ом*м: 5..10000
        public double Sigma_g { get; private set; } // Удельная электропроводность грунта

        // ======================== ПРОЧЕЕ ================================

        int slauBlockTSize; // размерность СЛАУ
        int slauSize; // размер блока уравнений для 1 трубы;

        LinearSystem resultSlau; // полученная матрица

        void Init()
        {
            dateTime = DateTime.Now;
            Sigma_g = 1 / ro_g;
            slauSize = 5 * Nfi * Pipes.Count - Pipes.Count;
            slauBlockTSize = 5 * Nfi - 1;

            anod.Init(L); // инициализация анода

            for (int i = 0; i < Pipes.Count; i++)
            {
                Pipes[i].Init(L, Nfi); // инциализация и рассчет параметров трубы
            }

            Z0 = 0;

            for (int i = 0; i < Pipes.Count; i++)
            {
                Z0 += 1d / Pipes[i].Zt;
            }

            Z0 = 1d / Z0;

            for (int i = 0; i < Pipes.Count; i++)
            {
                Pipes[i].Its = (anod.I0 * Z0) / Pipes[i].Zt;
            }
        }

        public CP()
        {
        }

        public void Solve()
        {
            Init();

            double[,] A = new double[slauSize, slauSize]; // матрица с коэффицентами системы уравенний
            double[] B = new double[slauSize]; // вектор и с изветными параметрами

            int isk = (int) Math.Floor(Nfi / 2d) + 1; // номер ФИ в точке подклюения к катодной станции

            for (int k = 0; k < Pipes.Count; k++) // k - номер трубы
            {
                int slauBlock =
                    slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k

                int ItkgY = slauBlock; // нач. коорд. блока уравнений Itkg
                int ItkxY = slauBlock + Nfi; // нач. коорд. блока уравнений Itkx
                int UtkgY = slauBlock + 2 * Nfi - 1; // нач. коорд. блока уравнений Utkg
                int UtkmY = slauBlock + 3 * Nfi - 1; // нач. коорд. блока уравнений Utkm
                int Utpr = slauBlock + 4 * Nfi - 1; // нач. коорд. блока уравнений Utpr

                int slau1X = slauBlock; // нач. коорд. блока уравнений по X
                int slau2X = slauBlock + Nfi; // нач. коорд. блока уравнений по X
                int slau3X = slauBlock + 2 * Nfi; // нач. коорд. блока уравнений по X
                int slau4X = slauBlock + 3 * Nfi - 1; // нач. коорд. блока уравнений по X
                int slau5X = slauBlock + 4 * Nfi - 1; // нач. коорд. блока уравнений по X

                //++++++++++++++++++ 1 блок уравнений ++++++++++++++++++++++
                //Законы Киргофа для ФИ по трубе
                //---------------при i=1-----------------------------------
                A[slau1X + 1, ItkgY + 1] = 1; //Itkg1
                A[slau1X + 1, ItkxY + 1] = -1; //Itkx1

                //---------------при i=2,…,isk-1---------------------------
                for (int i = 2; i <= isk - 1; i++)
                {
                    A[slau1X + i, ItkgY + i] = 1; //Itkg,i
                    A[slau1X + i, ItkxY + i - 1] = 1; //Itkx,i-1
                    A[slau1X + i, ItkxY + i] = -1; //Itkx,i
                }

                //---------------при i=isk--------------------------------


                for (int j = 0; j < Pipes.Count; j++)
                {
                    if (j == k)
                    {
                        A[slauBlock + isk, slauBlockTSize * j - 1 + isk] = 1; //Itkg,isk
                        A[slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk - 1] = 1; //Itkx,isk-1
                        A[slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk] = 1; //Itkx,isk
                    }
                }

                B[slau1X + isk] = Pipes[k].Its;

                //---------------при i=isk+1,…,N-1------------------------
                for (int i = isk + 1; i <= Nfi - 1; i++)
                {
                    A[slau1X + i, ItkgY + i] = 1; //Itkg,i
                    A[slau1X + i, ItkxY + i - 1] = -1; //Itkx,i-1
                    A[slau1X + i, ItkxY + i] = 1; //Itkx,i
                }

                //---------------при i=N-----------------------------------
                A[slau1X + Nfi, ItkgY + Nfi] = 1; //Itkg,N
                A[slau1X + Nfi, ItkxY + Nfi - 1] = -1; //Itkx,N-1


                //++++++++++++++++++ 2 блок уравнений ++++++++++++++++++++++
                //Граничные условия 3 рода

                for (int i = 1; i <= Nfi; i++)
                {
                    A[slau2X + i, ItkgY + i] = -Pipes[k].Rct;
                    A[slau2X + i, UtkgY + i] = 1; //Utkg,i
                    A[slau2X + i, UtkmY + i] = -1; //Utkm,i
                }

                //++++++++++++++++++ 3 блок уравнений ++++++++++++++++++++++
                //Закон Ома между соседними фиктивными источниками

                for (int i = 1; i <= Nfi; i++)
                {
                    if (i < isk)
                    {
                        A[slau3X + i, UtkmY + i + 1] = 1; //Utkm, i+1
                        A[slau3X + i, UtkmY + i] = -1; //Utkg,i
                        A[slau3X + i, ItkxY + i] = Pipes[k].RproT; //Itkx,i
                    }
                    //else if (i == isk)
                    //{
                    //	double sopr = 0;

                    //	for (int m = 0; m < Pipes.Count; m++)
                    //	{
                    //		sopr += 1d / Pipes[m].RproT;
                    //	}

                    //	sopr = 1d / sopr;

                    //	MessageBox.Show(Pipes[k].RproT + "  " + sopr.ToString());

                    //	A[slau3X + i, UtkmY + i + 1] = -1;
                    //	A[slau3X + i, UtkmY + i] = 1;
                    //	A[slau3X + i, ItkxY + i] = sopr;  //Itkx,i
                    //}
                    else
                    {
                        A[slau3X + i, UtkmY + i + 1] = -1; //Utkm, i+1
                        A[slau3X + i, UtkmY + i] = 1; //Utkg,i
                        A[slau3X + i, ItkxY + i] = Pipes[k].RproT; //Itkx,i
                    }
                }

                //++++++++++++++++++ 4 блок уравнений ++++++++++++++++++++++
                //Выражения для принципа электростатической аналогии

                for (int i = 1; i <= Nfi; i++)
                {
                    A[slau4X + i, UtkgY + i] = 4 * Math.PI * Sigma_g;

                    for (int l = 0; l < Pipes.Count; l++)
                    {
                        for (int j = 1; j <= Nfi; j++)
                        {
                            double distance = Pipes[k].getDistanceBetweenPoint(i - 1, Pipes[l].FIs[j - 1]);

                            if (distance > 1e-16)
                                A[slau4X + i, slauBlockTSize * l - 1 + i] += 1d / distance;
                        }
                    }

                    B[slau4X + i] = anod.I0 / Pipes[k].getDistanceBetweenPoint(i - 1, anod.pos);
                }

                //++++++++++++++++++ 5 блок уравнений ++++++++++++++++++++++
                //Защитный потенциал
                for (int i = 1; i <= Nfi; i++)
                {
                    A[slau5X + i, Utpr + i] = 1; //Utpr
                    A[slau5X + i, UtkgY + i] = -1; //Utkg,i
                    A[slau5X + i, UtkmY + i] = 1; //Utkm,i
                }
            }

            resultSlau = new LinearSystem(A, B);
        }

        double[] getSlauResult(int startCoord, int size, int mutiplayer = 1)
        {
            double[] outResult = new double[size];

            for (int i = 0; i < size; i++)
            {
                outResult[i] = mutiplayer * resultSlau.XVector[startCoord + i];
            }

            return outResult;
        }

        public double[] getX(int size)
        {
            double[] outResult = new double[size];

            double Lt = (double) L / size;

            for (int i = 1; i <= size; i++)
            {
                outResult[i - 1] = (int) (Lt * i);
            }

            return outResult;
        }

        public double[][] getItg(int k)
        {
            //int ItkgY = slauBlock; // нач. коорд. блока уравнений Itkg
            //int ItkxY = slauBlock + Nfi; // нач. коорд. блока уравнений Itkx
            //int UtkgY = slauBlock + 2 * Nfi - 1; // нач. коорд. блока уравнений Utkg
            //int UtkmY = slauBlock + 3 * Nfi - 1; // нач. коорд. блока уравнений Utkm
            //int Utpr = slauBlock + 4 * Nfi - 1; // нач. коорд. блока уравнений Utpr

            int startCoord = slauBlockTSize * k + 1;
            int size = Nfi - 2;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getItx(int k)
        {
            int startCoord = slauBlockTSize * k + Nfi;
            int size = Nfi - 1;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtg(int k)
        {
            int startCoord = slauBlockTSize * k + 2 * Nfi;
            int size = Nfi - 2;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            //XY[1] = getSlauResult(startCoord, size, -1);
            XY[1] = getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtm(int k)
        {
            int startCoord = slauBlockTSize * k + 3 * Nfi;
            int size = Nfi - 2;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtpr(int k)
        {
            int startCoord = slauBlockTSize * k + 4 * Nfi;
            int size = Nfi - 2;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            //XY[1] = getSlauResult(startCoord, size, -1);
            XY[1] = getSlauResult(startCoord, size);
            return XY;
        }

        //public double[] getIts(int k)
        //{
        //	int startCoord = slauBlockTSize * k + 5 * Nfi-1;
        //	int size = 1;
        //	return getSlauResult(startCoord, size);
        //}
    }
}