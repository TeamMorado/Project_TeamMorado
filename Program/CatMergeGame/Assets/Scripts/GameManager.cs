using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using com.adjust.sdk;

public class GameManager : MonoSingleton<GameManager>
{
    public enum eStateType
    {
        Idle,
        Move,
        Spawn,
        SetDestroy,
        Destroy,
        End,
        Freeze,
        ShowADS,
    }
    public static GameManager instance;
    public SpawnBall spawnBall;
    public UIManager uiManager;

    public float mousePosX;
    public float frontValue;
    public float distance;

    public List<Vector2> m_listVec = new List<Vector2>();

    public BallScript selectBall;
    public Vector2 startPos;
    public float fixFloorDistance = 4.8f;
    public float floorDistance = 4.8f;
    public float xPos = 2f;
    public float f_UseX;
    public float f_UseDistance;
    public int arrayCount = 10;

    public eStateType stateType;
    public eStateType prevStateType;
    private float presentTime = 0f;
    private float limitTime = 0.1f;
    private float spawnTime = 0.5f;
    public int mListPos = 0;
    private int spawnNum = 0;

    public int prefabNum = 0;
    public GameObject limitLineObject;

    private List<BallScript> m_pListBall;
    private float limitDestoryTime = 0.25f;

    public UIScore uiScore_Best;
    public UIScore uiScore_Normal;
    public List<Text> m_listTextScore;
    public List<int> m_listProbability = new List<int>();
    
    private string scoreKey = "ScoreKey";
    private bool bADSCheck = true;
    private bool bEndFirst = true;
    private int nScore = 0;

    public int nComboMax = 10;
    public int nCombo = 0;
    private int scoreSum = 0;

    public Slider slider_VFX;
    public Slider slider_BGM;
    public Text testText;

    private bool bFlagNextFrame = false;

    public void AddScore(int score, bool bCheckEnd, Vector2 spawnPos)
    {
        nScore += Mathf.CeilToInt(score * (1 + (float)nCombo / nComboMax));
        if(bCheckEnd == false)
        {
            nCombo += 1;
            uiManager.SetCombo(nCombo, spawnPos);
        }
        for (int i = 0; i< m_listTextScore.Count;i++)
        {
            Text textScore = m_listTextScore[i];
            if(textScore == null)
            {
                continue;
            }
            textScore.text = nScore.ToString();
        }
    }

    public bool CheckLimitLine()
    {
        if(limitLineObject == null)
        {
            return false;
        }
        return limitLineObject.activeSelf == false;
    }

    public void SetLimitLine()
    {
        if(limitLineObject == null)
        {
            return;
        }
        limitLineObject.SetActive(true);
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        spawnBall = GetComponent<SpawnBall>();
        bADSCheck = false;
    }

    private void Start()
    {
        AdmobManager.Instance.SetFrontAdsAction(Setup);
        SetState(eStateType.ShowADS);
        //Setup();
    }



    public void Setup()
    {
        bEndFirst = true;
        m_listProbability.Clear();
        if (uiScore_Best != null)
        {
            uiScore_Best.gameObject.SetActive(false);
        }
        if(uiScore_Normal != null)
        {
            uiScore_Normal.gameObject.SetActive(false);
        }
        if(uiManager != null)
        {
            uiManager.SetBackGround(false);
        }
        prefabNum = -1;
        int spawnIndex = m_listProbability.Count == 0 ? 0 : m_listProbability[Random.Range(0, m_listProbability.Count)];
        //selectBall = spawnBall.SpawnBallObject(spawnIndex, startPos);
        //limitTime = selectBall.ballSpeed;
        nScore = 0;
        AddScore(0, true, Vector2.zero);
        SetState(eStateType.Spawn, true);
        limitLineObject.SetActive(false);

        if (SoundManager.Instance != null && uiManager != null)
        {
            slider_VFX.onValueChanged.AddListener(SoundManager.Instance.SetVolumeChangeVFX);
            slider_BGM.onValueChanged.AddListener(SoundManager.Instance.SetVolumeChangeBGM);
            slider_VFX.value = SoundManager.Instance.prevVfxValue;
            slider_BGM.value = SoundManager.Instance.prevBgmValue;
            SoundManager.Instance.PlaySFX("Start");
        }
        
    }

