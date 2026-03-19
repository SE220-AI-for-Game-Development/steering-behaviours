namespace Ai4Gamedev.MiniMax.Behaviours
{
    using UnityEngine;

    public class AvoidEdges : DesiredVelocityProvider
    {
        [SerializeField]
        private Camera camera;
        
        private float edge = 0.05f;
        
        public override Vector3 GetDesiredVelocity()
        {
            var maxSpeed = Agent.VelocityLimit;
            var v = Agent.Velocity;
            var point = camera.WorldToViewportPoint(transform.position);

            if (point.x > 1 - edge)
            {
                return new Vector3(-maxSpeed, 0, 0);
                
            }
            if (point.x < edge)
            {
                return new Vector3(maxSpeed, 0, 0);
            }
            if (point.y > 1 - edge)
            {
                return new Vector3(0, 0, -maxSpeed);
            }
            if (point.y < edge)
            {
                return new Vector3(0, 0, maxSpeed);
            }

            return v;
        }
    }
}