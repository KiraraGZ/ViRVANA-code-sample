namespace Magia.Enemy.Puffer
{
    public class TracetPuffer : BasePuffer
    {
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (currentState == PufferState.STAGGERED || currentState == PufferState.DEAD)
            {
                return;
            }
            (combat as TracetPufferCombat).PhysicsUpdate();
        }
    }
}