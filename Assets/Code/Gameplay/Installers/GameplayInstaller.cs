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
        [SerializeField]
        private GameplayController gameplayCtrl;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayController>().FromInstance(gameplayCtrl);
        }
    }
}
