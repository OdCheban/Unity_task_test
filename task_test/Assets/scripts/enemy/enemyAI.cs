using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour {
    public float max_moveSpeed;
    public float max_rotationSpeed;
    public float mass;
    public float friction;
    [HideInInspector]
    public Transform target;

    public enum stateMove { Idle, Seek, Flee, Arrival, Wander, CollisionAvoidance, LeaderFollowing, Number }
    public stateMove state = stateMove.Idle;

    private Vector3 nowVelocity = Vector3.zero;

	void Update () {
		if(state != stateMove.Idle)
        {
            nowVelocity += GetVelocity()/mass;
            nowVelocity -= nowVelocity * friction;
            if(nowVelocity.magnitude > max_moveSpeed)
            {
                nowVelocity = nowVelocity.normalized * max_moveSpeed;
            }
            nowVelocity.y = 0;

            transform.position += nowVelocity * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nowVelocity), max_rotationSpeed * Time.deltaTime);
        }
	}

    Vector3 Seek(Transform targetEnd)
    {
        return ((targetEnd.position - transform.position).normalized * max_moveSpeed) - nowVelocity;
    }

    Vector3 GetVelocity()
    {
        if(state == stateMove.Seek)
        {
            return Seek(target);
        }
        return Vector3.zero;
    }

}
