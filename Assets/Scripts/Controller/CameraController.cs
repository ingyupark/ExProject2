using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private float moveSpeed; // 카메라가 따라갈 속도
    private Vector3 targetPosition; // 대상의 현재 위치
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player").gameObject;
        //moveSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //if (player.gameObject != null)
        //{
        //    // this는 카메라를 의미 (z값은 카메라값을 그대로 유지)
        //    targetPosition.Set(player.transform.position.x, player.transform.position.y, this.transform.position.z);

        //    // vectorA -> B까지 T의 속도로 이동
        //    this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        //}
    }
}
