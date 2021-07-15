using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGame : MonoBehaviour
{
    public GameObject exitUIObject;
    public Text textValue;

    private void Start()
    {
        if(textValue != null)
        {
            bool bKorean =  Application.systemLanguage == SystemLanguage.Korean;
            textValue.text = bKorean ? "정말 나가시겠습니까?" : "Are you sure?";
        }
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape) && exitUIObject.activeSelf == false)
        {
            exitUIObject.SetActive(true);
        }
    }

    public void Func_ExitGame()
    {
        Application.Quit();
    }

    public void CloseExitUI()
    {
        exitUIObject.SetActive(false);
    }
}
