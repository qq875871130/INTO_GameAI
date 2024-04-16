using System.Collections;
using System.Collections.Generic;
using CV.EventSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CV.Combat
{
    public class SimpleBullet : Bullet
    {
        private const float ToleranceStoppedSpeed = 0.01f;

        private float _flyStartTime = 0f;
        private float _currentSpeed = 0f;
        private bool _isDestroy = false;

        public override void StartFly(float speed)
        {
            _isDestroy = false;
            _flyStartTime = Time.time;
            _currentSpeed = speed;
        }

        public void Update()
        {
            if (_isDestroy) { return; }
            if (Time.time - _flyStartTime > LifeTime)   //Out of lift time
            {
                _isDestroy = true;
            }
            else if (_currentSpeed > ToleranceStoppedSpeed)
            {
                var nextPos = transform.position + gameObject.transform.forward * _currentSpeed * Time.deltaTime;

                //Damage hit target
                RaycastHit hit;
                if (Physics.Raycast(
                        transform.position,
                        nextPos - transform.position,
                        out hit,
                        (nextPos - transform.position).magnitude)
                    &&
                    hit.transform.gameObject.layer != gameObject.layer) //Exclude game object in same layer (bullet itself)
                {
                    var target = hit.transform.gameObject;
                    ExecuteEvents.ExecuteHierarchy<IHealthMessageHandle>(target, null, (x, y) => x.TakeDamage(Damage));
                    _isDestroy = true;
                }

                transform.position = nextPos;
            }
        }

        public void LateUpdate()
        {
            if (_isDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
