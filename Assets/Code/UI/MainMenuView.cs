using Assets.Code.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [Zenject.Inject] private GameplayController gameplayCtrl;
    [Zenject.Inject] private StatsManager statsManager;

    [SerializeField] private Button playButton;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private TMPro.TMP_Text pointsLabel;
    [SerializeField] private TMPro.TMP_Text hitsLabel;
    [SerializeField] private TMPro.TMP_Text timeLabel;
    private static readonly string volumeKey = "volumeKey";

    // Start is called before the first frame update
    private void Awake()
    {
        playButton.onClick.AddListener(gameplayCtrl.StartGame);
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChange);
    }

    private void OnVolumeSliderValueChange(float sliderValue)
    {
        AudioListener.volume = sliderValue;
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(gameplayCtrl.StartGame);
    }

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(volumeKey))
        {
            volumeSlider.value = AudioListener.volume = PlayerPrefs.GetFloat(volumeKey);
        }
        else
        {
            volumeSlider.value = AudioListener.volume = 1;
        }

        pointsLabel.text = statsManager.PreviousStats.Score.ToString();
        hitsLabel.text = statsManager.PreviousStats.ShotsMade.ToString();
        timeLabel.text = statsManager.PreviousStats.FormattedGameplayTime;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeKey, AudioListener.volume);
    }
}
