using UnityEngine;

namespace Values
{
    [CreateAssetMenu(menuName = "Dev/EnemyValues")]
    public class EnemyValues : ScriptableObject
    {
        public Sprite sprite;
        public int health;
        public int maxHealth;
        public int spellPotency;
        public int statusPotency;
        public int attackPotency;
        public int shieldPotency;

        [Range(0,100)]public float attackChance;
        [Range(0,100)]public float defenseChance;
        [Range(0,100)]public float spellChance;
    }
}
