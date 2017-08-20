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

    //flee
    public float runRadius;

    //arrival
    public float slowRadius;
    public float stopRadius;

    //wander
    Vector3 targetRotation;
    public float maxAngleRandom;
    public float timerRotation;
    float timer;

    //c.avoidance
    public float offsetRay;
    public float dist;

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
            if(state != stateMove.Wander)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nowVelocity), max_rotationSpeed * Time.deltaTime);
            else
            {
                transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * max_rotationSpeed);
                timer -= Time.deltaTime;
                if(timer <=0)
                {
                    targetRotation = new Vector3(0, Random.Range(0, maxAngleRandom), 0);
                    timer = timerRotation;
                }
            }
        }
	}

    Vector3 Seek(Transform targetEnd)
    {
        Vector3 willVelocity = (targetEnd.position - transform.position).normalized;
        willVelocity *= max_moveSpeed;
        return willVelocity - nowVelocity;
    }
    Vector3 Flee(Transform targetEnd)
    {
        float distance = Vector3.Distance(transform.position,targetEnd.position);
        if(distance < runRadius)
            return -((targetEnd.position - transform.position).normalized * max_moveSpeed - nowVelocity);
        else
            return Vector3.zero;
    }
    Vector3 Arrival(Transform targetEnd)
    {
        float distance = Vector3.Distance(transform.position, targetEnd.position);
        Vector3 willVelocity = (targetEnd.position - transform.position).normalized;
        if (distance < stopRadius)
            willVelocity = Vector3.zero;
        else if (distance < slowRadius)
            willVelocity *= max_moveSpeed * ((distance - stopRadius) / (slowRadius - stopRadius));
        else
            willVelocity *= max_moveSpeed;
        return willVelocity - nowVelocity;
    }
    Vector3 Wander()
    {
        return transform.TransformDirection(Vector3.forward);
    }
    Vector3 CollisionAvoidance(Transform targetEnd)
    {
        Vector3 willVelocity = (targetEnd.position - transform.position).normalized;
        willVelocity *= max_moveSpeed;
        Vector3 lRay = new Vector3(transform.position.x - offsetRay,transform.position.y,transform.position.z);
        Vector3 rRay = new Vector3(transform.position.x + offsetRay, transform.position.y, transform.position.z);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(transform.position,transform.forward,out hit,dist))
        {
            if(hit.transform !=transform)
            {
                willVelocity += hit.normal * 1.75f;
                Debug.DrawLine(transform.position, target.position, Color.red);
            }
            else
            {
                Debug.DrawLine(transform.position, target.position, Color.blue);
            }
        }
        if (Physics.Raycast(lRay, transform.forward, out hit, dist))
        {
            if (hit.transform != transform)
            {
                willVelocity += hit.normal * 1.75f;
                Debug.DrawLine(lRay, target.position, Color.red);
            }
            else
            {
                Debug.DrawLine(lRay, target.position, Color.blue);
            }
        }
        if (Physics.Raycast(rRay, transform.forward, out hit, dist))
        {
            if (hit.transform != transform)
            {
                willVelocity += hit.normal * 1.75f;
                Debug.DrawLine(rRay, target.position, Color.red);
            }
            else
            {
                Debug.DrawLine(rRay, target.position, Color.blue);
            }
        }
        //далее их можно сделать ещё умнее, добавить проверку на застревание + 2 рейкаста 
        return willVelocity;
    }
    Vector3 GetVelocity()
    {
        if(state == stateMove.Seek)
        {
            return Seek(target);
        }
        if(state == stateMove.Flee)
        {
            return Flee(target);
        }
        if(state == stateMove.Arrival)
        {
            return Arrival(target);
        }
        if(state == stateMove.Wander)
        {
            return Wander();
        }
        if(state == stateMove.CollisionAvoidance)
        {
            return CollisionAvoidance(target);
        }
        return Vector3.zero;
    }

}
