using Michiru.Calculation;
using System;

namespace ClassificationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            (ChiruMatrix trainX, ChiruMatrix trainY) = GenerateData(1000);
            
        }


        static (ChiruMatrix X, ChiruMatrix Y) GenerateData(int m)
        {
            Random rand = new Random();
            double[,] X = new double[2,m], Y = new double[1,m];
            for (int n = 0; n < 1000; n++)
            {
                X[0, n] = rand.NextDouble();
                X[1, n] = rand.NextDouble();
                if(X[0,n] >= 0)
                {
                    if (X[1, n] >= 0)
                        Y[0, n] = 1;
                    else
                        Y[0, n] = 0;
                }else
                {
                    if (X[1, n] >= 0)
                        Y[0, n] = 0;
                    else
                        Y[0, n] = 1;
                }
            }

            return (X.AsMatrix(), Y.AsMatrix());
        }
    }
}
