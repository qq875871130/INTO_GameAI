using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace CV.HealthSystem
{
    public class CharacterHealth : MonoBehaviour
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
                        OnDying?.Invoke();
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

        public void Damage(float damageVal)
        {
            if (CurrentHealth >= 0)
            {
                CurrentHealth -= damageVal;
            }
        }
    }
}
