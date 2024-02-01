using System;
using System.Collections.Generic;
using System.Diagnostics;
using Neural_Networks;
using UnityEngine;

public class TestPerformace : MonoBehaviour
{
    public ComputeShader shader;

    float[,] mat1 = { { -1, 1, 1 }, { 2, -2, 2 }, { 3, 3, -3 } };
    float[,] mat2 = { { 3, 2, 4 }, { 5, 2, 1 } };
    float[,] mat3 = { { -1, 1 }, { 2, -2 }, { -3, 3, } };
    float[,] mat4 = { { 1 }, { 2 }, { 3 }, { 4 } };
    float[,] mat5 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, { 10, 11, 12 } };
    float[,] mat6 = { { 1, 2, 3 } };
    float[,] mat7 = { { -1, 4, 1, 0 }, { 2, -2, 2, -1 } };

    void Start()
    {
        //TestCsTranspose();
        //TestCsDotProduct();
        //TestingFunction();
        //TestNeuralNetwork();
        CompareNN_GPU_To_CPU();
    }

    private void CompareNN_GPU_To_CPU()
    {
        var (x, y) = GenerateSinSample();

        int inputSize = x.GetLength(0);
        var matTest = new float[inputSize, 1];
        for (int i = 0; i < inputSize; i++)
        {
            matTest[i, 0] = x[i, 0];

            //matTest[i, 1] = y[i, 0];
        }

        var dense1 = new LayerDense(1, 256);
        var activation1 = new ActivationReLu();

        var result = new float[matTest.GetLength(0), dense1.Weights.GetLength(1)];

        var kernelHandle = shader.FindKernel("matrix_dot_product");
        var mat1Buffer = new ComputeBuffer(matTest.Length, sizeof(float));
        var mat2Buffer = new ComputeBuffer(dense1.Weights.Length, sizeof(float));
        var biases2Buffer = new ComputeBuffer(dense1.Biases.Length, sizeof(float));
        var resultBuffer = new ComputeBuffer(result.Length, sizeof(float));

        mat1Buffer.SetData(matTest);
        mat2Buffer.SetData(dense1.Weights);
        biases2Buffer.SetData(dense1.Biases);
        resultBuffer.SetData(result);

        shader.GetKernelThreadGroupSizes(kernelHandle, out var threadSizeX, out var threadSizeY, out _);
        
        shader.SetInt("column_size_1", matTest.GetLength(0));
        shader.SetInt("row_size_1", matTest.GetLength(1));
        shader.SetInt("column_size_2", dense1.Weights.GetLength(0));
        shader.SetInt("row_size_2", dense1.Weights.GetLength(1));
        shader.SetBuffer(kernelHandle, "mat_1", mat1Buffer);
        shader.SetBuffer(kernelHandle, "mat_2", mat2Buffer);
        shader.SetBuffer(kernelHandle, "biases", biases2Buffer);
        shader.SetBuffer(kernelHandle, "result_mat", resultBuffer);

        var stopwatch = new Stopwatch();
        const int inter = 200;

        stopwatch.Start();
        for (int i = 0; i < inter; i++)
        {
            dense1.Forward(matTest);
            activation1.Forward(dense1.Output);
        }

        stopwatch.Stop();
        print("CPU took: " + stopwatch.ElapsedMilliseconds + " ms");

        // for (int i = 0; i < activation1.Output.GetLength(0); i++)
        // {
        //     print("row n: " + (1 + i));
        //     for (int j = 0; j < activation1.Output.GetLength(1); j++)
        //     {
        //         print(activation1.Output[i, j]);
        //     }
        // }

        stopwatch.Restart();
        for (int i = 0; i < inter; i++)
        {
            shader.Dispatch(kernelHandle, Mathf.CeilToInt(result.GetLength(0) / (float)threadSizeX), 
                Mathf.CeilToInt(result.GetLength(1)/ (float)threadSizeY), 1);
            resultBuffer.GetData(result);
        }

        stopwatch.Stop();
        print("GPU took: " + stopwatch.ElapsedMilliseconds + " ms");

        // for (int i = 0; i < result.GetLength(0); i++)
        // {
        //     print("row n: " + (1 + i));
        //     for (int j = 0; j < result.GetLength(1); j++)
        //     {
        //         print(result[i, j]);
        //     }
        // }

        mat1Buffer.Release();
        mat2Buffer.Release();
        biases2Buffer.Release();
        resultBuffer.Release();
    }

    private void TestNeuralNetwork()
    {
        var (x, y) = GenerateSinSample();
        // var (x, y) = GenerateLinearSample();

        var dense1 = new LayerDense(1, 64);
        var activation1 = new ActivationReLu();

        var dense2 = new LayerDense(64, 64);
        var activation2 = new ActivationReLu();

        var dense3 = new LayerDense(64, 1);
        var activation3 = new ActivationLinear();

        var lossFunction = new LossMeanSquaredError();

        var evaluationMetrics = new RegressionEvaluator(y, 250, lossFunction);

        var optimizer = new OptimizerAdam();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int epoch = 0; epoch < 1001; epoch++)
        {
            dense1.Forward(x);
            activation1.Forward(dense1.Output);
            dense2.Forward(activation1.Output);
            activation2.Forward(dense2.Output);
            dense3.Forward(activation2.Output);
            activation3.Forward(dense3.Output);
            evaluationMetrics.CalculateMetrics(lossFunction.Calculate(activation3.Output, y), activation3.Output,
                dense1, dense2, dense3);

            if (epoch % 100 == 0)
            {
                print("Epoch: " + epoch + ", Accuracy: " + evaluationMetrics.Accuracy + ", Data loss: " +
                      evaluationMetrics.DataLoss
                      + " Current learning rate: " + optimizer.CurrentLearningRate);
            }

            // backwards pass
            lossFunction.Backward(activation3.Output, y);
            activation3.Backward(lossFunction.DInputs);
            dense3.Backward(activation3.DInputs);
            activation2.Backward(dense3.DInputs);
            dense2.Backward(activation2.DInputs);
            activation1.Backward(dense2.DInputs);
            dense1.Backward(activation1.DInputs);

            optimizer.PreUpdateParams();
            optimizer.UpdateParams(dense1);
            optimizer.UpdateParams(dense2);
            optimizer.UpdateParams(dense3);
            optimizer.PostUpdateParams();
        }

        dense1.Forward(x);
        activation1.Forward(dense1.Output);
        dense2.Forward(activation1.Output);
        activation2.Forward(dense2.Output);
        dense3.Forward(activation2.Output);
        activation3.Forward(dense3.Output);

        // for (int i = 0; i < y.GetLength(0); i++)
        // {
        //     for (int j = 0; j < y.GetLength(1); j++)
        //     {
        //        print("Value x: " + x[i, j] + ", true: " + y[i, j] + ", pred: " + activation3._output[i, j]);
        //      // print("pred: " + activation3._output[i, j]);
        //     }
        // }

        stopwatch.Stop();
        print("Elapsed Time is " + stopwatch.ElapsedMilliseconds + " ms");
    }

    private Tuple<float[,], float[,]> GenerateSinSample()
    {
        List<float> xValues = new();
        List<float> yValues = new();
        float timeAdditive = 0;

        while (timeAdditive <= 1.0f)
        {
            xValues.Add(timeAdditive);
            yValues.Add(MathF.Sin(Mathf.Deg2Rad * (58 * Mathf.PI * 2 * timeAdditive)));
            timeAdditive += Time.deltaTime / 6;
        }

        var x = new float[xValues.Count, 1];
        var y = new float[yValues.Count, 1];
        for (int i = 0; i < xValues.Count; i++)
        {
            x[i, 0] = xValues[i];
            y[i, 0] = yValues[i];
        }

        return new Tuple<float[,], float[,]>(x, y);
    }

    private Tuple<float[,], float[,]> GenerateLinearSample()
    {
        List<float> xValues = new();
        List<float> yValues = new();
        float timeAdditive = 0;

        while (timeAdditive <= 10.0f)
        {
            xValues.Add(timeAdditive);
            yValues.Add(2 * timeAdditive + 5);
            timeAdditive += Time.deltaTime / 2;
        }

        var x = new float[xValues.Count, 1];
        var y = new float[yValues.Count, 1];
        for (int i = 0; i < xValues.Count; i++)
        {
            x[i, 0] = xValues[i];
            y[i, 0] = yValues[i];
        }

        return new Tuple<float[,], float[,]>(x, y);
    }

    // private void TestCsTranspose()
    // {
    //     var matToUse = mat2;
    //     var result =  new float[matToUse.GetLength(1), matToUse.GetLength(0)];
    //
    //     var kernelHandle = shader.FindKernel("transpose_matrix");
    //     var mattBuffer = new ComputeBuffer(matToUse.Length, sizeof(float));
    //     var resultBuffer = new ComputeBuffer(matToUse.Length, sizeof(float));
    //
    //     mattBuffer.SetData(matToUse);
    //     shader.SetInt("row_size", matToUse.GetLength(1));
    //     shader.SetInt("column_size", matToUse.GetLength(0));
    //     shader.SetBuffer(kernelHandle, "mat_1", mattBuffer);
    //     shader.SetBuffer(kernelHandle, "result_mat", resultBuffer);
    //
    //     shader.Dispatch(kernelHandle, matToUse.GetLength(0), matToUse.GetLength(1), 1);
    //     resultBuffer.GetData(result);
    //     
    //     for (int i = 0; i < result.GetLength(0); i++)
    //     {
    //         print("row n: " + (1 + i));
    //         for (int j = 0; j < result.GetLength(1); j++)
    //         {
    //             print(result[i, j]);
    //         }
    //     }
    //
    //     mattBuffer.Release();
    //     resultBuffer.Release();
    // }

    private void TestCsDotProduct()
    {
        var matT1 = mat3;
        var matT2 = mat2;

        var result = new float[matT1.GetLength(0), matT2.GetLength(1)];

        var kernelHandle = shader.FindKernel("matrix_dot_product");
        var mat1Buffer = new ComputeBuffer(matT1.Length, sizeof(float));
        var mat2Buffer = new ComputeBuffer(matT2.Length, sizeof(float));
        var resultBuffer = new ComputeBuffer(result.Length, sizeof(float));

        mat1Buffer.SetData(matT1);
        mat2Buffer.SetData(matT2);
        resultBuffer.SetData(result);

        shader.SetInt("column_size_1", matT1.GetLength(0));
        shader.SetInt("row_size_1", matT1.GetLength(1));
        shader.SetInt("column_size_2", matT2.GetLength(0));
        shader.SetInt("row_size_2", matT2.GetLength(1));
        shader.SetBuffer(kernelHandle, "mat_1", mat1Buffer);
        shader.SetBuffer(kernelHandle, "mat_2", mat2Buffer);
        shader.SetBuffer(kernelHandle, "result_mat", resultBuffer);

        shader.Dispatch(kernelHandle, result.GetLength(0), result.GetLength(1), 1);
        resultBuffer.GetData(result);

        for (int i = 0; i < result.GetLength(0); i++)
        {
            print("row n: " + (1 + i));
            for (int j = 0; j < result.GetLength(1); j++)
            {
                print(result[i, j]);
            }
        }

        mat1Buffer.Release();
        mat2Buffer.Release();
        resultBuffer.Release();
    }


    private void TestingFunction()
    {
        // foreach (var var in test)
        // {
        //     print(var.val);
        // }

        // var tt = NNMath.TransposeMatrix(mat1);
        //
        // for (int i = 0; i < tt.GetLength(0); i++)
        // {
        //     print("row n: " + (1 + i));
        //     for (int j = 0; j < tt.GetLength(1); j++)
        //     {
        //         print(tt[i, j]);
        //     }
        // }
    }
}