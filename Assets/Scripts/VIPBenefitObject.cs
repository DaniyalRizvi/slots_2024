using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/VIPBenefitObject")]
public class VIPBenefitObject : ScriptableObject
{
    public int statusLevel;
    public string statusName;
    public int statusPointsNeeded;
    public float multiplier;
}

