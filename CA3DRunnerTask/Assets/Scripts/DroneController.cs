using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] Transform PlayerTransform;
    Vector3 newPosX;
    Vector3 newPosY;
    [SerializeField] float offsetY = 1f;
    [SerializeField] float offsetZ = -1.5f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        newPosX = transform.localPosition;
        newPosX.x = Mathf.Lerp(transform.localPosition.x, PlayerTransform.localPosition.x, Time.deltaTime * 3f);
        // transform.localPosition = newPosX;

        newPosY = transform.localPosition;
        newPosY.y = Mathf.Lerp(transform.localPosition.y, PlayerTransform.localPosition.y + offsetY, Time.deltaTime * 3f);
        // transform.localPosition = newPosY;

        transform.localPosition = new Vector3(newPosX.x,newPosY.y, offsetZ);
    }
}
