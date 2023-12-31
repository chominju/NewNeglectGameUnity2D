using UnityEngine;
using UnityEngine.UI;
public class Fixed : MonoBehaviour
{
    // Panel들의 배열
    public GameObject[] panels;

    private void OnEnable()
    {
        // 현재 디스플레이의 가로 해상도
        int screenWidth = Screen.width;

        // 현재 디스플레이의 세로 해상도
        int screenHeight = Screen.height;

        // 예상 기준 해상도 (1920x1080 등)
        int referenceWidth = 1920;
        int referenceHeight = 1080;

        // 해상도 비율 계산
        float widthRatio = (float)screenWidth / referenceWidth;
        float heightRatio = (float)screenHeight / referenceHeight;

        // Panel 크기 동적 조정
        foreach (GameObject panel in panels)
        {
            RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                // 해상도 비율에 따라 패널 크기 조정
                panelRectTransform.localScale = new Vector3(panelRectTransform.localScale.x * widthRatio, panelRectTransform.localScale.y * heightRatio, panelRectTransform.localScale.z);
            }
        }
    }

    //void Start()
    //{
    //    // 현재 디스플레이의 가로 해상도
    //    int screenWidth = Screen.width;

    //    // 현재 디스플레이의 세로 해상도
    //    int screenHeight = Screen.height;

    //    // 예상 기준 해상도 (1920x1080 등)
    //    int referenceWidth = 1920;
    //    int referenceHeight = 1080;

    //    // 해상도 비율 계산
    //    float widthRatio = (float)screenWidth / referenceWidth;
    //    float heightRatio = (float)screenHeight / referenceHeight;

    //    // Panel 크기 동적 조정
    //    foreach (GameObject panel in panels)
    //    {
    //        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
    //        if (panelRectTransform != null)
    //        {
    //            // 해상도 비율에 따라 패널 크기 조정
    //            panelRectTransform.localScale = new Vector3(panelRectTransform.localScale.x * widthRatio, panelRectTransform.localScale.y * heightRatio, panelRectTransform.localScale.z);
    //        }
    //    }
    //}
}