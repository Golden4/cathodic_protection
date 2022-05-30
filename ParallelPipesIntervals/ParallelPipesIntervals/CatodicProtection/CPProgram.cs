using System;
using System.Collections.Generic;
using MiscUtil;
using ParallelPipesIntervals.Core;

namespace ParallelPipesIntervals
{
    [Serializable]
    //ТРУБА
    public class Pipe<T>
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
        public T[] CtR;
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
            CtR = new T[Nfi];
            var x1 = new double[6]
            {
                // 0, L / 2, L
                0, 5000, 5100, 6000, 14000, L
            };
            var y1 = new Interval[6] {
                // new Interval(3000, 5000), new Interval(2000, 2500), new Interval(10000, 10000)
                // 200, 200, 200
                // 10000, 20000, 20000, 10000, 150000
                new Interval(29500, 30500),
                new Interval(100000, 101000),
                new Interval(100000, 100000),
                new Interval(30000, 30000),
                new Interval(30000, 30000),
                new Interval(10000, 10500)
            };
            var x = new double[2]
            {
                0, L
            };
            var y = new Interval[2] {
                new Interval(16000, 16100), new Interval(16000, 16100)
            };
            for (int i = 0; i < Nfi; i++)
            {
                CtR[i] = (T)(dynamic)Interpolation.LinearInterpolation(i * L / (Nfi - 1), x1, y1);
            }
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
            Za = 1, // Глубина точечного анода
            Xa = 10000;

        public Vector3<double> pos { get; private set; }

        public Anod()
        {
        }

