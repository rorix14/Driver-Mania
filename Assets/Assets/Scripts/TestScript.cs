using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using Neural_Networks;
using Debug = UnityEngine.Debug;

public class TestScript : MonoBehaviour
{
    // don't forget to test if for loops are faster then foreach
    private void Start()
    { 
        //TestingFunction();
        // TestNeuralNetwork();
        // TestLoadedModel();
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

        for (int epoch = 0; epoch < 5001; epoch++)
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

       ModelSaver.SaveModel("model_1", dense1, dense2, dense3);
       stopwatch.Stop();
        print("Elapsed Time is " + stopwatch.ElapsedMilliseconds + " ms");
    }

    private void TestLoadedModel()
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

        ModelSaver.LoadModel("model_1", dense1, dense2, dense3);

        dense1.Forward(x);
        activation1.Forward(dense1.Output);
        dense2.Forward(activation1.Output);
        activation2.Forward(dense2.Output);
        dense3.Forward(activation2.Output);
        activation3.Forward(dense3.Output);
        evaluationMetrics.CalculateMetrics(lossFunction.Calculate(activation3.Output, y), activation3.Output,
            dense1, dense2, dense3);

        print("Accuracy: " + evaluationMetrics.Accuracy + ", Data loss: " + evaluationMetrics.DataLoss);
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

   
    // used to right some data to a csv file format
    private void RightToFile(string path)
    {
        //string path = "D:/ML_Study/Projects/Heart-disease-project/SinData.csv";
        var (x, y) = GenerateSinSample();
        StreamWriter writer = new StreamWriter(path);

        writer.WriteLine("Y,X");
        for (int i = 0; i < x.GetLength(0); i++)
        {
            writer.WriteLine(y[i, 0].ToString(CultureInfo.InvariantCulture) + ", " +
                             x[i, 0].ToString(CultureInfo.InvariantCulture));
        }

        writer.Flush();
        writer.Close();
        print("Done Righting to file!");
    }

    // function just used for random testing of the code from the neural network model
    private void TestingFunction()
    {
        var layer1 = new LayerDense(3, 4);
        var activationLayer = new ActivationReLu();

        float[,] mat1 = { { -1, 1, 1 }, { 2, -2, 2 }, { 3, 3, -3 } };
        float[,] mat11 = { { -1, 4, 1 }, { 2, -2, 2 }, { 3, 3, -3 } };
        float[,] mat2 = { { 3, 2, 4 }, { 5, 2, 1 } };

        float[,] mat3 = { { -1, 1 }, { 2, -2 }, { -3, 3, } };
        float[,] mat4 = { { 1 }, { 2 }, { 3 }, { 4 } };

        var lossFunction = new LossMeanSquaredError();
        var regressionMetrics = new RegressionEvaluator(mat1, 1, lossFunction);

        float[,] mat5 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        float[,] mat6 = { { 1, 2, 3 } };
        
        // foreach (var var in test)
        // {
        //     print(var.val);
        // }
        
        //regressionMetrics.CalculateMetrics(0.0f, mat11);
        // print("Accuracy: " + regressionMetrics.Accuracy + ", deviation: " + regressionMetrics._accuracyPrecision);

        // for (int i = 0; i < tt.GetLength(0); i++)
        // {
        //     print("column n: " + (1 + i));
        //     for (int j = 0; j < tt.GetLength(1); j++)
        //     {
        //         print(tt[i, j]);
        //     }
        // }
    }
}