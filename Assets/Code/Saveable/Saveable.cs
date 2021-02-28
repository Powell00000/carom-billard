namespace Assets.Code.Saveable
{
    [System.Serializable]
    public abstract class Saveable<T, W> : ISaveable where W : new()
    {
        protected W savedValue;
        public W SavedValue => savedValue;

        public Saveable(T input)
        {
            Save(input);
        }

        protected abstract void Save(T input);
    }
}
