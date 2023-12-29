using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    AudioSource audioSource;                                    // 컴포넌트
    public AudioClip achievementRewardSound;                    // 임무 보상 사운드
    public AudioClip clickSound;                                // 아이템 클릭 사운드
    public AudioClip menuClickSound;                            // 메뉴 클릭 사운드
    public AudioClip failSound;                                 // 실패 관련 사운드
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
