using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent를 사용하는 스크립트를 GameObject에 추가하면 필요한 component가 GameObject에 자동으로 추가됩니다. 
[RequireComponent (typeof (PlayerController))]
public class Player : MonoBehaviour
{
    PlayerController controller;
    Camera camera;

    public float speed = 5;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        camera = Camera.main;
    }

    void Update()
    {
        Vector3 moveInout = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInout.normalized * speed;
        controller.Move(moveVelocity);

        Ray ray = camera.ScreenPointToRay(Input.mousePosition); //마우스 포인터의 위치를 반환해줌
        Plane groundPlane = new Plane(Vector3.up, Vector3Int.zero);
        float rayDistance;
        
        if(groundPlane.Raycast(ray, out rayDistance))   //out : 변수를 참조로 전달함.
        {
            //ray가 ground와 부딪힌거임.
            Vector3 point = ray.GetPoint(rayDistance);  //마우스 포인터의 위치를 보여줌
            Debug.DrawLine(ray.origin, point, Color.red);   //ray의 시작점부터 끝점까지를 빨간색 선으로 표시해줌.
            controller.LookAt(point);
        }
    }
}