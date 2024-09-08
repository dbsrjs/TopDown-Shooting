using System.Collections;

public static class Utility
{
    /// <summary>
    /// ���õ� �迭�� ��ȯ����.
    /// </summary>
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);   //��¥ ���� ���� ������

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
