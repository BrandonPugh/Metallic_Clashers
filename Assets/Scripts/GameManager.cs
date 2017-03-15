using System;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

using Combat;

using Library;

using StageSelection;

using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameState
    {
        Combat = 2,
        MainMenu = 0,
        StageSelection = 1,
        StagePreparation,
    }

    private const string savePath = "/PlayerData.xml";

    public GameState gameState;

    public PlayerData playerData;

    private int currentScene = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        if (File.Exists(Environment.CurrentDirectory + savePath))
            Load();
        else
        {
            playerData.staminaInformation = new StaminaInformation
            {
                value = 0,
                timeLastPlayed = DateTime.Now.ToString()
            };
            Save();
        }
        gameState = GameState.MainMenu;
        AddSceneListeners();
    }

    //private void OnApplicationQuit()
    //{
    //    playerData.staminaInformation.maxValue = StaminaManager.self.maxValue;
    //    playerData.staminaInformation.value = StaminaManager.self.value;
    //    playerData.staminaInformation.timeLastPlayed = DateTime.Now.ToString();
    //    Save();
    //}

    private void OnCombatEnd()
    {
        LoadScene(1);
    }

    private void OnStageSelectionEnd()
    {
        LoadScene(2);
    }

    private void AddSceneListeners()
    {
        switch (currentScene)
        {
            case 2:     // Combat
                CombatManager.self.onCombatEnd.AddListener(OnCombatEnd);
                CombatManager.self.onPlayerTurn.AddListener(AudioManager.self.PlayDragSound);
                gameState = GameState.Combat;
                break;

            case 1:     // Stage Selection
                StageSelectionManager.self.onStageSelectionEnd.AddListener(OnStageSelectionEnd);
                gameState = GameState.StageSelection;
                break;

            default:
                break;
        }
        AudioManager.self.ChangeMusic(currentScene);
    }

    [ContextMenu("Save Player")]
    private void Save()
    {
        //Saving PlayerData
        var playerPath = Environment.CurrentDirectory + savePath;
        var playerStream = File.Create(playerPath);

        var serializer = new XmlSerializer(typeof(PlayerData));
        serializer.Serialize(playerStream, playerData);
        playerStream.Close();
    }

    [ContextMenu("Load Player")]
    private void Load()
    {
        var reader = new XmlSerializer(typeof(PlayerData));
        var file = new StreamReader(Environment.CurrentDirectory + savePath);

        playerData = (PlayerData)reader.Deserialize(file);
        file.Close();
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    public bool NextScene()
    {
        if (currentScene >= SceneManager.sceneCountInBuildSettings)
            return false;

        StartCoroutine(LoadSceneCoroutine(currentScene + 1));
        return true;
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        currentScene = sceneIndex;

        while (!asyncOperation.isDone) { yield return null; }

        currentScene = sceneIndex;
        AddSceneListeners();
    }
}
