using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace CV.AI
{
    public abstract class AiState : SerializedMonoBehaviour
    {
        #region Field and Properties

        private bool _isRunning = false;

        [TitleGroup("Settings")]
        [BoxGroup("Settings/Initiation")]
        public bool InitOnAwake = false;

        [SerializeField, FoldoutGroup("Settings/Initiation/Events")]
        public UnityEvent OnStateRun;
        [SerializeField, FoldoutGroup("Settings/Initiation/Events")]
        public UnityEvent OnStateStop;

        public bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                if (value != _isRunning)
                {
                    if (value)
                    {
                        Init();
                    }
                    else
                    {
                        Exit();
                    }
                }
                _isRunning = value;
            }
        }

        #endregion

        #region Unity Lifespan

        protected virtual void Awake()
        {
            if (InitOnAwake)
            {
                Init();
            }
        }

        protected virtual void Update()
        {
            if (IsRunning)
            {
                OnRunning();
            }
        }

        #endregion

        public void Run()
        {
            IsRunning = true;
            OnStateRun?.Invoke();
        }

        public void Stop()
        {
            IsRunning = false;
            OnStateStop?.Invoke();
        }

        public abstract void Init();
        public abstract void Exit();
        public abstract AiState RunCurrentState(AiStateManager stateManager);

        /// <summary>
        /// Operation in Update()
        /// </summary>
        protected abstract void OnRunning();

        public virtual T GetController<T>(AiStateManager stateManager) where T : AiCharacterController
        {
            T controller = stateManager.gameObject.GetComponent<T>();
            return controller;
        }
    }
}
