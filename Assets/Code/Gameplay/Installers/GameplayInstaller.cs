using UnityEngine;
using Zenject;

namespace Assets.Code.Gameplay.Installers
{
    internal class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayController gameplayCtrl;
        [SerializeField] private CameraController cameraCtrl;
        [SerializeField] private HitBallController hitBallCtrl;
        [SerializeField] private InputController inputCtrl;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayController>().FromInstance(gameplayCtrl);
            Container.BindInterfacesAndSelfTo<CameraController>().FromInstance(cameraCtrl);
            Container.BindInterfacesAndSelfTo<HitBallController>().FromInstance(hitBallCtrl);
            Container.BindInterfacesAndSelfTo<InputController>().FromInstance(inputCtrl);

            Container.BindInterfacesAndSelfTo<PointsManager>().AsSingle();
        }
    }
}
