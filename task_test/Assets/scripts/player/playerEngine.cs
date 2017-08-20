using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEngine : MonoBehaviour {
    public float speed;

	void Update () {
        float x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
        float z = speed * Time.deltaTime * Input.GetAxis("Vertical");

        transform.Translate(x, 0, z);
    }
}
