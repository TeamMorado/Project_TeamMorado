using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSettingCtrl : MonoBehaviour
{
    //비율 기준 해상도 - 이 해상도 기준으로 비율을 고정
    private Vector4 basicWindow = new Vector4(720f, 1280f, 720f / 1280f, 1280f / 720f);
    //계산된 해상도
    private Vector4 nowWindow = new Vector4(Screen.width, Screen.height, Screen.width / (float)Screen.height, Screen.height / (float)Screen.width);

    [SerializeField]
    private Vector2 windowSize;//원하는 비율
    [SerializeField]
    private Vector2 basicSize;//해당 기기의 해상도

    private void Awake()
    {
        if (windowSize == Vector2.zero)
        {
            windowSize = new Vector2(16f, 9);
        }
        if (basicSize == Vector2.zero)
        {
            basicSize = new Vector2(Screen.width, Screen.height);
        }
        basicWindow = new Vector4(basicSize.x, basicSize.y, basicSize.x / basicSize.y, basicSize.y / basicSize.x);
    }

    [Header("Resize Setting")]
    [SerializeField]
    private bool isStartAuto = true;

    [SerializeField]
    [Range(0, 1)]
    private float match = 1f;//가로세로 기준 비율

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        if (isStartAuto)
        {
            viewportReSize();
        }
    }
    private void viewportReSize()
    {
        Rect newRect = new Rect();
        float width = (1f - (basicWindow.w / nowWindow.w)) * (1f - match);
        newRect.x = width * 0.5f;
        newRect.width = 1f - width;


        float height = (1f - (basicWindow.z / nowWindow.z)) * match;
        newRect.y = height * 0.5f;
        newRect.height = 1f - height;

        //Debug.Log($"{width} - {height}");
        cam.rect = newRect;

    }
#if UNITY_EDITOR
    /*
    [SerializeField]
    private bool isReset;//디버그용 인스펙터에서 체크하면 비율 계산후 적용
    private void OnGUI()
    {
        if (nowWindow.x != windowSize.x || nowWindow.y != windowSize.y || isReset)
        {
            isReset = false;
            nowWindow = new Vector4(windowSize.x, windowSize.y, windowSize.x / (float)windowSize.y, windowSize.y / (float)windowSize.x);
            basicWindow = new Vector4(basicSize.x, basicSize.y, basicSize.x / basicSize.y, basicSize.y / basicSize.x);
            //nowSize = new Vector4(Screen.width, Screen.height, Screen.width / (float)Screen.height, Screen.height / (float)Screen.width);
            viewportReSize();

        }
    }
    */
#endif
}