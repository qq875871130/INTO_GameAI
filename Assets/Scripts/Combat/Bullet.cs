using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CV.Combat
{
    public abstract class Bullet : MonoBehaviour
    {
        [TitleGroup("Settings")]
        [FoldoutGroup("Settings/Basic")]
        public float Damage = 20f;
        [FoldoutGroup("Settings/Basic")]
        public float LifeTime = 2.0f;

        public abstract void StartFly(float speed);
    }
}
