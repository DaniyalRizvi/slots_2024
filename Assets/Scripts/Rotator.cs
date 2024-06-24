using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotateAmount;
    [SerializeField] private GameObject rotatee; // to be rotated

    private void Update()
    {
        rotatee.transform.Rotate(new Vector3(0f, 0f, 1f) * rotateAmount * Time.deltaTime);
    }
}
