using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CV.Combat
{
    public abstract class Shooter : MonoBehaviour
    {
        [TitleGroup("Settings")]
        [FoldoutGroup("Settings/Basic")]
        public GameObject Bullet;
        [FoldoutGroup("Settings/Basic")]
        public Transform ShootOrigin;
        [FoldoutGroup("Settings/Basic"), Range(0f, 5f)]
        public float FireRate = 0.5f;

        private float _lastFireTime = 0f;

        public void Fire()
        {
            if (Bullet && (Time.time - _lastFireTime > FireRate))
            {
                _lastFireTime = Time.time;
                FireBullet();
            }
        }

        protected abstract void FireBullet();
    }
}
