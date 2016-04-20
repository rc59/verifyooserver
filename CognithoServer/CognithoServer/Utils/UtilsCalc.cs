using CognithoServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognithoServer.Utils
{
    public class UtilsCalc
    {
        public double CalcPercentage(double value1, double value2)
        {
            if(value1 == 0 || value2 == 0)
            {
                return 1;
            }

            value1 = Math.Abs(value1);
            value2 = Math.Abs(value2);

            double result = 0;

            if(value1 > value2)
            {
                result = value2 / value1;
            }
            else
            {
                result = value1 / value2;
            }

            return result;
        }    

        public int[] GenerateRandomVector(int size)
        {
            Random random = new Random();
            int[] vector = new int[size];

            for(int idx = 0; idx < size; idx++)
            {
                vector[idx] = idx;
            }


            int randomIdx, tempValue;

            for (int idx = 0; idx < size; idx++)
            {
                randomIdx = random.Next(size);
                tempValue = vector[idx];
                vector[idx] = vector[randomIdx];
                vector[randomIdx] = tempValue;
            }

            return vector;
        }

        public double CalcPitagoras(double x, double y)
        {
            double value = x * x + y * y;
            value = Math.Sqrt(value);

            return value;
        }

        public double GetMaxValue(double value1, double value2)
        {
            if (value1 > value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }
        public double GetMaxAbsValue(double value1, double value2)
        {
            value1 = Math.Abs(value1);
            value2 = Math.Abs(value2);

            if (value1 > value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }

        public double GetMinValue(double value1, double value2)
        {
            if (value1 < value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }


        public double GetMinAbsValue(double value1, double value2)
        {
            value1 = Math.Abs(value1);
            value2 = Math.Abs(value2);

            if (value1 < value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }
    }
}