namespace Ai4Gamedev.Steerings.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class GroupVelocityProvider : DesiredVelocityProvider
    {
        protected List<Vehicle> GetNeighbours(float radius)
        {
            var all = FindObjectsOfType<Vehicle>();
            var list = new List<Vehicle>();
            var myPos = transform.position;

            foreach (var v in all)
            {
                if (v == Vehicle)
                {
                    continue;
                }

                if ((v.transform.position - myPos).magnitude < radius)
                {
                    list.Add(v);
                }
            }

            return list;
        }
    }
}
