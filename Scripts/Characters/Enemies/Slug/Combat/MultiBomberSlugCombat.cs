namespace Magia.Enemy.Slug
{
    public class MultiBomberSlugCombat : SlugCombat
    {
        public MultiBombSkill multiBombSkill;

        public override void Initialize(BaseSlug _basePuffer)
        {
            base.Initialize(_basePuffer);

            skills = new() { multiBombSkill };

            multiBombSkill.Initialize(this, slug, Player, maxChargeTime);
        }

        public override void Dispose()
        {
            multiBombSkill.Dispose();

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