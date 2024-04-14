using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using CV.Marker;
using Unity.VisualScripting;
using UnityEngine;

namespace CV.AI
{
    public class AiPatrolState : AiState
    {
        [FoldoutGroup("Settings/Patrol")]
        public float DefaultCheckPointRange = 1f;
        [FoldoutGroup("Settings/Patrol")]
        public float PatrolSpeed = 1f;
        [FoldoutGroup("Settings/Patrol")]
        public Transform[] PatrolPoints;

        private int _currentPointIndex = -1;
        private bool _isInPosition;

        protected AiCharacterController Controller;

        public override void Init()
        {
            if (_currentPointIndex < 0) _currentPointIndex = 0;
            //Loop patrol points
            else if (_isInPosition && PatrolPoints.Length > 0)
            {
                _currentPointIndex = (_currentPointIndex + 1) % PatrolPoints.Length;
            }
        }

        public override void Exit()
        {
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (stateManager && !Controller) Controller = GetController<AiCharacterController>(stateManager); //获取控制器
            if (Controller.IsInSightRange)
            {
                if (Controller.IsInAttackRange)
                {
                    return stateManager.StateMap.GetState("Attack");
                }
                else return stateManager.StateMap.GetState("Chase");
            }
            if (_isInPosition || PatrolPoints.Length == 0 || PatrolPoints == null)
            {
                //Reach point or point lost, idle
                return stateManager.StateMap.GetState("Idle");
            }
            return this;
        }

        protected override void OnRunning()
        {
            if (Controller)
            {
                Transform point = PatrolPoints[_currentPointIndex];
                float distanceToPoint = Vector3.Distance(Controller.gameObject.transform.position, point.position);
                if (distanceToPoint > GetRange(point))
                {
                    _isInPosition = false;
                    Controller.Move(point.position, PatrolSpeed);
                }
                //到达巡逻点
                else
                {
                    _isInPosition = true;
                    Controller.Stop();
                }
            }
        }
        private float GetRange(Transform point)
        {
            MarkPoint patrolPoint = point.gameObject.GetComponent<MarkPoint>();
            return patrolPoint ? patrolPoint.Range : DefaultCheckPointRange;
        }
    }
}
