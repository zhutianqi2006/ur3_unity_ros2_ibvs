/****************************************************************************
MIT License
Copyright(c) 2021 Roman Parak
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: ur_data_processing.cs
****************************************************************************/

// System
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
// Unity 
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
// ROS2
using ROS2;

public class ur_data_processing : MonoBehaviour
{
    public static class GlobalVariables_Main_Control
    {
        public static bool connect, disconnect;
    }
    public static class UR_Stream_Data
    {
        // IP Port Number and IP Address
        public static string ip_address;
        // Comunication Speed (ms)
        public static int time_step;
        // Joint Space:
        //  Orientation {J1 .. J6} (rad)
        public static double[] J_Orientation = new double[6];
        // Cartesian Space:
        //  Position {X, Y, Z} (mm)
        public static double[] C_Position = new double[3];
        //  Orientation {Euler Angles} (rad):
        public static double[] C_Orientation = new double[3];
        // Class thread information (is alive or not)
        public static bool is_alive = false;
        // joint vel receive from ros2(rad/s)
        public static double[] desire_ros_joint_vel = new double[6];
        // joint vel receive from jog(rad/s)
        public static double[] desire_jog_joint_vel = new double[6];
    }
    // ROS2 
    private ROS2UnityCore ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<std_msgs.msg.Float64MultiArray> vel_sub;
    private IPublisher<sensor_msgs.msg.JointState> joint_pub;
    private std_msgs.msg.Float64MultiArray ros2_vel_msg;
    private sensor_msgs.msg.JointState ros2_joint_msg;
    void update_ros_joint_pos()
    {
        if (GlobalVariables_Main_Control.connect == true)
        {
            for (int i = 0; i < UR_Stream_Data.J_Orientation.Length; i++)
            {
                UR_Stream_Data.J_Orientation[i] += UR_Stream_Data.desire_ros_joint_vel[i] * Time.deltaTime;
            }
        }
    }
    void update_jog_joint_pos()
    {
        if (GlobalVariables_Main_Control.disconnect == true)
        {
            for (int i = 0; i < UR_Stream_Data.J_Orientation.Length; i++)
            {
                UR_Stream_Data.J_Orientation[i] += UR_Stream_Data.desire_jog_joint_vel[i] * Time.deltaTime;
            }
        }
    }
    void ros_vel_callback(std_msgs.msg.Float64MultiArray msg)
    {
        if (GlobalVariables_Main_Control.connect)
        {
            UR_Stream_Data.desire_ros_joint_vel[0] = msg.Data[0];
            UR_Stream_Data.desire_ros_joint_vel[1] = msg.Data[1];
            UR_Stream_Data.desire_ros_joint_vel[2] = msg.Data[2];
            UR_Stream_Data.desire_ros_joint_vel[3] = msg.Data[3];
            UR_Stream_Data.desire_ros_joint_vel[4] = msg.Data[4];
            UR_Stream_Data.desire_ros_joint_vel[5] = msg.Data[5];
        }
        else
        {
            UR_Stream_Data.desire_ros_joint_vel[0] = 0.0f;
            UR_Stream_Data.desire_ros_joint_vel[1] = 0.0f;
            UR_Stream_Data.desire_ros_joint_vel[2] = 0.0f;
            UR_Stream_Data.desire_ros_joint_vel[3] = 0.0f;
            UR_Stream_Data.desire_ros_joint_vel[4] = 0.0f;
            UR_Stream_Data.desire_ros_joint_vel[5] = 0.0f;
        }
    }

    void Start()
    {
        ros2Unity = new ROS2UnityCore();
        ros2_vel_msg = new std_msgs.msg.Float64MultiArray();
        ros2_joint_msg = new sensor_msgs.msg.JointState();
        List<string> jointNamesList = new List<string> { "joint1", "joint2", "joint3", "joint4", "joint5", "joint6" };
        ros2_joint_msg.Name = jointNamesList.ToArray();
        UR_Stream_Data.J_Orientation[0] = 1.57;
        UR_Stream_Data.J_Orientation[1] = -1.57;
        UR_Stream_Data.J_Orientation[2] = 1.57;
        UR_Stream_Data.J_Orientation[3] = -1.57;
        UR_Stream_Data.J_Orientation[4] = -1.57;
        UR_Stream_Data.J_Orientation[5] = 0;
        GlobalVariables_Main_Control.disconnect = true;
        GlobalVariables_Main_Control.connect = false;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        update_ros_joint_pos();
        update_jog_joint_pos();
        if (ros2Node == null && ros2Unity.Ok())
        {
            ros2Node = ros2Unity.CreateNode("ROS2UnityURControl");
            QualityOfServiceProfile qualityOfServiceProfile = new QualityOfServiceProfile();
            qualityOfServiceProfile.SetHistory(HistoryPolicy.QOS_POLICY_HISTORY_KEEP_LAST, 0);
            vel_sub = ros2Node.CreateSubscription<std_msgs.msg.Float64MultiArray>("simulink_mpc_result", ros_vel_callback, qualityOfServiceProfile);
            joint_pub = ros2Node.CreatePublisher<sensor_msgs.msg.JointState>("ur3_sim_joint_state");
        }
        List<double> jointPositionList = new List<double>
        {
             UR_Stream_Data.J_Orientation[0],
             UR_Stream_Data.J_Orientation[1],
             UR_Stream_Data.J_Orientation[2],
             UR_Stream_Data.J_Orientation[3],
             UR_Stream_Data.J_Orientation[4],
             UR_Stream_Data.J_Orientation[5]
        };
        ros2_joint_msg.Position = jointPositionList.ToArray();
        joint_pub.Publish(ros2_joint_msg);
    }


}
