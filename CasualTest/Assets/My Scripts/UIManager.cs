using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Text rampsCount;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}
	public void UpdatePicesText(int pices)
	{
		rampsCount.text = "Pieces " + pices.ToString();
	}
}
