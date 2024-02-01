using System;
using UnityEngine;

namespace Neural_Networks
{
    public static class NNMath
    {
        public static float ArrayMean(float[] arr)
        {
            float result = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                result += arr[i];
            }

            return result / arr.Length;
        }

        public static float[,] CopyMatrix(float[,] mat)
        {
            var copyMat = new float[mat.GetLength(0), mat.GetLength(1)];
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    copyMat[i, j] = mat[i, j];
                }
            }

            return copyMat;
        }

        public static float[,] MatrixDotProduct(float[,] mat1, float[,] mat2)
        {
            var output = new float[mat1.GetLength(0), mat2.GetLength(1)];
            for (int i = 0; i < mat2.GetLength(0); i++)
            {
                for (int j = 0; j < mat2.GetLength(1); j++)
                {
                    for (int k = 0; k < mat1.GetLength(0); k++)
                    {
                        output[k, j] += mat1[k, i] * mat2[i, j];
                    }
                }
            }

            return output;
        }

        public static float[,] TransposeMatrix(float[,] mat)
        {
            var transposedMat = new float[mat.GetLength(1), mat.GetLength(0)];
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    transposedMat[j, i] = mat[i, j];
                }
            }

            return transposedMat;
        }

        public static float StandardDivination(float[,] values)
        {
            float average = MatrixMean(values);
            float sum = 0;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    sum += MathF.Pow(values[i, j] - average, 2);
                }
            }

            return MathF.Sqrt((sum) / values.Length);
        }

        public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
        {
            float u;
            float s;
            
            do
            {
                u = 2.0f * UnityEngine.Random.value - 1.0f;
                var v = 2.0f * UnityEngine.Random.value - 1.0f;
                s = u * u + v * v;
            } while (s >= 1.0f); 

            var std = u * MathF.Sqrt(-2.0f * MathF.Log(s) / s);

            var mean = (minValue + maxValue) / 2.0f;
            var sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }

        public static float MatrixMean(float[,] mat)
        {
            float result = 0;
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    result += mat[i, j];
                }
            }

            return result / mat.Length;
        }
    }
}