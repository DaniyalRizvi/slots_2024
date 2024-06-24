using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIPBenefits : MonoBehaviour
{
    [NonReorderable]
    public VIPBenefitObject[] vipBenefits;

    public static VIPBenefits Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public int GetStatusPointsOfStatus(int statusLevel)
    {
        if (statusLevel == 0)
            statusLevel = 1;
        return vipBenefits[statusLevel].statusPointsNeeded;
    }

    public int GetStatusLevel(int statusPoints)
    {
        for (int i = 0; i < vipBenefits.Length; i++) 
        {
            if (statusPoints <= vipBenefits[i].statusPointsNeeded)
                return i;
        }
        return vipBenefits.Length - 1; // status Points maxed out
    }

    public float GetStatusMultiplier(int statusLevel)
    {
        return vipBenefits[statusLevel].multiplier;
    }

    public string GetVIPStatusName(int statusLevel)
    {
        if(statusLevel <= 1)
            return vipBenefits[statusLevel].statusName;
        else
            return vipBenefits[statusLevel-1].statusName;

    }
}
