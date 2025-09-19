using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Settings UI")]
    public GameObject settingsMenuHolder;       //설정 메뉴 홀더
    public Slider[] volumeSliders;              //볼륨 슬라이더
    public Toggle[] resolutionToggles;          //해상도 토글
    public Toggle fullscreenToggle;             //전체 화면 토글
    public int[] screenWidths;                  //해상도 배열

    private bool isPaused = false;
    private int activeScreenResIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 설정 값 초기화
        InitializeSettings();

        // 초기에는 설정 메뉴 비활성화
        settingsMenuHolder.SetActive(false);
    }


    /// <summary>
    /// 설정 값 초기화
    /// </summary>
    void InitializeSettings()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        // 해상도 토글 설정
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }

        if (isFullscreen)
        {
            for (int i = 0; i < resolutionToggles.Length; i++)
            {
                resolutionToggles[i].interactable = !isFullscreen;
            }
        }

        // 볼륨 슬라이더 설정
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        fullscreenToggle.isOn = isFullscreen;
    }

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenSettings();
        }
    }

    /// <summary>
    /// 설정 메뉴 열기 (게임 일시정지)
    /// </summary>
    public void OpenSettings()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f;                    //게임 시간 정지
            Cursor.visible = true;                  //마우스 커서 표시
            settingsMenuHolder.SetActive(true);     //설정 메뉴 활성화
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;                    //게임 시간 복원
            Cursor.visible = false;                 //마우스 커서 숨김
            settingsMenuHolder.SetActive(false);    //설정 메뉴 비활성화
        }
    }
       

    /// <summary>
    /// 메인 메뉴로 돌아가기
    /// </summary>
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;                    //게임 시간 복원
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;                    //게임 시간 복원
        SceneManager.LoadScene("Game");
    }


    /// <summary>
    /// 해상도 설정
    /// </summary>
    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;

            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 풀스크린 설정
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 마스터 볼륨 설정
    /// </summary>
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    /// <summary>
    /// 음악 볼륨 설정
    /// </summary>
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}