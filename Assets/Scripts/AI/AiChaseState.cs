using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CV.AI
{
    public class AiChaseState : AiState
    {
        [FoldoutGroup("Settings/Chase"), Range(0f, 5f)]
        public float ChaseSpeed = 1.5f;

        private AiCharacterController _controller;

        public override void Init()
        {
        }

        public override void Exit()
        {
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (stateManager && !_controller) _controller = GetController<AiCharacterController>(stateManager);
            if (_controller.IsInSightRange)
            {
                if (_controller.IsInAttackRange)
                {
                    return stateManager.StateMap.GetState("Attack");
                }
                return this;
            }
            return stateManager.StateMap.GetState("Idle");
        }

        protected override void OnRunning()
        {
            if (_controller && _controller.HostileTarget)
            {
                _controller.Move(_controller.HostileTarget.position, ChaseSpeed);
            }
        }
    }
}
