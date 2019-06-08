using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public static Dictionary<string, AxisHandler> inputs = new Dictionary<string, AxisHandler>();

    // Start is called before the first frame update
    void Start()
    {
        inputs.Add("Submit", new AxisHandler("Submit"));
        inputs.Add("Horizontal", new AxisHandler("Horizontal"));
        inputs.Add("Vertical", new AxisHandler("Vertical"));
        inputs.Add("Cancel", new AxisHandler("Cancel"));
        inputs.Add("Diagnol", new AxisHandler("Diagnol"));
        inputs.Add("Menu", new AxisHandler("Menu"));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (AxisHandler axis in inputs.Values) {
            if (Input.GetAxisRaw(axis.axis) != 0) {
                if(axis.downLastFrame == 0 && axis.down == 0) {
                    axis.downLastFrame = Input.GetAxisRaw(axis.axis);
                }
                axis.down = Input.GetAxisRaw(axis.axis);
            } else {
                if (axis.down != 0) {
                    axis.upLastFrame = axis.down;
                } else {
                    axis.upLastFrame = 0;
                }
                axis.down = 0;
                axis.downLastFrame = 0;
            }
        }
    }

    public class AxisHandler {
        public float downLastFrame = 0;
        public float down = 0;
        public float upLastFrame = 0;

        public string axis;

        public AxisHandler(string a) {
            axis = a;
        }
    }

}
