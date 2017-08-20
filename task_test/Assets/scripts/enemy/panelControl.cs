using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelControl : MonoBehaviour {
    public GameObject prefab;
    public float offsetSpawnEnemy;
    public List<GameObject> massObj;


    void SpawnEnemy()
    {
        float x = Random.Range(transform.position.x + offsetSpawnEnemy, transform.position.x - offsetSpawnEnemy);
        float z = Random.Range(transform.position.z + offsetSpawnEnemy, transform.position.z - offsetSpawnEnemy);
        GameObject unit = (GameObject)Instantiate(prefab, new Vector3(x,1, z), Quaternion.identity);
        massObj.Add(unit);
    }
    void DestroyEnemy()
    {
        foreach (GameObject obj in massObj)
            Destroy(obj);
        massObj.Clear();
    }

	void Update () {
		if(Input.GetKeyDown(KeyCode.Equals))
        {
            SpawnEnemy();
        }
        if(Input.GetKeyDown("-"))
        {
            DestroyEnemy();
        }
	}
}
