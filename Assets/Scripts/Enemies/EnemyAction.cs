using UnityEngine;

namespace DungeonPairs.Enemies
{
    public abstract class EnemyAction : ScriptableObject
    {
        public abstract void Execute(EnemyActionContext context);
    }
}