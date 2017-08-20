using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraPlayer : MonoBehaviour {
    private Transform target;
    private Vector3 cameraPos;
    public float speed;
	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        cameraPos = new Vector3(target.position.x,transform.position.y,target.position.z);
        transform.position = Vector3.Lerp(transform.position,cameraPos,Time.deltaTime*speed);
	}
}
