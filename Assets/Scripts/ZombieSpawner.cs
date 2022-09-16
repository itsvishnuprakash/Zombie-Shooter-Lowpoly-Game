using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // Static Refernce for easy access
    public static ZombieSpawner instance;

    // References
    [SerializeField] private GameObject zombiePrefab1;
    [SerializeField] private GameObject zombiePrefab2;
    public Transform[] spawningPoints;

    // variables
    [SerializeField] private int maxZombieCount;
    
    // List for storing disabled objects of zombies...for object pooling
    List<GameObject> pooledZombies=new List<GameObject>();

    int count ; // for checking count of active enemies on map
    bool isSpawning; //for activating spawning enemies throughout the game


    void Awake() 
    {
        if(instance!=this && instance!=null)
        {
            Destroy(this);
        }
        else
        {
            instance=this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PoolZombies();
        ScatterZombies();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isSpawning)
        {
            CheckZombieOnMapAndRespawn();
        }
    }

    // Instantiate a number of zombies in the begining and store them in a list after disabling them
    void PoolZombies()
    {
        for(int i=0;i<maxZombieCount;i++)
        {
            GameObject zombie;
            if(i%2==0)
            {
                zombie=Instantiate(zombiePrefab1);
            }
            else
            {
                zombie=Instantiate(zombiePrefab2);
            }
            zombie.SetActive(false);
            pooledZombies.Add(zombie);
        }
    }

    // Returns a random zombie from pooled list
    public GameObject GetRandomZombie()
    {
        while(true)
        {
            int random=Mathf.RoundToInt(Random.Range(0f,pooledZombies.Count-1));
            if(!pooledZombies[random].activeInHierarchy)
            {
                return pooledZombies[random];
            }
        }
    }

    // Scatters zombies randomly at different spawning points in the map initially
    void ScatterZombies()
    {
        for(int i=0;i<spawningPoints.Length;i++)
        {
            if(Random.value < 0.5f)
            {
                GameObject zombieObj=GetRandomZombie();
                zombieObj.transform.position=spawningPoints[i].transform.position;
                zombieObj.SetActive(true);
            }
        }
    }
    // Checks the zombiepool and if they are not active in hierarchy..zombies have been killed and can get from zombie pool again
    void CheckZombieOnMapAndRespawn()
    {
        count=0;
        for(int i=0;i<pooledZombies.Count;i++)
        {
            if(pooledZombies[i].activeInHierarchy)
            {
                count++;
            }
        }
        if(count<maxZombieCount)
        {
            isSpawning=true;
            StartCoroutine(SpawnEnemies());
        }
    } 

    // Coroutine which spawns a zombie after 2 seconds..called in update when zombies in map are killed
    IEnumerator SpawnEnemies()
    {
        while(count<maxZombieCount)
        {
            
            GameObject zombieObj=GetRandomZombie();

            int random=Mathf.RoundToInt(Random.Range(0f,spawningPoints.Length-1));
            zombieObj.transform.position=spawningPoints[random].transform.position;
            zombieObj.SetActive(true);

            count++;
            yield return new WaitForSeconds(2f);
        }
    }

}
