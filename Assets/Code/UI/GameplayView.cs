using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class GameplayView : MonoBehaviour
{
    [Zenject.Inject] private GameplayController gameplayCtrl;

    [SerializeField] private Button replayButton;

    private void Start()
    {
        replayButton.onClick.AddListener(Replay);
    }

    private void Replay()
    {
        gameplayCtrl.Replay();
    }
}
