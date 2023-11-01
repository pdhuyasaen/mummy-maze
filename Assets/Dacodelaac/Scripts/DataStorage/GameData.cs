using UnityEngine;

namespace Dacodelaac.DataStorage
{
    public static partial class GameData
    {
        const string STORY_SHOWN = "story_shown";

        public static bool StoryShown
        {
            get => Storage.Get(STORY_SHOWN, false);
            set => Storage.Set(STORY_SHOWN, value);
        }
    }
}