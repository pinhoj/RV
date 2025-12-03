using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadMainScene : MonoBehaviour
{
    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
