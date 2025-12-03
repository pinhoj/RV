using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<Vector3> occupiedPositions = new List<Vector3>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects(){
        for (int i = 0; i < 3; i++){
            gameObjects.Add(Instantiate(cubePrefab, GetNewPos(i), Quaternion.identity));
        }
    }


    Vector3 GetNewPos(int i){
        switch (i){
            case 0:
                return new Vector3(Random.Range(-4f,4f), 1f, Random.Range(-4f,4f));
            case 1:
                return new Vector3(Random.Range(-4f,4f), 9f, Random.Range(-4f,4f));
            case 2:
                return new Vector3(-4f, Random.Range(1f,9f), Random.Range(-4f,4f));
            default:
                return new Vector3(0f,0f,0f);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
