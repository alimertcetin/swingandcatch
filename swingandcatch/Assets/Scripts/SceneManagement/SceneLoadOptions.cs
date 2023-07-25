using UnityEngine.SceneManagement;

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
        /// The build index of the scene that needs to be unloaded
        /// </summary>
        public int sceneToUnload;
        /// <summary>
        /// Should wait for other systems to be ready in order to activate the loaded scene
        /// </summary>
        public bool activateImmediately;
        
        public static SceneLoadOptions MenuLoad(int sceneToLoad)
        {
            return MenuLoad(sceneToLoad, SceneManager.GetActiveScene().buildIndex);
        }
        
        public static SceneLoadOptions MenuLoad(int sceneToLoad, int sceneToUnload)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneToLoad,
                loadingScreenType = LoadingScreenType.MenuLoading,
                sceneToUnload = sceneToUnload,
                activateImmediately = true,
            };
        }

        public static SceneLoadOptions LevelLoad(int sceneToLoad)
        {
            return LevelLoad(sceneToLoad, SceneManager.GetActiveScene().buildIndex);
        }

        public static SceneLoadOptions LevelLoad(int sceneToLoad, int sceneToUnload)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneToLoad,
                loadingScreenType = LoadingScreenType.LevelLoading,
                sceneToUnload = sceneToUnload,
                activateImmediately = false,
            };
        }

        public static SceneLoadOptions LoadOption(int sceneToLoad)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = sceneToLoad,
                loadingScreenType = LoadingScreenType.None,
                sceneToUnload = -1,
                activateImmediately = false,
            };
        }

        public static SceneLoadOptions UnloadOption(int sceneToUnload)
        {
            return new SceneLoadOptions
            {
                sceneToLoad = -1,
                loadingScreenType = LoadingScreenType.None,
                sceneToUnload = sceneToUnload,
                activateImmediately = false,
            };
        }
    }
}