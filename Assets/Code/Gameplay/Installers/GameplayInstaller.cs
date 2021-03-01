using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Code.Gameplay.Installers
{
    internal class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameplayController gameplayCtrl;
        [SerializeField] private CameraController cameraCtrl;
        [SerializeField] private HitBallController hitBallCtrl;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayController>().FromInstance(gameplayCtrl);
            Container.BindInterfacesAndSelfTo<CameraController>().FromInstance(cameraCtrl);
            Container.BindInterfacesAndSelfTo<HitBallController>().FromInstance(hitBallCtrl);

            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }
    }
}
