namespace Ai4Gamedev.Steerings
{
    using UnityEngine;

    public abstract class DesiredVelocityProvider : MonoBehaviour
    {
        [SerializeField, Range(0,3)]
        private float weight = 1f;
        
        public float Weight => weight;
        
        protected Vehicle Vehicle;

        private void Awake()
        {
            Vehicle = GetComponent<Vehicle>();
        }

        public abstract Vector3 GetDesiredVelocity();
    }
}