using System.Collections;
using Neural_Networks;
using UnityEngine;

namespace Evolutionary_Strategies
{
    public class GaManager : MonoBehaviour
    {
        [SerializeField] private GA ga;
        [SerializeField] private int genRuntime;
        [SerializeField] private string modelsNamePrefix;
        [SerializeField] private int savedModels;

        void Start()
        {
            ga.GenerateInitialPopulation();
            StartCoroutine(RunSimulation());
        }

        //TODO: refactor, use DNA generic class instead of creating and destroying car models every generation 
        private IEnumerator RunSimulation()
        {
            yield return new WaitForSeconds(genRuntime);

            var oldPopolation = new GaAgent[ga.CurrentPopulation.Length];
            for (int i = 0; i < ga.CurrentPopulation.Length; i++)
            {
                var car = ga.CurrentPopulation[i];
                ((GaCar)car).ResetCar();
                oldPopolation[i] = car;
            }
            
            ga.RunGeneticAlgorithm();

            for (int i = oldPopolation.Length - 1; i >= 0; i--)
            {
                var contains = false;
                for (int j = 0; j < ga.CurrentPopulation.Length; j++)
                {
                    if (oldPopolation[i] == ga.CurrentPopulation[j])
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    Destroy(oldPopolation[i].gameObject);
                }
            }
            
            StartCoroutine(RunSimulation());
        }

        private void OnDestroy()
        {
            StopCoroutine(nameof(RunSimulation));

            for (int i = 0; i < savedModels; i++)
            {
                ModelSaver.SaveModel($"{modelsNamePrefix}_{i + 1}", ((GaCar)ga.CurrentPopulation[i]).GetBrain);
            }
        }
    }
}