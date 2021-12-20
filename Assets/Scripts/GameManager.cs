using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text loadingUi;
    public Image loadingPanel;
    public GameObject essentials;
    public GameObject player;
    public GameObject camera;
    public PlayerMovement movement;
    public MouseLook mouseLook;
    public string playerSpawnpointName = "PlayerSpawn";
    public float fadeSpeed = 0.01f;

    private bool fadingPanel = false;
    private AsyncOperation sceneOperation;
    private bool loadingScene = false;
    
    // Update is called once per frame
    void Update()
    {
        if (loadingScene && sceneOperation.isDone)
        {
            GameObject spawnpoint = GameObject.Find(playerSpawnpointName);
            if (spawnpoint != null)
            {
                movement.controller.enabled = false;
                movement.controller.transform.position = spawnpoint.transform.position;
                movement.controller.enabled = true;

                PlayerInitializer initializer = spawnpoint.GetComponent<PlayerInitializer>();
                if (initializer != null)
                    initializer.InitPlayer(player, camera, movement, mouseLook);
            }

            loadingUi.enabled = false;
            fadingPanel = true;
            loadingScene = false;
        }

        if (fadingPanel)
        {
            Color currentColor = loadingPanel.color;
            currentColor.a -= fadeSpeed * Time.deltaTime;

            loadingPanel.color = currentColor;

            if (currentColor.a <= 0)
                fadingPanel = false;
        }
    }

    public void LoadNextLevel()
    {
        if (loadingScene)
            return;
        
        loadingUi.enabled = true;
        DontDestroyOnLoad(essentials);

        sceneOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        loadingScene = true;
    }
}
