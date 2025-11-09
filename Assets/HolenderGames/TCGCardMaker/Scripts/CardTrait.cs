using UnityEngine;

namespace TCG_CardMaker
{
    [CreateAssetMenu(fileName = "CardTrait", menuName = "Create Trait", order = 3)]
    public class CardTrait : ScriptableObject
    {
        public string Name = "";
        [Multiline]
        public string Description = "";
        public TraitType Type;
       
    }
}
