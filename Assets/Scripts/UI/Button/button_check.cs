// ------------------------------------------------------------------------------------------------------------------------ //
// ----------------------------------------------------- LIBRARIES -------------------------------------------------------- //
// ------------------------------------------------------------------------------------------------------------------------ //

// -------------------- System -------------------- //
using System.Text;
// -------------------- Unity -------------------- //
using UnityEngine.EventSystems;
using UnityEngine;

public class button_check: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float speed;//rad/s
    // -------------------- Int -------------------- //
    public int index;

    // -------------------- Button -> Pressed -------------------- //
    public void OnPointerDown(PointerEventData eventData)
    {
        ur_data_processing.UR_Stream_Data.desire_jog_joint_vel[index] = speed;
    }

    // -------------------- Button -> Un-Pressed -------------------- //
    public void OnPointerUp(PointerEventData eventData)
    {
        ur_data_processing.UR_Stream_Data.desire_jog_joint_vel[index] = 0.0f;
    }
}
