using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankGroup : MonoBehaviour
{
    public List<GameObject> childPlanks;
    public int childDeActiveCount;
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            childPlanks.Add(this.transform.GetChild(i).gameObject);
        }
    } 

    void Update()
    {
        if(childDeActiveCount == childPlanks.Count)
        {
            StartCoroutine(ActivateAll());
            childDeActiveCount = 0;
        }
    }

    IEnumerator ActivateAll()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
