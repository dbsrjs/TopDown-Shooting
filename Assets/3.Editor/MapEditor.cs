using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    //�������� �ʰ� ��ȭ�� �� �� ����.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;       //target = MapGenerator  (CustomEditor Ű����� ������ ������Ʈ�� target���� ������ �� �ְ� ��)

        map.GenerateMap();
    }
}
