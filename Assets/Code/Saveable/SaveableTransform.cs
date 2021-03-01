using UnityEngine;

namespace Assets.Code.Saveable
{
    internal class SaveableTransform : Saveable<Transform, SaveableTransform.TransformData>
    {
        [System.Serializable]
        public class TransformData
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        public SaveableTransform(Transform transform) : base(transform)
        {
        }

        protected override void Save(Transform parameter)
        {
            savedValue = new TransformData()
            {
                Position = parameter.position,
                Rotation = parameter.rotation
            };
        }
    }
}