    private void LateUpdate()
    {
        for (int i = 0; i < m_listVec.Count - 1; i++)
        {
            Debug.DrawLine(m_listVec[i], m_listVec[i + 1],Color.red);
        }
        switch (stateType)
        {
            case eStateType.ShowADS:
                if (bADSCheck == false)
                {
                    bADSCheck = true;
                    Setup();
                    return;
                }
                AdmobManager.Instance.ShowFrontAd();
                SetState(eStateType.Freeze);
                return;
            case eStateType.Idle:
                SetVectorList();
                break;
            case eStateType.Move:

                presentTime += Time.deltaTime;
                if(presentTime >= limitTime)
                {
                    presentTime = 0f;
                    mListPos += 1;
                    if(mListPos >= m_listVec.Count)
                    {

                        SetState(eStateType.Spawn);
                        mListPos = 0;
                        return;
                    }
                    selectBall.transform.DOMove(m_listVec[mListPos], limitTime).SetEase(Ease.Linear);
                }
                break;
            case eStateType.Spawn:
                presentTime += Time.deltaTime;
                if (presentTime >= spawnTime)
                {
                    presentTime = 0f;
                    int spawnIndex = m_listProbability.Count == 0 ? 0 : m_listProbability[Random.Range(0, m_listProbability.Count)];
                    selectBall = spawnBall.SpawnBallObject(spawnIndex, startPos);
                    startPos.y = 4.2f + selectBall.mCollider2D.radius * selectBall.ballOption.m_fSize;
                    selectBall.transform.position = startPos;
                    limitTime = selectBall.ballSpeed;
                    SetState(eStateType.Idle);
                }
                break;
            case eStateType.Freeze:
                break;
            case eStateType.SetDestroy:
                presentTime = 0f;
                m_pListBall = spawnBall.GetSortList();
                SetState(eStateType.Destroy);
                break;
            case eStateType.Destroy:
                if(m_pListBall == null || m_pListBall.Count == 0)
                {
                    SetState(eStateType.End);
                    return;
                }
                presentTime += Time.deltaTime;
                if (presentTime < limitDestoryTime)
                {
                    return;
                }
                presentTime = 0f;
                BallScript disableBall = m_pListBall[0];
                AddScore(disableBall.m_Score_Pop, true, Vector2.zero);
                spawnBall.DisableBallObject(disableBall, true);
                break;
            case eStateType.End:
                if(bEndFirst == false)
                {
                    return;
                }
                bEndFirst = false;
                int maxScore = PlayerPrefs.GetInt(scoreKey, 0);
                bool bFlagMax = maxScore < nScore;
                var selectUI = bFlagMax ? uiScore_Best : uiScore_Normal;
                selectUI.SetScoreText(nScore);
                if(bFlagMax == true)
                {
                    AdjustEvent adjustEvent = new AdjustEvent("highest score achieved");
                    adjustEvent.addCallbackParameter("highstScore", nScore.ToString());
                    Adjust.trackEvent(adjustEvent);
                    PlayerPrefs.SetInt(scoreKey, nScore);
                }
                else if(bFlagMax == false)
                {
                    selectUI.SetBestScoreText(maxScore);
                }
                selectUI.gameObject.SetActive(true);
                UIManager.Instance.SetBackGround(true);
                break;
        }
    }

    public void SetState(eStateType _state, bool bFirst = false)
    {
        prevStateType = (bFirst) ? _state : stateType;
        stateType = _state;
    }

    public void SetPrevState()
    {
        stateType = prevStateType;
    }

    private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition
            = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();


        EventSystem.current
        .RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public void SetVectorList()
    {
        if (selectBall == null)
        {
            return;
        }
        if (IsPointerOverUIObject(Input.mousePosition))
        {
            if(bFlagNextFrame == false)
            {
                bFlagNextFrame = true;
            }
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(bFlagNextFrame == true)
            {
                bFlagNextFrame = false;
                return;
            }
            mousePosX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }
        else
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePosX, 3.05f), Vector2.down, Mathf.Infinity);
        Transform hitTransform = hit.transform;
        if(hitTransform == null)
        {
            floorDistance = fixFloorDistance;
        }
        else
        {
            string tag = hitTransform.tag;
            if (tag == "Wall" || tag == "Obstacle")
            {
                floorDistance = fixFloorDistance;
            }
            else
            {
                floorDistance = hit.transform.position.y * -1f;
            }
        }

        m_listVec.Clear();
        xPos = startPos.x;
        distance = Mathf.Abs(xPos - mousePosX);
        SetFirstValue();
        bool bCheckNaN = false;
        f_UseX = GetValue_X(-fixFloorDistance, ref bCheckNaN) * -1;
        if(bCheckNaN == true)
        {
            float distanceNaN = startPos.y + floorDistance;
            for (int i = 0; i < arrayCount; i++)
            {
                float nValue = startPos.y - i * (distanceNaN / arrayCount);
                m_listVec.Add(new Vector2(f_UseX, nValue));
            }
            m_listVec.Add(new Vector2(f_UseX, startPos.y - distanceNaN));
        }
        else
        {
            f_UseDistance = Mathf.Abs(xPos - f_UseX);
            bool bCheck = mousePosX < xPos;
            for (float i = 0; i < arrayCount; i++)
            {
                float nValue = 0f;
                if (bCheck)
                {
                    nValue = f_UseX + i * (f_UseDistance / arrayCount);
                }
                else
                {
                    nValue = xPos + i * (f_UseDistance / arrayCount);
                }
                m_listVec.Add(new Vector2(nValue, SetValue_Y(nValue)));
            }
            if (bCheck == true)
            {
                float nValue = f_UseX + f_UseDistance;
                m_listVec.Add(new Vector2(nValue, SetValue_Y(nValue)));
                m_listVec.Reverse();
            }
            else
            {
                float nValue = xPos + f_UseDistance;
                m_listVec.Add(new Vector2(nValue, SetValue_Y(nValue)));
            }
        }

        SetState(eStateType.Move);
        selectBall.transform.DOMove(m_listVec[mListPos], limitTime).SetEase(Ease.Linear);
        selectBall.bEnableMerge = true;
        selectBall.bSpawnWaiting = false;
        nCombo = 0;
    }

    public void StopMoveFunc()
    {
        mListPos = 0;
        SetState(eStateType.Spawn);
        selectBall.transform.DOKill();
    }

    public float SetValue_Y(float x)
    {
        return frontValue * (x - xPos) * (x - xPos) + startPos.y;
    }

    public void SetFirstValue()
    {
        frontValue = -1 * (startPos.y + floorDistance) / (distance * distance); //(startPos.y - floorDistance) / ((distance) * (- distance));
    }

    public float GetValue_X(float value_Y, ref bool bCheckNaN)
    {
        float a = frontValue;
        if(a == 0)
        {
            bCheckNaN = true;
            return -1 * startPos.x;
        }
        bCheckNaN = false;
        float b = startPos.y;
        float c = xPos;
        float checkMinus = (value_Y - b) / a;
        //Debug.Log(" (value_Y - b) / a : " + checkMinus);
        return Mathf.Sqrt(checkMinus) - c;
    }
}
