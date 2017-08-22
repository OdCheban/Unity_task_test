using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour {
    [Header("enemySettings")]
    public float max_moveSpeed;
    public float max_rotationSpeed;
    public float mass;
    public float friction;
    [HideInInspector]
    public GameObject target;

    public enum stateMove { Idle, Seek, Flee, Arrival, Wander, CollisionAvoidance, LeaderFollowing,PathFollowing,Pursuit,CollisionAvoidanceNavMesh, Number }
    public stateMove state = stateMove.Idle;

    private Vector3 nowVelocity = Vector3.zero;
    
    [Header("Steering Behaviors: Flee")]
    public float runRadius;
    
    [Header("Steering Behaviors: Arrival")]
    public float slowRadius;
    public float stopRadius;
    
    [Header("Steering Behaviors: Wander")]
    private Vector3 targetRotation;
    public float maxAngleRandom;
    public float timerRotation;
    float timer;
    
    [Header("Steering Behaviors: C.avoidance")]
    public float offsetRay;
    public float dist;
    
    [Header("Steering Behaviors: P.following")]
    public Transform[] pathGO;
    public int nowPoint;
    
    [Header("Steering Behaviors: Pursuit")]
    public GameObject targetCatch;
    public int kPurs = -1;

    [Header("Steering Behaviors: Leader Following")]
    public float distLeader;
    public float distOtherEnemy;
    public float goAway;
    public GameObject[] enemyMas;

    [Header("UI")]
    public bool click;
    public GameObject[] btn;
    public Text stateNowUI;

    private NavMeshAgent Agent;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        timer = timerRotation;
    }
    void Update () {
		if(state != stateMove.Idle)
        {
            if (state != stateMove.CollisionAvoidanceNavMesh)
            {
                nowVelocity += GetVelocity() / mass;
                nowVelocity -= nowVelocity * friction;
                if (nowVelocity.magnitude > max_moveSpeed)
                {
                    nowVelocity = nowVelocity.normalized * max_moveSpeed;
                }
                nowVelocity.y = 0;
                transform.position += nowVelocity * Time.deltaTime;
                if (state != stateMove.Wander)
                {
                    if(nowVelocity != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nowVelocity), max_rotationSpeed * Time.deltaTime);
                }
                else
                {
                    transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * max_rotationSpeed);
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        targetRotation = new Vector3(0, Random.Range(0, maxAngleRandom), 0);
                        timer = timerRotation;
                    }
                }
            }
            else
            {
                Agent.SetDestination(target.transform.position);
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
                Debug.DrawLine(transform.position, target.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(transform.position, target.transform.position, Color.blue);
            }
        }
        if (Physics.Raycast(lRay, transform.forward, out hit, dist))
        {
            if (hit.transform != transform)
            {
                willVelocity += hit.normal * 1.75f;
                Debug.DrawLine(lRay, target.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(lRay, target.transform.position, Color.blue);
            }
        }
        if (Physics.Raycast(rRay, transform.forward, out hit, dist))
        {
            if (hit.transform != transform)
            {
                willVelocity += hit.normal * 1.75f;
                Debug.DrawLine(rRay, target.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(rRay, target.transform.position, Color.blue);
            }
        }
        //далее их можно сделать ещё умнее, добавить проверку на застревание + 2 рейкаста 
        return willVelocity;
    }
    Vector3 PathFollowing(Transform targetEnd)
    {
        Vector3 willVelocity = Seek(targetEnd);
        willVelocity *= max_moveSpeed;
        float distance = Vector3.Distance(transform.position, targetEnd.position);
        if (distance < 2)//2 - радиус точки
        {
            nowPoint++;
            if (nowPoint == pathGO.Length)
                nowPoint = 0;
        }

        return willVelocity;
    }
    Vector3 Pursuit(Transform targetEnd)
    {
        float distance = Vector3.Distance(transform.position, targetCatch.transform.position);
        float timeTo = distance/max_moveSpeed;
        //float timeTo = distance / TargetCatch.max_moveSpeed;
        //Vector3 targetPoint = TargetCatch.transform.position + TargetCatch.nowVelocity * timeTo;
        Vector3 targetPoint = targetCatch.transform.position + nowVelocity * timeTo;
        Vector3 willVelocity = (targetPoint - transform.position).normalized;
        willVelocity *= max_moveSpeed;
        return willVelocity - nowVelocity;
    }
    Vector3 FollowLeader(Transform targetEnd)
    {
        Vector3 willVelocity = Vector3.zero;
        float distance = Vector3.Distance(transform.position, targetEnd.position);
        if (distance > distLeader)
        {
            willVelocity += (Arrival(target.transform));
        }
        Vector3 force = Vector3.zero;//относительно других юнитов, а не таргета
        int k = 0;
        for (int i = 0; i < enemyMas.Length; i++)
        {
            if ((enemyMas[i] != this) && (Vector3.Distance(enemyMas[i].transform.position, transform.position) < distOtherEnemy))
            {
                force.x += enemyMas[i].transform.position.x - transform.position.x;
                force.z += enemyMas[i].transform.position.z - transform.position.z;
                k++;
            }
        }
        if (k != 0)
        {
            force.x /= k;
            force.z /= k;
            force *= -1;
        }
        willVelocity += force.normalized;
        if (goAway > distance)
            willVelocity += Flee(target.transform);
        return willVelocity;
    }
    Vector3 GetVelocity()
    {
        switch(state)
        {
            case stateMove.Seek:
                return Seek(target.transform);
            case stateMove.Flee:
                return Flee(target.transform);
            case stateMove.Arrival:
                return Arrival(target.transform);
            case stateMove.Wander:
                return Wander();
            case stateMove.CollisionAvoidance:
                return CollisionAvoidance(target.transform);
            case stateMove.PathFollowing:
                return PathFollowing(pathGO[nowPoint]);
            case stateMove.Pursuit:
                {
                    if(targetCatch.tag != "Player")
                    if (targetCatch.GetComponent<enemyAI>().kPurs == -1)
                        stateNowUI.text = "error";
                    return Pursuit(targetCatch.transform);
                }
            case stateMove.LeaderFollowing:
                return FollowLeader(target.transform);
            default:
                return Vector3.zero;
        }
      
    }

    void searchTargetPursit()
    {
        //цепляемся за игроком или ищем последнего в очереди enemyMas
        bool mainEnemy = false;
        foreach(GameObject obj in enemyMas)
        {
            if(obj.GetComponent<enemyAI>().kPurs == 0)
            {
                mainEnemy = true;
            }
        }
        if(!mainEnemy)
        {
            kPurs = 0;
            targetCatch = target;
        }
        else
        {
            int max = 0;
            GameObject targetMax = null;
            for(int i = 0;i < enemyMas.Length; i++)
            {
                int nowElem = enemyMas[i].GetComponent<enemyAI>().kPurs;
                if ((nowElem != -1) && (nowElem >= max))
                {
                    targetMax = enemyMas[i];
                    max = nowElem;
                }
            }
            targetCatch = targetMax;
            kPurs = max + 1;
        }
    }
   public void ChangeUIText(int q)
    {
        stateNowUI.text = q.ToString();
    }
    void ContinueGame()
    {
        click = !click;
        foreach (GameObject s in btn)
        {
            s.SetActive(click);
        }
        if (click)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void Action(int q)
    {
        state = (stateMove)q;
        ChangeUIText(q);
        ContinueGame();
        if(q == 9)
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
        if(q == 8)
        {
            searchTargetPursit();
        }
        else
        {
            kPurs = -1;
        }
    }
    public void Menu()
    {
        ContinueGame();
    }

}
