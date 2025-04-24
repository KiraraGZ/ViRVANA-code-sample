using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magia.Enemy.Angel
{
    public class AngelTrippelGateSkill : AngelGateSkill
    {
        protected bool isCasting = false;

        public float GateInterval = 0.5f;
        private float nextGate;

        protected int gateCount;

        public override void PhysicsUpdate()
        {
            if (isCasting) HandleCasting();
            if (Time.time < nextCastTime) return;

            isCasting = true;
        }

        public void HandleCasting()
        {
            if (Time.time < nextGate) return;

            Fire();
            gateCount += 1;

            if (gateCount == 2)
            {
                nextCastTime = Time.time + (projectileAmount > 1 ? data.Interval : data.Cooldown);
                projectileAmount = projectileAmount > 1 ? projectileAmount - 1 : data.Amount;
            }
            else
            {
                nextGate = Time.time + GateInterval;
            }
        }
    }
}