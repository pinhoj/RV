using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections.Generic;
using System.Collections; // must include this

public class ObjectSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    private GameObject dummyPrefab;
    private GameObject objectPrefab;
    public GameObject tutorialCanvas;

    public LoadMainScene sceneLoader;
    public GameData gameData;
    
    [Header("Input Mapping")]
    // Action linked to the confirm button (e.g., back button)
    public InputActionProperty leftTriggerConfirm;
    public InputActionProperty rightTriggerConfirm;
    private bool isConfirming = false;
    private List<GameObject> gameObjects = new List<GameObject>();
    private List<Vector3> occupiedPositions = new List<Vector3>(); //TODO CHECK NON 
    private int active = 0;
    private float minSize = 0.2f;
    private float maxSize = 1.0f;

    [Header("Tutorial Icons")]
    public Sprite iconGrip;
    public Sprite iconButtonA;
    public Sprite iconTriggers;

    [Header("Tutorial Control")]
    public int currentTutorialStep = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        if (gameData.square){
            objectPrefab = cubePrefab;
            dummyPrefab = spherePrefab;
        }
        else {
            objectPrefab = spherePrefab;
            dummyPrefab = cubePrefab;
        }

        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(gameData.isTutorial);
        }

        if (gameData.isTutorial)
        {
            SpawnTutorialStage();
        }
        else
        {
            SpawnObjects();
            StartInitialActivation();
        }
    }

    

    void SpawnObjects(){
        for (int i = 0; i < 3; i++){
            (Vector3 position, Vector3 size) = GetSize(GetNewPos(i));

            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity);
            obj.transform.localScale = size;
            
            GameObject measure = GameObject.Find("Sphere Interactable");
            
            Activate mt = obj.GetComponent<Activate>();
            mt.measuringTool = measure;

            occupiedPositions.Add(position);
            gameObjects.Add(obj);
        }

        for (int i = 0; i < 3; i++){
            (Vector3 position, Vector3 size) = GetSize(GetNewPos(i + 3));
            GameObject obj = Instantiate(dummyPrefab, position, Quaternion.identity);
            obj.transform.localScale = size;
        }
    }

    void SpawnTutorialStage()
    {
        Vector3 tutorialPos = new Vector3(0f, -0.75f, 1.5f); 
        Vector3 tutorialScale = new Vector3(0.6f, 0.5f, 0.4f);

        GameObject obj = Instantiate(objectPrefab, tutorialPos, Quaternion.identity);
        obj.transform.localScale = tutorialScale;
        
        GameObject measure = GameObject.Find("Sphere Interactable");
        
        Activate mt = obj.GetComponent<Activate>();
        mt.measuringTool = measure;

        occupiedPositions.Add(tutorialPos);
        gameObjects.Add(obj);

        StartInitialActivation();
    }

    (Vector3 position, Vector3 scale) GetSize(Vector3 position){
        
        Vector3 scale = new Vector3(
            Random.Range(minSize, maxSize),
            Random.Range(minSize, maxSize),
            Random.Range(minSize, maxSize)
        );

        position.x = isSafe(position.x, scale.x);
        position.y = isSafeY(position.y, scale.y);
        position.z = isSafe(position.z, scale.z);

        return (position, scale);
    }

    float isSafe(float val, float scale){
        if ((val + scale/2) > 4){
            return 4 - scale/2;
        }
        if ((val - scale/2) < -4){
            return -4 + scale/2;
        }
        return val;
    }

    float isSafeY(float val, float scale){
        if ((val + scale/2) > 2){
            return 2 - scale/2;
        }
        if ((val - scale/2) < -2){
            return -2 + scale/2;
        }
        return val;
    }

    Vector3 GetNewPos(int i){
        switch (i){
            case 0:
                return new Vector3(Random.Range(-3f,3f), -1.5f, Random.Range(-3f,3f));
            case 1:
                return new Vector3(Random.Range(-3f,3f), 1.5f, Random.Range(-3f,3f));
            case 2:
                return new Vector3(-3.5f, Random.Range(-1f,1f), Random.Range(-3f,3f));
            case 3:
                return new Vector3(3.5f, Random.Range(-1f,1f), Random.Range(-3f,3f));
            case 4:
                return new Vector3(Random.Range(-3f,3f),Random.Range(-1f,1f), -3.5f);
            case 5:
                return new Vector3(Random.Range(-3f,3f),Random.Range(-1f,1f), 3.5f);
            default:
                return new Vector3(0f,0f,0f);

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameData.isTutorial){
            UpdateTutorialInstructions();
            HandleDualTriggerConfirmation();
        }
        else
            HandleDualTriggerConfirmation();
    }

    void UpdateTutorialInstructions()
    {
        GameObject measure = GameObject.Find("Sphere Interactable");
        var controller = measure.GetComponent<TutorialController>();

        GameObject currentObject = getActiveObject();
        var grabComponent = currentObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (currentTutorialStep == 1) 
        {
            controller.setText("STEP 1: GRAB the object using Side Grips");
            controller.setButtonImage(iconGrip);

            if (grabComponent != null) grabComponent.enabled = true; 
        }
        else if (currentTutorialStep == 2) 
        {
            controller.setText("STEP 2: PRESS BUTTON A to change selected Axis");
            controller.setButtonImage(iconButtonA);
            if (grabComponent != null) grabComponent.enabled = true;
        }
        else 
        {
            controller.setText("STEP 3: MATCH SIZE & PULL BOTH TRIGGERS to confirm");
            controller.setButtonImage(iconTriggers);
        }
    }

    public void NextStep()
    {
        Debug.Log("Entrou no Next Step");
        if (currentTutorialStep == 1 && gameData.nGrabs == 0) 
        {
            Debug.Log("Bloqueado: Precisas de agarrar o objeto primeiro!");
            return;
        }

        if (currentTutorialStep == 2 && gameData.axisChanges == 0) 
        {
            return;
        }

        if (currentTutorialStep < 3)
        {
            currentTutorialStep++;
            UpdateTutorialInstructions();
        }
    }

    public List<GameObject> getObjects(){
        return gameObjects;
    }

    public GameObject getActiveObject(){
        return gameObjects[active];
    }

    public void StartInitialActivation()
    {
        // Start the sequence with the first object.
        active = 0;
        ActivateObject(active);
    }

    private void ActivateObject(int index)
    {
        if (index >= 0 && index < gameObjects.Count)
        {
            // Activate the object
            Activate act = gameObjects[index].GetComponent<Activate>();
            act.activate(occupiedPositions[index]);
        }
    }

    void OnEnable()
    {
        leftTriggerConfirm.action.Enable();
        rightTriggerConfirm.action.Enable();
    }

    void OnDisable()
    {
        leftTriggerConfirm.action.Disable();
        rightTriggerConfirm.action.Disable();
    }


    private void HandleDualTriggerConfirmation()
    {
        if (gameData.isTutorial && currentTutorialStep < 3) return;

        float leftVal = leftTriggerConfirm.action.ReadValue<float>();
        float rightVal = rightTriggerConfirm.action.ReadValue<float>();

        if (leftVal > 0.8f && rightVal > 0.8f)
        {
            if (!isConfirming)
            {
                isConfirming = true;
                ExecuteConfirmation();
            }
        }
        else if (leftVal < 0.2f && rightVal < 0.2f)
        {
            isConfirming = false;
        }
    }

    private void ExecuteConfirmation()
    {
        if (active >= 0 && active < gameObjects.Count && gameData.nGrabs > 1)
        {           
            Activate act = gameObjects[active].GetComponent<Activate>();
            act.End(occupiedPositions[active]);

            active++;
            if (active < gameObjects.Count)
            {
                ActivateObject(active);
            }
            else 
            {
                sceneLoader.loadMenu();
            }
        }
    }

}
