using System.Collections;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameInitiator : MonoBehaviour
{
    [SerializeField] private SceneAsset mainMenuScene;

    [SerializeField] private SceneAsset gameScene;


    public async void Start()
    { 
        await Initialize(); 
    }

    // Update is called once per frame
    void Update()
    {

    }
 
    private async UniTask Initialize()
    {
        await SceneManager.LoadSceneAsync(gameScene.name, LoadSceneMode.Additive);

        Debug.Log("Game Scene Loaded");
    }


}
