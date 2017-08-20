using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panelControl : MonoBehaviour {
    public GameObject prefab;
    public float offsetSpawnEnemy;
    public List<GameObject> massObj;
    public Text stateNowUI;
    public enum stateMove { Idle,Seek,Flee,Arrival,Wander,CollisionAvoidance,LeaderFollowing,Number }
    public stateMove state = stateMove.Idle;


    void SpawnEnemy()
    {
        float x = Random.Range(transform.position.x + offsetSpawnEnemy, transform.position.x - offsetSpawnEnemy);
        float z = Random.Range(transform.position.z + offsetSpawnEnemy, transform.position.z - offsetSpawnEnemy);
        GameObject unit = (GameObject)Instantiate(prefab, new Vector3(x,1, z), Quaternion.identity);
        unit.GetComponent<enemyAI>().target = gameObject.transform;
        massObj.Add(unit);
    }
    void DestroyEnemy()
    {
        foreach (GameObject obj in massObj)
            Destroy(obj);
        massObj.Clear();
    }

    void getData(int q)
    {

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
        for(int i = 0; i < (int)stateMove.Number;i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                state = (stateMove)i;
                stateNowUI.text = stateMove.GetName(typeof(stateMove), i);
            }

            for(int j = 0; j < massObj.Count;j++)
            {
                massObj[j].GetComponent<enemyAI>().state = (enemyAI.stateMove)state;
            }

        }


	}
}
