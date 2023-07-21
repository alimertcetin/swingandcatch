using System.Collections.Generic;

namespace TheGame.SceneManagement
{
    public class LevelData
    {
        public int lastPlayedLevel { get; private set; }
        List<int> levelIndices;

        public LevelData(IEnumerable<int> levelIndices)
        {
            this.levelIndices = new List<int>(levelIndices);
            // Set last player level same as the first level index when initializing
            this.lastPlayedLevel = this.levelIndices[0];
        }

        public void SetLastPlayedLevel(int levelIndex)
        {
            this.lastPlayedLevel = levelIndex;
        }

        /// <summary>
        /// If false next level is the first level, meaning of we came back to beginning
        /// </summary>
        public bool TryGetNextLevel(int currentLevel, out int nextLevelBuildIndex)
        {
            int index = levelIndices.IndexOf(currentLevel);
            var nextIndex = (index + 1) % levelIndices.Count;
            nextLevelBuildIndex = levelIndices[nextIndex];
            return nextIndex != 0;
        }
    }
}