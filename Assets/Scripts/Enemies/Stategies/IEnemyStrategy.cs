namespace Enemies.Stategies
{
    public interface IEnemyStrategy
    {
        void Execute(Enemy enemy);
        
        void OnEnter(Enemy enemy);
        
        void OnExit(Enemy enemy);
    }
}