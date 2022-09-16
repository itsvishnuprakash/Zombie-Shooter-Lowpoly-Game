using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    // Scriptable Object
    [SerializeField] private Zombie zombie;

    // References
    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;

    // Variables
    int points;
    int health;
    int damage;
    float range;
    float speed;
    bool isAcidAttacking;
    int random;
    Vector3 targetPos;
    Transform player;
    bool playerDetected;
    bool startPatrolling=false;

    enum AnimationState
    {
        idle,
        patrol,
        attack
    }
    AnimationState state;

    bool isDead=false;

    
    // Start is called before the first frame update
    void Start()
    {
        anim=GetComponent<Animator>();
        agent=GetComponent<UnityEngine.AI.NavMeshAgent>();
        targetPos=transform.position;

        player=PlayerMovement.instance.GetComponent<Transform>();


        // Assigning values of specs from scriptable object
        GetSpecs();

        //Waiting 2 seconds in idle state when spawned
        StartCoroutine(IdleTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;
        }
        if(!startPatrolling)
        {
            return;
        }
        Patrol();
        CheckForPlayer();
        Attack();
        CheckHealth();

    }

    // Patrol between random spawning points
    void Patrol()
    {
        if(Vector3.Distance(transform.position,targetPos)>1f)
        {
            agent.SetDestination(targetPos);
        }
        else if(!playerDetected)
        {
            random=Mathf.RoundToInt(Random.Range(0f,ZombieSpawner.instance.spawningPoints.Length-1));
            targetPos=ZombieSpawner.instance.spawningPoints[random].position;
        }

    }
    
    //Setting the target position as player when player entered the range
    void CheckForPlayer()
    {
        if(Vector3.Distance(transform.position,player.position)<range && !playerDetected)
        {
            targetPos=player.position;
            playerDetected=true;
        }
        if(Vector3.Distance(transform.position,player.position)>range && playerDetected)
        {
            playerDetected=false;
            random=Mathf.RoundToInt(Random.Range(0f,ZombieSpawner.instance.spawningPoints.Length-1));
            targetPos=ZombieSpawner.instance.spawningPoints[random].position;
        }
    } 

    //Attacks when player is near
    void Attack()
    {
        if(Vector3.Distance(transform.position,player.position)<1.5f)
        {
            transform.LookAt(PlayerMovement.instance.transform.position);
            if(state==AnimationState.patrol)
            {
                state=AnimationState.attack;
                StartCoroutine(AttackPlayer());
                Debug.Log("attack");
               
            }
            

            
        }
        if(Vector3.Distance(transform.position,player.position)>1.5f && playerDetected)
        {
            if(state==AnimationState.attack)
            {
                state=AnimationState.patrol;
            }
        }
    } 

    IEnumerator AttackPlayer()
    {
        while(state==AnimationState.attack)
        {
            PlayerMovement.instance.GetDamage(damage);
            yield return new WaitForSeconds(6f);
        }
    }  

    // Assigning specifications from scriptable object
    void GetSpecs()
    {
        points=zombie.points;
        health=zombie.health;
        damage=zombie.damage;
        range=zombie.range;
        speed=zombie.speed;
        isAcidAttacking=zombie.acidAttacking;
    }

    // Incase of health <0 ,disable the enemy
    void CheckHealth()
    {
        if(health<=0)
        {
            agent.SetDestination(transform.position);
            isDead=true;
            anim.SetTrigger("die");
            StartCoroutine(Death());
        }
    }

    //Coroutine for waiting 2 seconds to change from idle state
    IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(2f);

        state=AnimationState.patrol;
        anim.SetInteger("state",(int) state);
        startPatrolling=true;
    }
    //This function get called when a shooting ray hits the enemy and called from the player script
    public void GetDamage(int damage)
    {
        health-=damage;
    }
    //called when players health less thn zero and called from the update function
    IEnumerator Death()
    {
        yield return new WaitForSeconds(1.4f);
        GameObject obj=CollectablePool.instance.GetRandomCollectable();
        obj.transform.position=transform.position;
        obj.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
