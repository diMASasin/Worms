using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _UI.BattleSettings
{
    public class MapSelectorItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _name;
        
        private MapSelectorItem _item;

        public event Action<MapSelectorItem> LevelSelected;
        
        public void Init(MapSelectorItem item)
        {
            _item = item;

            _name.text = item.Name;
        }

        private void OnEnable() => _button.onClick.AddListener(OnMapSelectorButtonClicked);

        private void OnDisable() => _button.onClick.RemoveListener(OnMapSelectorButtonClicked);

        private void OnMapSelectorButtonClicked() => LevelSelected?.Invoke(_item);
    }
}