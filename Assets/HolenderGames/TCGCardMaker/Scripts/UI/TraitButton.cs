using UnityEngine;
using TMPro;
using System;

namespace TCG_CardMaker
{
    public class TraitButton : MonoBehaviour
    {
        [SerializeField] public CardTrait trait;
        public event Action<CardTrait,bool> OnTraitStateChange;
        AlternateColorButton btn;
        private void Awake()
        {
            btn = GetComponent<AlternateColorButton>();
            btn.OnStateChange += onButtonStateChange;

        }

        public void SetState(bool state)
        {
            btn.Set(state,false);
        }


        private void onButtonStateChange(bool isEnabled)
        {
            OnTraitStateChange?.Invoke(trait, isEnabled);
        }

        public void SetTraitData(CardTrait trait)
        {
            this.trait = trait;
            GetComponentInChildren<TextMeshProUGUI>().text = trait.Name;
        }
    }
}
