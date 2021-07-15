using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public enum eIndex
    {
        UI_Option = 0,
        UI_Back,
        UI_RemoveADS,
        Button_Option,
        Button_OptionClose,
        Button_Back,
        Button_BackClose,
        Button_MoveScene,
        Button_Contact,
        Button_RemoveADS,
        Button_RemoveADSClose,
        Button_RemoveBall,
        Button_SpawnChuru,
        Sprite_Combo,
        Max
    }

    private static UIManager instance;

    public static UIManager Instance
    {
        get{ 
            if(UIManager.instance == null)
            {
                UIManager.instance = GameObject.FindObjectOfType<UIManager>();
            }
            return UIManager.instance;
        }
    }

    public Camera m_Camera;
    public Canvas rootCanvas;
    RectTransform CanvasRect;
    public RectTransform panelRect;
    public List<Button> m_listButton = new List<Button>();
    public List<GameObject> m_listUIObject = new List<GameObject>();
    public List<Image> m_listImage = new List<Image>();
    public List<Sprite> m_listComboSprite = new List<Sprite>();
    public List<float> m_listDelayTime = new List<float>();
    public MoveScene moveScene;
    public ExitGame exitGame;
    private Vector2 localVec = Vector2.one;

    public GameObject m_backGroundObject;
    public Sprite n_spriteDisableADSButton;
    private string removeAds = "remove_ads";
    private bool bCheckRemoveADS;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResolutionFix();
        SetupUI();
        Refresh();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            SetBackGround(true);
            SetBackUI(true);
        }
    }

    private void ResolutionFix()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = (float)720 / 1280;

        float differenceInSize = targetRatio / screenRatio;
        if (screenRatio >= targetRatio)
        {
            m_Camera.orthographicSize = 1280f / 2f * 0.01f;
        }
        else
        {
            m_Camera.orthographicSize = 1280f / 2f * differenceInSize * 0.01f;
        }
    }

    public void SetupUI()
    {
        CanvasRect = rootCanvas.GetComponent<RectTransform>();
        for (int i = 0; i < m_listButton.Count; i++)
        {
            int index = i;
            m_listButton[index].onClick.AddListener(delegate { OnButtonPress(index); });
        }
        SetPurchaseButton();
    }

    public void SetPurchaseButton()
    {
        bCheckRemoveADS = PlayerPrefs.GetInt(removeAds, 0) == 0;
        if (bCheckRemoveADS == false)
        {
            Button p_buttonRemoveADS = m_listButton[(int)eIndex.Button_RemoveADS - (int)eIndex.Button_Option];
            p_buttonRemoveADS.image.sprite = n_spriteDisableADSButton;
            p_buttonRemoveADS.GetComponentInChildren<Text>().color = new Color32(74, 74, 74, 255);
            p_buttonRemoveADS.enabled = bCheckRemoveADS;
        }
    }


    public void OnButtonPress(int index)
    {
        switch((eIndex)index + (int)eIndex.Button_Option)
        {
            case eIndex.Button_Option:
                SetBackGround(true);
                SetOptionUI(true);
                break;
            case eIndex.Button_OptionClose:
                SetBackGround(false);
                SetOptionUI(false);
                break;
            case eIndex.Button_Back:
                SetBackGround(true);
                SetBackUI(true);
                break;
            case eIndex.Button_BackClose:
                SetBackGround(false);
                SetBackUI(false);
                break;
            case eIndex.Button_MoveScene:
                moveScene.LoadMainScene();
                break;
            case eIndex.Button_Contact:
                Application.OpenURL("https://www.facebook.com/indiemorado");
                break;
            case eIndex.Button_RemoveADS:
                if(bCheckRemoveADS == false)
                {
                    return;
                }
                SetOptionUI(false);

                SetBackGround(true);
                SetRemoveADSUI(true);
                break;
            case eIndex.Button_RemoveADSClose:
                SetBackGround(false);
                SetRemoveADSUI(false);
                break;
            case eIndex.Button_RemoveBall:
                AdmobManager.Instance.ShowRewardAd(0);
                break;
            case eIndex.Button_SpawnChuru:
                AdmobManager.Instance.ShowRewardAd(1);
                break;
        }
    }

    public void SetRemoveADSUI(bool bActive)
    {
        m_listUIObject[(int)eIndex.UI_RemoveADS].SetActive(bActive);
    }

    public void SetOptionUI(bool bActive)
    {
        m_listUIObject[(int)eIndex.UI_Option].SetActive(bActive);
    }

    public void SetBackUI(bool bActive)
    {
        m_listUIObject[(int)eIndex.UI_Back].SetActive(bActive);
    }

    public void SetCombo(int pCombo, Vector2 pPos)
    {
        int imagePos = GetComboSprite(pCombo);
        if (imagePos == -1)
        {
            return;
        }
        Image pComboImage = null;
        lock(m_listImage)
        {
            for(int i = 0; i < m_listImage.Count; i++)
            {
                Image nImage = m_listImage[i];
                if (nImage.gameObject.activeSelf == false)
                {
                    pComboImage = nImage;
                }
            }
        }
        if(pComboImage == null)
        {
            GameObject comboObject = new GameObject("ComboObject");
            comboObject.transform.parent = rootCanvas.transform;
            comboObject.transform.localScale = Vector3.one;
            comboObject.AddComponent<RectTransform>();
            pComboImage = comboObject.AddComponent<Image>();
        }
        pComboImage.sprite = m_listComboSprite[imagePos];
        pComboImage.SetNativeSize();

        Vector2 ViewportPosition = m_Camera.WorldToViewportPoint(pPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        float limitDistance = CanvasRect.sizeDelta.x * 0.5f - pComboImage.rectTransform.rect.width * 0.5f;
        if (WorldObject_ScreenPosition.x < -limitDistance)
        {
            WorldObject_ScreenPosition.x = -limitDistance;
        }
        else if(WorldObject_ScreenPosition.x > limitDistance)
        {
            WorldObject_ScreenPosition.x = limitDistance;
        }
        WorldObject_ScreenPosition.y += pComboImage.rectTransform.rect.height * 0.5f;
        //now you can set the position of the ui element
        pComboImage.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        pComboImage.transform.localScale = localVec * 0.8f;
        pComboImage.transform.DOScale(localVec * 1.1f, 0.15f).OnKill(() =>
        {
            pComboImage.transform.DOScale(localVec * 0f, 0.15f).SetDelay(m_listDelayTime[imagePos]).OnKill(() =>
            {
                pComboImage.gameObject.SetActive(false);
            });
        });
        //pComboSpriteRenderer.transform.position = 
    }

    private int GetComboSprite(int pCombo)
    {
        if(pCombo <= 1)
        {
            return -1;
        }
        else if(pCombo <= 3)
        {
            return 0;
        }
        else if(pCombo <= 5)
        {
            return 1;
        }
        return 2;
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();
        ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panelRect.anchorMin = anchorMin;
        panelRect.anchorMax = anchorMax;
    }

    public void SetBackGround(bool bActive)
    {
        if(bActive != m_backGroundObject.activeSelf)
        {
            m_backGroundObject.SetActive(bActive);
        }
    }
}
