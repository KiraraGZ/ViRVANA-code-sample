using Magia.GameLogic;
using Magia.Player;
using Magia.UI;
using UnityEngine;

namespace Magia.Enemy.Puffer
{
    public class VoidPuffer : BasePuffer
    {
        private const string INVISIBLE_WARN_KEY = "Void_Puffer_Invisible";

        [Header("Void Mechanics")]
        [SerializeField] private float invisibleTime = 0.2f;

        protected DialogueVisualizer dialogueVisualizer;

        public override void Initialize(PlayerController _player)
        {
            base.Initialize(_player);
        }

        public override void Dispose()
        {
            base.Dispose();

            dialogueVisualizer = null;
        }

        protected override void UpdateFollowState()
        {
            var speedMultiplier = 1f;
            var distance = Vector3.Distance(Player.transform.position, transform.position);
            var isDodge = true;

            if (Physics.Raycast(transform.position, Player.transform.position - transform.position, out var hit, 20f))
            {
                isDodge = hit.collider.GetComponent<PlayerController>() == false;
            }

            if (distance < revolveRange)
            {
                speedMultiplier = 1 + Mathf.Pow(1f - distance / revolveRange, 2f);
            }

            movement.PhysicsUpdate(speedMultiplier, isDodge: isDodge);

            if (combat.CheckSkillToCast())
            {
                EnterHoldState();
            }
        }

        protected override bool ChangeState(PufferState state)
        {
            if (state == PufferState.STRAFE)
            {
                animator.SetTargetClip(0, invisibleTime);
                animator.CloseMouth();
                UIManager.Instance.ToggleIndicatorVisible(transform, false);
                dialogueVisualizer.PlayWarnDialogue(INVISIBLE_WARN_KEY, 1);
            }

            if (currentState == PufferState.STRAFE)
            {
                animator.SetTargetClip(1, invisibleTime);
                UIManager.Instance.ToggleIndicatorVisible(transform, true);
            }

            return base.ChangeState(state);
        }

        protected override void OnReady()
        {
            base.OnReady();
            dialogueVisualizer ??= GameplayController.Instance.DialogueVisualizer;
        }
    }
}