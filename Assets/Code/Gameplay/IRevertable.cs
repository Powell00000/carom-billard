namespace Assets.Code.Gameplay
{
    internal interface IRevertable
    {
        void Store();
        void Revert();
    }
}
