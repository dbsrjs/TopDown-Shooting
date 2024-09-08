using System.Collections;

public static class Utility
{
    /// <summary>
    /// 셔플된 배열을 반환해줌.
    /// </summary>
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);   //가짜 랜덤 숫자 생성기

        for (int i = 0; i < array.Length-1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
