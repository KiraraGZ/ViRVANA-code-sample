namespace Magia.Enemy.Slug
{
    public class BomberSlugCombat : SlugCombat
    {
        public BombSkill bombSkill;

        public override void Initialize(BaseSlug basePiller)
        {
            base.Initialize(basePiller);

            bombSkill.Initialize(this, basePiller, Player, maxChargeTime);

            skills = new() { bombSkill };
        }

        public override void Dispose()
        {
            bombSkill.Dispose();

            skills = null;

            base.Dispose();
        }

        public override void EnterIdleState()
        {
            throw new System.NotImplementedException();
        }

        public override void EnterAlertState()
        {
            throw new System.NotImplementedException();
        }

        public override void EnterDeadState()
        {
            throw new System.NotImplementedException();
        }
    }
}