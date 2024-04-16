using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CV.Combat
{
    public class SimpleShooter : Shooter
    {
        [FoldoutGroup("Settings/Advance"), Range(0f, 10f)]
        public float BulletSpeed = 1f;

        private Bullet _bulletComp;

        protected override void FireBullet()
        {
            if (ShootOrigin)
            {
                var bullet = Instantiate(Bullet, ShootOrigin.position, ShootOrigin.rotation);
                _bulletComp = bullet.GetComponent<Bullet>();
                _bulletComp?.StartFly(BulletSpeed);
            }
        }
    }
}
