using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    AudioSource audioSource;                                    // ������Ʈ
    public AudioClip achievementRewardSound;                    // �ӹ� ���� ����
    public AudioClip clickSound;                                // ������ Ŭ�� ����
    public AudioClip menuClickSound;                            // �޴� Ŭ�� ����
    public AudioClip failSound;                                 // ���� ���� ����
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    static public SoundManager GetInstance()
    {
        return instance;
    }

    public void PlayAchievementRewardSound()
    {
        audioSource.PlayOneShot(achievementRewardSound);
    }

    public void PlayMenuClickSound()
    {
        audioSource.PlayOneShot(menuClickSound);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayFailSound()
    {
        audioSource.PlayOneShot(failSound);
    }

    // Update is called once per frame
}
