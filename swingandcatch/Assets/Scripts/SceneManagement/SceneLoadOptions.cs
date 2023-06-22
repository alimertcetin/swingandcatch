namespace TheGame.SceneManagement
{
    public enum LoadingScreenType
    {
        None = 0,
        LevelLoading,
        MenuLoading
    }
    
    public struct SceneLoadOptions
    {
        /// <summary>
        /// The build index of the scene that needs to be loaded
        /// </summary>
        public int sceneToLoad;
        /// <summary>
        /// What type of loading screen should the loading system use
        /// </summary>
        public LoadingScreenType loadingScreenType;
        /// <summary>
        /// Should unload the active scene
        /// </summary>
        public bool unloadActiveScene;
        /// <summary>
        /// Should wait for other systems to be ready in order to activate the loaded scene
        /// </summary>
        public bool activateImmediately;
        
        public static SceneLoadOptions MenuLoad(int sceneIndex)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneIndex,
                loadingScreenType = LoadingScreenType.MenuLoading,
                unloadActiveScene = true,
                activateImmediately = true,
            };
        }

        public static SceneLoadOptions LevelLoad(int sceneIndex)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneIndex,
                loadingScreenType = LoadingScreenType.LevelLoading,
                unloadActiveScene = true,
                activateImmediately = false,
            };
        }
    }
}