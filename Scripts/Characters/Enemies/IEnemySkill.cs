namespace Magia.Enemy
{
    public interface IEnemySkill
    {
        public bool IsAvailable();
        public void UpdateLogic();
        public void Cast();
    }
}
