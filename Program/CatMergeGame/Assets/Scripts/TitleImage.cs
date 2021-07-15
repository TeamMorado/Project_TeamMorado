using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleImage : MonoBehaviour
{
    public Image titleImage;
    public Sprite[] m_listSprite = new Sprite[2];
    // Start is called before the first frame update
    void Start()
    {
        bool bKorean = Application.systemLanguage == SystemLanguage.Korean;
        titleImage.sprite = m_listSprite[bKorean == true ? 0 : 1];
    }
}
