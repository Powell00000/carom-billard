using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour
    {
        private float radius;
        private Vector3 currentForce;

        [SerializeField]
        private float drag = 1f;

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            CalculateRadius();
        }

        protected virtual void FixedUpdate()
        {
            StickToTable();
            ApplyForceToPosition();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        private void StickToTable()
        {

            if (Physics.Raycast(transform.position, Vector3.down, out var raycastHit, LayerMask.GetMask("Table")))
            {
                transform.position = transform.position.WithY(raycastHit.point.y + radius);
            }
        }

        protected virtual void ApplyForceToPosition()
        {
            transform.position += currentForce * Time.deltaTime;

            var flippedForce = currentForce.Flipped();
            flippedForce = flippedForce.normalized;
            flippedForce *= drag;
            flippedForce *= Time.deltaTime;

            if (currentForce.magnitude <= flippedForce.magnitude)
            {
                currentForce = Vector3.zero;
            }
            else
            {
                currentForce += flippedForce;
            }
        }

        public void AddForce(Vector3 force)
        {
            currentForce += force;
        }

        [ContextMenu("Push")]
        private void Push()
        {
            AddForce(Vector3.forward * 2);
        }
    }
}