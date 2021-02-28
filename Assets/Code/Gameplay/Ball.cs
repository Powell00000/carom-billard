using Assets.Code.Saveable;
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
        private string serializedTransform;

        private SaveableTransform savedTransform;

        private float radius;

        public bool IsMoving => !rgbd.IsSleeping();

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
            rgbd.AddForce(force, ForceMode.Impulse);
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
    }
}