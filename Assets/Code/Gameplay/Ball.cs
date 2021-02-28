using Assets.Code.Saveable;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rgbd;

        [SerializeField]
        private string serializedTransform;

        private SaveableTransform savedTransform;

        private float radius;

        private void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            CalculateRadius();
        }

        private void CalculateRadius()
        {
            radius = transform.localScale.y / 2;
        }

        public void AddForce(Vector3 force)
        {
            savedTransform = new SaveableTransform(transform);
            rgbd.AddForce(force, ForceMode.Impulse);
        }

        [ContextMenu("Revert")]
        private void Revert()
        {
            transform.position = savedTransform.SavedValue.Position;
            transform.rotation = savedTransform.SavedValue.Rotation;
        }
    }
}