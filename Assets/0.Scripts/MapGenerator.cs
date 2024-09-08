using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0f, 1f)] //범위를 0 ~ 1로 지정
    public float outlinePercent;    //테두리 퍼센트

    List<Coord> alltileCoords;  //타일 좌표에 대한 list
    Queue<Coord> shuffledTileCords; //셔플 완료된 타일들.

    public int seed = 10;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        alltileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                alltileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCords = new Queue<Coord>(Utility.ShuffleArray(alltileCoords.ToArray(), seed));

        string holderName = "Generated Map";
        if (transform.Find(holderName))
            DestroyImmediate(transform.Find(holderName).gameObject);

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //타일 생성 반복문
        //좌측 상단 -> 우측 하단
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePos = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90)) as Transform;    //as : 형변환
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        int obstacleCount = 15; //생성할 장애물의 개수

        for(int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
        }
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    /// <summary>
    /// shuffledTileCords(Queue)로부터 다음 아이템을 얻어 랜덤 좌표를 반환해줌.
    /// </summary>
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCords.Dequeue();    //셔플된 타일 좌표 큐의 첫 아이템을 가짐.
        shuffledTileCords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}
