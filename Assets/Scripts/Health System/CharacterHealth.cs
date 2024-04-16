using System;
using System.Collections;
using System.Collections.Generic;
using CV.EventSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace CV.HealthSystem
{
    public class CharacterHealth : MonoBehaviour, IHealthMessageHandle
    {
        private const float ToleranceHealth = 0.05f;

        private float _currentHealth;

        [TitleGroup("Health Settings")]
        [Range(0f, 100f)]
        public float MaxHealth = 100f;

        [FoldoutGroup("Health Settings/Events")]
        public UnityEvent<float> OnHealthChange;
        [FoldoutGroup("Health Settings/Events")]
        public UnityEvent OnDying;

        public bool IsAlive { get; protected set; } = true;

        public float CurrentHealth
        {
            get => _currentHealth;
            protected set
            {
                if (Math.Abs(value - _currentHealth) > ToleranceHealth)
                {
                    _currentHealth = value;
                    if (value <= 0)
                    {
                        IsAlive = false;
                        OnDying?.Invoke();
                    }
                    else
                    {
                        IsAlive = true;
                    }
                    OnHealthChange?.Invoke(value);
                    return;
                }
                _currentHealth = value;
            }
        }

        private void Start()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(float damageVal)
        {
            if (CurrentHealth >= 0)
            {
                CurrentHealth -= damageVal;
            }
        }
    }
}
