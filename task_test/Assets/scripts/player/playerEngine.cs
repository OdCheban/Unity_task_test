using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEngine : MonoBehaviour {
    public float speed;

	void Update () {
        float x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
        float z = speed * Time.deltaTime * Input.GetAxis("Vertical");

        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(transform.position,transform.forward,out hit,0.5f))
        {
            if(hit.transform != transform)
            {
                if(z > 0)
                {
                    z = 0;
                }
            }
        }
        if (Physics.Raycast(transform.position, transform.forward*-1, out hit, 0.5f))
        {
            if (hit.transform != transform)
            {
                if (z < 0)
                {
                    z = 0;
                }
            }
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, 0.5f))
        {
            if (hit.transform != transform)
            {
                if (x > 0)
                {
                    x = 0;
                }
            }
        }
        if (Physics.Raycast(transform.position, transform.right * -1, out hit, 0.5f))
        {
            if (hit.transform != transform)
            {
                if (x < 0)
                {
                    x = 0;
                }
            }
        }


        transform.Translate(x, 0, z);
    }
}
