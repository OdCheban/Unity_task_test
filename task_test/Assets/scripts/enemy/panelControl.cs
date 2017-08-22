using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class panelControl : MonoBehaviour {
    [Header("Settings")]
    public GameObject prefab;
    public float offsetSpawnEnemy;
    public List<GameObject> massObj;
    public Text stateNowUI;
    public enum stateMove { Idle, Seek, Flee, Arrival, Wander, CollisionAvoidance, LeaderFollowing, PathFollowing, Pursuit, CollisionAvoidanceNavMesh, Number }
    public stateMove state = stateMove.Idle;

    [Header("Steering Behaviors: Flee")]
    public GameObject fleeRender;
    [Header("Steering Behaviors: Path Following")]
    public Transform[] pathGO;

    void SpawnEnemy()
    {
        float x = Random.Range(transform.position.x + offsetSpawnEnemy, transform.position.x - offsetSpawnEnemy);
        float z = Random.Range(transform.position.z + offsetSpawnEnemy, transform.position.z - offsetSpawnEnemy);
        GameObject unit = (GameObject)Instantiate(prefab, new Vector3(x,1, z), Quaternion.identity);
        unit.GetComponent<enemyAI>().pathGO = pathGO;
        massObj.Add(unit);
        getData(massObj.Count - 1);
    }
    void DestroyEnemy()
    {
        foreach (GameObject obj in massObj)
            Destroy(obj);
        massObj.Clear();
    }

    void getData(int q)
    {
        massObj[q].GetComponent<enemyAI>().state = (enemyAI.stateMove)state;
        massObj[q].GetComponent<enemyAI>().target = GameObject.FindGameObjectWithTag("Player");
        massObj[q].GetComponent<enemyAI>().ChangeUIText((int)state);
        if(state == stateMove.Pursuit)
        {
            if(q!=0)
            {
                massObj[q].GetComponent<enemyAI>().targetCatch = massObj[q - 1];
            }
            else
            {
                massObj[q].GetComponent<enemyAI>().targetCatch = gameObject;
            }
            massObj[q].GetComponent<enemyAI>().kPurs = q;
        }
        else
        {
            massObj[q].GetComponent<enemyAI>().kPurs = -1;
        }
        if(state == stateMove.CollisionAvoidanceNavMesh)
        {
            massObj[q].GetComponent<NavMeshAgent>().enabled = true;
        }
        else
        {
            massObj[q].GetComponent<NavMeshAgent>().enabled = false;
        }
        for (int j = 0; j < massObj.Count; j++)
        {
            massObj[j].GetComponent<enemyAI>().enemyMas = GameObject.FindGameObjectsWithTag("Enemy");
        }
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

                if (state == stateMove.Flee)
                    fleeRender.SetActive(true);
                else
                    fleeRender.SetActive(false);

                for (int j = 0; j < massObj.Count; j++)
                {
                    getData(j);
                }

            }
            

        }


	}
}
