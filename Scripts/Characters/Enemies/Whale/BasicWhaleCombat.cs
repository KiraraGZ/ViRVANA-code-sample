using Magia.Enemy.Skills;
using Magia.GameLogic;
using UnityEngine;

namespace Magia.Enemy.Whale
{
    public class BasicWhaleCombat : WhaleCombat
    {
        private BombAttackSkill bombAttackSkill;
        private OrbitFireSkill orbitFireSkill;
        private SpiritFireSkill spiritFireSkill;
        private HomingFireSkill homingFireSkill;
        private ShockwaveSkill shockwaveSkill;
        private GateHomingFireSkill revolveFireSkill;

        [SerializeField] private BombAttackSkillSO bombAttack;
        [SerializeField] private OrbitFireSkillSO orbitPhaseOne;
        [SerializeField] private OrbitFireSkillSO orbitPhaseTwo;
        [SerializeField] private SpiritFireSkillSO spiritFire;
        [SerializeField] private HomingFireSkillSO homingFirePhaseOne;
        [SerializeField] private HomingFireSkillSO homingFirePhaseTwo;
        [SerializeField] private ShockwaveSkillSO shockwave;
        [SerializeField] private GateHomingFireSkillSO revolveFire;

        private const string SHOCKWAVE_WARN_KEY = "Whale_Shockwave";
        private const int SHOCKWAVE_WARN_LENGTH = 1;
        private const string ULTIMATE_FIRST_WARN_KEY = "Whale_Ultimate_First";
        private const int ULTIMATE_FIRST_WARN_LENGTH = 2;
        private const string ULTIMATE_SECOND_WARN_KEY = "Whale_Ultimate_Second";
        private const int ULTIMATE_SECOND_WARN_LENGTH = 1;

        private DialogueVisualizer dialogueVisualizer;

        public override void Initialize(BaseWhale baseWhale)
        {
            base.Initialize(baseWhale);
        }

        public override void Dispose()
        {
            ExitPhaseOne();
            ExitPhaseTwo();

            base.Dispose();

            dialogueVisualizer = null;
        }

        public override void PhysicsUpdate(WhaleState state)
        {
            if (currentSkill != null)
            {
                base.PhysicsUpdate(state);
                return;
            }

            if (state != WhaleState.RevolveAttack)
            {
                CheckSkillsToCast();
            }

            base.PhysicsUpdate(state);
        }

        private void CheckSkillsToCast()
        {
            var angleToPlayer = baseWhale.GetAngleToPlayer();

            if (orbitFireSkill.IsAvailable())
            {
                currentSkill = orbitFireSkill;
                orbitFireSkill.Cast();
                // DebugVisualizer.Instance.UpdateLog("SKILL", "ORBIT FIRE");
            }

            if (spiritFireSkill.IsAvailable())
            {
                currentSkill = spiritFireSkill;
                spiritFireSkill.Cast();
                // DebugVisualizer.Instance.UpdateLog("SKILL", "SPIRIT FIRE");
            }

            if (angleToPlayer > -0.5f && angleToPlayer < 0.5f && shockwaveSkill.IsAvailable())
            {
                currentSkill = shockwaveSkill;
                shockwaveSkill.Cast();
                PlayWarnDialogue(SHOCKWAVE_WARN_KEY, SHOCKWAVE_WARN_LENGTH);
                // DebugVisualizer.Instance.UpdateLog("SKILL", "SHOCKWAVE");
            }

            if (angleToPlayer < 0 && homingFireSkill.IsAvailable())
            {
                currentSkill = homingFireSkill;
                homingFireSkill.Cast(currentPhase == 0 ?
                                     homingFirePhaseOne :
                                     homingFirePhaseTwo);
                // DebugVisualizer.Instance.UpdateLog("SKILL", "HOMING FIRE");
            }

            //TODO: make bomb skill performed earlier before whale passing player.
            if (angleToPlayer > 0 && bombAttackSkill.IsAvailable())
            {
                currentSkill = bombAttackSkill;
                bombAttackSkill.Cast();
                // DebugVisualizer.Instance.UpdateLog("SKILL", "BOMB");
            }

            if (currentSkill == null) return;

            OnSkillPerformed();
        }

        public override void EnterPhase(int phase)
        {
            base.EnterPhase(phase);

            if (phase == 0)
            {
                EnterPhaseOne();
            }

            if (phase == 1)
            {
                ExitPhaseOne();
                EnterPhaseTwo();
                currentSkill = revolveFireSkill;
                revolveFireSkill.Cast();
                OnSkillPerformed();
                PlayWarnDialogue(ULTIMATE_FIRST_WARN_KEY, ULTIMATE_FIRST_WARN_LENGTH);
            }

            if (phase == 2)
            {
                ExitPhaseTwo();
                EnterPhaseTwo();
                currentSkill = revolveFireSkill;
                revolveFireSkill.Cast();
                OnSkillPerformed();
                PlayWarnDialogue(ULTIMATE_SECOND_WARN_KEY, ULTIMATE_SECOND_WARN_LENGTH);
            }
        }

