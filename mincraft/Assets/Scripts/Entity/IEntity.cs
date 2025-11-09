using UnityEngine;

namespace Entity {
    public interface IEntity {
        
        //==================================================||Data 
         bool IsAlive { get; }
         int MaxHp { get; }
         int Hp { get; }
         
         float Speed { get; }
         
        //==================================================||Position Info 
         Vector3 FixedPos { get; }
         Vector3Int FootPos { get; }
        
        //==================================================||Methods 
         void GetDamage(int pAmount);
         void Heal(int pAmount);
    }
}