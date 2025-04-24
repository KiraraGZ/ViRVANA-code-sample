using UnityEngine;

namespace Magia.Enemy.Seraph
{
    [RequireComponent(typeof(Rigidbody))]
    public class SeraphMovement : MonoBehaviour
    {
        private static float MaxmimumHeight = 150f;
        private static float ManifestDuration = 15f;
        // private static float RetreatDuration = 3f;

        [SerializeField] private Rigidbody rb;
        public float MinimumHeight;
        public AnimationCurve manifestCurve;
        public AnimationCurve retreatCurve;

        private SeraphMovementState movementMode;
        private float startTime;
        public bool EnableCombat => movementMode == SeraphMovementState.COMBAT;

        public void Initialize()
        {
            movementMode = SeraphMovementState.MANIFEST;

            manifestCurve = TransformCurve(manifestCurve, MinimumHeight, MaxmimumHeight);
            retreatCurve = TransformCurve(retreatCurve, MinimumHeight, MaxmimumHeight);

            MinimumHeight = 40f;

            rb.transform.position = new Vector3(rb.transform.position.x, MaxmimumHeight, rb.transform.position.z);
            startTime = Time.time;
        }

        public void Dispose()
        {
            manifestCurve = TransformCurve(manifestCurve, 0, 1);
            retreatCurve = TransformCurve(retreatCurve, 0, 1);
        }

        public void PhysicsUpdate()
        {
            switch (movementMode)
            {
                case SeraphMovementState.MANIFEST:
                    if (Time.time - startTime >= ManifestDuration)
                    {
                        movementMode = SeraphMovementState.COMBAT;
                        break;
                    }
                    rb.transform.position = new Vector3(rb.transform.position.x, manifestCurve.Evaluate((Time.time - startTime) / ManifestDuration), rb.transform.position.z);
                    break;
                case SeraphMovementState.RETREAT:
                    break;
            }
        }

        public AnimationCurve TransformCurve(AnimationCurve curve, float a, float b)
        {
            AnimationCurve transformedCurve = new();

            foreach (var key in curve.keys)
            {
                float transformedY = a + (b - a) * key.value;

                Keyframe transformedKey = new(key.time, transformedY, key.inTangent, key.outTangent);
                transformedCurve.AddKey(transformedKey);
            }

            return transformedCurve;
        }
    }

    public enum SeraphMovementState
    {
        MANIFEST,
        COMBAT,
        RETREAT,
    }
}