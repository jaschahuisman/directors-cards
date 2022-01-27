using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class MusicManager : NetworkBehaviour
{
    private AudioSource source;
    [Scene] [SerializeField] private string gameplayScene = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.Play();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene current, Scene next)
    {
        if (gameplayScene.Contains(next.name))
        {
            source.Stop();
        }
        else
        {
            if(!source.isPlaying)
                source.Play();
        }
    }
}
