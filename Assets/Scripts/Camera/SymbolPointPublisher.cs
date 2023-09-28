using UnityEngine;
using System.Collections;
using ROS2;

public class SymbolPointPublisher : MonoBehaviour
{
    // 所需游戏物体
    public GameObject Symbol0;                                       // 特征点0
    public GameObject Symbol1;                                       // 特征点1
    public GameObject Symbol2;                                       // 特征点2
    public GameObject Symbol3;                                       // 特征点3
    public Camera Cam;                                               // 视觉伺服所用相机
    // ROS 组件
    //private ROS2UnityComponent ros2Unity;
    private ROS2UnityCore ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float64MultiArray> ibvs_pub;
    private std_msgs.msg.Float64MultiArray ros2_symbol_msg;
    // 内部状态
    private double[] symbol_point_pos = new double[8];                // 4个特征点的u v 数组
    private double[] last_symbol_point_pos = new double[8];           // 上一次可以看见 4个特征点时的u v 数组
    private Vector3 symbol0_pos;                                      // 特征点0 u v 位置
    private Vector3 symbol1_pos;                                      // 特征点1 u v 位置
    private Vector3 symbol2_pos;                                      // 特征点2 u v 位置
    private Vector3 symbol3_pos;                                      // 特征点3 u v 位置
    private bool fov_state;

    void Start()
    {
        ros2Unity = new ROS2UnityCore();
        ros2_symbol_msg = new std_msgs.msg.Float64MultiArray();
        symbol_point_pos[0] = last_symbol_point_pos[0] = 0.0;
        symbol_point_pos[1] = last_symbol_point_pos[1] = 0.0;
        symbol_point_pos[2] = last_symbol_point_pos[2] = 0.0;
        symbol_point_pos[3] = last_symbol_point_pos[3] = 0.0;
        symbol_point_pos[4] = last_symbol_point_pos[4] = 0.0;
        symbol_point_pos[5] = last_symbol_point_pos[5] = 0.0;
        symbol_point_pos[6] = last_symbol_point_pos[6] = 0.0;
        symbol_point_pos[7] = last_symbol_point_pos[7] = 0.0;
    }

    void Update()
    {
        // 判断特征点是否在视野范围内
        fov_state = (fov_detect(Symbol0) && fov_detect(Symbol1) && fov_detect(Symbol2) && fov_detect(Symbol3));
        if (fov_state)
        {
            symbol0_pos = Cam.WorldToScreenPoint(Symbol0.transform.position);
            symbol1_pos = Cam.WorldToScreenPoint(Symbol1.transform.position);
            symbol2_pos = Cam.WorldToScreenPoint(Symbol2.transform.position);
            symbol3_pos = Cam.WorldToScreenPoint(Symbol3.transform.position);
            symbol_point_pos[0] = symbol0_pos.x - 320;
            symbol_point_pos[1] =- symbol0_pos.y + 240;
            symbol_point_pos[2] = symbol1_pos.x - 320;
            symbol_point_pos[3] = -symbol1_pos.y + 240;
            symbol_point_pos[4] = symbol2_pos.x - 320;
            symbol_point_pos[5] = -symbol2_pos.y + 240;
            symbol_point_pos[6] = symbol3_pos.x - 320;
            symbol_point_pos[7] = -symbol3_pos.y + 240;
        }
        last_symbol_point_pos = symbol_point_pos;
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("ROS2UnityIBVSPubNode");
                ibvs_pub = ros2Node.CreatePublisher<std_msgs.msg.Float64MultiArray>("current_pixel");
            }
            ros2_symbol_msg.Data = symbol_point_pos;
            ibvs_pub.Publish(ros2_symbol_msg);
        }
        
    }

    //判断物体是否在视野范围内
    private bool fov_detect(GameObject Object)
    {

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Cam);
        if (GeometryUtility.TestPlanesAABB(planes, Object.GetComponent<Collider>().bounds))
            return true;
        else
            return false;
    }
}