        #region phase
        private void EnterPhaseOne()
        {
            InitializeBombSkill(bombAttack);
            InitializeOrbitSkill(orbitPhaseOne);
            InitializeSpiritSkill(spiritFire);
            InitializeHomingSkill(homingFirePhaseOne);
            InitializeShockwaveSkill(shockwave);
        }

        private void ExitPhaseOne()
        {
            currentSkill = null;

            DisposeBombSkill();
            DisposeOrbitSkill();
            DisposeSpiritSkill();
            DisposeHomingSkill();
            DisposeShockwaveSkill();
        }

        private void EnterPhaseTwo()
        {
            InitializeBombSkill(bombAttack);
            InitializeOrbitSkill(orbitPhaseTwo);
            InitializeSpiritSkill(spiritFire);
            InitializeHomingSkill(homingFirePhaseTwo);
            InitializeShockwaveSkill(shockwave);
            InitializeRevolveSkill(revolveFire);
        }

        private void ExitPhaseTwo()
        {
            currentSkill = null;

            DisposeBombSkill();
            DisposeOrbitSkill();
            DisposeSpiritSkill();
            DisposeHomingSkill();
            DisposeShockwaveSkill();
            DisposeRevolveSkill();
        }
        #endregion

        #region setup skill
        private void InitializeBombSkill(BombAttackSkillSO data)
        {
            if (bombAttackSkill != null) return;

            bombAttackSkill = new();
            bombAttackSkill.Initialize(data, baseWhale);
            bombAttackSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeBombSkill()
        {
            if (bombAttackSkill == null) return;

            bombAttackSkill.EventSkillEnd -= OnSkillEnd;
            bombAttackSkill.Dispose();
            bombAttackSkill = null;
        }

        private void InitializeOrbitSkill(OrbitFireSkillSO data)
        {
            if (orbitFireSkill != null) return;

            orbitFireSkill = new();
            orbitFireSkill.Initialize(data, baseWhale);
            orbitFireSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeOrbitSkill()
        {
            if (orbitFireSkill == null) return;

            orbitFireSkill.EventSkillEnd -= OnSkillEnd;
            orbitFireSkill.Dispose();
            orbitFireSkill = null;
        }

        private void InitializeSpiritSkill(SpiritFireSkillSO data)
        {
            if (spiritFireSkill != null) return;

            spiritFireSkill = new();
            spiritFireSkill.Initialize(data, baseWhale);
            spiritFireSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeSpiritSkill()
        {
            if (spiritFireSkill == null) return;

            spiritFireSkill.EventSkillEnd -= OnSkillEnd;
            spiritFireSkill.Dispose();
            spiritFireSkill = null;
        }

        private void InitializeHomingSkill(HomingFireSkillSO data)
        {
            if (homingFireSkill != null) return;

            homingFireSkill = new();
            homingFireSkill.Initialize(data, baseWhale);
            homingFireSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeHomingSkill()
        {
            if (homingFireSkill == null) return;

            homingFireSkill.EventSkillEnd -= OnSkillEnd;
            homingFireSkill.Dispose();
            homingFireSkill = null;
        }

        private void InitializeShockwaveSkill(ShockwaveSkillSO data)
        {
            if (shockwaveSkill != null) return;

            shockwaveSkill = new();
            shockwaveSkill.Initialize(data, baseWhale, baseWhale.AudioSource);
            shockwaveSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeShockwaveSkill()
        {
            if (shockwaveSkill == null) return;

            shockwaveSkill.EventSkillEnd -= OnSkillEnd;
            shockwaveSkill.Dispose();
            shockwaveSkill = null;
        }

        private void InitializeRevolveSkill(GateHomingFireSkillSO data)
        {
            if (revolveFireSkill != null) return;

            revolveFireSkill = new();
            revolveFireSkill.Initialize(data, baseWhale);
            revolveFireSkill.EventSkillEnd += OnSkillEnd;
        }

        private void DisposeRevolveSkill()
        {
            if (revolveFireSkill == null) return;

            revolveFireSkill.EventSkillEnd -= OnSkillEnd;
            revolveFireSkill.Dispose();
            revolveFireSkill = null;
        }
        #endregion

        private void PlayWarnDialogue(string key, int length)
        {
            dialogueVisualizer ??= GameplayController.Instance.DialogueVisualizer;
            dialogueVisualizer.PlayWarnDialogue(key, length);
        }
    }
}
