using System.Collections;
using System.Collections.Generic;
using CV.Combat;
using CV.HealthSystem;
using CV.Helper;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace CV.AI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AiStateManager))]
    public class AiCharacterController : MonoBehaviour
    {
        #region Field and properties


        [TitleGroup("Basic Parameters")]
        public Transform Eyes;
        [PropertySpace]

        [SerializeField, FoldoutGroup("Basic Parameters/Detection"), PropertyRange(0f, 360f)]
        public float DegreeFov = 120f;
        [SerializeField, FoldoutGroup("Basic Parameters/Detection"), MinValue(0f)]
        public float RadiusFov = 10f;
        [SerializeField, FoldoutGroup("Basic Parameters/Detection"), MinValue(0f)]
        public float RadiusAttack = 2f;

        [FoldoutGroup("Basic Parameters/Detection")]
        public LayerMask HostileMask;
        [FoldoutGroup("Basic Parameters/Detection")]
        public LayerMask ObstacleMask;

        [FoldoutGroup("Basic Parameters/Health")]
        public CharacterHealth Health;
        [FoldoutGroup("Basic Parameters/Combat")]
        public Shooter Shooter;

        [TitleGroup("Debug")]
        [SerializeField, FoldoutGroup("Debug/Gizmos")]
        public bool IsGizmosEnable = true;
        [SerializeField, FoldoutGroup("Debug/Gizmos")]
        public bool IsGizmosInPlayMode = false;

        [HideInInspector] public Vector3 DirectionToHostile;

        [SerializeField, FoldoutGroup("Debug/Watchdog"), ReadOnly]
        public Transform HostileTarget;

        [ShowInInspector, FoldoutGroup("Debug/Watchdog"), ReadOnly]
        public bool IsInSightRange { get; protected set; } = false;
        [ShowInInspector, FoldoutGroup("Debug/Watchdog"), ReadOnly]
        public bool IsInAttackRange { get; protected set; } = false;
        [ShowInInspector, FoldoutGroup("Debug/Watchdog"), ReadOnly]
        public bool IsLookHostile { get; protected set; } = false;
        [ShowInInspector, FoldoutGroup("Debug/Watchdog"), ReadOnly]
        public bool IsAlive { get; protected set; } = true;

        protected NavMeshAgent NavMeshAgent;
        #endregion

        #region Unity lifespan

        protected virtual void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Health = Health ? Health : GetComponent<CharacterHealth>();
            Shooter = Shooter ? Shooter : GetComponent<Shooter>();
        }

        protected virtual void Update()
        {
            Transform eyes = Eyes ? Eyes : transform;
            //Update detector
            IsInSightRange = TargetDetector.CheckTargetRange(
                                 eyes,
                                 RadiusFov,
                                 DegreeFov,
                                 HostileMask,
                                 out HostileTarget,
                                 out DirectionToHostile)
                             &&
                             !Physics.Raycast(Eyes.position, DirectionToHostile,
                                 (HostileTarget.position - Eyes.position).magnitude, ObstacleMask);

            IsInAttackRange = IsInSightRange && TargetDetector.CheckTargetRange(
                eyes,
                RadiusAttack,
                DegreeFov,
                HostileMask,
                out _,
                out _);
        }

        #endregion

        public virtual void Move(Vector3 destination, float speed)
        {
            if (!NavMeshAgent) return;

            NavMeshAgent.isStopped = false;
            NavMeshAgent.speed = speed;
            NavMeshAgent.SetDestination(destination);
        }

        public virtual void Stop()
        {
            if (!NavMeshAgent) return;

            NavMeshAgent.isStopped = true;
            NavMeshAgent.speed = 0;
        }

        public virtual void LookTarget()
        {
            if (HostileTarget)
            {
                gameObject.transform.LookAt(HostileTarget, Vector3.up);
                IsLookHostile = true;
            }
        }

        public virtual void StopLookTarget()
        {
            IsLookHostile = false;
        }

#if UNITY_EDITOR
        protected virtual void DebugGizmos()
        {
            Transform eye = Eyes ? Eyes : transform;
            Vector3 leftRayDir = Quaternion.AngleAxis(-DegreeFov / 2, Vector3.up) * gameObject.transform.forward;
            Vector3 rightRayDir = Quaternion.AngleAxis(DegreeFov / 2, Vector3.up) * gameObject.transform.forward;

            Gizmos.color = Color.blue;
            Handles.color = new Color(0.09f, 0.78f, 0.98f, 0.2f);
            Handles.DrawSolidArc(eye.position, Vector3.up, leftRayDir, DegreeFov, RadiusFov);
            Gizmos.DrawWireSphere(eye.position, RadiusFov);
            Gizmos.DrawLine(eye.position, eye.position + leftRayDir * RadiusFov);
            Gizmos.DrawLine(eye.position, eye.position + rightRayDir * RadiusFov);

            Handles.color = new Color(1, 0, 0, 0.2f);
            Gizmos.color = Color.red;
            Handles.DrawSolidArc(eye.position + Vector3.up * 0.2f, Vector3.up, leftRayDir, DegreeFov, RadiusAttack);
            Gizmos.DrawWireSphere(eye.position, RadiusAttack);
        }
        private void OnDrawGizmos()
        {
            if (IsGizmosInPlayMode && IsGizmosEnable) DebugGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            if (!IsGizmosInPlayMode && IsGizmosEnable) DebugGizmos();
        }
#endif
    }
}
