using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;  // Required for UI elements
using TMPro;

public class TutorialController : MonoBehaviour
{
    // Defines possible scaling modes.
    public enum ScaleMode { Uniform, AxisX, AxisY, AxisZ }

    [Header("Global Data")]
    public GameData gameData;

    // --- Public Fields ---
    
    [Header("Scaling Settings")]
    public ScaleMode currentMode = ScaleMode.Uniform;
    
    [Header("Input Mapping")]
    // Action linked to the mode switch button (e.g., Primary Button / A/X)
    public InputActionProperty modeSwitchInput; 

    [Header("Visual Feedback")]
    // Reference to the TextMeshProUGUI element in the scene.
    public TextMeshProUGUI  modeDisplay; 

    [Header("Visual Volume")]
    // Reference to the TextMeshProUGUI element in the scene.
    public TextMeshProUGUI  volumeDisplay; 

    [Header("Tutorial Image")]
    public UnityEngine.UI.Image tutorialImage;

    // --- Private Fields ---
    
    private const int NUM_MODES = 4;
    private bool modeSwitchTriggered = false; 
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
    }

    void OnDisable()
    {
        modeSwitchInput.action.Disable();
    }

    void Update()
    {
        // Handles mode switching based on button press.
        HandleModeSwitchXR();
        UpdateVolumeText();
    }

    void LateUpdate()
    {
        // Enforces the selected axis lock after the XR Grab Interactable applies its scale.
        EnforceAxisScaling();
    }

    /// <summary>
    /// Cycles the scaling mode when the mapped button is pressed.
    /// </summary>
    void HandleModeSwitchXR()
    {
        ObjectSpawner spawner = FindFirstObjectByType<ObjectSpawner>();

        if (spawner != null && spawner.currentTutorialStep >= 2)
        {
            if (modeSwitchInput.action.ReadValue<float>() > 0.5f && !modeSwitchTriggered)
            {
                modeSwitchTriggered = true; 
                int nextModeIndex = ((int)currentMode + 1) % NUM_MODES;
                currentMode = (ScaleMode)nextModeIndex;
                if (gameData != null) gameData.axisChanges++;
                UpdateDisplay(); 
            }
            else if (modeSwitchInput.action.ReadValue<float>() < 0.1f)
            {
                modeSwitchTriggered = false;
            }
        }
    }
    
    public void setPreviousScale(Vector3 scale){
        _previousScale = scale;
    }

    public void setText(string text){
        if (modeDisplay != null) modeDisplay.text = text;
    }

    public void setButtonImage(Sprite icon)
    {
        if (tutorialImage != null)
        {
            tutorialImage.sprite = icon;
            tutorialImage.gameObject.SetActive(icon != null);
        }
    }

    /// <summary>
    /// Updates the UI text to show the current scaling mode.
    /// </summary>
    void UpdateDisplay()
    {
        if (modeDisplay != null && tutorialImage == null)
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
        if (rend == null) return 0f;
        // Calculate the scaled radius
        Vector3 sizes = rend.bounds.size; // size in world units
        return sizes.x * sizes.y * sizes.z;
    }

    public float getAccuracy(){
        float vol = getMeasuringVolume();
        float accuracy = (1 - Mathf.Abs(vol - 3) / 3);
        accuracy = Mathf.Clamp01(accuracy);
        return accuracy * 100;
    }


    void UpdateVolumeText()
    {
        // Update the UI Text
        if (volumeDisplay != null)
        {
            volumeDisplay.text = "Volume: " + getMeasuringVolume().ToString("F2") + "\nAccuracy:" + getAccuracy().ToString("F1") + "%";
        }
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
        }

        // Update the previous scale for comparison in the next frame.
        _previousScale = transform.localScale;
    }
}