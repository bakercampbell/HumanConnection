using UnityEngine;

namespace baker {
    [CreateAssetMenu(fileName = "MonsterScriptableObject", menuName = "ScriptableObjects/Monster")]
    public class MonsterScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public int health { get; set; } = 100;
        [field: SerializeField]
        public float speed { get; set; } = 3f;
        [field: SerializeField]
        public MonsterAttackScriptableObject monsterAttackType { get; set; }     
    }
}