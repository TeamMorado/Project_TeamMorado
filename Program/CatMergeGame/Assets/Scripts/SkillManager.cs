using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public GameManager m_GameManager;
    public SpawnBall m_SpawnBall;
    [Header("Destroy Same Type")]
    public int m_nDestroyNum = 0;
    public float m_limitTime = 0.1f;
    private Coroutine corDisableBallObject = null;

    [Header("Move Churu")]
    public float m_churuDuration = 1f;
    public SpriteRenderer m_churuSpriteRenderer = null;
    public SpriteRenderer m_churuSpriteRendererEffect = null;
    public AnimationCurve m_churuAnimationCurve;
    private Coroutine m_corSpawnChuru = null;
    public Camera cam;
    private float m_camDistance = 0f;
    private Vector3 m_churuScreenPos;
    private Vector3 m_churuSpriteRendererSize;
    private Vector3 m_churuSpriteLocalSize;
    private Vector3 m_churuSpriteRendererEffectSize;
    private Vector3 m_churuSpriteLocalEffectSize;
    private Vector3 m_churuStartPos;
    private Vector3 m_churuEndPos;

    public List<Image> m_listImage = new List<Image>();
    public List<Sprite> m_listSprite = new List<Sprite>();
    public List<Text> m_listText = new List<Text>();


    private void Awake()
    {
        Setup();
    }

    private void Start()
    {
        AdmobManager.Instance.SetRewardAdsAction(RewardAdsFunc);
    }

    public void Setup()
    {
        if(cam == null)
        {
            cam = GameObject.FindObjectOfType<Camera>();
        }
        m_churuScreenPos = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, -cam.transform.position.z));
        m_churuScreenPos.x = 0;

        m_churuSpriteRendererSize = m_churuSpriteRenderer.sprite.bounds.size;
        m_churuSpriteLocalSize = m_churuSpriteRenderer.transform.localScale;

        m_churuSpriteRendererEffectSize = m_churuSpriteRendererEffect.sprite.bounds.size;
        m_churuSpriteLocalEffectSize = m_churuSpriteRendererEffect.transform.localScale;
    }

    public void SetButtonInfo(int nIndex, bool bActive, string szHour, string szCount)
    {
        Image buttonImage = m_listImage[nIndex];
        if(buttonImage != null)
        {
            buttonImage.sprite = m_listSprite[(nIndex == 0 ? 0 : 2) + (bActive == true ? 0 : 1)];
        }
        Text buttonText = m_listText[nIndex];
        Text buttonHourText = m_listText[nIndex + 2];
        buttonHourText.gameObject.SetActive(!bActive);
        if (bActive == false)
        {
            buttonHourText.text = string.Format("{0}h", szHour);
        }
        buttonText.text = szCount;
    }

    public void RewardAdsFunc(int nIndex)
    {
        var stateType = GameManager.instance.stateType;
        switch(stateType)
        {
            case GameManager.eStateType.Destroy:
            case GameManager.eStateType.End:
                return;
        }

        switch(nIndex)
        {
            case 0:
                if(m_SpawnBall != null)
                {
                    m_nDestroyNum = m_SpawnBall.GetMaximumBall();
                }
                DestroyBallType(m_nDestroyNum);
                break;

            case 1:
                SpawnChuru(SpawnBall.instance.maxPosX);
                break;
        }
    }

    public void DestroyBallType(int nDestroyNum)
    {
        if (m_SpawnBall == null)
        {
            return;
        }
        m_GameManager.SetState(GameManager.eStateType.Freeze);
        m_SpawnBall.SetBallFreeze(true);
        List<BallScript> m_listDisableBall = m_SpawnBall.GetDisableBallList(nDestroyNum);

        if(corDisableBallObject != null)
        {
            StopCoroutine(corDisableBallObject);
            corDisableBallObject = null;
        }
        corDisableBallObject = StartCoroutine(IDisableBallObject(m_listDisableBall));
    }

    public IEnumerator IDisableBallObject(List<BallScript> m_listDisableBall)
    {
        for (int i = 0; i < m_listDisableBall.Count; i++)
        {
            yield return new WaitForSeconds(m_limitTime);
            BallScript p_ballScript = m_listDisableBall[i];
            if (p_ballScript.bSpawnWaiting == true)
            {
                continue;
            }
            SpawnBall.instance.SetParticle(p_ballScript.transform.position);
            GameManager.instance.AddScore(p_ballScript.m_Score_Pop, true, Vector2.zero);
            m_SpawnBall.DisableBallObject(p_ballScript);
        }
        m_SpawnBall.SetBallFreeze(false);
        m_GameManager.SetPrevState();
        corDisableBallObject = null;
    }

    public void SpawnChuru(float p_posX)
    {
        if(m_corSpawnChuru != null)
        {
            StopCoroutine(m_corSpawnChuru);
            m_corSpawnChuru = null;
        }
        m_GameManager.SetState(GameManager.eStateType.Freeze);
        m_SpawnBall.SetBallFreeze(true);
        m_corSpawnChuru = StartCoroutine(ISpawnChuru(p_posX));
    }

    public IEnumerator ISpawnChuru(float p_posX)
    {
        float startTime = 0f;
        m_churuSpriteRenderer.transform.position = m_churuStartPos;
        yield return null;
        float height = (m_churuSpriteRendererSize.y * m_churuSpriteLocalSize.y) + (m_churuSpriteRendererEffectSize.y * m_churuSpriteLocalEffectSize.y);
        m_churuStartPos = m_churuScreenPos + new Vector3(p_posX, height, 0f);
        m_churuEndPos = -m_churuScreenPos + new Vector3(p_posX, -height, 0f);

        while (startTime < m_churuDuration)
        {
            m_churuSpriteRenderer.transform.position = Vector3.Lerp(m_churuStartPos, m_churuEndPos, m_churuAnimationCurve.Evaluate((startTime / m_churuDuration)));
            startTime += Time.deltaTime;
            yield return null;
        }
        m_churuSpriteRenderer.transform.position = m_churuEndPos;
        m_GameManager.SetPrevState();
        m_SpawnBall.SetBallFreeze(false);
        //*/
    }
}
