using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePool : MonoBehaviour
{
    public static CollectablePool instance;
    [SerializeField] private GameObject[] collectables; 
    [SerializeField] private int maxCollectables=2;
    GameObject obj;
    List<GameObject> pooledCollectables=new List<GameObject>();

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
        for(int i=0;i<maxCollectables;i++)
        {
            for(int j=0;j<collectables.Length;j++)
            {
                GameObject collectable=Instantiate(collectables[j]);
                collectable.SetActive(false);
                pooledCollectables.Add(collectable);
            }
        }
        
    }

    // Returns a random collectable from pooled list
    public GameObject GetRandomCollectable()
    {
        while(true)
        {
            int random=Mathf.RoundToInt(Random.Range(0f,pooledCollectables.Count-1));
            if(!pooledCollectables[random].activeInHierarchy)
            {
                obj=pooledCollectables[random];
                StartCoroutine(DisableAfterSometime());
                return obj;
            }
        }
    }

    IEnumerator DisableAfterSometime()
    {
        yield return new WaitForSeconds(15f);
        obj.SetActive(false);
    }
}
