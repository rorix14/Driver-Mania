using UnityEngine;

namespace Neural_Networks
{
    // Dense layer
    public class LayerDense
    {
        public readonly float[,] Weights;
        public readonly float[,] Biases;
        private float[,] _inputs;
        public float[,] Output;

        public float[,] DWeights;
        public float[,] DBiases;
        public float[,] DInputs;

        public readonly float WeightRegularizerL2;
        public readonly float BiasRegularizerL2;

        public LayerDense(int nInputs, int nNeurons, float weightRegularizerL2 = 0, float biasRegularizerL2 = 0)
        {
            //Random.InitState(42);
            Weights = new float[nInputs, nNeurons];
            Biases = new float[1, nNeurons];

            // TODO: Change to a more normal weight a bias initialization once finished righting the GA algorithm
            for (int i = 0; i < Weights.GetLength(1); i++)
            {
                //Biases[0, i] = 0.01f * NNMath.RandomGaussian(-50.0f, 50.0f);
                Biases[0, i] = 0.01f * NNMath.RandomGaussian(-4.0f, 4.0f);
                for (int j = 0; j < Weights.GetLength(0); j++)
                {
                    //Weights[j, i] = 0.01f * NNMath.RandomGaussian(-50.0f, 50.0f); // for GA to have a large diversity
                    Weights[j, i] = 0.01f * NNMath.RandomGaussian(-4.0f, 4.0f); //* Random.value;
                }
            }

            WeightRegularizerL2 = weightRegularizerL2;
            BiasRegularizerL2 = biasRegularizerL2;
        }

        // Forward pass
        public void Forward(float[,] inputs)
        {
            _inputs = inputs;

            Output = NNMath.MatrixDotProduct(_inputs, Weights);
            for (int i = 0; i < Output.GetLength(1); i++)
            {
                var neuronBias = Biases[0, i];
                for (int j = 0; j < Output.GetLength(0); j++)
                {
                    Output[j, i] += neuronBias;
                }
            }
        }

        // Backward pass
        public void Backward(float[,] dValues)
        {
            DWeights = NNMath.MatrixDotProduct(NNMath.TransposeMatrix(_inputs), dValues);

            DBiases = new float[1, dValues.GetLength(1)];
            for (int i = 0; i < dValues.GetLength(1); i++)
            {
                for (int j = 0; j < dValues.GetLength(0); j++)
                {
                    DBiases[0, i] += dValues[j, i];
                }
            }

            if (WeightRegularizerL2 > 0)
            {
                for (int i = 0; i < DWeights.GetLength(0); i++)
                {
                    for (int j = 0; j < DWeights.GetLength(1); j++)
                    {
                        DWeights[i, j] += 2 * WeightRegularizerL2 * Weights[i, j];
                    }
                }
            }

            if (BiasRegularizerL2 > 0)
            {
                for (int i = 0; i < DBiases.GetLength(1); i++)
                {
                    DBiases[0, i] += 2 * BiasRegularizerL2 * Biases[0, i];
                }
            }

            DInputs = NNMath.MatrixDotProduct(dValues, NNMath.TransposeMatrix(Weights));
        }
    }
}