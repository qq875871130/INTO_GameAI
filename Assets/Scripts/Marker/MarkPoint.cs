using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CV.Marker
{
    public class MarkPoint : MonoBehaviour
    {
        public string GizmosIconName = "Marker.png";
        public float Range = 3;
        public Color RangeColor = new Color(0.72f, 1, 0.6f, 0.7f);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, GizmosIconName, true);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = RangeColor;
            Gizmos.DrawSphere(transform.position, Range);
        }
#endif
    }
}
