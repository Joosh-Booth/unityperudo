using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public bool moving = false;

    public float start;
    public float end;
    float speed = 1;

    public bool direction = true;
    public float timeTakenDuringLerp = 1f;
    public float timeTakenDuringLerpAng = 5f;
    public float _timeStartedLerping;
    private void Start()
    {
        start = transform.position.y;
        end = transform.position.y+.4f;
        _timeStartedLerping = Time.time;
    }
    void LateUpdate()
    {
        if (moving) {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            if (direction == true )
            {
                Debug.Log("Direction == true");
                transform.position = Vector3.Lerp(new Vector3(transform.position.x,start,transform.position.z), new Vector3(transform.position.x, end, transform.position.z), percentageComplete);
                if (percentageComplete>=1.0f) 
                {
                    direction = false;
                    _timeStartedLerping = Time.time;
                }
            }
            else if (direction == false)
            {
                Debug.Log("Direction == false");
                transform.position = Vector3.Lerp(new Vector3(transform.position.x, end, transform.position.z), new Vector3(transform.position.x, start, transform.position.z), percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    direction = true;
                    _timeStartedLerping = Time.time;
                }
            }

        }
    }
}
