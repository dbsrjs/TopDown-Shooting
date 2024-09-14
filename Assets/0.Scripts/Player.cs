using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent�� ����ϴ� ��ũ��Ʈ�� GameObject�� �߰��ϸ� �ʿ��� component�� GameObject�� �ڵ����� �߰��˴ϴ�. 
[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
    Camera camera;
    PlayerController controller;
    GunController gunController;

    public Crosshairs crosshairs;    //������


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
        //�̵� ����
        Vector3 moveInout = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInout.normalized * speed;
        controller.Move(moveVelocity); 

        //�ٶ󺸴� ��
        Ray ray = camera.ScreenPointToRay(Input.mousePosition); //���콺 �������� ��ġ�� ��ȯ����
        Plane groundPlane = new Plane(Vector3.up,Vector3.up * gunController.GunHeight);
        float rayDistance;
        
        if(groundPlane.Raycast(ray, out rayDistance))   //out : ������ ������ ������.
        {
            //ray�� ground�� �ε�������.
            Vector3 point = ray.GetPoint(rayDistance);  //���콺 �������� ��ġ�� ������
            //Debug.DrawLine(ray.origin, point, Color.red);   //ray�� ���������� ���������� ������ ������ ǥ������.
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)  //�������� �ʿ� �ʹ� �����̰��� ���� ȸ���ϱ� ������ ���� �Ÿ� �̻��� ���� �۵��ϵ��� ��.
                gunController.Aim(point);
        }

        #region ���� ����
        
        //��Ƽ踦 ���
        if (Input.GetMouseButton(0))
            gunController.OnTriggerHold();

        //��Ƽ踦 ����
        if (Input.GetMouseButtonUp(0))
            gunController.OnTriggerRelease();

        //����
        if(Input.GetKeyDown(KeyCode.R))
            gunController.Reload();
        #endregion
    }
}