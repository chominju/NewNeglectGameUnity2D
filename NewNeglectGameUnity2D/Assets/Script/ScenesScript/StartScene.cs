using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEditor;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public string nextSceneName = "MainScene";
    public InputField playerNameInput;
    public GameObject startImage;
    public GameObject createPlayerPanel;

    public GameObject dataManager;
    public GameObject startButton;

    public void StartButton()
    {
       // DataManager.GetDataManager().RemoveAllData();
        SoundManager.GetInstance().PlayMenuClickSound();

        if (DataManager.GetDataManager().LoadPlayerData()) // 참이면 존재함
        {
            DataManager.GetDataManager().AllDataLoad();
            LoadNextScene();    // 다음 씬으로 넘어가야함.
        }
        else
        {
            // 플레이어 데이터가 존재하지않음. 생성해야함.
            startButton.SetActive(false);
            startImage.SetActive(false);
            createPlayerPanel.SetActive(true);
        }
    }

    public void InputNextButton()
    {
        Debug.Log("InputNextButton");
        CreatePlayerData();

        DataManager.GetDataManager().InitAllData();

        LoadNextScene();    // 다음 씬으로 넘어가야함.
    }

    public void LoadNextScene()
    {
        // 비동기적으로 Scene을 불러오기 위해 Coroutine을 사용한다.
        StartCoroutine(LoadMyAsyncScene());
    }

    public void CreatePlayerData()
    {
        DataManager.GetDataManager().CreatePlayerData(playerNameInput.text);
        //PlayerData playerData = ScriptableObject.CreateInstance<PlayerData>();
        //playerData.playerName = playerNameInput.text;

        //AssetDatabase.CreateAsset(playerData, playerDataSavePath);
    }


    IEnumerator LoadMyAsyncScene()
    {
        // AsyncOperation을 통해 Scene Load 정도를 알 수 있다.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // Scene을 불러오는 것이 완료되면, AsyncOperation은 isDone 상태가 된다.
        while (!asyncLoad.isDone)
        {

            DontDestroyOnLoad(dataManager);
            //testText.text = "로딩완료...";
            yield return null;
        }
    }

}
