using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CV.Helper
{
    public class TargetDetector
    {
        public static bool CheckTargetRange(
            Transform origin,
            float radius,
            float angleOfView,
            LayerMask targetMask,
            out Transform target,
            out Vector3 directionToTarget,
            int targetIndex = 0)
        {
            directionToTarget = Vector3.zero;
            target = null;
            if (Physics.CheckSphere(origin.position, radius, targetMask)) //Detect by sphere
            {
                //Update target and direction
                target = Physics.OverlapSphere(origin.position, radius, targetMask)[targetIndex].transform;
                directionToTarget = (target.position - origin.position).normalized;
                //Calculate if target in fov
                if (Vector3.Angle(origin.forward, directionToTarget) <= (angleOfView / 2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
