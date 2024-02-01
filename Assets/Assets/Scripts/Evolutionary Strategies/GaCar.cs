using System.Collections.Generic;
using CarUtils;
using DrivingData;
using Neural_Networks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Evolutionary_Strategies
{
    public class GaCar : GaAgent
    {
        private CarPhysics _carPhysics;
        private DataGatherer _dataGatherer;
        private GaDNA _carBrain;

        private Vector3 _startPos;
        private Quaternion _startRot;

        private List<float> _brainOutputs;
        private int _wallsCollided;
        private int _checkpointCollected;
        private float _raceTime;
        private float _lapTime;

        public LayerDense[] GetBrain => _carBrain.DenseLayers;

        private void Awake()
        {
            _carBrain = new GaDNA();
            _carPhysics = GetComponent<CarPhysics>();
            _dataGatherer = GetComponent<DataGatherer>();
            _dataGatherer.OnCheckpointChangedEvent += CheckCollectedCheckpoint;

            var carTrans = transform;
            _startPos = carTrans.position;
            _startRot = carTrans.rotation;

            _brainOutputs = new List<float>(2000);
        }

        private void FixedUpdate()
        {
            _raceTime += Time.fixedDeltaTime;
            var currentFeatures = _dataGatherer.GatherData();
            var x = new float[1, currentFeatures.Length];
            for (int i = 0; i < currentFeatures.Length; i++)
            {
                x[0, i] = currentFeatures[i];
            }

            var output = _carBrain.DoAction(x);
            _brainOutputs.Add(output);
            var steerValue = Mathf.Clamp(output, -1.0f, 1.0f);
            _carPhysics.MoveWithCustomPhysics(1.0f, steerValue);
        }

        public void ResetCar()
        {
            var carTrans = transform;
            carTrans.position = _startPos;
            carTrans.rotation = _startRot;
            _carPhysics.ResetCar();
            _dataGatherer.ResetCar();
        }

        private void CheckCollectedCheckpoint(RaceCheckPoint newCheck)
        {
            if (_dataGatherer.CurrentCheckPoint.NextCheckPoint == newCheck)
            {
                if (_dataGatherer.StartCheckPoint == newCheck)
                {
                    _lapTime = _raceTime;
                }
                
                _checkpointCollected++;
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            if ((_dataGatherer.WallMask & 1 << collisionInfo.gameObject.layer) == 1 << collisionInfo.gameObject.layer)
            {
                _wallsCollided++;
            }
        }

        private void OnDestroy()
        {
            _dataGatherer.OnCheckpointChangedEvent -= CheckCollectedCheckpoint;
        }

        public override void CalculateFitness()
        {
            foreach (var outPut in _brainOutputs)
            {
                if (outPut < -1 || outPut > 1)
                {
                    _fitness -= 0.3f;
                }
            }

            _fitness -= _wallsCollided * 0.3f;
            _fitness += _checkpointCollected * 20.0f;

            if (_lapTime > 0.0f)
            {
                _fitness += Mathf.Pow(35.05f - _lapTime, 2.0f);
            }


            if (_fitness < 1)
            {
                _fitness = 1;
            }

            _fitness = Mathf.Pow(_fitness, 2.0f);

            //print($"outputs: {_brainOutputs.Count}, wall collided:  {_wallsCollided}, checkpoints: {_checkpointCollected}, time: {_lapTime}");
            _brainOutputs.Clear();
        }

        public override void Mutate(float geneMutationProbability)
        {
            foreach (var denseLayer in _carBrain.DenseLayers)
            {
                for (int i = 0; i < denseLayer.Weights.GetLength(1); i++)
                {
                    var geneMutation = Random.Range(1, 101);
                    if (geneMutation <= geneMutationProbability)
                    {
                        denseLayer.Biases[0, i] += 0.1f * NNMath.RandomGaussian(-14.0f, 14.0f);
                    }

                    for (int j = 0; j < denseLayer.Weights.GetLength(0); j++)
                    {
                        geneMutation = Random.Range(1, 101);
                        if (geneMutation <= geneMutationProbability)
                        {
                            denseLayer.Weights[j, i] += 0.1f * NNMath.RandomGaussian(-14.0f, 14.0f);
                        }
                    }
                }
            }
        }

        public override (GaAgent, GaAgent) Crossover(GaAgent parentTwo)
        {
            var childOne = Instantiate(this, _startPos, _startRot);
            var childTwo = Instantiate(this, _startPos, _startRot);

            for (int i = 0; i < _carBrain.DenseLayers.Length; i++)
            {
                var denseLayer = _carBrain.DenseLayers[i];
                var crossoverPoint = Random.Range(1, denseLayer.Weights.GetLength(1));

                for (int j = 0; j < denseLayer.Weights.GetLength(1); j++)
                {
                    if (j < crossoverPoint)
                    {
                        childOne._carBrain.DenseLayers[i].Biases[0, j] = denseLayer.Biases[0, j];
                        childTwo._carBrain.DenseLayers[i].Biases[0, j] =
                            ((GaCar)parentTwo)._carBrain.DenseLayers[i].Biases[0, j];
                    }
                    else
                    {
                        childOne._carBrain.DenseLayers[i].Biases[0, j] =
                            ((GaCar)parentTwo)._carBrain.DenseLayers[i].Biases[0, j];
                        childTwo._carBrain.DenseLayers[i].Biases[0, j] = denseLayer.Biases[0, j];
                    }

                    for (int k = 0; k < denseLayer.Weights.GetLength(0); k++)
                    {
                        if (j < crossoverPoint)
                        {
                            childOne._carBrain.DenseLayers[i].Weights[k, j] = denseLayer.Weights[k, j];
                            childTwo._carBrain.DenseLayers[i].Weights[k, j] =
                                ((GaCar)parentTwo)._carBrain.DenseLayers[i].Weights[k, j];
                        }
                        else
                        {
                            childOne._carBrain.DenseLayers[i].Weights[k, j] =
                                ((GaCar)parentTwo)._carBrain.DenseLayers[i].Weights[k, j];
                            childTwo._carBrain.DenseLayers[i].Weights[k, j] = denseLayer.Weights[k, j];
                        }
                    }
                }
            }

            return (childOne, childTwo);
        }
    }
}