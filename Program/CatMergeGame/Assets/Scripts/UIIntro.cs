using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIIntro : MonoBehaviour
{
    private const string szTutorialKey = "TutorialKey";
    private const string szIntoKey = "IntroKey";
    private const string szDateKey = "DateKey";
    private const string szSpriteDateKey = "SpriteDateKey";

    public Button[] m_listButton = new Button[1];

    public GameObject tutorialObject;
    public GameObject[] m_listTutorialLangauge = new GameObject[2];
    public GameObject introObject;
    public GameObject[] m_listIntroLangauge = new GameObject[2];

    private bool bTutorialCheck;
    private bool bIntroCheck;
    private string szDateCheck;
    private bool bDateCheck;

    [SerializeField]
    private bool bTestVer;

    private void Awake()
    {
        SetupUI();
        bTutorialCheck = PlayerPrefs.GetInt(szTutorialKey, 0) == 0;

        szDateCheck = PlayerPrefs.GetString(szDateKey, null);
        string szNowData = DateTime.Now.ToString("yyyy-MM-dd");
        bDateCheck = szDateCheck == null || szDateCheck != szNowData;
        if(bDateCheck || bTestVer == true)
        {
            PlayerPrefs.SetInt(szIntoKey, 0);
            PlayerPrefs.SetString(szDateKey, szNowData);
            int spriteKey = PlayerPrefs.GetInt(szSpriteDateKey, -1);
            spriteKey += 1;
            if(spriteKey == 10)
            {
                spriteKey = 0;
            }
            PlayerPrefs.SetInt(szSpriteDateKey, spriteKey);
            bIntroCheck = true;
        }
        else
        {
            bIntroCheck = PlayerPrefs.GetInt(szIntoKey, 0) == 0;
        }

        if (bTutorialCheck == true)
        {
            SetTutorial(true);
        }
        else
        {
            SetIntro(bIntroCheck);
        }
    }

    public void SetupUI()
    {
        for (int i = 0; i < m_listButton.Length; i++)
        {
            int index = i;
            m_listButton[index].onClick.AddListener(delegate { OnButtonPress(index); });
        }
    }

    public void SetTutorial(bool bActive)
    {
        UIManager.Instance.SetBackGround(bActive);
        tutorialObject.SetActive(bActive);
        if (bActive == true)
        {
            bool bLangaugeKorea = Application.systemLanguage == SystemLanguage.Korean;
            m_listTutorialLangauge[0].SetActive(bLangaugeKorea);
            m_listTutorialLangauge[1].SetActive(!bLangaugeKorea);
        }
        else
        {
            PlayerPrefs.SetInt(szTutorialKey, 1);
        }
    }

    public void SetIntro(bool bActive)
    {
        UIManager.Instance.SetBackGround(bActive);
        introObject.SetActive(bActive);
        if (bActive == true)
        {
            bool bLangaugeKorea = Application.systemLanguage == SystemLanguage.Korean;
            m_listIntroLangauge[0].SetActive(bLangaugeKorea);
            m_listIntroLangauge[1].SetActive(!bLangaugeKorea);
        }
        else
        {
            PlayerPrefs.SetInt(szIntoKey, 1);
        }
    }

    public void OnButtonPress(int index)
    {
        switch(index)
        {
            case 0:
                SetTutorial(false);
                break;
            case 1:
                SetIntro(false);
                break;
        }
    }
}
