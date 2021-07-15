using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    public UIManager uiManager;
    private string removeAds = "remove_ads";
    public Text textValue;

    private void Start()
    {
        bool bKorean = Application.systemLanguage == SystemLanguage.Korean;
        textValue.text = bKorean ?
            "앱을 삭제 할 경우\n결제 정보가 사라집니다.\n결제를 진행하시려면\n아래 버튼을 눌러주세요." :
            "If you uninstall the app, you will lose all your game data including payment details.";
    }

    public void OnPurchaseComplete(Product product)
    {
        if(product.definition.id == removeAds)
        {
            PlayerPrefs.SetInt(removeAds, 1);
           
            AdmobManager.Instance.bCheckRemoveADS = true;
            AdmobManager.Instance.ToggleBannerAd(false);
            StartCoroutine(SetUI_PurchaseSuccess());
        }
    }

    private IEnumerator SetUI_PurchaseSuccess()
    {
        yield return new WaitForEndOfFrame();
        uiManager.SetPurchaseButton();
        uiManager.OnButtonPress((int)UIManager.eIndex.Button_RemoveADSClose - (int)UIManager.eIndex.Button_Option);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of " + product.definition.id + " failed due to " + reason);
    }
}
