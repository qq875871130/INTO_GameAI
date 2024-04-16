using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CV.AI
{
    public class AiAttackState : AiState
    {
        [FoldoutGroup("Settings/Attack"), Range(0f, 5f)]
        public float ShootTimeInterval = 1f;

        private AiCharacterController _controller;
        private float _attackTime;

        public override void Init()
        {
            _controller?.Stop();
        }

        public override void Exit()
        {
            _controller?.StopLookTarget();
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (stateManager && !_controller) _controller = GetController<AiCharacterController>(stateManager); //Get controller
            if (_controller.IsInSightRange)
            {
                if (!_controller.IsInAttackRange)
                {
                    return stateManager.StateMap.GetState("Chase"); //Chase when out of attack range
                }
                return this;    //在范围内开始攻击
            }
            return stateManager.StateMap.GetState("Idle");  //Idle when out of FOV
        }

        protected override void OnRunning()
        {
            if (_controller)
            {
                _controller.Stop();
                _controller.LookTarget();
                if (Time.time - _attackTime > ShootTimeInterval)    //Auto shoot target
                {
                    _attackTime = Time.time;
                    _controller.Shooter?.Fire();
                }
            }
        }
    }
}
