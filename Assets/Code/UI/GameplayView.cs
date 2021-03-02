using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class GameplayView : MonoBehaviour
{
    [Zenject.Inject] private GameplayController gameplayCtrl;
    [Zenject.Inject] private HitBallController hitBallCtrl;

    [SerializeField] private Button replayButton;

    private void Start()
    {
        replayButton.onClick.AddListener(Replay);
        gameplayCtrl.OnGameStart += DisableButton;
        hitBallCtrl.BallHit += EnableButton;
        DisableButton();
    }

    private void DisableButton()
    {
        replayButton.interactable = false;
    }

    private void EnableButton()
    {
        replayButton.interactable = true;
    }

    private void Replay()
    {
        gameplayCtrl.Replay();
    }

    private void OnDestroy()
    {
        replayButton.onClick.RemoveListener(Replay);
        gameplayCtrl.OnGameStart -= DisableButton;
        hitBallCtrl.BallHit -= EnableButton;
    }
}
