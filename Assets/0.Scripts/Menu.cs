using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenu;     //메인 메뉴 오브젝트
    public GameObject optionMenu;   //옵션 메뉴 오브젝트

    public Slider[] volumeSliders;       //볼륨 슬라이더
    public Toggle[] resolutionToggles;   //해상도 토글
    public Toggle fullscreenToggle;
    public int[] screenWidths;           //해상도

    int activeScreenResIndex;   //사용 해상도

    private void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") ==1 ) ? true : false;

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
		volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
		volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }

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
    /// 옵션 화면 활성화
    /// </summary>
    public void OptionMenu()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    /// <summary>
    /// 메인 화면 활성화
    /// </summary>
    public void MainMenu()
    {
        mainMenu.SetActive(true);
        optionMenu.SetActive(false);
    }

    /// <summary>
    /// 해상도 변경
    /// </summary>
    public void SetScrrenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false); //해상도 변경, 전체 화면이 아님
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 풀스크린 설정
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            activeScreenResIndex = i;     // 활성화된 해상도 인덱스를 현재 인덱스로 설정
            resolutionToggles[i].interactable = !isFullscreen;   // 전체 화면이 아니면 토글을 활성화, 전체 화면이면 비활성화
        }

        if(isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;       //모든 해상도 갖고 오기
            Resolution maxResolution = allResolutions[allResolutions.Length - 1]; //가장 큰 해상도 선정
            Screen.SetResolution(maxResolution.width, maxResolution.height, true); //가장 큰 해상도로 전체 화면 설정
        }
        else
        {
            SetScrrenResolution(activeScreenResIndex);  //이전에 활성화 했던 해상도로 변경
        }

        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
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
    /// 효과 볼륨 설정
    /// </summary>
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}