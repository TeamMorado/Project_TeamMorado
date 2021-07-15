using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : MonoSingleton<AdmobManager>
{
    public bool isTestMode;
    public Action actionForFrontAds;
    private string removeAds = "remove_ads";
    public bool bCheckRemoveADS;

    private int m_nIndex = 0;
    public Action<int> actionForRewardAds;

    void Start()
    {
        LoadRewardAd();
        //PlayerPrefs.SetInt(removeAds,0);
        bCheckRemoveADS = PlayerPrefs.GetInt(removeAds, 0) == 1;
        if(bCheckRemoveADS)
        {
            return;
        }
        LoadBannerAd();
        LoadFrontAd();
    }

    AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().AddTestDevice("8340F700EA323EDC").AddTestDevice("3324672E78CBEF23").AddTestDevice("B3BE573BA0D8AC22").Build();
    }



    #region 배너 광고
    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "ca-app-pub-1052508683573671/9768908490";
    BannerView bannerAd;


    void LoadBannerAd()
    {
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID,
            AdSize.SmartBanner, AdPosition.Bottom);
        bannerAd.LoadAd(GetAdRequest());
        //ToggleBannerAd(false);
    }

    public void ToggleBannerAd(bool b)
    {
        if (b) bannerAd.Show();
        else bannerAd.Hide();
    }
    #endregion



    #region 전면 광고
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = "ca-app-pub-1052508683573671/8072683445";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdClosed += (sender, e) =>
        {
            actionForFrontAds?.Invoke();
            //LogText.text = "전면광고 성공";
        };
        frontAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        actionForFrontAds?.Invoke();
    }

    public void ShowFrontAd()
    {
        if(bCheckRemoveADS == true)
        {
            actionForFrontAds?.Invoke();
            return;
        }

        if(frontAd.IsLoaded() == false)
        {
            return;
        }
        frontAd.Show();
        LoadFrontAd();
    }
    #endregion



    #region 리워드 광고
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "";
    RewardedAd rewardAd;


    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
            actionForRewardAds?.Invoke(m_nIndex);
            //LogText.text = "리워드 광고 성공";
        };
    }

    public void ShowRewardAd(int p_nIndex)
    {
        if(rewardAd.IsLoaded() == false)
        {
            return;
        }
        m_nIndex = p_nIndex;
        rewardAd.Show();
        LoadRewardAd();
    }
    #endregion
}
