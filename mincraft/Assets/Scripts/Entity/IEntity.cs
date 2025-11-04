namespace Entity {
    public interface IEntity {
         bool IsAlive { get; }
         int MaxHp { get; }
         int Hp { get; }
         
         float Speed { get; }

         void GetDamage(int pAmount);
         void Heal(int pAmount);
    }
}