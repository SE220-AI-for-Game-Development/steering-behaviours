namespace Ai4Gamedev.Steerings.Behaviours
{
    using UnityEngine;

    public class Flee: DesiredVelocityProvider
    {
        [SerializeField]
        private Transform objectToFlee;
        
        public override Vector3 GetDesiredVelocity()
        {
            return -(objectToFlee.position - transform.position).normalized * Vehicle.VelocityLimit;
        }
    }
}