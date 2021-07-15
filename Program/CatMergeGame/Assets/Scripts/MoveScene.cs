using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public string sceneName;
    public string bgmName = "BGM";

    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgmName);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
