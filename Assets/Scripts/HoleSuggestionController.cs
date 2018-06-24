using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSuggestionController : MonoBehaviour
{
    public float RotateSpeed;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(15, 30, 45), this.RotateSpeed);
    }
}
