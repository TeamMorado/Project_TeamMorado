using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScore : MonoBehaviour
{
    public List<Text> m_listScoreText = new List<Text>();
    public List<Text> m_listBestScoreText = new List<Text>();
    public List<Button> m_listButton = new List<Button>();

    public string sceneName = "Title";

    private void Start()
    {
        for (int i = 0; i < m_listButton.Count; i++)
        {
            int index = i;
            m_listButton[i].onClick.AddListener(()=> {
                OnButtonPress(index);
            });
        }
    }

    public void SetScoreText(int score)
    {
        for (int i = 0; i < m_listScoreText.Count; i++)
        {
            m_listScoreText[i].text = score.ToString();
        }
    }

    public void SetBestScoreText(int score)
    {
        for (int i = 0; i < m_listBestScoreText.Count; i++)
        {
            m_listBestScoreText[i].text = score.ToString();
        }
    }

    public void OnButtonPress(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.instance.SetState(GameManager.eStateType.ShowADS);
                //AdmobManager.Instance.ShowFrontAd();
                //GameManager.instance.Setup();
                break;
            case 1:
                SceneManager.LoadScene(sceneName);
                break;
        }
    }
}
