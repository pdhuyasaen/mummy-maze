using System.Collections.Generic;
using Dacodelaac.Core;
using UnityEngine;

namespace Dev.Scripts.Level
{
    [CreateAssetMenu(menuName = "Levels Data")]
    public class LevelsData : BaseSO
    {
        [SerializeField] private List<LevelMap> levels;

        public int LevelsCount => levels.Count;

        public LevelMap GetLevel(int level)
        {
            level %= levels.Count;
            return levels[level];
        }
    }
}