using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadMainScene : MonoBehaviour
{
    public GameData gameData;
    
    public void LoadTutorialMode(string sceneName) {
        gameData.isTutorial = true; 
        gameData.inGame = false;
        gameData.axisChanges = 0;
        gameData.nGrabs = 0;
        
        gameData.square = false; 

        SceneManager.LoadScene(sceneName);
    }

    public void LoadExperimentalMode(string sceneName) {
        if (gameData.gameId > -1) {
            gameData.isTutorial = false;
            gameData.inGame = true;
            
            gameData.square = (Random.value < 0.5f);
            
            gameData.axisChanges = 0;
            gameData.nGrabs = 0;
            SceneManager.LoadScene(sceneName);
        }
    }
    
    public void loadMenu(){
        SceneManager.LoadScene("BasicScene");
        gameData.inGame = false;
        gameData.gameId = -1;
    }
}
