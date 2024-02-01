using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Evolutionary_Strategies
{
    [Serializable]
    public class GA
    {
        [SerializeField] private int maxGenerations = int.MaxValue;
        [SerializeField] private int populationSize = 100;
        [SerializeField] private int elitismFactor = 8;
        [SerializeField] private int mutationProbability = 10;
        [SerializeField] private int geneMutationProbability = 1;
        [SerializeField] private GaAgent agentPrefab;

        private GaAgent[] _currentPopulation;
        private int _generationCount;
        
        public GaAgent[] CurrentPopulation => _currentPopulation;

        public void GenerateInitialPopulation()
        {
            _currentPopulation = new GaAgent[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                _currentPopulation[i] = Object.Instantiate(agentPrefab);
                _currentPopulation[i].gameObject.SetActive(true);
            }
        }

        private void EvaluatePopulation()
        {
            for (int i = 0; i < populationSize; i++)
            {
                if (_currentPopulation[i].Fitness == 0)
                {
                    _currentPopulation[i].CalculateFitness();
                }
            }
        }

        private (int parentOne, int parentTwo) Selection()
        {
            var totalFitness = 0.0f;
            for (int i = 0; i < populationSize; i++)
            {
                totalFitness += _currentPopulation[i].Fitness;
            }

            var parentOne = SelectParent(totalFitness, -1);
            var parentTwo = SelectParent(totalFitness - _currentPopulation[parentOne].Fitness, parentOne);
            
           // Debug.Log($"Parent one: {parentOne}, Parent two: {parentTwo}");
            return (parentOne, parentTwo);
        }

        private int SelectParent(float maxRange, int lastParent)
        {
            var index = -1;
            var cutoff = UnityEngine.Random.Range(1, maxRange);

            while (cutoff > 0)
            {
                index++;
                if (index == lastParent) continue;

                cutoff -= _currentPopulation[index].Fitness;
            }
            
            return index;
        }

        private (GaAgent child1, GaAgent child2) Crossover(GaAgent parent1, GaAgent parent2)
        {
            var (childOne, childTwo) = parent1.Crossover(parent2);
            return (childOne, childTwo);
        }

        private GaAgent Mutation(GaAgent individual)
        {
            if (UnityEngine.Random.Range(0, 101) <= mutationProbability)
            {
                individual.Mutate(geneMutationProbability);
            }

            return individual;
        }

        public void RunGeneticAlgorithm()
        {
            if (maxGenerations <= _generationCount) return;

            _generationCount++;
            EvaluatePopulation();
            Array.Sort(_currentPopulation, (x, y) => y.Fitness.CompareTo(x.Fitness));
            Debug.Log("Best individual of generation: " + _generationCount + ": " + _currentPopulation[0].Fitness);

            var newPopulation = new GaAgent[populationSize];
            var populationCount = 0;

            while (populationCount < populationSize)
            {
                if (populationCount < elitismFactor)
                {
                    newPopulation[populationCount] = _currentPopulation[populationCount];
                    populationCount++;
                }
                else
                {
                    var (parentOne, parentTwo) = Selection();
                    var (childOne, childTwo) = Crossover(_currentPopulation[parentOne], _currentPopulation[parentTwo]);

                    newPopulation[populationCount] = Mutation(childOne);

                    if (populationCount + 1 >= populationSize) break;
                    
                    newPopulation[populationCount + 1] = Mutation(childTwo);
                    populationCount += 2;
                }
            }

            _currentPopulation = newPopulation;
        }
    }
}