using UnityEngine;



    [CreateAssetMenu(fileName = "MonsterAttackScriptableObject", menuName = "ScriptableObjects/MonsterAttack")]
    public class MonsterAttackScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public float attackDamage { get; private set; } = 20f;
        [field: SerializeField]
        public float attackRange { get; private set; } = 10f;
    }

