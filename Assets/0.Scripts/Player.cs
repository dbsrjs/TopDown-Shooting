using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent�� ����ϴ� ��ũ��Ʈ�� GameObject�� �߰��ϸ� �ʿ��� component�� GameObject�� �ڵ����� �߰��˴ϴ�. 
[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
    PlayerController controller;
    GunController gunController;
    Camera camera;

    public float speed = 5;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        camera = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        //�̵� ����
        Vector3 moveInout = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInout.normalized * speed;
        controller.Move(moveVelocity); 

        //�ٶ󺸴� ��
        Ray ray = camera.ScreenPointToRay(Input.mousePosition); //���콺 �������� ��ġ�� ��ȯ����
        Plane groundPlane = new Plane(Vector3.up, Vector3Int.zero);
        float rayDistance;
        
        if(groundPlane.Raycast(ray, out rayDistance))   //out : ������ ������ ������.
        {
            //ray�� ground�� �ε�������.
            Vector3 point = ray.GetPoint(rayDistance);  //���콺 �������� ��ġ�� ������
            Debug.DrawLine(ray.origin, point, Color.red);   //ray�� ���������� ���������� ������ ������ ǥ������.
            controller.LookAt(point);
        }

        //���� ����
        if(Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}