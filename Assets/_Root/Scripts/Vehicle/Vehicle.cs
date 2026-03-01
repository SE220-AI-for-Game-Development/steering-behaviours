namespace Ai4Gamedev.Steerings
{
    using UnityEngine;

    public class Vehicle : MonoBehaviour
    {
        private Vector3 velocity;

        private Vector3 acceleration;

        [SerializeField]
        private float mass = 1;

        [SerializeField]
        private float frictionValue = 0.5f;

        [SerializeField, Range(1, 60)]
        private float velocityLimit = 3;

        [SerializeField]
        private float Epsilon = 0.05f;

        public float VelocityLimit => velocityLimit;

        public Vector3 Velocity => velocity;

        public void ApplyForce(Vector3 force)
        {
            force /= mass;
            acceleration += force;
        }

        private void Update()
        {
            ApplyFriction();
            
            ApplyForces();

            void ApplyFriction()
            {
                if (velocity.magnitude < Epsilon)
                {
                    return;
                }

                var friction = -velocity.normalized * frictionValue;
                ApplyForce(friction);
            }

            void ApplyForces()
            {
                velocity += acceleration * Time.deltaTime;
                velocity = Vector3.ClampMagnitude(velocity, velocityLimit);

                if (velocity.magnitude < Epsilon)
                {
                    velocity = Vector3.zero;
                }
                else
                {
                    // Only rotate the car when moving forward so that reversing
                    // doesn't flip transform.forward and break input direction next frame
                    bool movingForward = Vector3.Dot(velocity, transform.forward) >= 0f;
                    if (movingForward)
                    {
                        transform.rotation = Quaternion.LookRotation(velocity);
                        transform.position += velocity * Time.deltaTime;
                    }
                }

                acceleration = Vector3.zero;
            }
        }

        
    }
}