        public void Init(int L)
        {
            pos = new Vector3<double>(L / 2, 0, Za);
            // pos = new Vector3<double>(Xa, 0, Za);
        }
    }

    [Serializable]
    public class CP<T> where T: struct
    {
        public string name;
        public DateTime dateTime;

        public int L = 24000; // длина труб
        public int Nfi = 100; // кол-во ФИ трубы

        public List<Pipe<T>> Pipes = new List<Pipe<T>>(); // трубы
        public List<Anod> Anods = new List<Anod>(); // Аноды
        public Anod anod = new Anod(); // анод
        public double Z0 { get; private set; } // общее входное сопротивление, Ом

        // ======================== ГРУНТ =================================
        public double ro_g = 500; // Уд сопр грунта, Ом*м: 5..10000
        public double Sigma_g { get; private set; } // Удельная электропроводность грунта
        
        public T[] ro_g_r { get; private set; } // Удельная электропроводность грунта

        // ======================== ПРОЧЕЕ ================================

        int slauBlockTSize; // размерность СЛАУ
        int slauSize; // размер блока уравнений для 1 трубы;

        private T[,] A;
        private T[] B;

        LinearSystemInterval<T> resultSlau; // полученная матрица

        void Init()
        {
            dateTime = DateTime.Now;
            Sigma_g = 1 / ro_g;
            ro_g_r = new T[Nfi];
            // var x = new double[4]
            // {
            //     0, L / 4, L / 2, L
            // };
            var x = new double[2]
            {
                0, L
            };
            var y = new Interval[2] {
                new Interval(160, 240), new Interval(160, 240)
            };
   
            // var y = new double[4] {
            //     100000, 100, 1000, 50000
            // };
            for (int i = 0; i < Nfi; i++)
            {
                ro_g_r[i] = Operator.Convert<Interval, T>(Interpolation.LinearInterpolation(i * L / (Nfi - 1), x, y));
            }
            slauBlockTSize = 5 * Nfi - 1;
            slauSize = Pipes.Count * slauBlockTSize;

            anod.Init(L); // инициализация анода

            // Anods.Add(new Anod()
            // {
            //     I0 = 1,
            //     Za = 1,
            //     Xa = 5000
            // });
            // Anods.Add(new Anod()
            // {
            //     I0 = 1,
            //     Za = 0.5,
            //     Xa = 20000
            // });
            Anods.Add(anod);

            foreach (var anode in Anods)
            {
                anode.Init(L);
            }

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

            A = new T[slauSize, slauSize]; // матрица с коэффицентами системы уравенний
            B = new T[slauSize]; // вектор и с изветными параметрами

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
                SetMatA(slau1X + 1, ItkgY + 1, 1); //Itkg1
                SetMatA(slau1X + 1, ItkxY + 1, -1); //Itkx1

                //---------------при i=2,…,isk-1---------------------------
                for (int i = 2; i <= isk - 1; i++)
                {
                    SetMatA(slau1X + i, ItkgY + i, 1); //Itkg,i
                    SetMatA(slau1X + i, ItkxY + i - 1, 1); //Itkx,i-1
                    SetMatA(slau1X + i, ItkxY + i, -1); //Itkx,i
                }

                //---------------при i=isk--------------------------------

                for (int j = 0; j < Pipes.Count; j++)
                {
                    if (j == k)
                    {
                        SetMatA(slauBlock + isk, slauBlockTSize * j - 1 + isk, 1); //Itkg,isk
                        SetMatA(slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk - 1, 1); //Itkx,isk-1
                        SetMatA(slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk, 1); //Itkx,isk
                    }
                }

                SetVecB(slau1X + isk, Anods[k].I0);// Pipes[k].Its);

                //---------------при i=isk+1,…,N-1------------------------
                for (int i = isk + 1; i <= Nfi - 1; i++)
                {
                    SetMatA(slau1X + i, ItkgY + i, 1); //Itkg,i
                    SetMatA(slau1X + i, ItkxY + i - 1, -1); //Itkx,i-1
                    SetMatA(slau1X + i, ItkxY + i, 1); //Itkx,i
                }

                //---------------при i=N-----------------------------------
                SetMatA(slau1X + Nfi, ItkgY + Nfi, 1); //Itkg,N
                SetMatA(slau1X + Nfi, ItkxY + Nfi - 1, -1); //Itkx,N-1

                //++++++++++++++++++ 2 блок уравнений ++++++++++++++++++++++
                //Граничные условия 3 рода

                for (int i = 1; i <= Nfi; i++)
                {
                    SetMatA(slau2X + i, ItkgY + i,
                        Operator.DivideAlternative(Operator.Negate(Pipes[k].CtR[i - 1]), Pipes[k].St));
                    SetMatA(slau2X + i, UtkgY + i, 1); //Utkg,i
                    SetMatA(slau2X + i, UtkmY + i, -1); //Utkm,i
                }

                //++++++++++++++++++ 3 блок уравнений ++++++++++++++++++++++
                //Закон Ома между соседними фиктивными источниками

                for (int i = 1; i <= Nfi; i++)
                {
                    if (i < isk)
                    {
                        SetMatA(slau3X + i, UtkmY + i + 1, 1); //Utkm, i+1
                        SetMatA(slau3X + i, UtkmY + i, -1); //Utkg,i
                        SetMatA(slau3X + i, ItkxY + i, Pipes[k].RproT); //Itkx,i
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
                        SetMatA(slau3X + i, UtkmY + i + 1, -1); //Utkm, i+1
                        SetMatA(slau3X + i, UtkmY + i, 1); //Utkg,i
                        SetMatA(slau3X + i, ItkxY + i, Pipes[k].RproT); //Itkx,i
                    }
                }

                //++++++++++++++++++ 4 блок уравнений ++++++++++++++++++++++
                //Выражения для принципа электростатической аналогии

                for (int i = 1; i <= Nfi; i++)
                {
                    SetMatA(slau4X + i, UtkgY + i, Operator.DivideAlternative(Operator.Convert<double, T>(4 * Math.PI), ro_g_r[i - 1]));

                    for (int l = 0; l < Pipes.Count; l++)
                    {
                        for (int j = 1; j <= Nfi; j++)
                        {
                            double distance = Pipes[k].getDistanceBetweenPoint(i - 1, Pipes[l].FIs[j - 1]);

                            if (distance > 1e-16)
                                SetMatA(slau4X + i, slauBlockTSize * l - 1 + i,
                                    Operator.AddAlternative(A[slau4X + i, slauBlockTSize * l - 1 + i], 1d / distance));
                        }
                    }

                    for (int l = 0; l < Anods.Count; l++)
                    {
                        SetVecB(slau4X + i, Anods[l].I0 / Pipes[k].getDistanceBetweenPoint(i - 1, Anods[l].pos));
                    }
                }

                //++++++++++++++++++ 5 блок уравнений ++++++++++++++++++++++
                //Защитный потенциал
                for (int i = 1; i <= Nfi; i++)
                {
                    SetMatA(slau5X + i, Utpr + i, 1); //Utpr
                    SetMatA(slau5X + i, UtkgY + i, -1); //Utkg,i
                    SetMatA(slau5X + i, UtkmY + i, 1); //Utkm,i
                }
            }

            resultSlau = new LinearSystemInterval<T>(A, B);
        }

        void SetMatA(int slauIndex, int blockIndex, double value)
        {
            SetMatA(slauIndex, blockIndex, Operator.Convert<double, T>(value));
        }

        void SetMatA(int slauIndex, int blockIndex, T value)
        {
            A[slauIndex, blockIndex] = value;
        }

        void SetVecB(int index, double value)
        {
            B[index] = Operator.Convert<double, T>(value);
        }

        T[] getSlauResult(int startCoord, int size, int mutiplayer = 1)
        {
            T[] outResult = new T[size];
            for (int i = 0; i < size; i++)
            {
                outResult[i] = Operator.MultiplyAlternative(resultSlau.XVector[startCoord + i], (double)mutiplayer);
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

        public (double[] x, T[] y) getItg(int k)
        {
            int startCoord = slauBlockTSize * k + 1;
            int size = Nfi - 2;
            return (getX(size), getSlauResult(startCoord, size));
        }

        public (double[] x, T[] y) getItx(int k)
        {
            int startCoord = slauBlockTSize * k + Nfi;
            int size = Nfi - 1;
            return (getX(size), getSlauResult(startCoord, size));
        }

        public (double[] x, T[] y) getUtg(int k)
        {
            int startCoord = slauBlockTSize * k + 2 * Nfi;
            int size = Nfi - 2;
            return (getX(size), getSlauResult(startCoord, size));
        }

        public (double[] x, T[] y) getUtm(int k)
        {
            int startCoord = slauBlockTSize * k + 3 * Nfi;
            int size = Nfi - 2;
            return (getX(size), getSlauResult(startCoord, size));
        }

        public (double[] x, T[] y) getUtpr(int k)
        {
            int startCoord = slauBlockTSize * k + 4 * Nfi;
            int size = Nfi - 2;
            return (getX(size), getSlauResult(startCoord, size));
        }

        //public double[] getIts(int k)
        //{
        //	int startCoord = slauBlockTSize * k + 5 * Nfi-1;
        //	int size = 1;
        //	return getSlauResult(startCoord, size);
        //}
    }
}