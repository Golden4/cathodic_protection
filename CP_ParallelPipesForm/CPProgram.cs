using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
			double x1 = (vec1.x - vec2.x) * (vec1.x - vec2.x);
			double y1 = (vec1.y - vec2.y) * (vec1.y - vec2.y);
			double z1 = (vec1.z - vec2.z) * (vec1.z - vec2.z);

			return Math.Sqrt(x1 + y1 + z1);
		}
	}


	[System.Serializable]
	//�����
	public class Pipe
	{
		public string name;

		public double
		Ht = 1, // ������� �� ������ ����� �� ������� �.�, �
		ro_t = 2.45e-7, // �� ���� �����, ��*�
		Dt2 = 1.22, // ������� ������� �, �: 0.3..1.5
		Det = 0.022, // ������� ������ �, �: 0.01..0.025
		Lta = 200; // ���������� �� �����
		
		public double Rt2 { get; private set; } // ����� ������ �
		public double Rt0 { get; private set; } // ����� ������ �
		public double SechT { get; private set; } // ������� ��� ������� �
		public double RproT { get; private set; } // ���������� ������������� �����, ��/�
		public double RproT1 { get; private set; } // ���������� ������������� ����� �� ������� �����, ��*�
		public double Sigma_t { get; private set; }  // �������� ������������������ ������� �����
		public double Lfi { get; private set; }  // ����� ����� ����� �� �����
		public double St { get; private set; } // ������� ������� ������������
		public double Zt { get; private set; }  //������� ������������� ������������, ��
		public double Its; // ���, ��������� � ����� ������� 
		public Vector3[] FIs; // ���������� ��

		// ======================== �������� ����� ========================
		public double Ct = 300000; // ��. ������������� �������� �����, ��*�2
		public double Rct { get; private set; } // ������������� �������� �����, ��*�
		public double Rct1 { get; private set; } // ������������� �������� �� ������� �����, ��*�
		
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

			FIs = new Vector3[Nfi];

			for (int i = 0; i < Nfi; i++)
			{
				FIs[i] = new Vector3((i * Lfi + Lfi / 2), Lta, (Ht + Rt2));
			}
		}

		public double getDistanceBetweenFIs(int fiIndex1, int fiIndex2) // ���������� �/� ���������� �����������
		{
			return Vector3.Distance(FIs[fiIndex1], FIs[fiIndex2]);
		}

		public double getDistanceBetweenPoint(int fiIndex, Vector3 point) // ���������� �/� ��������� ���������� � ������
		{
			return Vector3.Distance(FIs[fiIndex], point);
		}
	}
	[System.Serializable]
	//����
	public class Anod
	{
		public double
		I0 = 0.3, // ���� ���� �����
		Za = 1; // ������� ��������� �����
		public Vector3 pos { get; private set; }

		public Anod()
		{
		}
		public void Init(int L)
		{
			pos = new Vector3(L / 2, 0, Za);
		}
	}
	[System.Serializable]
	public class CP
	{
		public string name;
		public DateTime dateTime;

		public int
		L = 24000, // ����� ����
		Nfi = 100; // ���-�� �� �����

		public List<Pipe> Pipes = new List<Pipe>();// �����
		public Anod anod = new Anod(); // ����
		public double Z0 { get; private set; } // ����� ������� �������������, ��

		// ======================== ����� =================================
		public double ro_g = 500; // �� ���� ������, ��*�: 5..10000
		public double Sigma_g { get; private set; } // �������� ������������������ ������

		// ======================== ������ ================================

		int slauBlockTSize;// ����������� ����
		int slauSize;// ������ ����� ��������� ��� 1 �����;

		LinearSystem resultSlau; // ���������� �������
		void Init()
		{
			dateTime = DateTime.Now;
			Sigma_g = 1 / ro_g;
			slauSize = 5 * Nfi * Pipes.Count - Pipes.Count;
			slauBlockTSize = 5 * Nfi - 1;

			anod.Init(L); // ������������� �����

			for (int i = 0; i < Pipes.Count; i++)
			{
				Pipes[i].Init(L, Nfi); // ������������ � ������� ���������� �����
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

			double[,] A = new double[slauSize, slauSize]; // ������� � ������������� ������� ���������
			double[] B = new double[slauSize]; // ������ � � ��������� �����������

			int isk = (int)Math.Floor(Nfi / 2d) + 1; // ����� �� � ����� ���������� � �������� �������

			for (int k = 0; k < Pipes.Count; k++) // k - ����� �����
			{
				int slauBlock = slauBlockTSize * k - 1; // ��������� ���������� ����� �������� �� ��������� ��� ����� � ������� k

				int ItkgY = slauBlock; // ���. �����. ����� ��������� Itkg
				int ItkxY = slauBlock + Nfi; // ���. �����. ����� ��������� Itkx
				int UtkgY = slauBlock + 2 * Nfi - 1; // ���. �����. ����� ��������� Utkg
				int UtkmY = slauBlock + 3 * Nfi - 1; // ���. �����. ����� ��������� Utkm
				int Utpr = slauBlock + 4 * Nfi - 1; // ���. �����. ����� ��������� Utpr

				int slau1X = slauBlock; // ���. �����. ����� ��������� �� X
				int slau2X = slauBlock + Nfi; // ���. �����. ����� ��������� �� X
				int slau3X = slauBlock + 2 * Nfi; // ���. �����. ����� ��������� �� X
				int slau4X = slauBlock + 3 * Nfi - 1; // ���. �����. ����� ��������� �� X
				int slau5X = slauBlock + 4 * Nfi - 1; // ���. �����. ����� ��������� �� X

				//++++++++++++++++++ 1 ���� ��������� ++++++++++++++++++++++
				//������ ������� ��� �� �� �����
				//---------------��� i=1-----------------------------------
				A[slau1X + 1, ItkgY + 1] = 1;                   //Itkg1
				A[slau1X + 1, ItkxY + 1] = -1;                  //Itkx1

				//---------------��� i=2,�,isk-1---------------------------
				for (int i = 2; i <= isk - 1; i++)
				{
					A[slau1X + i, ItkgY + i] = 1;               //Itkg,i
					A[slau1X + i, ItkxY + i - 1] = 1;           //Itkx,i-1
					A[slau1X + i, ItkxY + i] = -1;              //Itkx,i
				}

				//---------------��� i=isk--------------------------------


				for (int j = 0; j < Pipes.Count; j++)
				{

					if (j == k)
					{
						A[slauBlock + isk, slauBlockTSize * j - 1 + isk] = 1;          //Itkg,isk
						A[slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk - 1] = 1;   //Itkx,isk-1
						A[slau1X + isk, slauBlockTSize * j - 1 + Nfi + isk] = 1;       //Itkx,isk
					}
				}

				B[slau1X + isk] = Pipes[k].Its;

				//---------------��� i=isk+1,�,N-1------------------------
				for (int i = isk + 1; i <= Nfi - 1; i++)
				{
					A[slau1X + i, ItkgY + i] = 1;               //Itkg,i
					A[slau1X + i, ItkxY + i - 1] = -1;          //Itkx,i-1
					A[slau1X + i, ItkxY + i] = 1;               //Itkx,i
				}

				//---------------��� i=N-----------------------------------
				A[slau1X + Nfi, ItkgY + Nfi] = 1;     //Itkg,N
				A[slau1X + Nfi, ItkxY + Nfi - 1] = -1; //Itkx,N-1


				//++++++++++++++++++ 2 ���� ��������� ++++++++++++++++++++++
				//��������� ������� 3 ����

				for (int i = 1; i <= Nfi; i++)
				{
					A[slau2X + i, ItkgY + i] = -Pipes[k].Rct;
					A[slau2X + i, UtkgY + i] = 1;               //Utkg,i
					A[slau2X + i, UtkmY + i] = -1;              //Utkm,i
				}

				//++++++++++++++++++ 3 ���� ��������� ++++++++++++++++++++++
				//����� ��� ����� ��������� ���������� �����������

				for (int i = 1; i <= Nfi; i++)
				{
					if (i < isk)
					{
						A[slau3X + i, UtkmY + i + 1] = 1;               //Utkm, i+1
						A[slau3X + i, UtkmY + i] = -1;              //Utkg,i
						A[slau3X + i, ItkxY + i] = Pipes[k].RproT;  //Itkx,i
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
						A[slau3X + i, UtkmY + i + 1] = -1;               //Utkm, i+1
						A[slau3X + i, UtkmY + i] = 1;              //Utkg,i
						A[slau3X + i, ItkxY + i] = Pipes[k].RproT;  //Itkx,i
					}
				}
				
				//++++++++++++++++++ 4 ���� ��������� ++++++++++++++++++++++
				//��������� ��� �������� ������������������ ��������

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

				//++++++++++++++++++ 5 ���� ��������� ++++++++++++++++++++++
				//�������� ���������
				for (int i = 1; i <= Nfi; i++)
				{
					A[slau5X + i, Utpr + i] = 1;               //Utpr
					A[slau5X + i, UtkgY + i] = -1;             //Utkg,i
					A[slau5X + i, UtkmY + i] = 1;              //Utkm,i
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

			double Lt = (double)L / size;

			for (int i = 1; i <= size; i++)
			{
				outResult[i-1] = (int)(Lt * i);
			}

			return outResult;
		}
		public double[][] getItg(int k) {
			
			//int ItkgY = slauBlock; // ���. �����. ����� ��������� Itkg
			//int ItkxY = slauBlock + Nfi; // ���. �����. ����� ��������� Itkx
			//int UtkgY = slauBlock + 2 * Nfi - 1; // ���. �����. ����� ��������� Utkg
			//int UtkmY = slauBlock + 3 * Nfi - 1; // ���. �����. ����� ��������� Utkm
			//int Utpr = slauBlock + 4 * Nfi - 1; // ���. �����. ����� ��������� Utpr

			int startCoord = slauBlockTSize * k+1;
			int size = Nfi-2;

			double[][] XY = new double[2][];
			XY[0] = getX(size);
			XY[1] = getSlauResult(startCoord, size);
			return XY;
		}

		public double[][] getItx(int k)
		{
			int startCoord = slauBlockTSize * k + Nfi;
			int size = Nfi-1;

			double[][] XY = new double[2][];
			XY[0] = getX(size);
			XY[1] = getSlauResult(startCoord, size);
			return XY;
		}

		public double[][] getUtg(int k)
		{
			int startCoord = slauBlockTSize * k + 2 * Nfi;
			int size = Nfi-2;

			double[][] XY = new double[2][];
			XY[0] = getX(size);
			//XY[1] = getSlauResult(startCoord, size, -1);
			XY[1] = getSlauResult(startCoord, size);
			return XY;
		}
		public double[][] getUtm(int k)
		{
			int startCoord = slauBlockTSize * k + 3 * Nfi;
			int size = Nfi-2;

			double[][] XY = new double[2][];
			XY[0] = getX(size);
			XY[1] = getSlauResult(startCoord, size);
			return XY;
		}

		public double[][] getUtpr(int k)
		{
			int startCoord = slauBlockTSize * k + 4 * Nfi;
			int size = Nfi-2;

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
	[System.Serializable]
	public class LinearSystem
	{
		[field: NonSerializedAttribute()]
		private double[,] a_matrix;
		[field: NonSerializedAttribute()]
		private double[] b_vector;
		[field: NonSerializedAttribute()]
		private double eps;
		[field: NonSerializedAttribute()]
		private int size;

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
		}
		public double[] XVector
		{
			get
			{
				return x_vector;
			}
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
					throw new Exception("������� ��������� �����������.");
				else
					throw new Exception("������� ��������� ����� ��������� �������..");
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
					for (int k = i + 1; k < size; ++k)
					{

						double p = a_matrix[k, index[i]];
						for (int j = i; j < size; ++j)
							a_matrix[k, index[j]] -= a_matrix[i, index[j]] * p;
						b_vector[k] -= b_vector[i] * p;
						a_matrix[k, index[i]] = 0.0;
					}
				
			}
		}

		private void GaussBackwardStroke(int[] index)
		{
			for (int i = size - 1; i >= 0; --i)
			{
				double x_i = b_vector[i];
				for (int j = i + 1; j < size; ++j)
				{
					x_i -= x_vector[index[j]] * a_matrix[i, index[j]];
				}
				x_vector[index[i]] = x_i;
			}
		}
	}
}
