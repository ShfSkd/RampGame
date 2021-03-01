using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int playerRank;
    public bool playerCrownActivity;
    public List<Transform> playerPositions;
    public TMP_Text numOfRampPieces;


    [Header("Panels")]
    public GameObject generalGroup;
    public GameObject LevelComplete;
    public GameObject LevelFail;

    private int pieces;
   
    private void Awake()
    {
        instance = this;

        if (numOfRampPieces != null) numOfRampPieces.text = "0/" + pieces.ToString();
    }

    //public void GeneralPanelOff()
    //{
    //    generalGroup.SetActive(false);
    //}

    public void LevelCompletePanelOn()
    {
        StartCoroutine(LevelCompleteDelay());
    }

    public void LevelFailPanelOn()
    {
        StartCoroutine(LevelFailDelay());
    }

    public void PlayerRankIncrement()
    {
        playerRank++;
    }

    public void PlayerCrownActive()
    {
        playerCrownActivity = true;
    }

    public void PlayerCrownNotActive()
    {
        playerCrownActivity = false;
    }

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(2); 
        generalGroup.SetActive(false);
        LevelComplete.SetActive(true);
    }

    IEnumerator LevelFailDelay()
    {
        yield return new WaitForSeconds(2);
        generalGroup.SetActive(false);
        LevelFail.SetActive(true);
    }
    public void AddPieces(int piecesToAdd)
	{
        pieces += piecesToAdd;

		if (numOfRampPieces != null) numOfRampPieces.text = pieces.ToString() + "/3";

	}

}
