using UnityEngine;
using TMPro;

public class GameIdController : MonoBehaviour
{
    public GameData gamedata;
    public TextMeshProUGUI text;

    private string inputString = ""; 
    private const int MAX_DIGITS = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gamedata.gameId != -1) inputString = gamedata.gameId.ToString();
        updateDisplay();
    }

    public void AddDigit(string digit)
    {
        if((gamedata.gameId == -1 || gamedata.gameId==0) && digit=="0")
        {

        }
        else if (inputString.Length < MAX_DIGITS)
        {
            inputString += digit;
            if (int.TryParse(inputString, out int result))
            {
                gamedata.gameId = result;
            }
            updateDisplay();
        }
    }

    public void ClearID()
    {
        if (!string.IsNullOrEmpty(inputString))
        {
            inputString = inputString.Substring(0, inputString.Length - 1);
            if (inputString.Length == 0)
            {
                gamedata.gameId = -1;
            }
            else if (int.TryParse(inputString, out int result))
            {
                gamedata.gameId = result;
            }
        }
        else
        {
            gamedata.gameId = -1;
        }

        updateDisplay();
    }

    void updateDisplay(){
        if (gamedata.gameId == -1){
            text.text = "None";
        }
        else {
            text.text = gamedata.gameId.ToString();
        }
    }

}
