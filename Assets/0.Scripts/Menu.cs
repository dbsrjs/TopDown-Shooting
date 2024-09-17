using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenu;     //���� �޴� ������Ʈ
    public GameObject optionMenu;   //�ɼ� �޴� ������Ʈ

    public Slider[] volumeSliders;       //���� �����̴�
    public Toggle[] resolutionToggles;   //�ػ� ���
    public Toggle fullscreenToggle;
    public int[] screenWidths;           //�ػ�

    int activeScreenResIndex;   //��� �ػ�

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
    /// ���� ����
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void Quit()
    {
        #if UNITY_EDITOR
        // �� �ڵ�� Unity �����Ϳ����� �����
        UnityEditor.EditorApplication.isPlaying = false;    //���� �� ���� ����
        #endif
        Application.Quit();
    }

    /// <summary>
    /// �ɼ� ȭ�� Ȱ��ȭ
    /// </summary>
    public void OptionMenu()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    /// <summary>
    /// ���� ȭ�� Ȱ��ȭ
    /// </summary>
    public void MainMenu()
    {
        mainMenu.SetActive(true);
        optionMenu.SetActive(false);
    }

    /// <summary>
    /// �ػ� ����
    /// </summary>
    public void SetScrrenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false); //�ػ� ����, ��ü ȭ���� �ƴ�
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Ǯ��ũ�� ����
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            activeScreenResIndex = i;     // Ȱ��ȭ�� �ػ� �ε����� ���� �ε����� ����
            resolutionToggles[i].interactable = !isFullscreen;   // ��ü ȭ���� �ƴϸ� ����� Ȱ��ȭ, ��ü ȭ���̸� ��Ȱ��ȭ
        }

        if(isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;       //��� �ػ� ���� ����
            Resolution maxResolution = allResolutions[allResolutions.Length - 1]; //���� ū �ػ� ����
            Screen.SetResolution(maxResolution.width, maxResolution.height, true); //���� ū �ػ󵵷� ��ü ȭ�� ����
        }
        else
        {
            SetScrrenResolution(activeScreenResIndex);  //������ Ȱ��ȭ �ߴ� �ػ󵵷� ����
        }

        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    /// <summary>
    /// ȿ�� ���� ����
    /// </summary>
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}