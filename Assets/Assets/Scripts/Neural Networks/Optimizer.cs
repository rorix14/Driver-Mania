using System;
using System.Collections.Generic;

namespace Neural_Networks
{
    public class OptimizerAdam
    {
        public float CurrentLearningRate => _currentLearningRate;

        private readonly float _learningRate;
        private float _currentLearningRate;
        private readonly float _decay;
        private int _iteration;
        private readonly float _epsilon;
        private readonly float _beta1;
        private readonly float _beta2;

        private readonly Dictionary<LayerDense, float[,]> _layerToWeightsMomentum;
        private readonly Dictionary<LayerDense, float[,]> _layerToWeightsCache;
        private readonly Dictionary<LayerDense, float[,]> _layerToBiasesMomentum;
        private readonly Dictionary<LayerDense, float[,]> _layerToBiasesCache;

        public OptimizerAdam(float learningRate = 0.001f, float decay = 0.0f, float epsilon = 1e-7f,
            float beta1 = 0.9f, float beta2 = 0.999f)
        {
            _currentLearningRate = learningRate;
            _learningRate = learningRate;
            _decay = decay;
            _iteration = 0;
            _epsilon = epsilon;
            _beta1 = beta1;
            _beta2 = beta2;

            _layerToWeightsMomentum = new Dictionary<LayerDense, float[,]>();
            _layerToWeightsCache = new Dictionary<LayerDense, float[,]>();
            _layerToBiasesMomentum = new Dictionary<LayerDense, float[,]>();
            _layerToBiasesCache = new Dictionary<LayerDense, float[,]>();
        }

        public void PreUpdateParams()
        {
            if (_decay > 0)
                _currentLearningRate = _learningRate * (1.0f / (1.0f + _decay * _iteration));
        }

        public void UpdateParams(LayerDense layer)
        {
            CheckLayerInit(layer);

            var weightsRowLength = layer.DWeights.GetLength(0);
            var weightsColumnLength = layer.DWeights.GetLength(1);
            var biasesRowLength = layer.DBiases.GetLength(0);
            var biasesColumnLength = layer.DBiases.GetLength(1);

            // Update momentum with current gradients
            for (int i = 0; i < weightsRowLength; i++)
            {
                float weightMomentumCorrected;
                float weightCacheCorrected;

                for (int j = 0; j < weightsColumnLength; j++)
                {
                    _layerToWeightsMomentum[layer][i, j] = _beta1 * _layerToWeightsMomentum[layer][i, j] +
                                                           (1 - _beta1) * layer.DWeights[i, j];
                    weightMomentumCorrected =
                        _layerToWeightsMomentum[layer][i, j] / (1 - MathF.Pow(_beta1, _iteration + 1));

                    _layerToWeightsCache[layer][i, j] = _beta2 * _layerToWeightsCache[layer][i, j] +
                                                        (1 - _beta2) * MathF.Pow(layer.DWeights[i, j], 2);
                    weightCacheCorrected = _layerToWeightsCache[layer][i, j] /
                                           (1 - MathF.Pow(_beta2, _iteration + 1));

                    layer.Weights[i, j] += -_currentLearningRate * weightMomentumCorrected /
                                            (MathF.Sqrt(weightCacheCorrected) + _epsilon);
                }
            }
            
            for (int i = 0; i < biasesRowLength; i++)
            {
                float biasMomentumCorrected;
                float biasCacheCorrected;
                for (int j = 0; j < biasesColumnLength; j++)
                {
                    _layerToBiasesMomentum[layer][i, j] = _beta1 * _layerToBiasesMomentum[layer][i, j] +
                                                          (1 - _beta1) * layer.DBiases[i, j];
                    biasMomentumCorrected =
                        _layerToBiasesMomentum[layer][i, j] / (1 - MathF.Pow(_beta1, _iteration + 1));

                    _layerToBiasesCache[layer][i, j] = _beta2 * _layerToBiasesCache[layer][i, j] +
                                                       (1 - _beta2) * MathF.Pow(layer.DBiases[i, j], 2);

                    biasCacheCorrected = _layerToBiasesCache[layer][i, j] / (1 - MathF.Pow(_beta2, _iteration + 1));

                    layer.Biases[i, j] += -_currentLearningRate * biasMomentumCorrected /
                                           (MathF.Sqrt(biasCacheCorrected) + _epsilon);
                }
            }
        }

