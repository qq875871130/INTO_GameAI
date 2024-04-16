using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CV.AI
{
    [Serializable]
    public struct StateInstance
    {
        public MonoScript SourceScript;
        [SerializeField]
        public AiState State;
    }

    [Serializable]
    public class AiStateMap : Dictionary<string, StateInstance>
    {
        public AiState GetState(string key)
        {
            if (TryGetValue(key, out var instance))
            {
                return instance.State;
            }
            return null;
        }

        public bool SetState(string key, AiState state)
        {
            if (TryGetValue(key, out var instance))
            {
                instance.State = state;
                this[key] = instance;
                return true;
            }
            return false;
        }

        public bool ContainsSourceScript(string key)
        {
            if (TryGetValue(key, out var instance))
            {
                return instance.SourceScript != null;
            }
            return false;
        }

        public MonoScript GetSourceScript(string key)
        {
            if (TryGetValue(key, out var instance))
            {
                return instance.SourceScript;
            }
            return null;
        }
    }

    [Serializable]
    public class AiStateManager : SerializedMonoBehaviour
    {
        [HideInInspector]
        public const string StateMachineName = "#StateMachine";

        #region Field and properties

        [TitleGroup("Configuration")]
        [FoldoutGroup("Configuration/State", Expanded = true)]
        public AiState CurrentState;
        [FoldoutGroup("Configuration/State", Expanded = true)]
        public AiCharacterController CharacterController;

        [OdinSerialize, NonSerialized, FoldoutGroup("Configuration/State/StateMap", GroupName = "Map"), PropertyOrder(1), DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "State")]
        public AiStateMap StateMap;

        [HideInInspector]
        public UnityEvent<AiStateManager> OnStateInit;
        [HideInInspector]
        public UnityEvent<AiStateManager> OnStateUpdate;
        [HideInInspector]
        public UnityEvent<AiStateManager> OnStateDisable;

        #endregion

        #region Editor Operation

        [ButtonGroup("Configuration/State/StateMap/Buttons"), LabelText("Update"), PropertyOrder(0)]
        private void UpdateStateMachineByMap()
        {
            var stateMachine = GetStateMachineObject();
            List<string> stateNamesInSm = new List<string>();
            foreach (var inChild in stateMachine.GetComponentsInChildren<Transform>())
            {
                if (!inChild.gameObject.Equals(stateMachine))    //Foreach children and cache names
                {
                    stateNamesInSm.Add(inChild.gameObject.name);
                }
            }

            IList<string> stateNamesInMap = new List<string>(StateMap.Keys);
            if (stateNamesInSm.Count == 0)  //Push all state of map to state machine
            {
                foreach (var name in stateNamesInMap)
                {
                    AddStateInStateMachine(stateMachine, name);
                }
            }
            else    //Add new state by map
            {
                foreach (var name in stateNamesInMap)
                {
                    if (!stateNamesInSm.Contains(name))
                    {
                        stateNamesInSm.Add(name);
                        AddStateInStateMachine(stateMachine, name);
                    }
                }
            }

            for (int i = stateNamesInSm.Count - 1; i >= 0; i--)  //Remove redundant states
            {
                var stateNameInSm = stateNamesInSm[i];
                if (!StateMap.ContainsKey(stateNameInSm))
                {
                    stateNamesInSm.RemoveAt(i);
                    RemoveStateInStateMachine(stateMachine, stateNameInSm);
                }
            }
        }

        [ButtonGroup("Configuration/State/StateMap/Buttons"), LabelText("Reset"), PropertyOrder(0)]
        private void ResetStateMachineByMap()
        {
            var stateMachine = GetStateMachineObject();
            ClearStateInStateMachine(stateMachine);
            UpdateStateMachineByMap();
        }

        [ButtonGroup("Configuration/State/StateMap/Buttons"), LabelText("Clear"), PropertyOrder(0)]
        private void ClearStateMap()
        {
            StateMap?.Clear();
        }

        private GameObject GetStateMachineObject()
        {
            foreach (Transform item in gameObject.transform)
            {
                var child = item.gameObject;
                if (child.name.Equals(StateMachineName))
                {
                    return child;
                }
            }
            //Create New StateMachine
            var stateMachine = new GameObject(StateMachineName);
            stateMachine.transform.SetParent(gameObject.transform);
            return stateMachine;
        }

        private void ClearStateInStateMachine(GameObject stateMachineObj)
        {
            var parent = stateMachineObj.transform;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }

        private void RemoveStateInStateMachine(GameObject stateMachineObj, string stateName)
        {
            if (stateMachineObj == null)
                return;

            Transform matchedChild = stateMachineObj.transform.Find(stateName);
            if (matchedChild)
            {
                DestroyImmediate(matchedChild.gameObject);
            }
        }

        private void AddStateInStateMachine(GameObject stateMachineObj, string stateName)
        {
            if (stateMachineObj == null || !StateMap.ContainsKey(stateName) || !StateMap.ContainsSourceScript(stateName))
                return;

            var stateObj = new GameObject(stateName);
            stateObj.transform.SetParent(stateMachineObj.transform);
            var stateComp = stateObj.AddComponent(StateMap.GetSourceScript(stateName).GetClass()) as AiState;
            StateMap.SetState(stateName, stateComp);
        }

        #endregion

        #region Unity lifespan

        private void Awake()
        {
            OnStateInit?.Invoke(this);
            CharacterController = GetComponent<AiCharacterController>();
        }

        private void Update()
        {
            OnStateUpdate?.Invoke(this);
            RunStateMachine();
        }

        #endregion

        private void RunStateMachine()
        {
            var nextState = CurrentState?.RunCurrentState(this);
            SwitchState(nextState);
        }

        private void SwitchState(AiState state)
        {
            if (state == null) return;
            if (!state.Equals(CurrentState))
            {
                CurrentState?.Stop();
            }
            CurrentState = state;
            CurrentState.Run();
        }

        public void JumpToState(string stateKey)
        {
            if (StateMap != null)
            {
                SwitchState(StateMap.GetState(stateKey));
            }
        }
    }
}
