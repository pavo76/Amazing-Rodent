using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Variable holding player object to determine wether player is dead
    public PlayerController player;

    private Scene currentScene;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }


    // Update is called once per frame
    void Update () {
		if(player.IsDead)
        {
            ResetLevel();
        }
    }

    // Resets current level
    private void ResetLevel()
    {
        SceneManager.LoadScene(currentScene.name);
    }
}
