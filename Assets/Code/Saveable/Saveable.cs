
using UnityEngine;

namespace Assets.Code.Saveable
{
    [System.Serializable]
    public abstract class Saveable<T> : ISaveable where T : Component
    {
        T parameter;
        public T Parameter => parameter;

        public Saveable(T parameter)
        {
            this.parameter = parameter;
        }
    }
}
