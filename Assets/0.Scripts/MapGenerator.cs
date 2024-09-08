using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;        //타일
    public Transform obstaclePrefab;    //장애물
    public Transform navmeshFloor; 
    public Transform navmeshMaskPrefab; //나브메쉬

    public Vector2 mapSize;
    public Vector2 maxMapSize;  //최대 맵 크기

    [Range(0f, 1f)] //범위를 0 ~ 1로 지정
    public float outlinePercent;    //테두리 퍼센트

    [Range(0f, 1f)] //범위를 0 ~ 1로 지정
    public float obstaclePercent;   //장애물 생성 개수

    public float tileSize;

    List<Coord> alltileCoords;      //타일 좌표에 대한 list
    Queue<Coord> shuffledTileCords; //셔플 완료된 타일들.

    public int seed = 10;
    Coord mapCenter;

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
        mapCenter = new Coord((int)mapSize.x/2, (int)mapSize.y/2 );

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
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent); //생성할 장애물의 개수
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if(randomCoord != mapCenter && MaplsFullyAccessible(obstacleMap, currentObstacleCount)) //장애물이 생성할 위치가 맵의 중앙이 아니고, 맵 전체에 접근이 가능하면.
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (mapSize.x + maxMapSize.x) / 4 * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (mapSize.x + maxMapSize.x) / 4 * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y)/2) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    /// <summary>
    /// 맵 전체가 접근 가능한가?
    /// currentObstacleCount : 여태 얼마나 장애물이 생성 됐는지.
    /// </summary>
    /// <returns></returns>
    bool MaplsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);   //Queue에 mapCenter을 넣음.
        mapFlags[mapCenter.x, mapCenter.y] = true;  //중앙 타일이 비어있다는걸 아니까, 이곳을 true로 체크 해둠.

        int accessibleTileCount = 1;    //접근 가능한 타일 개수

        //Flood Fill Algorithm
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();   //큐의 첫번째 아이템을 갖고 옴, 그것을 큐에서 삭제함.

            for (int x = -1; x <= 1; x++) 
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;    //이웃타일 X
                    int neighbourY = tile.y + y;    //이웃타일 Y
                    if(x == 0 || y == 0)    //대각선은 체크하지 않음.
                    {
                        //좌표가 obstacleMap 내부에 있는지 확인.
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            //이 타일을 이전에 체크하지 않았다면 && 이것이 장애물이 아닌가?
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;    //이 타일을 체크했으니 true로 해줌.    
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTile = (int)(mapSize.x * mapSize.y - currentObstacleCount); //장애물이 아닌 타일의 개수.
        return targetAccessibleTile == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * tileSize;
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

        public static bool operator ==(Coord c1,  Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
}
