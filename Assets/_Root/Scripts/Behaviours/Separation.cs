namespace Ai4Gamedev.Steerings.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Separation : GroupVelocityProvider
    {
        [SerializeField, Min(0.1f)]
        private float neighbourRadius = 3f;

        [SerializeField]
        private bool inverseDistanceWeight = true;

        public override Vector3 GetDesiredVelocity()
        {
            var neighbours = GetNeighbours(neighbourRadius);
            if (neighbours.Count == 0)
            {
                return Vector3.zero;
            }

            var sum = Vector3.zero;
            var myPos = transform.position;

            foreach (var n in neighbours)
            {
                var diff = myPos - n.transform.position;
                var dist = diff.magnitude;
                if (dist < 0.001f)
                {
                    continue;
                }

                var weight = inverseDistanceWeight ? 1f / dist : 1f;
                sum += diff.normalized * weight;
            }

            if (sum.sqrMagnitude < 0.001f)
            {
                return Vector3.zero;
            }

            return sum.normalized * Agent.VelocityLimit;
        }
    }
}
