using UnityEngine;
using UnityEngine.UI;
public class Fixed : MonoBehaviour
{
    public static Fixed instance; 

    public static float setWidth;
    public static float setheight;
    private void Start()
    {
        setWidth = Screen.width / 1920.0f;
        setheight = Screen.height / 1080.0f;

        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        Debug.Log(setWidth);
        Debug.Log(setheight);

        var temp = GetComponent<CanvasScaler>().referenceResolution;
        GetComponent<CanvasScaler>().referenceResolution= new Vector2(temp.x * setWidth, temp.y * setheight);
        // SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
}