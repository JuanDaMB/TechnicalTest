using System;
using UnityEngine;

namespace Values
{
    [CreateAssetMenu(menuName = ("Dev/PlayerValues"))][Serializable]
    public class PlayerValues : ScriptableObject
    {
        public int health;
        public int maxHealth;
        public int gold;
        public int spellPotency;
        public int attackPotency;
        public int shieldPotency;
        public int mana;
    }
}
