namespace Ai4Gamedev.MiniMax
{
    using UnityEngine;

    public abstract class DesiredVelocityProvider : MonoBehaviour
    {
        [SerializeField, Range(0,3)]
        private float weight = 1f;
        
        public float Weight => weight;
        
        protected Agent Agent;

        private void Awake()
        {
            Agent = GetComponent<Agent>();
        }

        public abstract Vector3 GetDesiredVelocity();
    }
}