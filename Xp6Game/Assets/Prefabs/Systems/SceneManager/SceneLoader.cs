using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public List<SceneRef> scenes = new List<SceneRef>();

    EventBinding<MainMenuPlayButtonClickedEvent> playButtonBinding;

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
        playButtonBinding = new EventBinding<MainMenuPlayButtonClickedEvent>(OnMainMenuPlayButtonClicked);
        EventBus<MainMenuPlayButtonClickedEvent>.Register(playButtonBinding);

        await UniTask.CompletedTask;
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
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
            rootObject.SetActive(isVisible);
        }
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
    GameOver,
}