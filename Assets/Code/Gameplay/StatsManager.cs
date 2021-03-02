using System;
using UnityEngine;
using Zenject;

namespace Assets.Code.Gameplay
{
    public class StatsManager : IInitializable
    {
        [System.Serializable]
        public struct Stats
        {
            public int Score;
            public int ShotsMade;
            public TimeSpan TimeSpent;
        }

        private Stats currentStats;
        private Stats previousStats;
        private static readonly string statsKey = "stats";

        public Stats PreviousStats => previousStats;

        public void Initialize()
        {
            if (PlayerPrefs.HasKey(statsKey))
            {
                Stats loadedStats = JsonUtility.FromJson<Stats>(PlayerPrefs.GetString(statsKey));
                previousStats = loadedStats;
            }
        }

        public void SaveStats()
        {
            previousStats = currentStats;
            PlayerPrefs.SetString(statsKey, JsonUtility.ToJson(currentStats));
        }

        public void SetScore(int score)
        {
            currentStats.Score = score;
        }

        public void SetShotsMade(int shotsMade)
        {
            currentStats.ShotsMade = shotsMade;
        }

        public void SetTimeSpent(TimeSpan timeSpent)
        {
            currentStats.TimeSpent = timeSpent;
        }
    }
}
