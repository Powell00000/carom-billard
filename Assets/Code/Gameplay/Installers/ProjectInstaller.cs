using Zenject;

namespace Assets.Code.Gameplay.Installers
{
    internal class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StatsManager>().AsSingle();
        }
    }
}
