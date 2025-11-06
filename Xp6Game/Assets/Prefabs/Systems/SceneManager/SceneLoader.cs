using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public List<SceneRef> scenes = new List<SceneRef>();

    EventBinding<StartGameEvent> playButtonBinding;

    EventBinding<GameWinEvent> gameWinBinding;

    EventBinding<GameSceneLoaded> gameSceneLoadedBinding;
    EventBinding<GameOverEvent> gameOverBinding;



    async void Start()
    {
        
        await Initialize();
    }

    public async UniTask Initialize()
    {
        SceneManager.sceneLoaded += (Scene _scene, LoadSceneMode mode) =>
        {
            AddSceneToList(scene: _scene);
        };

        SceneManager.sceneUnloaded += (Scene _scene) =>
        {
            RemoveSceneFromList(scene: _scene);
        };

        await BindEvents();
    }
    async UniTask BindEvents()
    {
        playButtonBinding = new EventBinding<StartGameEvent>(OnMainMenuPlayButtonClicked);
        EventBus<StartGameEvent>.Register(playButtonBinding);

        gameWinBinding = new EventBinding<GameWinEvent>(OnGameWin); 

        gameSceneLoadedBinding = new EventBinding<GameSceneLoaded>(() =>
        {
            DesactivateSceneByName("LoadingScreen");
        });
        EventBus<GameSceneLoaded>.Register(gameSceneLoadedBinding);



        gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
        EventBus<GameOverEvent>.Register(gameOverBinding);




        await UniTask.CompletedTask;
    }

    private async void OnGameOver(GameOverEvent arg0)
    {
        await CreateSceneByName("GameOver");
        ActivateSceneByName("GameOver");
        SetMainScene("GameOver");
        // DesactivateSceneByName("Game");
        UnloadSceneByName("Game");
        // UnloadSceneByName("LoadingScreen");
    }

    private async void OnGameWin(GameWinEvent arg0)
    {
        await CreateSceneByName("GameWin");
        ActivateSceneByName("GameWin");
        SetMainScene("GameWin");
        // DesactivateSceneByName("Game");
        UnloadSceneByName("Game");
        // UnloadSceneByName("LoadingScreen");

    }

    private void OnMainMenuPlayButtonClicked()
    {
        ActivateSceneByName(SceneNames.LoadingScreen.ToString());
    }


    public void AddSceneToList(Scene scene)
    {
        // activeScenes.Add(scene);
        scenes.Add(new SceneRef { sceneName = scene.name, reference = scene });

        ChangeRootObjectsState(scene, false);
    }

    private void RemoveSceneFromList(Scene scene)
    {
        // activeScenes.Remove(scene);
        foreach (var sceneRef in scenes)
        {
            if (sceneRef.reference == scene)
            {
                scenes.Remove(sceneRef);
                break;
            }
        }
    }
    public async UniTask CreateSceneByName(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        await UniTask.CompletedTask;
    }
    public void ActivateSceneByName(string sceneName)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneName == sceneName)
            {
                ChangeRootObjectsState(scene.reference, true);
                return;
            }
        }
    }
    public void DesactivateSceneByName(string sceneName)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneName == sceneName)
            {
                ChangeRootObjectsState(scene.reference, false);
                return;
            }
        }
    }

    private void ChangeRootObjectsState(Scene scene, bool isVisible)
    {
        Debug.Log(scene.name);
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            rootObject.SetActive(isVisible);
        }
    }

    public void UnloadSceneByName(string v)
    {
        if (isSceneLoaded(v))
            SceneManager.UnloadSceneAsync(v);
    }
    public void SetMainScene(string sceneName)
    {
        Scene? _s = GetSceneByName(sceneName);
        if (_s != null)
        {
            SceneManager.SetActiveScene((Scene)_s);
        }
    }

    Scene? GetSceneByName(string sceneName)
    {

        foreach (var scene in scenes)
        {
            if (scene.sceneName == sceneName)
            {
                return scene.reference;
            }
        }

        return null;
    }

    bool isSceneLoaded(string sceneName)
    {
        foreach (var scene in scenes)
        {
            if (scene.sceneName == sceneName)
            {
                return scene.reference.IsValid();
            }
        }

        return false;
    }

    public void TransferObjects(string fromScene, string toScene)
    {
        Scene toSceneRef = default;
        Scene fromSceneRef = default;
        foreach (var scene in scenes)
        {
            if (toScene == scene.sceneName)
            {
                toSceneRef = scene.reference;
            }
            if (fromScene == scene.sceneName)
            {
                fromSceneRef = scene.reference;
            }
        }

        if (!toSceneRef.IsValid())
        {
            return;

        }
        Debug.Log("Merging Scenes");
        SceneManager.MergeScenes(fromSceneRef, toSceneRef);


    }

}

[System.Serializable]
public struct SceneRef
{
    public string sceneName;
    public Scene reference;
}

public enum SceneNames
{
    MainMenu,
    LoadingScreen,
    Game,
    GameWin,
    GameOver,
}