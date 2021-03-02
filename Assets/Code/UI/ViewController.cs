using Assets.Code.Gameplay;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [Zenject.Inject] private GameplayController gameplayCtrl;

    [SerializeField] private GameObject gameplayView;

    // Start is called before the first frame update
    private void Start()
    {
        gameplayCtrl.OnStateChanged += GameStateChanged;
    }

    private void GameStateChanged(GameplayController.GameState currentState)
    {
        gameplayView.SetActive(currentState == GameplayController.GameState.Playing);
    }

    private void OnDestroy()
    {
        gameplayCtrl.OnStateChanged -= GameStateChanged;
    }
}
