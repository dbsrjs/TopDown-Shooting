/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    //실행하지 않고도 변화를 볼 수 있음.
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;       //target = MapGenerator  (CustomEditor 키워드로 선언한 오브젝트는 target으로 접근할 수 있게 됨)

        if (DrawDefaultInspector()) //인스펙터에서의 값이 갱신 됐을 때 true를 반환. 
            map.GenerateMap();

        if (GUILayout.Button("Generate Map"))    //Inspector에 'Generate Map'이라는 버튼을 생성함
            map.GenerateMap();
    }
}*/