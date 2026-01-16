using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;  // Required for UI elements
using System;

public class SphereController : MonoBehaviour
{
    // Defines possible scaling modes.
    public enum ScaleMode { Uniform, AxisX, AxisY, AxisZ }

    // --- Public Fields ---
    
    [Header("Scaling Settings")]
    public ScaleMode currentMode = ScaleMode.Uniform;
    
    [Header("Input Mapping")]
    // Action linked to the mode switch button (e.g., Primary Button / A/X)
    public InputActionProperty modeSwitchInput; 

    [Header("Visual Feedback")]
    // Reference to the TextMeshProUGUI element in the scene.
    public Text modeDisplay; 

    [Header("Visual Volume")]
    // Reference to the TextMeshProUGUI element in the scene.
    public Text volumeDisplay; 

    [Header("Dual Trigger Confirmation")]
    public InputActionProperty leftTriggerAction;
    public InputActionProperty rightTriggerAction;
    // --- Private Fields ---
    
    private const int NUM_MODES = 4;
    public GameData gameData;
    private bool modeSwitchTriggered = false; 
    private bool confirmationTriggered = false;
    private Vector3 _previousScale; // Stores the scale from the previous frame to lock axes.

    void Start()
    {
        // Initialize previous scale and the display text.
        _previousScale = transform.localScale;
        UpdateDisplay();
    }

    void OnEnable()
    {
        modeSwitchInput.action.Enable();
        leftTriggerAction.action.Enable();
        rightTriggerAction.action.Enable();
    }

    void OnDisable()
    {
        modeSwitchInput.action.Disable();
        leftTriggerAction.action.Disable();
        rightTriggerAction.action.Disable();
    }

    void Update()
    {
        // Handles mode switching based on button press.
        HandleModeSwitchXR();
        HandleDualTriggerConfirmation();
        UpdateVolumeText();
    }

    void LateUpdate()
    {
        // Enforces the selected axis lock after the XR Grab Interactable applies its scale.
        EnforceAxisScaling();
    }

    void HandleDualTriggerConfirmation()
    {
        float leftVal = leftTriggerAction.action.ReadValue<float>();
        float rightVal = rightTriggerAction.action.ReadValue<float>();

        if (leftVal > 0.8f && rightVal > 0.8f)
        {
            if (!confirmationTriggered)
            {
                confirmationTriggered = true;
            }
            _previousScale = new Vector3(0.5f,0.5f,0.5f);
        }
        else if (leftVal < 0.2f && rightVal < 0.2f)
        {
            confirmationTriggered = false;
        }
    }

    /// <summary>
    /// Cycles the scaling mode when the mapped button is pressed.
    /// </summary>
    void HandleModeSwitchXR()
    {
        ObjectSpawner spawner = FindFirstObjectByType<ObjectSpawner>();

        bool canChange = !gameData.isTutorial || (spawner != null && spawner.currentTutorialStep >= 2);

        if (canChange)
        {
            // ReadValue is used for 'Button' type actions where > 0.5f usually means pressed.
            if (modeSwitchInput.action.ReadValue<float>() > 0.5f && !modeSwitchTriggered)
            {
                modeSwitchTriggered = true; 
                int nextModeIndex = ((int)currentMode + 1) % NUM_MODES;
                currentMode = (ScaleMode)nextModeIndex;
                gameData.axisChanges++; 

                UpdateDisplay(); 
            }
            else if (modeSwitchInput.action.ReadValue<float>() < 0.1f)
            {
                // Reset trigger when the button is released.
                modeSwitchTriggered = false;
            }
        }
    }

    /// <summary>
    /// Updates the UI text to show the current scaling mode.
    /// </summary>
    void UpdateDisplay()
    {
        if (modeDisplay != null)
        {
            string modeText;
            
            switch (currentMode)
            {
                case ScaleMode.Uniform:
                    modeText = "MODE: UNIFORM (X, Y, Z)";
                    break;
                case ScaleMode.AxisX:
                    modeText = "MODE: AXIS X SELECTED";
                    break;
                case ScaleMode.AxisY:
                    modeText = "MODE: AXIS Y SELECTED";
                    break;
                case ScaleMode.AxisZ:
                    modeText = "MODE: AXIS Z SELECTED";
                    break;
                default:
                    modeText = "Unknown Mode";
                    break;
            }
            modeDisplay.text = modeText;
        }
    }


    public float getMeasuringVolume(){
        Renderer rend = GetComponentInChildren<Renderer>();
        // Calculate the scaled radius
        Vector3 sizes = rend.bounds.size; // size in world units
        if (gameData.square){
            return sizes.x * sizes.y * sizes.z;
        }
        else {
            return 4/3 * Mathf.PI * sizes.x * sizes.y * sizes.z / 8;
        }
    }


    void UpdateVolumeText()
    {
        // Update the UI Text
        volumeDisplay.text = "Volume: " + getMeasuringVolume().ToString("F2");
    
    }
    

    /// <summary>
    /// Intercepts the scale applied by the XR Grab Interactable and locks non-selected axes.
    /// This runs AFTER the XR system has tried to scale the object.
    /// </summary>
    void EnforceAxisScaling()
    {
        Vector3 currentScale = transform.localScale;
        
        // Check if the XR system changed the scale at all this frame.
        if (currentScale != _previousScale)
        {
            Vector3 enforcedScale = currentScale;

            if (currentMode != ScaleMode.Uniform)
            {
                // Lock unselected axes back to their previous size.
                // This forces the scale change to apply only to the active axis.
                
                if (currentMode != ScaleMode.AxisX)
                    enforcedScale.x = _previousScale.x;
                
                if (currentMode != ScaleMode.AxisY)
                    enforcedScale.y = _previousScale.y;

                if (currentMode != ScaleMode.AxisZ)
                    enforcedScale.z = _previousScale.z;
            }

            // Apply the enforced scale.
            transform.localScale = enforcedScale;
            gameData.nGrabs++;
        }

        // Update the previous scale for comparison in the next frame.
        _previousScale = transform.localScale;
    }
}