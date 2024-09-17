using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;

    private void Start()
    {
        OnLevelWasLoaded(0);
    }

    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", 0.2f);
        }
    }

    void PlayMusic()
    {
        AudioClip clipPlay = null;

        if (sceneName.Equals("Menu"))
            clipPlay = menuTheme;
        else if(sceneName.Equals("Game"))
            clipPlay = mainTheme;

        if(clipPlay != null)
        {
            AudioManager.instance.PlayMusic(clipPlay, 2);
            Invoke("PlayMusic", clipPlay.length);
        }
    }
}