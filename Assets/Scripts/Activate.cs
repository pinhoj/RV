using UnityEngine;
using System;
using System.IO;

public class Activate : MonoBehaviour
{   
    public GameData gameData;
    public GameObject measuringTool;

    public Material activatedMaterial;
    public Material deactivatedMaterial;

    private float volume = 0f;

    private DateTime initialTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void activate(Vector3 position){

        Renderer rend = GetComponent<Renderer>();

        rend.material = activatedMaterial;
        Vector3 sizes = rend.bounds.size; // size in world units

        Collider col = GetComponent<Collider>();

        if (col is BoxCollider box)
        {
            volume = sizes.x * sizes.y * sizes.z;
        }
        else if (col is SphereCollider sphere)
        {
            volume = 4/3 * Mathf.PI * sizes.x * sizes.y * sizes.z/8;
        }
        

        Rigidbody rb = measuringTool.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        // Renderer mtRend = measuringTool.GetComponentInChildren<Renderer>();
        measuringTool.transform.position = position;
        measuringTool.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        rb.isKinematic = false;
        
        initialTime = DateTime.Now;
    }

    public void End(Vector3 position)
    {   
        if (gameData.inGame == false && gameData.isTutorial == true){
            gameData.axisChanges = 0;
            gameData.nGrabs = 0;
            return;
        }
        Renderer rend = GetComponent<Renderer>();
        rend.material = deactivatedMaterial;

        SphereController controller = measuringTool.GetComponent<SphereController>();
        float measuredVolume = controller.getMeasuringVolume();

        TimeSpan finalTime = DateTime.Now - initialTime;

        float accuracy = (1 - Mathf.Abs(measuredVolume - volume) / volume);
        accuracy = Mathf.Clamp01(accuracy);
        accuracy = accuracy * 100;

        string path = Path.Combine(Application.persistentDataPath, "log.csv");
        using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(gameData.gameId + "," + gameData.square + ","
                    + DateTime.Now + "," + finalTime + "," 
                    + position.x + "," + position.y + "," + position.z + ","
                    + volume + "," + measuredVolume + "," + accuracy + ","
                    + gameData.axisChanges + "," + gameData.nGrabs);            
            }
        gameData.axisChanges=0;
        gameData.nGrabs=0;
    }
}
