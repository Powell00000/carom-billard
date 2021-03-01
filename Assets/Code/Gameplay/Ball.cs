using Assets.Code.Saveable;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour, IMoveable, IRevertable
    {
        public enum BallColor { White, Red, Yellow };

        [Zenject.Inject]
        private HitBallController hitBallCtrl;

        [SerializeField]
        internal BallColor color;

        [SerializeField]
        private Rigidbody rgbd;

        [SerializeField]
        private SphereCollider additionalSphereCollider;

        [SerializeField]
        private LineRenderer lineRenderer;
        private Vector3 lastPositionDelta;

        private SaveableTransform savedTransform;
        private float radius;
        private Vector3 forceDirection;
        private Vector3[] hitPoints;
        private bool drawLine;
        private RaycastHit[] hitInfos;
        private HashSet<Rigidbody> rigidbodiesHit;
        private Collider[] overlapingColliders;

        private static readonly short maxLinePoints = 3;

        public float Speed => CurrentVelocity.magnitude;
        public Vector3 CurrentVelocity;
        public bool IsMoving => !rgbd.IsSleeping();

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            hitPoints = new Vector3[maxLinePoints];
            overlapingColliders = new Collider[5];
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
            var energyLoss = CurrentVelocity.normalized * rgbd.mass;
            if (energyLoss.magnitude >= velocity.magnitude)
            {
                return;
            }

            velocity -= energyLoss;
            Debug.Log($"{name} added velocity {velocity}");
            CurrentVelocity += velocity;
        }

        private void ReflectVelocityWithLoss(Vector3 reflectionNormal)
        {
            var energyLoss = CurrentVelocity.normalized * rgbd.mass;
            if (energyLoss.magnitude >= CurrentVelocity.magnitude)
            {
                CurrentVelocity = Vector3.zero;
                return;
            }
            else
            {
                CurrentVelocity -= energyLoss;
            }

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

        private void FixedUpdate()
        {
            StickToTable();
            if (IsMoving)
            {
                CastForOverlap();
                CastForObjects();
            }
            CalculateSpeed();

            ApplyVelocityToTransform();
        }

        private void OnCollisionEnter(Collision collision)
        {
            return;
            if (collision.rigidbody.TryGetComponent<Ball>(out var otherBall))
            {
                OtherBallHit(ref otherBall);
            }
        }

        private void CastForOverlap()
        {
            return;
            int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, overlapingColliders, LayerMask.GetMask("Ball", "Band"));
            for (int i = 0; i < hits; i++)
            {
                var collider = overlapingColliders[i];
                if (collider.attachedRigidbody == rgbd)
                {
                    continue;
                }

                //move back position by one frame
                var previousPosition = transform.position - lastPositionDelta;
                //calculate closest point on collider
                var closestPoint = collider.ClosestPoint(previousPosition);
                var surfaceNormal = (transform.position - closestPoint).normalized;

                var dot = Vector3.Dot(CurrentVelocity.normalized, surfaceNormal);
                float penetrationDepth = Vector3.Distance(closestPoint, transform.position);
                penetrationDepth += Mathf.Sign(dot) * radius;

                transform.position -= CurrentVelocity.normalized * penetrationDepth;
            }
        }

        private void CastForObjects()
        {
            Vector3 extrapolatedPositon = transform.position + ApplyTimeScale(ApplyDrag(CurrentVelocity));
            additionalSphereCollider.transform.position = extrapolatedPositon;
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

                    var velocitySum = otherVelocity + thisVelocity;

                    var dot = Vector3.Dot(CurrentVelocity.normalized, hitInfo.normal);
                    var computedVelocity = CurrentVelocity * dot;
                    otherBall.AddVelocity(hitInfo.normal * -computedVelocity.magnitude);
                }
                Debug.Log($"{name} hit {hitInfo.rigidbody.gameObject.name}");
                ReflectVelocityWithLoss(hitInfo.normal);
            }

            rigidbodiesHit.Clear();
        }

        protected virtual void OtherBallHit(ref Ball otherBall)
        {
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
            return velocity + velocity.normalized * -1 * Time.deltaTime;
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

            transform.position = savedTransform.SavedValue.Position;
            transform.rotation = savedTransform.SavedValue.Rotation;
        }

        public void Store()
        {
            savedTransform = new SaveableTransform(transform);
        }

        private void DrawHitLine()
        {
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
                        break;
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