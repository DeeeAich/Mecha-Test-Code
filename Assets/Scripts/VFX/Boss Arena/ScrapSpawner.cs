using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapSpawner : MonoBehaviour
{
    public List<GameObject> scrapPrefabs;

    public bool isClockwise;
    public GameObject clockwiseRotator;
    public GameObject antiClockwiseRotator;

    void Start()
    {
        SpawnScrap();
    }



    void SpawnScrap()
    {
        GameObject scrap = Instantiate(scrapPrefabs[Random.Range(0, scrapPrefabs.Count)]);
        scrap.transform.position = transform.position;
        StartCoroutine(WaitToRotate(scrap));
    }

    IEnumerator WaitToRotate(GameObject scrap)
    {
        if(isClockwise)
        {
            scrap.transform.parent = clockwiseRotator.transform;
        }
        else
        {
            scrap.transform.parent = antiClockwiseRotator.transform;
        }

        yield return new WaitForSeconds(Random.Range(1, 4));
        SpawnScrap();
    }
}
