using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CV.AI
{
    public class AiSleepState : AiState
    {
        [FoldoutGroup("Settings/Sleep"), MinValue(0f)]
        public float SleepTime = 30f;

        private bool _isWakeup = false;

        public override void Init()
        {
        }

        public override void Exit()
        {
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (_isWakeup && stateManager)
            {
                return stateManager.StateMap.GetState("Idle");
            }
            return this;
        }

        protected override void OnRunning()
        {
            if (SleepTime <= 0)
            {
                _isWakeup = true;
            }
            else
            {
                _isWakeup = false;
                SleepTime -= Time.deltaTime;
            }
        }
    }
}
