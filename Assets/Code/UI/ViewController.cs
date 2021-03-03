using Assets.Code.Gameplay;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [Zenject.Inject] private GameplayController gameplayCtrl;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameplayView;
    [SerializeField] private GameObject endGameView;

    // Start is called before the first frame update
    private void Start()
    {
        gameplayCtrl.OnStateChanged += GameStateChanged;
        GameStateChanged(gameplayCtrl.CurrentState);
    }

    private void GameStateChanged(GameplayController.GameState currentState)
    {
        mainMenu.SetActive(currentState == GameplayController.GameState.MainMenu);
        gameplayView.SetActive(currentState == GameplayController.GameState.Playing);
        endGameView.SetActive(currentState == GameplayController.GameState.GameEnded);
    }

    private void OnDestroy()
    {
        gameplayCtrl.OnStateChanged -= GameStateChanged;
    }
}
