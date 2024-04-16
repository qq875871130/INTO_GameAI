using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CV.EventSystem
{
    public interface IHealthMessageHandle : IEventSystemHandler
    {
        public void TakeDamage(float damage);
    }
}
