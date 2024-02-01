using UnityEngine;

namespace Evolutionary_Strategies
{
    public abstract class GaAgent : MonoBehaviour
    {
        protected float _fitness;

        public float Fitness => _fitness;
        
        public abstract void CalculateFitness();

        public abstract void Mutate(float geneMutationProbability);

        public abstract (GaAgent, GaAgent) Crossover(GaAgent parentTwo);
    }
}

