using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnBall : MonoBehaviour
{
    public Vector2 startPos;
    public int testNum;

    public Dictionary<int, BallScript> map_BallPrefab = new Dictionary<int, BallScript>();
    public List<BallScript> map_Use = new List<BallScript>();
    public Dictionary<int, List<BallScript>> map_Recycle = new Dictionary<int, List<BallScript>>();
    public List<BallScript> mList_BallPrefab = new List<BallScript>();
    public GameObject m_ParticlePrefab;
    public List<ParticleSystem> mList_Particle = new List<ParticleSystem>();
    public static SpawnBall instance;
    public int spawnNum = 0;

    [SerializeField]
    private bool bTestVer;
    [SerializeField]
    private int nTestSpawnBallType;

    private float fEndTime = 0f;
    public float maxPosX = 0f;
    public float maxHeight = 0f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < mList_BallPrefab.Count; i++)
        {
            BallScript ballPrefab = mList_BallPrefab[i];
            if(ballPrefab == null)
            {
                continue;
            }
            map_BallPrefab.Add(ballPrefab.ballType, ballPrefab);
            map_Recycle.Add(ballPrefab.ballType, new List<BallScript>());
        }
    }
    private void Update()
    {
        maxHeight = -10f;
        for (int i = 0; i < map_Use.Count; i++)
        {
            var ballObject = map_Use[i];
            if(ballObject.bCheckHeight == false)
            {
                continue;
            }
            CircleCollider2D circleCollider = ballObject.mCollider2D;
            float ballPos = ballObject.transform.position.y + ballObject.transform.localScale.y * circleCollider.radius;
            if(maxHeight < ballPos)
            {
                maxHeight = ballPos;
                maxPosX = ballObject.transform.position.x;
                //GameManager.instance.floorDistance = Mathf.Abs(maxHeight);
            }
        }
        if(maxHeight < 3f)
        {
            fEndTime = 0f;
        }
        if(maxHeight >= 3f && GameManager.instance.CheckLimitLine())
        {
            GameManager.instance.SetLimitLine();
        }
        else if(maxHeight >= 3.9f)
        {
            //GetSortList();
            fEndTime += Time.deltaTime;
            if(fEndTime >= 2f)
            {
                fEndTime = 0f;
                SoundManager.Instance.PlaySFX("Fail");
                GameManager.instance.SetState(GameManager.eStateType.SetDestroy);
            }
        }
    }

    public void SetParticle(Vector2 spawnPos)
    {
        ParticleSystem particleSystem = null;
        lock (mList_Particle)
        {
            for(int i = 0; i < mList_Particle.Count; i ++)
            {
                ParticleSystem nParticleSystem = mList_Particle[i];
                if (nParticleSystem.isPlaying == false)
                {
                    particleSystem = nParticleSystem;
                    break;
                }
            }
        }
        if(particleSystem == null)
        {
            GameObject nObject = Instantiate(m_ParticlePrefab);
            if (nObject == null)
            {
                return;
            }
            particleSystem = nObject.GetComponent<ParticleSystem>();
        }
        mList_Particle.Add(particleSystem);
        particleSystem.gameObject.transform.position = spawnPos;
        particleSystem.Play();

        SoundManager.Instance.PlaySFX("Merge");
    }

    public BallScript SpawnBallObject(int ballTypeNum, Vector2 spawnPos, bool bSpawn = false)
    {
        int ballTypeSpawnNum = (bTestVer == true) ? nTestSpawnBallType : ballTypeNum;
        if(map_BallPrefab.ContainsKey(ballTypeSpawnNum) == false)
        {
            return null;
        }
        lock(map_Recycle)
        {
            List<BallScript> m_pListRecycle = map_Recycle[ballTypeSpawnNum];
            BallScript nBallScript = null;
            if (m_pListRecycle.Count == 0)
            {
                nBallScript = Instantiate(map_BallPrefab[ballTypeSpawnNum]);
            }
            else
            {
                BallScript pBallScript = m_pListRecycle[0];
                nBallScript = pBallScript;
                m_pListRecycle.Remove(pBallScript);
            }
            nBallScript.SetData(spawnNum, bSpawn);
            spawnNum += 1;
            nBallScript.transform.position = spawnPos;
            nBallScript.mRigidBody2D.constraints = RigidbodyConstraints2D.None;
            nBallScript.mRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            nBallScript.gameObject.SetActive(true);
            float fBallSize = nBallScript.ballOption.m_fSize;
            nBallScript.transform.localScale = Vector3.one * fBallSize * 0.8f;
            nBallScript.transform.DOScale(fBallSize * 1.1f, 0.1f).OnKill(() =>
            {
                nBallScript.transform.DOScale(fBallSize * 1.0f, 0.1f).OnKill(() =>
                {
                    nBallScript.transform.localScale = Vector3.one * fBallSize;
                });
            });
            map_Use.Add(nBallScript);

            if (GameManager.instance.prefabNum < ballTypeNum)
            {
                GameManager.instance.prefabNum = ballTypeNum;
                int count = nBallScript.probability;
                for(int i = 0; i < count; i++)
                {
                    GameManager.instance.m_listProbability.Add(ballTypeNum);
                }
            }
            return nBallScript;
        }
    }

    public int SortedList_Height(BallScript ball_A, BallScript ball_B)
    {
        float height_A = ball_A.transform.position.y;
        float height_B = ball_B.transform.position.y;
        if (height_A > height_B)
        {
            return -1;
        }
        else if (height_A == height_B)
        {
            return 0;
        }
        return 1;
    }

    public List<BallScript> GetSortList()
    {
        map_Use.Sort(SortedList_Height);
        return map_Use;
    }

    public void DisableBallObject(BallScript nBallScript, bool bCheckEnd = false)
    {
        if(bCheckEnd == true)
        {
            SetParticle(nBallScript.transform.position);
        }
        map_Use.Remove(nBallScript);
        List<BallScript> m_pListRecycle = map_Recycle[nBallScript.ballType];
        m_pListRecycle.Add(nBallScript);
        nBallScript.mRigidBody2D.gravityScale = 0f;
        nBallScript.gameObject.SetActive(false);
    }

    public void SetBallFreeze(bool bCheckFreeze)
    {
        lock(map_Use)
        {
            for (int i = 0; i < map_Use.Count; i++)
            {
                BallScript pBallScript = map_Use[i];
                pBallScript.mRigidBody2D.constraints = bCheckFreeze ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
            }
        }
    }

    public List<BallScript> GetDisableBallList(int nBallNum)
    {
        var p_listDisable =
            map_Use
            .Where(nBall => nBall.ballType == nBallNum)
            .OrderBy(nBall => nBall.transform.position.y)
            .ToList();

        return p_listDisable;
    }
}
