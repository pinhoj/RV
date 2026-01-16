using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using System.Collections.Generic;
using System.Collections; // must include this

public class Tutorial : MonoBehaviour
{
    public GameData gameData;
    public GameObject objectPrefab;
    public GameObject SphereInteractable;
    private Vector3 position = new Vector3(0f,-1f,-6f);

    [Header("Input Mapping")]
    // Action linked to the confirm button (e.g., back button)
    public InputActionProperty leftTriggerConfirm;
    public InputActionProperty rightTriggerConfirm;
    private bool isConfirming = false;

    public void OnTutorialButtonClick()
    {
        gameData.isTutorial = true;
        gameData.inGame = false;
        
        ActivateObject(); 
    }

    public void OnStartButtonClick()
    {
        gameData.isTutorial = false;
        gameData.inGame = true;
        
        ActivateObject();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }


    private void ActivateObject()
    {
        // Activate the object
        Activate act = objectPrefab.GetComponent<Activate>();
        act.activate(position);
    
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
        float leftVal = leftTriggerConfirm.action.ReadValue<float>();
        float rightVal = rightTriggerConfirm.action.ReadValue<float>();

        if (leftVal > 0.8f && rightVal > 0.8f)
        {
            if (!isConfirming)
            {
                isConfirming = true;
                ExecuteConfirmation();
            }
            
            SphereInteractable.GetComponent<TutorialController>().setPreviousScale(new Vector3(0.5f,0.5f,0.5f));
            SphereInteractable.GetComponent<TutorialController>().setText("ITS WORKING");

        }
        else if (leftVal < 0.2f && rightVal < 0.2f)
        {
            isConfirming = false;
        }
    }

    private void ExecuteConfirmation()
    {
        Activate act = objectPrefab.GetComponent<Activate>();
        act.End(position);             
        
        ActivateObject();

    }



}
