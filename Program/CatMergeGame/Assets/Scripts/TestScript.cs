using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestScript : MonoBehaviour
{
    public List<BallScript> m_listBallScript = new List<BallScript>();
    public float m_fSpawnTime = 0.5f;
    public float maxHeight = -100f;
    private float maxPosX;
    [Header("타입 없애기")]
    public int m_nDestroyNum;
    private Coroutine corDisableBallObject;

    [Header("츄르 움직이기")]
    public float m_churuDuration = 2f;
    public SpriteRenderer m_churuSpriteRenderer = null;
    public AnimationCurve m_churuAnimationCurve;
    private Coroutine m_corSpawnChuru = null;
    public Camera cam;
    private float m_camDistance = 0f;
    private Vector3 m_churuScreenPos;
    private Vector3 m_churuSpriteRendererSize;
    private Vector3 m_churuSpriteLocalSize;
    private Vector3 m_churuStartPos;
    private Vector3 m_churuEndPos;


    private List<BallScript> m_listBallUse = new List<BallScript>();
    public List<BallScript> ListBallUse { get { return m_listBallUse; } }
    private List<int> m_listProbability = new List<int>();
    private float presentTime = 0f;
    private bool bEndCheck = false;

    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        for (int i = 0; i < m_listBallScript.Count; i++)
        {
            BallScript ballScript = m_listBallScript[i];
            int nCount = ballScript.ballOption.m_Probability;
            for (int nIndex = 0; nIndex < nCount; nIndex++)
            {
                m_listProbability.Add(ballScript.ballType);
            }
        }
        m_churuScreenPos = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, -cam.transform.position.z));
        m_churuScreenPos.x = 0;

        m_churuSpriteRendererSize = m_churuSpriteRenderer.sprite.bounds.size;
        m_churuSpriteLocalSize = m_churuSpriteRenderer.transform.localScale;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            RewardAdsFunc(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            RewardAdsFunc(1);
        }
        if (bEndCheck == true)
        {
            return;
        }
        for (int i = 0; i < m_listBallUse.Count; i++)
        {
            BallScript ballObject = m_listBallUse[i];
            float ballPos = ballObject.transform.position.y + ballObject.transform.localScale.y * ballObject.mCollider2D.radius;
            if(ballObject.bFirst == false && maxHeight < ballPos)
            {
                maxHeight = ballPos;
                maxPosX = ballObject.transform.position.x;
            }
        }
        if (maxHeight >= 3.9f)
        {
            bEndCheck = true;
            return;
        }
        presentTime += Time.deltaTime;
        if(presentTime >= m_fSpawnTime)
        {
            SpawnBallObject();
            presentTime = 0f;
        }
    }

    public void SpawnBallObject()
    {
        int ballTypeNum = m_listProbability[Random.Range(0, m_listProbability.Count)];
        BallScript spawnPrefab = m_listBallScript[ballTypeNum];

        BallScript spawnObject = Instantiate(spawnPrefab);
        float posX = Random.Range(-3.5f, 3.5f);
        bool bMinus = posX < 0;
        posX += spawnObject.mCollider2D.radius * (bMinus ? 1 : -1);
        Vector2 spawnPos = new Vector2(posX, 4.5f);
        spawnObject.transform.position = spawnPos;
        spawnObject.mRigidBody2D.gravityScale = spawnPrefab.ballOption.m_fGravity;
        spawnObject.bSpawnWaiting = false;
        spawnObject.bEnableMerge = true;
        m_listBallUse.Add(spawnObject);
    }


    public void RewardAdsFunc(int nIndex)
    {
        switch (nIndex)
        {
            case 0:
                DestroyBallType(m_nDestroyNum);
                break;

            case 1:
                SpawnChuru(maxPosX);
                break;
        }
    }

    private void Freeze(bool bCheck)
    {
        lock (m_listBallUse)
        {
            for (int i = 0; i < m_listBallUse.Count; i++)
            {
                m_listBallUse[i].mRigidBody2D.constraints = bCheck ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
            }
        }
    }

    public void DestroyBallType(int nDestroyNum)
    {
        List<BallScript> m_listDisableBall = m_listBallUse.Where(n => n.ballType == m_nDestroyNum).ToList();

        if (corDisableBallObject != null)
        {
            StopCoroutine(corDisableBallObject);
            corDisableBallObject = null;
        }
        corDisableBallObject = StartCoroutine(IDisableBallObject(m_listDisableBall));
    }

    public IEnumerator IDisableBallObject(List<BallScript> m_listDisableBall)
    {
        Freeze(true);
        lock (m_listDisableBall)
        {
            int count = m_listDisableBall.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.1f);
                BallScript p_ballScript = m_listDisableBall[0];
                m_listBallUse.Remove(p_ballScript);
                m_listDisableBall.Remove(p_ballScript);
                Destroy(p_ballScript.gameObject);
            }
        }
        Freeze(false);
        corDisableBallObject = null;
    }

    public void SpawnChuru(float p_posX)
    {
        if (m_corSpawnChuru != null)
        {
            StopCoroutine(m_corSpawnChuru);
            m_corSpawnChuru = null;
        }
        Freeze(true);
        m_corSpawnChuru = StartCoroutine(ISpawnChuru(p_posX));
    }

    public IEnumerator ISpawnChuru(float p_posX)
    {
        float startTime = 0f;
        yield return null;
        
        m_churuStartPos = m_churuScreenPos + new Vector3(p_posX, m_churuSpriteRendererSize.y * m_churuSpriteLocalSize.y * 0.5f, 0f);
        m_churuEndPos = -m_churuScreenPos + new Vector3(p_posX, -m_churuSpriteRendererSize.y * m_churuSpriteLocalSize.y * 0.5f, 0f);

        m_churuSpriteRenderer.transform.position = m_churuStartPos;
        while (startTime < m_churuDuration)
        {
            m_churuSpriteRenderer.transform.position = Vector3.Lerp(m_churuStartPos, m_churuEndPos, m_churuAnimationCurve.Evaluate((startTime / m_churuDuration)));
            startTime += Time.deltaTime;
            yield return null;
        }
        m_churuSpriteRenderer.transform.position = m_churuEndPos;
        Freeze(false);

    }
}
