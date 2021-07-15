using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallScript : MonoBehaviour
{
    private const string szSpriteDateKey = "SpriteDateKey";

    public BallOption ballOption;
    public int ballType = 0;
    public int spawnNum = 0;
    public SpriteRenderer mSpriteRenderer;
    public CircleCollider2D mCollider2D;
    public Rigidbody2D mRigidBody2D;
    public float ballSpeed;
    public bool bSpawnWaiting = true;
    public bool isConnenct = false;
    public bool bEnableMerge = false;
    public bool bFirst = true;
    public bool bCheckHeight = false;
    public int probability = 0;

    private Vector2 deltaPos;

    public int m_Score_Merge;
    public int m_Score_Pop;

    private void Awake()
    {
        mCollider2D = GetComponent<CircleCollider2D>();
        mRigidBody2D = GetComponent<Rigidbody2D>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        deltaPos = transform.position;
    }

    public void SetData(int _spawnNum, bool isSpawn)
    {
        isConnenct = false;
        bFirst = !isSpawn;
        bSpawnWaiting = !isSpawn;
        bCheckHeight = false;
        this.spawnNum = _spawnNum;
        mCollider2D.enabled = true;
        mRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
        mRigidBody2D.constraints = RigidbodyConstraints2D.None;
        if (ballOption != null)
        {
            List<Sprite> p_listSprite = ballOption.m_listSprite;
            if(p_listSprite.Count == 1)
            {
                mSpriteRenderer.sprite = p_listSprite[0];
            }
            else
            {
                int spritePos = PlayerPrefs.GetInt(szSpriteDateKey, 0);
                mSpriteRenderer.sprite = p_listSprite[spritePos];
            }
            this.transform.localScale = Vector3.one * ballOption.m_fSize;

            mCollider2D.sharedMaterial.bounciness = ballOption.m_fBounce;
            ballSpeed = ballOption.m_BallSpeed;
            mRigidBody2D.drag = ballOption.m_BallSpeed * 10;
            mRigidBody2D.mass = ballOption.m_fMass;
            mRigidBody2D.gravityScale = isSpawn == true ? ballOption.m_fGravity : 0f;
            this.m_Score_Merge = ballOption.m_ScoreMerge;
            this.m_Score_Pop = ballOption.m_ScorePop;
            this.probability = ballOption.m_Probability;
            bEnableMerge = isSpawn;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(bSpawnWaiting == true)
        {
            return;
        }
        if (collision.transform.tag == "Churu")
        {
            SpawnBall.instance.DisableBallObject(this);
            SpawnBall.instance.SetParticle(this.transform.position);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bSpawnWaiting == true)
        {
            return;
        }

        if (bEnableMerge == false)
        {
            return;
        }

        if (collision.transform.tag != "Obstacle" && bCheckHeight == false)
        {
            mRigidBody2D.gravityScale = 1f;
            bCheckHeight = true;
        }

        if (bFirst == true)
        {
            bFirst = false;
            mRigidBody2D.AddForce(ReflectionFunc(collision) * 150f, ForceMode2D.Force);
            BallScript otherBall = collision.transform.GetComponent<BallScript>();
            SoundManager.Instance.PlaySFX(string.Format("Cat_00{0}", Random.Range(0, 3) + 1));
            if(otherBall != null)
            {
                otherBall.mRigidBody2D.AddForce(ReflectionFunc(collision) * -150f, ForceMode2D.Force);
            }
        }

        if(GameManager.instance.stateType == GameManager.eStateType.Destroy || GameManager.instance.stateType == GameManager.eStateType.End)
        {
            return;
        }

        if (GameManager.instance.selectBall == this)
        {
            mRigidBody2D.gravityScale = ballOption.m_fGravity;
            GameManager.instance.StopMoveFunc();
            
        }
        BallScript ballScript_Connect = collision.transform.GetComponent<BallScript>();
        if (ballScript_Connect == null)
        {
            return;
        }
        if (this.ballType != ballScript_Connect.ballType)
        {
            return;
        }
        if(this.ballType >= SpawnBall.instance.mList_BallPrefab.Count - 1)
        {
            return;
        }
        if (this.spawnNum <= ballScript_Connect.spawnNum || this.isConnenct == true || ballScript_Connect.isConnenct == true)
        {
            return;
        }
        MergeBall(ballScript_Connect);
    }

    private void MergeBall(BallScript ballScript_Connect)
    {
        this.isConnenct = true;
        ballScript_Connect.isConnenct = true;
        mCollider2D.enabled = false;
        mRigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        ballScript_Connect.mRigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        ballScript_Connect.mRigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        Vector2 collisionPos = ballScript_Connect.transform.position;
        
        this.transform.DOMove(collisionPos, 0.075f).OnKill(() => {
            SpawnNewObject(ballScript_Connect);
            collisionPos.y += ballScript_Connect.mCollider2D.radius + 0.75f;
            GameManager.instance.AddScore(this.m_Score_Merge, false, collisionPos);
        });
    }

    private void SpawnNewObject(BallScript ballScript_Connect)
    {
        int n_BallType = ballScript_Connect.ballType;
        Vector2 spawnPos = ballScript_Connect.transform.position;
        SpawnBall.instance.SetParticle(spawnPos);
        SpawnBall.instance.DisableBallObject(ballScript_Connect);
        SpawnBall.instance.DisableBallObject(this);
        SpawnBall.instance.SpawnBallObject(n_BallType + 1, spawnPos, true);
    }

    private Vector2 ReflectionFunc(Collision2D collision)
    {
        var m_list = GameManager.instance.m_listVec;
        int m_pos = m_list.Count - 1;
        if(m_pos == 0)
        {
            m_pos += 1;
        }
        Vector2 incomingVec = (m_list[m_pos] - m_list[m_pos - 1]).normalized;
        //Vector2 incomingVec = ((Vector2)this.transform.position - deltaPos).normalized;

        Vector3 normalVec = collision.contacts[0].normal;       // 법선벡터

        Vector3 reflectVec = Vector3.Reflect(incomingVec, normalVec);
        return reflectVec;
    }
}