        public void PostUpdateParams()
        {
            ++_iteration;
        }

        private void CheckLayerInit(LayerDense layer)
        {
            if (!_layerToBiasesMomentum.ContainsKey(layer))
            {
                _layerToWeightsMomentum.Add(layer, new float[layer.Weights.GetLength(0), layer.Weights.GetLength(1)]);
                _layerToWeightsCache.Add(layer, new float[layer.Weights.GetLength(0), layer.Weights.GetLength(1)]);
                _layerToBiasesMomentum.Add(layer, new float[layer.Biases.GetLength(0), layer.Biases.GetLength(1)]);
                _layerToBiasesCache.Add(layer, new float[layer.Biases.GetLength(0), layer.Biases.GetLength(1)]);
            }
        }
    }
}

//Get corrected momentum
// var weightMomentumCorrected = new float[weightsRowLength, weightsColumnLength];
// for (int i = 0; i < weightsRowLength; i++)
// {
//     for (int j = 0; j < weightsColumnLength; j++)
//     {
//         weightMomentumCorrected[i, j] =
//             _layerToWeightsMomentum[layer][i, j] / (1 - MathF.Pow(_beta1, _iteration + 1));
//     }
// }

// Update cache with squared current gradients
// for (int i = 0; i < weightsRowLength; i++)
// {
//     for (int j = 0; j < weightsColumnLength; j++)
//     {
//         _layerToWeightsCache[layer][i, j] = _beta2 * _layerToWeightsCache[layer][i, j] +
//                                             (1 - _beta2) * MathF.Pow(layer._dWeights[i, j], 2);
//     }
// }
// Get corrected cache
// var weightCacheCorrected = new float[weightsRowLength, weightsColumnLength];
// for (int i = 0; i < weightsRowLength; i++)
// {
//     for (int j = 0; j < weightsColumnLength; j++)
//     {
//         weightCacheCorrected[i, j] =
//             _layerToWeightsCache[layer][i, j] / (1 - MathF.Pow(_beta2, _iteration + 1));
//     }
// }
// Vanilla SGD parameter update + normalization
// with square rooted cache
// for (int i = 0; i < weightsRowLength; i++)
// {
//     for (int j = 0; j < weightsColumnLength; j++)
//     {
//         layer._weights[i, j] += - _currentLearningRate * weightMomentumCorrected[i, j] /
//                                 (MathF.Sqrt(weightCacheCorrected[i, j]) + _epsilon);
//     }
// }

// var biasMomentumCorrected = new float[biasesRowLength, biasesColumnLength];
// for (int i = 0; i < biasesRowLength; i++)
// {
//     for (int j = 0; j < biasesColumnLength; j++)
//     {
//         biasMomentumCorrected[i, j] =
//             _layerToBiasesMomentum[layer][i, j] / (1 - MathF.Pow(_beta1, _iteration + 1));
//     }
// }

// for (int i = 0; i < biasesRowLength; i++)
// {
//     for (int j = 0; j < biasesColumnLength; j++)
//     {
//         _layerToBiasesCache[layer][i, j] = _beta2 * _layerToBiasesCache[layer][i, j] +
//                                            (1 - _beta2) * MathF.Pow(layer._dBiases[i, j], 2);
//     }
// }

//var biasCacheCorrected = new float[biasesRowLength, biasesColumnLength];
// for (int i = 0; i < biasesRowLength; i++)
// {
//     for (int j = 0; j < biasesColumnLength; j++)
//     {
//         biasCacheCorrected[i, j] =
//             _layerToBiasesCache[layer][i, j] / (1 - MathF.Pow(_beta2, _iteration + 1));
//     }
// }

// for (int i = 0; i < biasesRowLength; i++)
// {
//     for (int j = 0; j < biasesColumnLength; j++)
//     {
//         layer._biases[i, j] += -_currentLearningRate * biasMomentumCorrected[i, j] /
//                                (MathF.Sqrt(biasCacheCorrected[i, j]) + _epsilon);
//     }
// }