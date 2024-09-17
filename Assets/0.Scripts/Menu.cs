using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject mainMenuHolder;       //메인 메뉴 오브젝트
    public GameObject optionsMenuHolder;    //옵션 메뉴 오브젝트

    public Slider[] volumeSliders;           //볼륨 슬라이더
    public Toggle[] resolutionToggles;       //해상도 토글
    public Toggle fullscreenToggle;          //전체 화면 토글
    public int[] screenWidths;               //해상도
    int activeScreenResIndex;                //사용 해상도 인덱스

    void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

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

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        fullscreenToggle.isOn = isFullscreen;
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        // 이 코드는 Unity 에디터에서만 실행됨
        UnityEditor.EditorApplication.isPlaying = false;    //빌드 전 게임 종료
#endif

        Application.Quit();
    }

    /// <summary>
    /// 옵션 메뉴 활성화
    /// </summary>
    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    /// <summary>
    /// 메인 메뉴 활성화
    /// </summary>
    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    /// <summary>
    /// 해상도 변경
    /// </summary>
    /// <param name="i">해상도 인덱스</param>
    public void SetScreenResolution(int i)
    {
        // 선택한 해상도 토글이 켜져 있는지 확인
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;   // 현재 해상도 인덱스를 업데이트
            float aspectRatio = 16 / 9f;

            // 주어진 해상도와 비율로 화면 해상도 설정, 전체 화면이 아님
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 풀스크린 설정
    /// </summary>
    /// <param name="isFullscreen">전체 화면 여부</param>
    public void SetFullscreen(bool isFullscreen)
    {
        // 전체 화면 여부에 따라 해상도 토글의 활성화 여부 설정
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            // 모든 지원되는 해상도 목록을 가져옴
            Resolution[] allResolutions = Screen.resolutions;
            // 가장 큰 해상도를 선택
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            // 선택한 해상도로 전체 화면 설정
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            // 전체 화면을 해제하면 이전에 선택한 해상도로 복원
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 마스터 볼륨 설정
    /// </summary>
    /// <param name="value">마스터 볼륨 값</param>
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    /// <summary>
    /// 음악 볼륨 설정
    /// </summary>
    /// <param name="value">음악 볼륨 값</param>
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    /// <param name="value">효과음 볼륨 값</param>
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

}