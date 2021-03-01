using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{

    public CinemachineVirtualCamera cam;
    public TextMeshProUGUI levelNoText;
    int levelNo;
    int levelCounter;
    int screenWidth;
    int screenHeight;

    private void Start()
    {
        levelNo = SceneManager.GetActiveScene().buildIndex;
        levelCounter = PlayerPrefs.GetInt("Level Counter", 1);
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        if(screenWidth == 720 && screenHeight == 1280)
        {
            cam.m_Lens.FieldOfView = 60;
        }

        if(screenWidth == 1080 && (screenHeight == 2340 || screenHeight == 2400))
        {
            cam.m_Lens.FieldOfView = 70;
        }

        //levelNoText.text = "Level " + levelNo.ToString();
        levelNoText.text = "Level " + levelCounter.ToString();

    }

    public void LevelComplete()
    {
        levelNo++;
        levelCounter++;
        if(levelNo > 25)
        {
            levelNo = 1;
        }
        PlayerPrefs.SetInt("Level Counter", levelCounter);
        PlayerPrefs.SetInt("LevelNumber", levelNo);
        SceneManager.LoadScene(levelNo);
        FindObjectOfType<AdManager>().ShowAdmobInterstitial();
    }

    public void LevelFail()
    {
        SceneManager.LoadScene(levelNo);
        FindObjectOfType<AdManager>().ShowAdmobInterstitial();
    }
    public void PrivacyPolicy()
    {
        Application.OpenURL("https://unconditionalgames.blogspot.com/2020/01/privacy-policy-this-privacy-policy.html");
    }
}
