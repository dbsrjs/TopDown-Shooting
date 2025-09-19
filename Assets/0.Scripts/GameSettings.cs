using UnityEngine;

/// <summary>
/// 게임 전역 설정을 관리하는 싱글톤 클래스
/// </summary>
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    [Header("Map Settings")]
    public bool useRandomMaps = false;  // 랜덤 맵 사용 여부

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 랜덤 맵 모드 설정
    /// </summary>
    public void SetRandomMapMode(bool useRandom)
    {
        useRandomMaps = useRandom;
    }

    /// <summary>
    /// 랜덤 맵 모드 여부 반환
    /// </summary>
    public bool IsRandomMapMode()
    {
        return useRandomMaps;
    }
}