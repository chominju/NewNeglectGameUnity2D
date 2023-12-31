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

        if (DataManager.GetDataManager().LoadPlayerData()) // ���̸� ������
        {
            DataManager.GetDataManager().AllDataLoad();
            LoadNextScene();    // ���� ������ �Ѿ����.
        }
        else
        {
            // �÷��̾� �����Ͱ� ������������. �����ؾ���.
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

        LoadNextScene();    // ���� ������ �Ѿ����.
    }

    public void LoadNextScene()
    {
        // �񵿱������� Scene�� �ҷ����� ���� Coroutine�� ����Ѵ�.
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
        // AsyncOperation�� ���� Scene Load ������ �� �� �ִ�.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // Scene�� �ҷ����� ���� �Ϸ�Ǹ�, AsyncOperation�� isDone ���°� �ȴ�.
        while (!asyncLoad.isDone)
        {

            DontDestroyOnLoad(dataManager);
            //testText.text = "�ε��Ϸ�...";
            yield return null;
        }
    }

}
