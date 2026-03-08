namespace Ai4Gamedev.Steerings.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Alignment : GroupVelocityProvider
    {
        [SerializeField, Min(0.1f)]
        private float neighbourRadius = 3f;

        public override Vector3 GetDesiredVelocity()
        {
            var neighbours = GetNeighbours(neighbourRadius);
            if (neighbours.Count == 0)
            {
                return Vector3.zero;
            }

            var sum = Vector3.zero;
            foreach (var n in neighbours)
            {
                sum += n.Velocity;
            }

            sum /= neighbours.Count;
            if (sum.sqrMagnitude < 0.001f)
            {
                return Vector3.zero;
            }

            return sum.normalized * Agent.VelocityLimit;
        }
    }
}
