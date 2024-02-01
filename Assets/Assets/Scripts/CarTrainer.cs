using System;
using System.Diagnostics;
using DrivingData;
using Neural_Networks;
using UnityEngine;

public class CarTrainer : MonoBehaviour
{
    [SerializeField] private string dataSetName;
    [SerializeField] private string modelName;

    private void Start()
    {
        StartTraining();
    }

    private void StartTraining()
    {
        var (x, y) = LoadData();

        var dense1 = new LayerDense(21, 64);
        var activation1 = new ActivationReLu();

        var dense2 = new LayerDense(64, 64);
        var activation2 = new ActivationReLu();

        var dense3 = new LayerDense(64, 64);
        var activation3 = new ActivationReLu();

        var dense4 = new LayerDense(64, 1);
        var activation4 = new ActivationLinear();

        var lossFunction = new LossMeanSquaredError();

        var evaluationMetrics = new RegressionEvaluator(y, 4, lossFunction);

        var optimizer = new OptimizerAdam(0.01f, 1e-3f);

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
            dense4.Forward(activation3.Output);
            activation4.Forward(dense4.Output);

            evaluationMetrics.CalculateMetrics(lossFunction.Calculate(activation4.Output, y), activation4.Output,
                dense1, dense2, dense3, dense4);

            if (epoch % 100 == 0)
            {
                print("Epoch: " + epoch + ", Accuracy: " + evaluationMetrics.Accuracy + ", Data loss: " +
                      evaluationMetrics.DataLoss
                      + " Current learning rate: " + optimizer.CurrentLearningRate);
            }

            // backwards pass
            lossFunction.Backward(activation4.Output, y);
            activation4.Backward(lossFunction.DInputs);
            dense4.Backward(activation4.DInputs);
            activation3.Backward(dense4.DInputs);
            dense3.Backward(activation3.DInputs);
            activation2.Backward(dense3.DInputs);
            dense2.Backward(activation2.DInputs);
            activation1.Backward(dense2.DInputs);
            dense1.Backward(activation1.DInputs);

            optimizer.PreUpdateParams();
            optimizer.UpdateParams(dense1);
            optimizer.UpdateParams(dense2);
            optimizer.UpdateParams(dense3);
            optimizer.UpdateParams(dense4);
            optimizer.PostUpdateParams();
        }

        stopwatch.Stop();
        print("Elapsed Time is " + stopwatch.ElapsedMilliseconds + " ms");
        ModelSaver.SaveModel(modelName, dense1, dense2, dense3, dense4);
    }

    private Tuple<float[,], float[,]> LoadData()
    {
        var dataset = FileHandler.ReadListFromJSON<DriveFeatures>
            ("Assets/SavedData/Datasets/" + dataSetName + ".json");
        
        var labels = FileHandler.ReadListFromJSON<DriveLabels>
            ("Assets/SavedData/Datasets/" + dataSetName + "Labels.json");
        
        var columnCount = dataset[0].features.Length;
        var x = new float[dataset.Count, columnCount];
        for (int i = 0; i < dataset.Count; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                x[i, j] = dataset[i].features[j];
            }
        }
        
        var y = new float[labels.Count, 1];
        for (int i = 0; i < labels.Count; i++)
        {
            y[i, 0] = labels[i].inputSteer;
        }
        
        return new Tuple<float[,], float[,]>(x, y);
    }
    
    // var columnCount = dataset[0].wallDistances.Length + 8;
    // var x = new float[dataset.Count, columnCount];
    //
    // for (int i = 0; i < dataset.Count; i++)
    // {
    //     for (int j = 0; j < dataset[i].wallDistances.Length; j++)
    //     {
    //         x[i, j] = dataset[i].wallDistances[j];
    //     }
    //     
    //     for (int j = 0; j < 3; j++)
    //     {
    //         x[i, dataset[i].wallDistances.Length + j] = dataset[i].velocity[j];
    //         x[i, dataset[i].wallDistances.Length + 3 + j] = dataset[i].acceleration[j];
    //     }
    //
    //     x[i, dataset[i].wallDistances.Length + 6] = (float)dataset[i].cornerType;
    //     x[i, dataset[i].wallDistances.Length + 7] = dataset[i].angleBetweenCarAndTrack;
    // }
}