using Assets.Code.Saveable;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour, IMoveable, IRevertable
    {
        [Zenject.Inject]
        private GameplayController gameplayCtrl;
        private static readonly uint maxPoints = 3;

        [SerializeField]
        private Rigidbody rgbd;

        [SerializeField]
        private SphereCollider sphereCollider;

        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private float speed;

        private SaveableTransform savedTransform;
        private float radius;
        private Vector3 forceDirection;
        private Vector3[] hitPoints;
        private bool drawLine;

        public float Speed => CurrentVelocity.magnitude;
        public Vector3 CurrentVelocity;

        public bool IsMoving => CurrentVelocity.magnitude > 0;//!rgbd.IsSleeping();

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            hitPoints = new Vector3[maxPoints];
            CalculateRadius();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        public void AddVelocity(Vector3 velocity)
        {
            Debug.Log($"{name} added {velocity} velocity");
            //rgbd.AddForce(velocity, ForceMode.Impulse);
            CurrentVelocity += velocity;
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

        private void Update()
        {
            StickToTable();

            Vector3 extrapolatedPositon = transform.position + CurrentVelocity * Time.deltaTime;
            float distance = Vector3.Distance(transform.position, extrapolatedPositon);
            bool hit = Physics.SphereCast(transform.position, radius, CurrentVelocity.normalized, out var hitInfo, distance, LayerMask.GetMask("Ball", "Band"));
            if (hit)
            {
                if (hitInfo.rigidbody.TryGetComponent<Ball>(out var otherBall))
                {
                    if (otherBall != this)
                    {
                        var dot = Vector3.Dot(CurrentVelocity.normalized, hitInfo.normal);
                        var computedVelocity = CurrentVelocity * dot;
                        otherBall.AddVelocity(hitInfo.normal * -computedVelocity.magnitude);
                        CurrentVelocity = Vector3.Reflect(CurrentVelocity, hitInfo.normal);
                    }
                }
                else
                {
                    CurrentVelocity = Vector3.Reflect(CurrentVelocity, hitInfo.normal);
                }
            }

            if (Speed > 0)
            {
                var nextVelocity = CurrentVelocity + CurrentVelocity.normalized * -1 * Time.deltaTime;
                if (Vector3.Dot(CurrentVelocity, nextVelocity) > 0)
                {
                    CurrentVelocity = nextVelocity;
                }
                else
                {
                    CurrentVelocity = Vector3.zero;
                }
            }

            transform.position += CurrentVelocity * Time.deltaTime;

            speed = Speed;
        }

        private void FixedUpdate()
        {
            return;
            Vector3 extrapolatedPositon = transform.position + rgbd.velocity * Time.fixedDeltaTime;
            float distance = Vector3.Distance(transform.position, extrapolatedPositon);
            bool hit = Physics.SphereCast(transform.position, radius, rgbd.velocity.normalized, out var hitInfo, distance, LayerMask.GetMask("Ball", "Band"));
            if (hit)
            {
                if (hitInfo.rigidbody.TryGetComponent<Ball>(out var ball))
                {
                    if (ball != this)
                    {
                        ball.AddVelocity(hitInfo.normal + rgbd.velocity * rgbd.mass);
                    }
                }
            }
        }

        private void LateUpdate()
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
                return;
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

        private void OnDrawGizmos()
        {
            Vector3 extrapolatedPositon = transform.position + CurrentVelocity * Time.fixedDeltaTime;
            float distance = Vector3.Distance(transform.position, extrapolatedPositon);
            Gizmos.DrawSphere(transform.position + CurrentVelocity.normalized * distance, radius);
        }

    }
}