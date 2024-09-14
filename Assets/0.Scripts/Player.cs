using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent를 사용하는 스크립트를 GameObject에 추가하면 필요한 component가 GameObject에 자동으로 추가됩니다. 
[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
    Camera camera;
    PlayerController controller;
    GunController gunController;

    public Crosshairs crosshairs;    //조준점


    public float speed = 5;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        camera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    protected override void Start()
    {
        base.Start();
    }

    void OnNewWave(int waveNumber)
    {
        health = startHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void Update()
    {
        //이동 로직
        Vector3 moveInout = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInout.normalized * speed;
        controller.Move(moveVelocity); 

        //바라보는 곳
        Ray ray = camera.ScreenPointToRay(Input.mousePosition); //마우스 포인터의 위치를 반환해줌
        Plane groundPlane = new Plane(Vector3.up,Vector3.up * gunController.GunHeight);
        float rayDistance;
        
        if(groundPlane.Raycast(ray, out rayDistance))   //out : 변수를 참조로 전달함.
        {
            //ray가 ground와 부딪힌거임.
            Vector3 point = ray.GetPoint(rayDistance);  //마우스 포인터의 위치를 보여줌
            //Debug.DrawLine(ray.origin, point, Color.red);   //ray의 시작점부터 끝점까지를 빨간색 선으로 표시해줌.
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)  //조준점이 초에 너무 가까이가면 총이 회전하기 때문에 일정 거리 이상일 때만 작동하도록 함.
                gunController.Aim(point);
        }

        #region 무기 조작
        
        //방아쇠를 당김
        if (Input.GetMouseButton(0))
            gunController.OnTriggerHold();

        //방아쇠를 놓음
        if (Input.GetMouseButtonUp(0))
            gunController.OnTriggerRelease();

        //장전
        if(Input.GetKeyDown(KeyCode.R))
            gunController.Reload();
        #endregion
    }
}