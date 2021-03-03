using Assets.Code.Saveable;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour, IMoveable, IRevertable
    {
        public enum BallColor { White, Red, Yellow };

        [SerializeField]
        internal BallColor color;

        [SerializeField]
        private Rigidbody rgbd;

        [SerializeField]
        private SphereCollider additionalSphereCollider;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField] private AudioSource hitAudioSource;
        private Vector3 lastPositionDelta;

        private SaveableTransform initialTransform;
        private SaveableTransform savedTransform;
        private float radius;
        private Vector3 forceDirection;
        private Vector3[] hitPoints;
        private bool drawLine;
        private RaycastHit[] hitInfos;
        private HashSet<Rigidbody> rigidbodiesHit;

        private static readonly short maxLinePoints = 3;

        public float Speed => CurrentVelocity.magnitude;
        public Vector3 CurrentVelocity;
        public bool IsMoving => Speed > 0;

        public virtual void Initialize()
        {
            savedTransform = null;
            if (initialTransform == null)
            {
                initialTransform = new SaveableTransform(transform);
            }
            else
            {
                ApplySavedTransform(initialTransform);
            }

            hitPoints = new Vector3[maxLinePoints];
            hitInfos = new RaycastHit[5];
            rigidbodiesHit = new HashSet<Rigidbody>();
            CalculateRadius();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        public void AddVelocity(Vector3 velocity)
        {
            CurrentVelocity += velocity;
            CastForObjects();
        }

        public void SetVelocity(Vector3 velocity)
        {
            CurrentVelocity = velocity;
            CastForObjects();
        }

        private void ReflectVelocity(Vector3 reflectionNormal)
        {
            CurrentVelocity = Vector3.Reflect(CurrentVelocity, reflectionNormal);
        }

        public void DrawExtrapolatedLine(Vector3 direction)
        {
            if (drawLine)
            {
                return;
            }

            drawLine = true;
            forceDirection = direction.normalized;
            DrawHitLine();
        }

        private void StickToTable()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var raycastHit, LayerMask.GetMask("Table")))
            {
                transform.position = transform.position.WithY(raycastHit.point.y + radius);
            }
        }

        public void CustomUpdate()
        {
            StickToTable();
            if (IsMoving)
            {
                CastForObjects();
            }
            CalculateSpeed();
            ApplyVelocityToTransform();
            rigidbodiesHit.Clear();
        }

        private void CastForObjects()
        {
            Vector3 extrapolatedPositon = transform.position + ApplyTimeScale(ApplyDrag(CurrentVelocity));
            float distance = Vector3.Distance(transform.position, extrapolatedPositon);
            int hits = Physics.SphereCastNonAlloc(transform.position, radius, CurrentVelocity.normalized, hitInfos, distance, LayerMask.GetMask("Ball", "Band"));
            for (int i = 0; i < hits; i++)
            {
                var hitInfo = hitInfos[i];

                if (hitInfo.rigidbody == rgbd)
                {
                    continue;
                }

                if (rigidbodiesHit.Contains(hitInfo.rigidbody))
                {
                    continue;
                }

                rigidbodiesHit.Add(hitInfo.rigidbody);

                if (hitInfo.rigidbody.TryGetComponent<Ball>(out var otherBall))
                {
                    OtherBallHit(ref otherBall);

                    var otherVelocity = otherBall.CurrentVelocity;
                    var thisVelocity = CurrentVelocity;

                    var otherNewVelocity = thisVelocity;
                    var thisNewVelocity = otherVelocity;

                    var dotOfVelocities = Vector3.Dot(thisVelocity.normalized, otherVelocity.normalized);
                    if (dotOfVelocities > 0)
                    {

                    }
                    else if (dotOfVelocities < 0)
                    {
                        var perpendicularNormal = Vector3.Cross(Vector3.up, hitInfo.normal).normalized;
                        otherNewVelocity = Vector3.Reflect(thisVelocity, perpendicularNormal);
                        thisNewVelocity = Vector3.Reflect(otherVelocity, perpendicularNormal);
                    }
                    else if (dotOfVelocities == 0)
                    {
                        otherNewVelocity = thisVelocity;
                        thisNewVelocity = Vector3.Reflect(thisVelocity, hitInfo.normal);
                    }

                    otherBall.SetVelocity(otherNewVelocity);
                    SetVelocity(thisNewVelocity);

                    /*
                    var velocitySum = otherVelocity + thisVelocity;

                    var dot = Vector3.Dot(CurrentVelocity.normalized, hitInfo.normal);
                    var computedVelocity = CurrentVelocity * dot;
                    otherBall.AddVelocity(hitInfo.normal * -computedVelocity.magnitude);
                    */
                }
                else
                {
                    ReflectVelocity(hitInfo.normal);
                }

            }

        }

        protected virtual void OtherBallHit(ref Ball otherBall)
        {
            hitAudioSource.volume = Speed / HitBallController.MaxForce;
            hitAudioSource.Play();
        }

        private void CalculateSpeed()
        {
            if (Speed > 0)
            {
                var nextVelocity = ApplyDrag(CurrentVelocity);
                if (Vector3.Dot(CurrentVelocity, nextVelocity) > 0)
                {
                    CurrentVelocity = nextVelocity;
                }
                else
                {
                    CurrentVelocity = Vector3.zero;
                }
            }
        }

        private Vector3 ApplyDrag(Vector3 velocity)
        {
            return velocity + velocity.normalized * -1 * Time.fixedDeltaTime;
        }

        private Vector3 ApplyTimeScale(Vector3 velocity)
        {
            return velocity * Time.fixedDeltaTime;
        }

        private void ApplyVelocityToTransform()
        {
            lastPositionDelta = ApplyTimeScale(CurrentVelocity);
            transform.position += lastPositionDelta;
        }

        private void Update()
        {
            lineRenderer.enabled = drawLine;
            if (drawLine)
            {
                drawLine = false;
            }
        }

        public void Revert()
        {
            if (savedTransform == null)
            {
                return;
            }

            ApplySavedTransform(savedTransform);
        }

        private void ApplySavedTransform(SaveableTransform saveableTransform)
        {
            transform.position = saveableTransform.SavedValue.Position;
            transform.rotation = saveableTransform.SavedValue.Rotation;
        }

        public void Store()
        {
            savedTransform = new SaveableTransform(transform);
        }

        private void DrawHitLine()
        {
            if (hitPoints == null)
            {
                return;
            }

            Vector3 currentDir = forceDirection;
            Vector3 currentPos = transform.position;
            hitPoints[0] = currentPos;
            for (int i = 1; i < hitPoints.Length; i++)
            {
                bool hit = Physics.SphereCast(currentPos, radius, currentDir, out var hitInfo);
                if (hit)
                {
                    currentPos = hitInfo.point + hitInfo.normal * radius;
                    hitPoints[i] = currentPos;
                    lineRenderer.positionCount = i + 1;
                    if (hitInfo.rigidbody.TryGetComponent<Ball>(out var ball))
                    {
                        if (ball != this)
                        {
                            ball.DrawExtrapolatedLine(hitInfo.normal * -1);
                        }
                        //break;
                    }

                    currentDir = Vector3.Reflect(currentDir, hitInfo.normal);
                }
            }
            for (int i = 0; i < hitPoints.Length - 1; i++)
            {
                Debug.DrawLine(hitPoints[i], hitPoints[i + 1]);
            }

            lineRenderer.SetPositions(hitPoints);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 extrapolatedPositon = transform.position + ApplyTimeScale(ApplyDrag(CurrentVelocity));
            float distance = Vector3.Distance(transform.position, extrapolatedPositon);
            Gizmos.DrawSphere(transform.position + CurrentVelocity.normalized * distance, radius);
        }
    }
#endif
}