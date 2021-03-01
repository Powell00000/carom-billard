using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class GameplayView : MonoBehaviour
{
    [Zenject.Inject]
    GameplayController gameplayCtrl;

    [SerializeField]
    Button replayButton;

    private void Start()
    {
        replayButton.onClick.AddListener(Replay);
    }

    void Replay()
    {
        gameplayCtrl.Replay();
    }
}
