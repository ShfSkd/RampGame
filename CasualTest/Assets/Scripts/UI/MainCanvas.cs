using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCanvas : MonoBehaviour
{
    private void Start()
    {
        int levelNo;

        levelNo = PlayerPrefs.GetInt("LevelNumber", 1);

        SceneManager.LoadScene(levelNo);
    }
}
