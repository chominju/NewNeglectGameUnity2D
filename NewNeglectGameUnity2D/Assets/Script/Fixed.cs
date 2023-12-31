using UnityEngine;
using UnityEngine.UI;
public class Fixed : MonoBehaviour
{
    // Panel���� �迭
    public GameObject[] panels;

    private void OnEnable()
    {
        // ���� ���÷����� ���� �ػ�
        int screenWidth = Screen.width;

        // ���� ���÷����� ���� �ػ�
        int screenHeight = Screen.height;

        // ���� ���� �ػ� (1920x1080 ��)
        int referenceWidth = 1920;
        int referenceHeight = 1080;

        // �ػ� ���� ���
        float widthRatio = (float)screenWidth / referenceWidth;
        float heightRatio = (float)screenHeight / referenceHeight;

        // Panel ũ�� ���� ����
        foreach (GameObject panel in panels)
        {
            RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                // �ػ� ������ ���� �г� ũ�� ����
                panelRectTransform.localScale = new Vector3(panelRectTransform.localScale.x * widthRatio, panelRectTransform.localScale.y * heightRatio, panelRectTransform.localScale.z);
            }
        }
    }

    //void Start()
    //{
    //    // ���� ���÷����� ���� �ػ�
    //    int screenWidth = Screen.width;

    //    // ���� ���÷����� ���� �ػ�
    //    int screenHeight = Screen.height;

    //    // ���� ���� �ػ� (1920x1080 ��)
    //    int referenceWidth = 1920;
    //    int referenceHeight = 1080;

    //    // �ػ� ���� ���
    //    float widthRatio = (float)screenWidth / referenceWidth;
    //    float heightRatio = (float)screenHeight / referenceHeight;

    //    // Panel ũ�� ���� ����
    //    foreach (GameObject panel in panels)
    //    {
    //        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
    //        if (panelRectTransform != null)
    //        {
    //            // �ػ� ������ ���� �г� ũ�� ����
    //            panelRectTransform.localScale = new Vector3(panelRectTransform.localScale.x * widthRatio, panelRectTransform.localScale.y * heightRatio, panelRectTransform.localScale.z);
    //        }
    //    }
    //}
}