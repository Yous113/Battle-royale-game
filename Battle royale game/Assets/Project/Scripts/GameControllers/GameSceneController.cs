using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    //[Header("Gameplay")]
    //[SerializeField] private Player player;
    
    // Start is called before the first frame update
    void Start()
    {
        //player.OnPlayerDied += OnPlayerDied;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayerDied()
    {
        Invoke("ReloadScene", 3);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
