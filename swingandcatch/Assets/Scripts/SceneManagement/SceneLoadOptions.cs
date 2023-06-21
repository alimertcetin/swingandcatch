namespace TheGame.SceneManagement
{
    public struct SceneLoadOptions
    {
        public int sceneToLoad;
        public bool displayLoadingScreen;
        public bool unloadActiveScene;
        public bool activateImmediately;
        
        public static SceneLoadOptions MenuLoad(int sceneIndex)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneIndex,
                displayLoadingScreen = false,
                unloadActiveScene = true,
                activateImmediately = true,
            };
        }

        public static SceneLoadOptions LevelLoad(int sceneIndex)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneIndex,
                displayLoadingScreen = true,
                unloadActiveScene = true,
                activateImmediately = false,
            };
        }
    }
}