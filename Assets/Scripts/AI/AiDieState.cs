using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CV.AI
{
    public class AiDieState : AiState
    {
        [FoldoutGroup("Settings/Dead"), Range(0f, 10f)]
        public float TimeToDestroy = 2f;

        private AiCharacterController _controller;

        public override void Init()
        {
            _controller?.Stop();
        }

        public override void Exit()
        {
        }

        public override AiState RunCurrentState(AiStateManager stateManager)
        {
            if (!_controller) _controller = GetController<AiCharacterController>(stateManager);
            return this;
        }

        protected override void OnRunning()
        {
            if (!_controller || !_controller.gameObject) return;
            Destroy(_controller.gameObject, TimeToDestroy);
        }
    }
}
