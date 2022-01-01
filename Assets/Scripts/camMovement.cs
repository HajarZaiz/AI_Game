using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform cam;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float xOffset;
    [SerializeField] private float zOffset;
    private Vector3 follow = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        cam.position = new Vector3(player.position.x + xOffset, cam.position.y, player.position.z - zOffset);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        follow.x = player.position.x + xOffset;
        follow.y = cam.position.y;
        follow.z = player.position.z - zOffset;
        cam.position = Vector3.Lerp(cam.position, follow, moveSpeed * Time.deltaTime);
    }
}
