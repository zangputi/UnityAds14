using UnityEngine;
using System.Collections;

public class RefuseME : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return false;
    }
}