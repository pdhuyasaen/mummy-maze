using _Root.Scripts.Pattern;
using Dacodelaac.Core;
using Dacodelaac.Events;
using UnityEngine;

namespace Dev.Scripts.Level
{
    public class LevelRoot : BaseMono
    {
        [SerializeField] private LevelsData levels;
        [SerializeField] private Transform parent;
        [SerializeField] private BooleanEvent showLoadingLevelEvent;

        private LevelMap curLevelPrefab;

        public void LoadLevel(int currentLevel)
        {
            parent.Clear();
            var level = levels.GetLevel(currentLevel);
            if (level != null)
            {
                curLevelPrefab = Instantiate(level, parent);
                curLevelPrefab.Initialize();
            }

            showLoadingLevelEvent.Raise(false);
        }
        
    }
}