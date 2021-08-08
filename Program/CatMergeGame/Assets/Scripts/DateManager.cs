using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class DateManager : MonoBehaviour
{
    public SkillManager m_SkillManager;
    private static bool bApplicationStart = true;
    private const string szKey_PrevDateTime_RemoveBall = "prevDateTime_RemoveBall";
    private const string szKey_PrevDateTime_CreateChuru = "prevDateTime_CreateChuru";

    private string szKey_RemoveBall = "Skill_RemoveBall";
    private string szKey_CreateChuru = "Skill_CreateChuru";
    public int nSkillCount_RemoveBallTotalCount = 2;
    public int nSkillCount_CreateChuruTotalCount = 1;
    public bool bReset;
    private int nSkillCount_RemoveBall;
    private int nSkillCount_CreateChuru;
    private bool bSkillEnable_RemoveBall;
    private bool bSkillEnable_CreateChuru;

    private CultureInfo provider = CultureInfo.InvariantCulture;
    private const string dateTimeFormat = "yyyy-MM-dd-HH-mm";

    private void Awake()
    {
        if(bReset)
        {
            PlayerPrefs.DeleteAll();
        }
        //PlayerPrefs.DeleteAll();
        InitSkillCount();
    }

    private void Update()
    {
        if(bSkillEnable_RemoveBall == false)
        {
            CheckDateTimeChange(szKey_PrevDateTime_RemoveBall);
            m_SkillManager.SetButtonInfo(0, bSkillEnable_RemoveBall, GetLimitHour().ToString(), nSkillCount_RemoveBall.ToString());
        }
        if(bSkillEnable_CreateChuru == false)
        {
            CheckDateTimeChange(szKey_PrevDateTime_CreateChuru);
            m_SkillManager.SetButtonInfo(1, bSkillEnable_CreateChuru, GetLimitHour().ToString(), nSkillCount_CreateChuru.ToString());
        }
    }

    public bool CheckApplicationStart()
    {
        bool returnValue = bApplicationStart;
        if (bApplicationStart == true)
        {
            bApplicationStart = false;
        }
        return returnValue;
    }

    public int GetSkillCount(int skillType)
    {
        switch(skillType)
        {
            case 0:
                return nSkillCount_RemoveBall;
            case 1:
                return nSkillCount_CreateChuru;
        }
        return -1;
    }

    public bool CheckEnableSkill(int skillType)
    {
        switch(skillType)
        {
            case 0:
                {
                    if(nSkillCount_RemoveBall > 0)
                    {
                        return true;
                    }
                }
                break;
            case 1:
                {
                    if (nSkillCount_CreateChuru > 0)
                    {
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    public void ReduceADSCount(int skillType)
    {
        switch (skillType)
        {
            case 0:
                {
                    nSkillCount_RemoveBall -= 1;
                    if (nSkillCount_RemoveBall <= 0)
                        bSkillEnable_RemoveBall = false;
                    PlayerPrefs.SetInt(szKey_RemoveBall, nSkillCount_RemoveBall);
                    m_SkillManager.SetButtonInfo(0, bSkillEnable_RemoveBall, GetLimitHour().ToString(), nSkillCount_RemoveBall.ToString());
                }
                break;
            case 1:
                {
                    nSkillCount_CreateChuru -= 1;
                    if (nSkillCount_CreateChuru <= 0)
                        bSkillEnable_CreateChuru = false;
                    PlayerPrefs.SetInt(szKey_CreateChuru, nSkillCount_CreateChuru);
                    m_SkillManager.SetButtonInfo(1, bSkillEnable_CreateChuru, GetLimitHour().ToString(), nSkillCount_CreateChuru.ToString());
                }
                break;
        }
    }

    private void InitSkillCount()
    {
        nSkillCount_RemoveBall = PlayerPrefs.GetInt(szKey_RemoveBall, nSkillCount_RemoveBallTotalCount);
        nSkillCount_CreateChuru = PlayerPrefs.GetInt(szKey_CreateChuru, nSkillCount_CreateChuruTotalCount);

        bSkillEnable_RemoveBall = nSkillCount_RemoveBall > 0;
        bSkillEnable_CreateChuru = nSkillCount_CreateChuru > 0;
        m_SkillManager.SetButtonInfo(0, bSkillEnable_RemoveBall, GetLimitHour().ToString(), nSkillCount_RemoveBall.ToString());
        m_SkillManager.SetButtonInfo(1, bSkillEnable_CreateChuru, GetLimitHour().ToString(), nSkillCount_CreateChuru.ToString());
    }

    private int GetLimitHour()
    {
        string szPrevDateTime = DateTime.Now.ToString(dateTimeFormat);
        if(szPrevDateTime == null || szPrevDateTime == "")
        {
            szPrevDateTime = DateTime.Now.ToString(dateTimeFormat);
        }
        DateTime prevDateTime = DateTime.ParseExact(szPrevDateTime, dateTimeFormat, provider);
        return 24 - prevDateTime.Hour;
    }

    private void CheckDateTimeChange(string szKey_PrevDateTime)
    {
        DateTime presentDateTime = DateTime.Now;
        string szPresentDateTime = presentDateTime.ToString(dateTimeFormat);
        string szPrevDateTime = PlayerPrefs.GetString(szKey_PrevDateTime, null);
        bool bCheckChange = CheckDateTimeChange(szPrevDateTime, szPresentDateTime);
        if (bCheckChange == true)
        {
            bool checkRemoveBall = szKey_PrevDateTime == szKey_PrevDateTime_RemoveBall;
            if(checkRemoveBall == true)
            {
                nSkillCount_RemoveBall = nSkillCount_RemoveBallTotalCount;
                bSkillEnable_RemoveBall = true;
                PlayerPrefs.SetInt(szKey_RemoveBall, nSkillCount_RemoveBall);
            }
            else
            {
                nSkillCount_CreateChuru = nSkillCount_CreateChuruTotalCount;
                bSkillEnable_CreateChuru = true;
                PlayerPrefs.SetInt(szKey_CreateChuru, nSkillCount_CreateChuru);
            }
            PlayerPrefs.SetString(szKey_PrevDateTime, szPresentDateTime);
        }
    }

    private bool CheckDateTimeChange(string szPrevDateTime, string szPresentDateTime)
    {
        if (szPrevDateTime == null || szPrevDateTime == "")
        {
            return true;
        }
        DateTime prevDateTime = DateTime.ParseExact(szPrevDateTime, dateTimeFormat, provider);
        DateTime presentDateTime = DateTime.ParseExact(szPresentDateTime, dateTimeFormat, provider);

        TimeSpan dateDiff = presentDateTime - prevDateTime;
        return dateDiff.Days > 0;
    }
}
