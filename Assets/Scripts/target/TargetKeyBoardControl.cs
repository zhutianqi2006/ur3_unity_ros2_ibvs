using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetKeyBoardControl : MonoBehaviour
{
    public GameObject target;
    private float move_speed;
    private float v_x,v_y,v_z,v_ry;
    // Start is called before the first frame update
    void Start()
    {
        move_speed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        var object_pos = target.transform.position;
        var object_rot = target.transform.eulerAngles;
        v_x = 0;
        v_y = 0;
        v_z = 0;
        v_ry = 0;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            v_z += move_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            v_z += -move_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            v_x += -move_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            v_x += move_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            v_ry += 360*move_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            v_ry -= 360*move_speed * Time.deltaTime;
        }
        object_pos.z += v_z;
        object_pos.x += v_x;
        object_rot.y += v_ry;
        target.transform.position = object_pos;
        target.transform.eulerAngles = object_rot;
    }
    public void speed_change(float new_speed)
    {
        move_speed = new_speed;
    }
}
