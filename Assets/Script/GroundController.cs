using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField] GameObject LeftWall;
    [SerializeField] GameObject RightWall;
    private void Start()
    {
        LeftWall.transform.position = new Vector3(LeftWall.transform.position.x, LeftWall.transform.position.y, transform.localScale.z * 5+0.25f);
        RightWall.transform.position = new Vector3(transform.localScale.x * 5 + 0.25f, RightWall.transform.position.y);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //LeftWall.transform.position = new Vector3(LeftWall.transform.position.x, LeftWall.transform.position.y, transform.localScale.z * 5);
        //RightWall.transform.position = new Vector3(transform.localScale.x * 5, RightWall.transform.position.y);
    }
#endif
}
