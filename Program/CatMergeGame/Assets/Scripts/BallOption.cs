using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallOption", menuName = "BallOption", order = 2)]
public class BallOption : ScriptableObject
{
    [Header("Ball Size")]
    public float m_fSize;
    [Header("Bounce")]
    public float m_fBounce;
    [Header("Ball Mass(질량)")]
    public float m_fMass;
    [Header("Fall Speed (떨어지는 속도)")]
    public float m_BallSpeed;
    [Header("Ball Gravity")]
    public float m_fGravity;
    [Header("Ball Image")]
    public List<Sprite> m_listSprite;
    [Header("Ball Image")]
    public int m_ScoreMerge;
    public int m_ScorePop;

    [Header("Ball ")]
    public int m_Probability;
}
