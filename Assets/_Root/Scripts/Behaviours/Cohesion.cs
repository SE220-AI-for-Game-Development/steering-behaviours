namespace Ai4Gamedev.MiniMax.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Cohesion : GroupVelocityProvider
    {
        [SerializeField, Min(0.1f)]
        private float neighbourRadius = 5f;

        public override Vector3 GetDesiredVelocity()
        {
            var neighbours = GetNeighbours(neighbourRadius);
            if (neighbours.Count == 0)
            {
                return Vector3.zero;
            }

            var center = Vector3.zero;
            foreach (var n in neighbours)
            {
                center += n.transform.position;
            }

            center /= neighbours.Count;
            var toCenter = center - transform.position;

            if (toCenter.sqrMagnitude < 0.001f)
            {
                return Vector3.zero;
            }

            return toCenter.normalized * Agent.VelocityLimit;
        }
    }
}
