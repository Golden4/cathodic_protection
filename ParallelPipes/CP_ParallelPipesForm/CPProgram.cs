using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP_ParallelPipesForm.Core;

namespace CP_ParallelPipesForm
{
    [System.Serializable]
    public class Vector3
    {
        public double x;
        public double y;
        public double z;

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static double Distance(Vector3 vec1, Vector3 vec2)
        {
            var vecDiffX = (vec1.x - vec2.x);
            var vecDiffY = (vec1.y - vec2.y);
            var vecDiffZ = (vec1.z - vec2.z);
            double x1 = vecDiffX * vecDiffX;
            double y1 = vecDiffY * vecDiffY;
            double z1 = vecDiffZ * vecDiffZ;
            vecDiffX = 0;
            vecDiffY = 0;
            vecDiffZ = 0;
            double dist = Math.Sqrt(x1 + y1 + z1);
            return dist;
        }
    }


    [System.Serializable]
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
        public double Sigma_t { get; private set; } // Удельная электропроводность металла трубы
        public double Lfi { get; private set; } // длина одной части ФИ трубы
        public double St { get; private set; } // площади боковых поверхностей
        public Vector3[] FIs; // координаты ФИ

        // ======================== ИЗОЛЯЦИЯ ТРУБЫ ========================
        public double Ct = 300000; // уд. сопротивление изоляции трубы, Ом*м2
        public double[] CtX;
        public Interval[] CtIntervals;
        public Interval[] iCt;
        public double Rct { get; private set; } // сопротивление изоляции трубы, Ом*м

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

            FIs = new Vector3[Nfi];

            for (int i = 0; i < Nfi; i++)
            {
                FIs[i] = new Vector3((i * Lfi + Lfi / 2), Lta, -(Ht + Rt2));
            }

            iCt = new Interval[Nfi];
            if (CtIntervals == null)
            {
                CtX = new double[2]
                {
                    0, L
                };
                CtIntervals = new Interval[2]
                {
                    new Interval(Ct, Ct),
                    new Interval(Ct, Ct)
                };
            }

