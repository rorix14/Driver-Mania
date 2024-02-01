using System;

namespace Neural_Networks
{
    public class RegressionEvaluator
    {
        public float Accuracy;
        public float Loss;
        public float DataLoss;
        public float RegularizationLoss;

        private readonly float _accuracyPrecision;
        private readonly float[,] _yTrue;
        private readonly Loss _lossFunction;

        public RegressionEvaluator(float[,] yTrue, float allowance, Loss lossFunction)
        {
            _accuracyPrecision = NNMath.StandardDivination(yTrue) / allowance;
            _yTrue = yTrue;
            _lossFunction = lossFunction;
        }
        
        public void CalculateMetrics(float dataLoss, float[,] yPred, params LayerDense[] layers)
        {
            DataLoss = dataLoss;
            RegularizationLoss = 0.0f;

            for (int i = 0; i < layers.Length; i++)
            {
                RegularizationLoss += _lossFunction.RegularizationLoss(layers[i]);
            }

            Loss = DataLoss + RegularizationLoss;
            var sum = 0.0f;
            for (int i = 0; i < yPred.GetLength(0); i++)
            {
                for (int j = 0; j < yPred.GetLength(1); j++)
                {
                    sum +=  MathF.Abs(_yTrue[i, j] - yPred[i, j]) < _accuracyPrecision ? 1 : 0;
                }
            }
            Accuracy = sum / yPred.Length;
        }
    }
}