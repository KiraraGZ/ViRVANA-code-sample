using System;
using Magia.Player;
using Magia.UI;
using UnityEngine;

namespace Magia.Environment
{
    [RequireComponent(typeof(Collider))]
    public class CapturePoint : MonoBehaviour
    {
        public event Action EventCaptureDone;

        public enum State
        {
            INACTIVE = 0,
            CHARGE = 1,
            ACTIVE = 2,
            DISRUPTED = 3
        }

        [SerializeField] private float target;
        [SerializeField] private float chargeDuration;
        [SerializeField] private float disruptLimit;

        private State state;
        private float progress;
        private float charge;
        private float disrupt;

        private UIManager uiManager;

        public void Initialize()
        {
            state = State.INACTIVE;
            progress = 0;
            charge = 0;
            disrupt = 0;

            uiManager = UIManager.Instance;
            uiManager.CreateDestinationIndicator(transform);
        }

        public void Dispose()
        {
            state = State.INACTIVE;
            gameObject.SetActive(false);

            uiManager.RemoveIndicator(transform);
        }

        private void FixedUpdate()
        {
            switch (state)
            {
                case State.CHARGE:
                    {
                        charge += Time.deltaTime;
                        uiManager.DisplayCaptureProgress((int)state, charge, chargeDuration);

                        if (charge < chargeDuration) break;

                        state = State.ACTIVE;
                        break;
                    }
                case State.ACTIVE:
                    {
                        progress += Time.deltaTime;
                        uiManager.DisplayCaptureProgress((int)state, progress, target);

                        if (progress < target) break;

                        state = State.INACTIVE;
                        EventCaptureDone?.Invoke();
                        uiManager.DisplayCaptureProgress((int)state, target, target);
                        Dispose();
                        break;
                    }
                case State.DISRUPTED:
                    {
                        disrupt -= Time.deltaTime;
                        uiManager.DisplayCaptureProgress((int)state, disrupt, disruptLimit);

                        if (disrupt > 0) break;

                        state = State.INACTIVE;
                        progress = 0;
                        uiManager.DisplayCaptureProgress((int)state, 0, 1);
                        break;
                    }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out var _)) return;

            state = State.CHARGE;
            uiManager.DisplayCaptureProgress((int)state, 0, chargeDuration);
            uiManager.ToggleIndicatorVisible(transform, false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out var _)) return;
            if (state != State.ACTIVE) return;

            state = State.DISRUPTED;
            charge = 0;
            disrupt = disruptLimit;
            uiManager.DisplayCaptureProgress((int)state, disrupt, disruptLimit);
            uiManager.ToggleIndicatorVisible(transform, true);
        }
    }
}
