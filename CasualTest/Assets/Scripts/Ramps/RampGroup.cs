using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampGroup : MonoBehaviour
{
    public List<GameObject> childRamp;
    public int childDeactivate;
	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			childRamp.Add(transform.GetChild(i).gameObject);
		}
	}
	private void Update()
	{
		if (childDeactivate == childRamp.Count)
		{
			StartCoroutine(ActivateAll());
			childDeactivate = 0;
		}
	}
	IEnumerator ActivateAll()
	{
		yield return new WaitForSeconds(3);
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}
}
