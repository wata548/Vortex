using System;
using UnityEngine;

namespace Entity.Enemy {
    public class EnemyData: IEntity {

        public EnemyData(string pName, int x = 0, int y = 60, int z = 0)
            : this(Resources.Load<EnemyBase>($"Enemies/{pName}")) 
        {
            FixedPos = new(x, y, z);
        }
        
        public EnemyData(EnemyBase pEnemy) {
            IsAlive = pEnemy.IsAlive;
            MaxHp = pEnemy.MaxHp;
            Hp = pEnemy.Hp;
            Speed = pEnemy.Speed;
            FixedPos = pEnemy.FixedPos;
            FootPos = pEnemy.FootPos;
            _enemyType = pEnemy.name;
        }

        public EnemyBase Load() {
            var enemy = GameObject.Instantiate(Resources.Load<EnemyBase>($"Enemies/{_enemyType}"));
            enemy.name = _enemyType;
            enemy.SetUp(this);
            return enemy;
        }

        private readonly string _enemyType;  
        public bool IsAlive { get; }
        public int MaxHp { get; }
        public int Hp { get; }
        public float Speed { get; }
        public Vector3 FixedPos { get; }
        public Vector3Int FootPos { get; }
        
        public void GetDamage(int pAmount) {
            throw new System.NotImplementedException();
        }

        public void Heal(int pAmount) {
            throw new System.NotImplementedException();
        }
    }
}