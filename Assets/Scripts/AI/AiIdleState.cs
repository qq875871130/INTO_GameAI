using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace CV.AI
{
    public class AiIdleState : AiState
    {
        [FoldoutGroup("Settings/Idle")] public bool AlwaysIdle = false;
        [FoldoutGroup("Settings/Idle"), MinValue(0f)] public float IdleTime = 3f;

        private float _timeRemains;

        protected AiCharacterController Controller;

        public override void Init()
        {
            Controller?.Stop();
            _timeRemains = IdleTime;
        }

        public override void Exit()
        {
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (!Controller)
            {
                Controller = GetController<AiCharacterController>(stateManager);
            }

            if (_timeRemains > 0 || AlwaysIdle)
            {
                if (Controller.IsInSightRange)
                {
                    if (Controller.IsInAttackRange)
                    {
                        return stateManager.StateMap.GetState("Attack");
                    }
                    else
                    {
                        return stateManager.StateMap.GetState("Chase");
                    }
                }

                return this;
            }

            return stateManager.StateMap.GetState("Patrol");
        }

        protected override void OnRunning()
        {
            _timeRemains -= Time.deltaTime;
        }
    }
}
