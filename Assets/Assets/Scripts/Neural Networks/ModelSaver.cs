using System;
using System.Collections.Generic;

namespace Neural_Networks
{
    public static class ModelSaver
    {
        public static void SaveModel(string fileName, params LayerDense[] denseLayers)
        {
            var model = new List<SavableLayer>(denseLayers.Length);
            foreach (var layer in denseLayers)
            {
                var neurons = new List<Neuron>();
                for (int i = 0; i < layer.Weights.GetLength(1); i++)
                {
                    var weights = new List<float>();
                    for (int j = 0; j < layer.Weights.GetLength(0); j++)
                    {
                        weights.Add(layer.Weights[j, i]);
                    }

                    neurons.Add(new Neuron(weights.ToArray(), layer.Biases[0, i]));
                }

                model.Add(new SavableLayer(neurons.ToArray()));
            }

            //D:\My Projects\8th Semester\Project\DriverMania\Assets\Assets\SavedData\Models
            FileHandler.SaveToJSON(model, "Assets/SavedData/Models/" + fileName + ".json");
        }

        public static void LoadModel(string fileName, params LayerDense[] denseLayers)
        {
            var savedModel = FileHandler.ReadListFromJSON<SavableLayer>
                ("Assets/SavedData/Models/" + fileName + ".json");
            // var savedModel = FileHandler.ReadListFromJSON<SavableLayer>
            //     (fileName + ".json");

            for (int i = 0; i < savedModel.Count; i++)
            {
                for (int j = 0; j < savedModel[i].neurons.Length; j++)
                {
                    for (int k = 0; k < savedModel[i].neurons[j].weights.Length; k++)
                    {
                        denseLayers[i].Weights[k, j] = savedModel[i].neurons[j].weights[k];
                    }

                    denseLayers[i].Biases[0, j] = savedModel[i].neurons[j].bias;
                }
            }
        }
    }

    [Serializable]
    public struct SavableLayer
    {
        public Neuron[] neurons;

        public SavableLayer(Neuron[] neurons)
        {
            this.neurons = neurons;
        }
    }

    [Serializable]
    public struct Neuron
    {
        public float[] weights;
        public float bias;

        public Neuron(float[] weights, float bias)
        {
            this.weights = weights;
            this.bias = bias;
        }
    }
}