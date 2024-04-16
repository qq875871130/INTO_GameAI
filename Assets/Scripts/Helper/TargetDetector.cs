using System;
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
            out Vector3 directionToTarget)
        {
            directionToTarget = Vector3.zero;
            target = null;
            if (Physics.CheckSphere(origin.position, radius, targetMask)) //Detect by sphere
            {
                //Update target and direction
                var targets = Physics.OverlapSphere(origin.position, radius, targetMask);
                target = TargetSelector(origin, targets, TargetSelectorStrategy.MinDistance).transform;
                directionToTarget = (target.position - origin.position).normalized;
                //Calculate if target in fov
                if (Vector3.Angle(origin.forward, directionToTarget) <= (angleOfView / 2))
                {
                    return true;
                }
            }
            return false;
        }

        public static Collider TargetSelector(Transform origin, Collider[] targets, TargetSelectorStrategy selectorStrategy)
        {
            const float Tolerance = 0.001f;
            Collider nearestTarget = null;
            if (selectorStrategy == TargetSelectorStrategy.MinDistance)
            {
                float minDistance = -1;
                foreach (var target in targets)
                {
                    var curDistance = Vector3.Distance(target.transform.position, origin.position);
                    minDistance = (Math.Abs(minDistance - (-1)) < Tolerance) ? curDistance : MathF.Min(minDistance, curDistance);
                    if (Math.Abs(minDistance - curDistance) < Tolerance) //This iterator target is nearest the origin.
                    {
                        nearestTarget = target;
                    }
                }
            }

            return nearestTarget;
        }

        public enum TargetSelectorStrategy
        {
            MinDistance
        }
    }
}