            for (int i = 0; i < Nfi; i++)
            {
                iCt[i] = Interpolation.LinearInterpolation((double) i * L / (Nfi - 1), CtX, CtIntervals);
            }
        }

        public double getDistanceBetweenFIs(int fiIndex1, int fiIndex2) // расстояние м/у фиктивными источниками
        {
            return Vector3.Distance(FIs[fiIndex1], FIs[fiIndex2]);
        }

        public double
            getDistanceBetweenPoint(int fiIndex, Vector3 point) // расстояние м/у фиктивным источником и точкой
        {
            return Vector3.Distance(FIs[fiIndex], point);
        }
    }

    [System.Serializable]
    //Анод
    public class Anod
    {
        public double
            I0 = 0.3, // сила тока анода
            Za = 1; // Глубина точечного анода

        public Vector3 pos { get; private set; }

        public void Init(int L)
        {
            pos = new Vector3(L / 2, 0, -Za);
        }
    }

    [System.Serializable]
    public class CP
    {
        public string name;
        public DateTime dateTime;
        public bool useIntervals;

        public int
            L = 24000, // длина труб
            Nfi = 49; // кол-во ФИ трубы

        private double MaxLta; // Расстоние до самой удалененной точки трубы

        public List<Pipe> Pipes = new List<Pipe>(); // трубы
        public Anod anod = new Anod(); // анод

        // ======================== ГРУНТ =================================
        public double ro_g = 500; // Уд сопр грунта, Ом*м: 5..10000
        public Interval iRoG = new Interval(500, 500);
        public double Sigma_g { get; private set; } // Удельная электропроводность грунта

        // ======================== ПРОЧЕЕ ================================

        int slauBlockTSize; // размерность СЛАУ
        int slauSize; // размер блока уравнений для 1 трубы;

        LinearSystem resultSlau; // полученная матрица
        private Interval[][] UtprIntervals;
        private Interval[][] ItgIntervals;

        void Init()
        {
            dateTime = DateTime.Now;
            Sigma_g = 1 / ro_g;
            slauSize = 5 * Nfi * Pipes.Count;
            slauBlockTSize = 5 * Nfi;

            anod.Init(L); // инициализация анода
            for (int i = 0; i < Pipes.Count; i++)
            {
                Pipes[i].Init(L, Nfi); // инциализация и рассчет параметров трубы
                if (Pipes[i].Lta + Pipes[i].Rt2 > MaxLta)
                {
                    MaxLta = Pipes[i].Lta + Pipes[i].Rt2;
                }
            }
        }

        public CP()
        {
        }

        public void Solve()
        {
            Init();
            double[][] CtMid = new double[Nfi][];
            for (int m = 0; m < Pipes.Count; m++)
            {
                CtMid[m] = new double[Nfi];
                for (int i = 0; i < Nfi; i++)
                {
                    CtMid[m][i] = Pipes[m].iCt[i].Mid();
                }
            }
            var SigmaGr = 1d / iRoG;
            var (A, B) = getSlau(SigmaGr.Mid(), CtMid);
            resultSlau = new LinearSystem(A, B);
            if (useIntervals)
            {
                Interval[][] CtPipes = new Interval[Pipes.Count][];
                for (int i = 0; i < CtPipes.Length; i++)
                {
                    CtPipes[i] = Pipes[i].iCt;
                }

                UtprIntervals = getUtprIntervals(SigmaGr, CtPipes);
                ItgIntervals = getItgIntervals(SigmaGr, CtPipes);
            }
        }

        public Interval[][] getUtprIntervals(Interval SigmaG, Interval[][] Ct)
        {
            Interval[][] Utpr = new Interval[Pipes.Count][];
            double[][] curCt = new double[Pipes.Count][];
            for (int p = 0; p < Pipes.Count; p++)
            {
                Utpr[p] = new Interval[Nfi];
                curCt[p] = new double[Nfi];
                for (int j = 0; j < Nfi; j++)
                {
                    Utpr[p][j] = new Interval();
                    Utpr[p][j].x1 = double.MaxValue;
                    Utpr[p][j].x2 = double.MinValue;
                    curCt[p][j] = Ct[p][j].x2;
                }
            }

            double curSigma = SigmaG.x2;
            double hSigma = SigmaG.x2 - SigmaG.x1;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var (A, B) = getSlau(curSigma, curCt);
                    var linearSystem = new LinearSystem(A, B);
                    for (int p = 0; p < Pipes.Count; p++)
                    {
                        var rsUtpr = linearSystem.getSlauResult(getStartCoordUtpr(p), Nfi);
                        for (int l = 0; l < Nfi; l++)
                        {
                            if (rsUtpr[l] < Utpr[p][l].x1)
                            {
                                Utpr[p][l].x1 = rsUtpr[l];
                            }

                            if (rsUtpr[l] > Utpr[p][l].x2)
                            {
                                Utpr[p][l].x2 = rsUtpr[l];
                            }
                        }
                    }

                    curSigma += hSigma;
                }

                for (int p = 0; p < Pipes.Count; p++)
                {
                    for (int l = 0; l < Nfi; l++)
                    {
                        double hCt = Ct[p][l].x2 - Ct[p][l].x1;
                        curCt[p][l] += hCt;
                    }
                }

                curSigma = SigmaG.x1;
            }

            return Utpr;
        }

        public Interval[][] getItgIntervals(Interval SigmaG, Interval[][] Ct)
        {
            Interval[][] Itg = new Interval[Pipes.Count][];
            for (int p = 0; p < Pipes.Count; p++)
            {
                Itg[p] = new Interval[Nfi];
                for (int j = 0; j < Nfi; j++)
                {
                    Itg[p][j] = new Interval();
                    Itg[p][j].x1 = double.MaxValue;
                    Itg[p][j].x2 = double.MinValue;
                }
            }

            Parallel.For(0, Nfi, (i) =>
            {
                double curSigma = SigmaG.x1;
                double[][] curCt = new double[Pipes.Count][];
                for (int p = 0; p < Pipes.Count; p++)
                {
                    curCt[p] = new double[Nfi];
                    for (int j = 0; j < Nfi; j++)
                    {
                        curCt[p][j] = Ct[p][j].x1;
                        if (i == j)
                        {
                            curCt[p][j] = Ct[p][j].x2;
                        }
                    }
                }

                for (int j = 0; j < 2; j++)
                {
                    var (A, B) = getSlau(curSigma, curCt);
                    var linearSystem = new LinearSystem(A, B);
                    A = null;
                    B = null;
                    for (int p = 0; p < Pipes.Count; p++)
                    {
                        var rsItg = linearSystem.getSlauResult(getStartCoordItkgY(p), Nfi);
                        for (int l = 0; l < Nfi; l++)
                        {
                            if (rsItg[l] < Itg[p][l].x1)
                            {
                                Itg[p][l].x1 = rsItg[l];
                            }

                            if (rsItg[l] > Itg[p][l].x2)
                            {
                                Itg[p][l].x2 = rsItg[l];
                            }
                        }
                    }

                    curSigma = SigmaG.x2;
                }

                curSigma = SigmaG.x1;
                curCt = new double[Pipes.Count][];
                for (int p = 0; p < Pipes.Count; p++)
                {
                    curCt[p] = new double[Nfi];
                    for (int j = 0; j < Nfi; j++)
                    {
                        curCt[p][j] = Ct[p][j].x2;
                        if (i == j)
                        {
                            curCt[p][j] = Ct[p][j].x1;
                        }
                    }
                }

                for (int j = 0; j < 2; j++)
                {
                    var (A, B) = getSlau(curSigma, curCt);
                    var linearSystem = new LinearSystem(A, B);
                    for (int p = 0; p < Pipes.Count; p++)
                    {
                        var rsItg = linearSystem.getSlauResult(getStartCoordItkgY(p), Nfi);
                        for (int l = 0; l < Nfi; l++)
                        {
                            if (rsItg[l] < Itg[p][l].x1)
                            {
                                Itg[p][l].x1 = rsItg[l];
                            }

                            if (rsItg[l] > Itg[p][l].x2)
                            {
                                Itg[p][l].x2 = rsItg[l];
                            }
                        }
                    }

                    curSigma = SigmaG.x2;
                }
            });

            return Itg;
        }

        public (double[,], double[]) getSlau(double SigmaG, double[][] Ct)
        {
            double[,] A = new double[slauSize, slauSize]; // матрица с коэффицентами системы уравенний
            double[] B = new double[slauSize]; // вектор и с изветными параметрами

            int isk = (int) Math.Floor(Nfi / 2d) + 1; // номер ФИ в точке подклюения к катодной станции

            for (int k = 0; k < Pipes.Count; k++) // k - номер трубы
            {
                int slauBlock =
                    slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k

                int ItkgY = getStartCoordItkgY(k); // нач. коорд. блока уравнений Itkg
                int ItkxY = getStartCoordItkxY(k); // нач. коорд. блока уравнений Itkx
                int UtkgY = getStartCoordUtkgY(k); // нач. коорд. блока уравнений Utkg
                int UtkmY = getStartCoordUtkmY(k); // нач. коорд. блока уравнений Utkm
                int Utpr = getStartCoordUtpr(k); // нач. коорд. блока уравнений Utpr
                int Is = getStartCoordIts(k); // нач. коорд. блока уравнений Is

                int slau1X = slauBlock; // нач. коорд. блока уравнений по X
                int slau2X = slauBlock + Nfi; // нач. коорд. блока уравнений по X
                int slau3X = slauBlock + 2 * Nfi; // нач. коорд. блока уравнений по X
                int slau4X = slauBlock + 3 * Nfi - 1; // нач. коорд. блока уравнений по X
                int slau5X = slauBlock + 4 * Nfi - 1; // нач. коорд. блока уравнений по X
                int slau6X = slauBlock + 5 * Nfi; // нач. коорд. блока уравнений по X

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

                A[slau1X + isk, ItkgY + isk] = 1; //Itkg,isk
                A[slau1X + isk, ItkxY + isk - 1] = 1; //Itkx,isk-1
                A[slau1X + isk, ItkxY + isk] = 1; //Itkx,isk
                A[slau1X + isk, Is] = -1;

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

                // Сумма токов в точке дренажа
                if (k == 0)
                {
                    for (int i = 0; i < Pipes.Count; i++)
                    {
                        A[slau6X, getStartCoordIts(i)] = 1;
                    }
                    B[slau6X] = anod.I0;
                }

                //++++++++++++++++++ 2 блок уравнений ++++++++++++++++++++++
                //Граничные условия 3 рода

                for (int i = 1; i <= Nfi; i++)
                {
                    A[slau2X + i, ItkgY + i] = -Ct[k][i - 1] / Pipes[k].St; //Pipes[k].Rct;
                    A[slau2X + i, UtkgY + i] = 1; //Utkg,i
                    A[slau2X + i, UtkmY + i] = -1; //Utkm,i
                }

                //++++++++++++++++++ 3 блок уравнений ++++++++++++++++++++++
                //Закон Ома между соседними фиктивными источниками

                for (int i = 1; i <= Nfi - 1; i++)
                {
                    if (i < isk)
                    {
                        A[slau3X + i, UtkmY + i + 1] = 1; //Utkm, i+1
                        A[slau3X + i, UtkmY + i] = -1; //Utkg,i
                        A[slau3X + i, ItkxY + i] = Pipes[k].RproT; //Itkx,i
                    }
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
                    A[slau4X + i, UtkgY + i] = 4 * Math.PI * SigmaG;

                    for (int l = 0; l < Pipes.Count; l++)
                    {
                        for (int j = 1; j <= Nfi; j++)
                        {
                            if (k != l || i != j)
                            {
                                var dist = MirrorReflectionDistance(Pipes[k].FIs[i - 1], Pipes[l].FIs[j - 1]);
                                A[slau4X + i, getStartCoordItkgY(k) + i] += dist;
                            }
                        }
                    }

                    var distA = MirrorReflectionDistance(anod.pos, Pipes[k].FIs[i - 1]);
                    B[slau4X + i] = anod.I0 * distA;
                }

                //++++++++++++++++++ 5 блок уравнений ++++++++++++++++++++++
                //Защитный потенциал
                for (int i = 1; i <= Nfi; i++)
                {
                    A[slau5X + i, Utpr + i] = 1; //Utpr
                    A[slau5X + i, UtkgY + i] = -1; //Utkg,i
                    A[slau5X + i, UtkmY + i] = 1; //Utkm,i
                }

                //++++++++++++++++++ 6 блок уравнений ++++++++++++++++++++++
                // 2 закон Кирхгорфа в точках подключения к катодной станции
                if (k > 0)
                {
                    A[slau6X, UtkmY + isk] = 1;
                    A[slau6X, UtkgY + isk] = 1;
                    A[slau6X, getStartCoordUtkmY(k - 1) + isk] = -1;
                    A[slau6X, getStartCoordUtkgY(k - 1) + isk] = -1;
                }
            }

            return (A, B);
        }
        // Учет зеркальных ФИ
        double MirrorReflectionDistance(Vector3 p1, Vector3 p2)
        {
            // ФИ зерк от-но x=0, x=L
            double[] xVariants = new double[3] {p2.x, -p2.x, 2 * L - p2.x};
            double[] yVariants = new double[1] {p2.y}; // 2 * MaxLta - p2.y
            // ФИ зерк от-но z=0
            double[] zVariants = new double[2] {p2.z, -p2.z};
            double sum = 0;
            for (int xi = 0; xi < xVariants.Length; xi++)
            {
                for (int yi = 0; yi < yVariants.Length; yi++)
                {
                    for (int zi = 0; zi < zVariants.Length; zi++)
                    {
                        var x = xVariants[xi];
                        var y = yVariants[yi];
                        var z = zVariants[zi];
                        Vector3 vec3 = new Vector3(x, y, z);
                        double dist = Vector3.Distance(p1, vec3);
                        sum += 1d / dist;
                    }
                }
            }
            return sum;
        }

        public int getStartCoordItkgY(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock;
        }

        public int getStartCoordItkxY(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock + Nfi;
        }

        public int getStartCoordUtkgY(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock + 2 * Nfi - 1;
        }

        public int getStartCoordUtkmY(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock + 3 * Nfi - 1;
        }

        public int getStartCoordUtpr(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock + 4 * Nfi - 1;
        }

        public int getStartCoordIts(int k)
        {
            int slauBlock =
                slauBlockTSize * k - 1; // начальная координата блока уравений по вертикали для трубы с номером k
            return slauBlock + 5 * Nfi;
        }

        public double[] getX(int size)
        {
            double[] outResult = new double[size];

            double Lt = (double) L / (size - 1);

            for (int i = 1; i <= size; i++)
            {
                outResult[i - 1] = (int) (Lt * (i - 1));
            }

            return outResult;
        }

        public double[][] getItg(int k)
        {
            int startCoord = getStartCoordItkgY(k);
            int size = Nfi;
            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = resultSlau.getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getItx(int k)
        {
            int startCoord = getStartCoordItkxY(k);
            int size = Nfi - 1;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = resultSlau.getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtg(int k)
        {
            int startCoord = getStartCoordUtkgY(k);
            int size = Nfi;
            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = resultSlau.getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtm(int k)
        {
            int startCoord = getStartCoordUtkmY(k);
            int size = Nfi;

            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = resultSlau.getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getUtpr(int k)
        {
            int startCoord = getStartCoordUtpr(k);
            int size = Nfi;
            double[][] XY = new double[2][];
            XY[0] = getX(size);
            XY[1] = resultSlau.getSlauResult(startCoord, size);
            return XY;
        }

        public double[][] getJt(int k)
        {
            var Itg = getItg(k);
            var Jtg = new double[Nfi];
            for (int i = 0; i < Nfi; i++)
            {
                Jtg[i] = Itg[1][i] / Pipes[k].St;
            }

            return new double[2][] {Itg[0], Jtg};
        }
        
        public double[][] getCt(int k)
        {
            double[] CtMid = new double[Nfi];
            for (int i = 0; i < Nfi; i++)
            {
                CtMid[i] = Pipes[k].iCt[i].Mid();
            }
            return new double[2][] {getX(Nfi), CtMid};
        }

        public (double[], Interval[]) getItgInterval(int k)
        {
            int size = Nfi;
            return (getX(size), ItgIntervals[k]);
        }

        public double getIts(int k)
        {
            var its = resultSlau.XVector[getStartCoordIts(k)];
            return its;
        }

        // public (double[], Interval[]) getUtgInterval(int k)
        // {
        //     int size = Nfi;
        //     return (getX(size), iUtg[k]);
        // }
        //
        // public (double[], Interval[]) getUtmInterval(int k)
        // {
        //     int size = Nfi;
        //     return (getX(size), iUtm[k]);
        // }

        public (double[], Interval[]) getUtprInterval(int k)
        {
            int size = Nfi;
            return (getX(size), UtprIntervals[k]);
        }

        public (double[], Interval[]) getJtInterval(int k)
        {
            int size = Nfi;
            var Itg = ItgIntervals[k];
            var Jtg = new Interval[Nfi];
            for (int i = 0; i < Nfi; i++)
            {
                Jtg[i] = Itg[i] / Pipes[k].St;
            }

            return (getX(size), Jtg);
        }
        
        public (double[], Interval[]) getCtInterval(int k)
        {
            return (getX(Nfi), Pipes[k].iCt);
        }
    }

    [System.Serializable]
    public class LinearSystem
    {
        [field: NonSerializedAttribute()] private double[,] a_matrix;
        [field: NonSerializedAttribute()] private double[] b_vector;
        [field: NonSerializedAttribute()] private double eps;
        [field: NonSerializedAttribute()] private int size;

        private double[] x_vector;

        public LinearSystem(double[,] a_matrix, double[] b_vector)
            : this(a_matrix, b_vector, 1e-16)
        {
        }

        public LinearSystem(double[,] a_matrix, double[] b_vector, double eps)
        {
            int b_length = b_vector.Length;
            int a_length = a_matrix.Length;
            this.a_matrix = a_matrix;
            this.b_vector = b_vector;
            this.x_vector = new double[b_length];
            this.size = b_length;
            this.eps = eps;
            GaussSolve();
            this.a_matrix = null;
            this.b_vector = null;
        }

        public double[] XVector
        {
            get { return x_vector; }
        }

        private int[] InitIndex()
        {
            int[] index = new int[size];
            for (int i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        private double FindR(int row, int[] index)
        {
            int max_index = row;
            double max = a_matrix[row, index[max_index]];
            double max_abs = Math.Abs(max);

            for (int cur_index = row + 1; cur_index < size; ++cur_index)
            {
                double cur = a_matrix[row, index[cur_index]];
                double cur_abs = Math.Abs(cur);
                if (cur_abs > max_abs)
                {
                    max_index = cur_index;
                    max = cur;
                    max_abs = cur_abs;
                }
            }

            if (max_abs < eps)
            {
                if (Math.Abs(b_vector[row]) > eps)
                    throw new Exception("Система уравнений несовместна.");
                else
                    throw new Exception("Система уравнений имеет множество решений..");
            }

            int temp = index[row];
            index[row] = index[max_index];
            index[max_index] = temp;
            return max;
        }

        private void GaussSolve()
        {
            int[] index = InitIndex();
            GaussForwardStroke(index);
            GaussBackwardStroke(index);
        }

        private void GaussForwardStroke(int[] index)
        {
            for (int i = 0; i < size; ++i)
            {
                double r = FindR(i, index);
                for (int j = 0; j < size; ++j)
                    a_matrix[i, j] /= r;
                b_vector[i] /= r;
                Parallel.For(i + 1, size, (k) =>
                {
                    double p = a_matrix[k, index[i]];
                    for (int j = i; j < size; ++j)
                        a_matrix[k, index[j]] -= a_matrix[i, index[j]] * p;
                    b_vector[k] -= b_vector[i] * p;
                    a_matrix[k, index[i]] = 0.0;
                });
            }
        }

        private void GaussBackwardStroke(int[] index)
        {
            for (int i = size - 1; i >= 0; --i)
            {
                x_vector[index[i]] = b_vector[i];
                for (int j = i + 1; j < size; ++j)
                {
                    x_vector[index[i]] -= x_vector[index[j]] * a_matrix[i, index[j]];
                }
            }
        }

        public double[] getSlauResult(int startCoord, int size)
        {
            double[] outResult = new double[size];

            for (int i = 1; i <= size; i++)
            {
                outResult[i - 1] = XVector[startCoord + i];
            }

            return outResult;
        }
    }
}