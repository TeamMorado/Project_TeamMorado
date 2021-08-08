using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;

public class AdmobManager : MonoSingleton<AdmobManager>, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOsGameId;
    [SerializeField] bool _testMode = true;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;
    private const string removeAds = "remove_ads";

    [SerializeField] InterstitialAdExample m_InterstitialAd;
    [SerializeField] RewardedAdsButton m_RewardedAdsButton;
    [SerializeField] BannerAdExample m_BannerAdExample;

    private Action m_frontAdsAction;
    private Action<int> m_rewardAdsAction;

    protected override void Setup()
    {
        bCheckRemoveADS = PlayerPrefs.GetInt(removeAds, 0) == 1;
        InitializeAds();
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, _enablePerPlacementMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        if(bCheckRemoveADS == false)
        {
            m_InterstitialAd.LoadAd();
            m_BannerAdExample.LoadBanner();
        }
        m_RewardedAdsButton.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void SetFrontAdsAction(Action frontAdsAction)
    {
        m_frontAdsAction = frontAdsAction;
        m_InterstitialAd.actionForFrontAds = frontAdsAction;
    }

    public void SetRewardAdsAction(Action<int> rewardAdsAction)
    {
        m_rewardAdsAction = rewardAdsAction;
        m_RewardedAdsButton.actionForRewardAds = rewardAdsAction;
    }


    public bool bCheckRemoveADS;

    public void ShowFrontAd()
    {
        if(bCheckRemoveADS == true || Application.isEditor)
        {
            m_frontAdsAction.Invoke();
            return;
        }
        m_InterstitialAd.ShowAd();
    }

    public void ToggleBannerAd(bool bValue)
    {
        if(bValue == false)
        {
            m_BannerAdExample.HideBannerAd();
        }
    }

    public void ShowRewardAd(int nIndex)
    {
        if (Application.isEditor)
        {
            m_rewardAdsAction?.Invoke(nIndex);
            return;
        }
        m_RewardedAdsButton.SetSelectIndex(nIndex);
        m_RewardedAdsButton.ShowAd();
    }

    /*
    void Start()
    {
        //LoadRewardAd();
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
    //*/
}
