/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    //�������� �ʰ� ��ȭ�� �� �� ����.
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;       //target = MapGenerator  (CustomEditor Ű����� ������ ������Ʈ�� target���� ������ �� �ְ� ��)

        if (DrawDefaultInspector()) //�ν����Ϳ����� ���� ���� ���� �� true�� ��ȯ. 
            map.GenerateMap();

        if (GUILayout.Button("Generate Map"))    //Inspector�� 'Generate Map'�̶�� ��ư�� ������
            map.GenerateMap();
    }
}*/