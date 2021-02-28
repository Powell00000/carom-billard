using Assets.Code.Saveable;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour, IMoveable, IRevertable
    {
        [Zenject.Inject]
        private GameplayController gameplayCtrl;

        [SerializeField]
        private Rigidbody rgbd;

        [SerializeField]
        private SphereCollider sphereCollider;

        [SerializeField]
        private LineRenderer lineRenderer;

        private SaveableTransform savedTransform;
        private float radius;
        private Vector3 forceDirection;
        private Vector3[] hitPoints;

        public bool IsMoving => !rgbd.IsSleeping();

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            hitPoints = new Vector3[6];
            CalculateRadius();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        public void AddForce(Vector3 force)
        {
            rgbd.AddForce(force, ForceMode.Impulse);
        }

        public void SetForceDirection(Vector3 direction)
        {
            forceDirection = direction.normalized;
        }

        public void Revert()
        {
            transform.position = savedTransform.SavedValue.Position;
            transform.rotation = savedTransform.SavedValue.Rotation;
        }

        public void Store()
        {
            savedTransform = new SaveableTransform(transform);
        }

        private void Update()
        {
            DrawHitLine();
        }

        private void DrawHitLine()
        {
            Vector3 currentDir = forceDirection;
            Vector3 currentPos = transform.position;
            hitPoints[0] = currentPos;
            for (int i = 0; i < 5; i++)
            {
                bool hit = Physics.SphereCast(currentPos, radius, currentDir, out var hitInfo);
                if (hit)
                {
                    currentPos = hitInfo.point + hitInfo.normal * radius;
                    hitPoints[i + 1] = currentPos;
                    currentDir = Vector3.Reflect(currentDir, hitInfo.normal);
                    if (hitInfo.rigidbody.TryGetComponent<Ball>(out var ball))
                    {
                        Vector3 hitDirection = hitInfo.point + hitInfo.normal * -1;
                        ball.SetForceDirection(hitDirection.normalized);
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        hitPoints[i] = hitPoints[i - 1];
                    }
                }



                lineRenderer.SetPositions(hitPoints);
            }

        }
    }
}