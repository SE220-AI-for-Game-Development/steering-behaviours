namespace Ai4Gamedev.Steerings.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class GroupVelocityProvider : DesiredVelocityProvider
    {
        protected List<Agent> GetNeighbours(float radius)
        {
            var all = FindObjectsOfType<Agent>();
            var list = new List<Agent>();
            var myPos = transform.position;

            foreach (var v in all)
            {
                if (v == Agent)
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
