using UnityEngine;

namespace Values
{
    [CreateAssetMenu(menuName = "Dev/CardValues")]
    public class CardValues : ScriptableObject
    {
        public int cost;
        public ActionType type;
        public SpellType spellType;
        public int potency;
    }
}
