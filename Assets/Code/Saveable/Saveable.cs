namespace Assets.Code.Saveable
{
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

    public class SaveableStruct<T> : Saveable<T, T> where T : struct
    {
        public SaveableStruct(T input) : base(input)
        {

        }

        protected override void Save(T input)
        {
            savedValue = input;
        }
    }
}
