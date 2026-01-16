using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public bool inGame = false;
    public int gameId = -1;
    public bool square = true;
    public int axisChanges = 0;
    public int nGrabs = 0;
    public bool isTutorial = false;
}
