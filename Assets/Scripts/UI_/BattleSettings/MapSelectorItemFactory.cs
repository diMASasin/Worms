using System;
using System.Collections.Generic;
using System.Linq;
using _UI.BattleSettings;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

[Serializable]
public class MapSelectorItemFactory : IDisposable
{
    [SerializeField] private MapSelectorItemView _prefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private MapSelectorConfig _mapSelectorConfig;

    private List<MapSelectorItemView> _views = new();
    
    public event Action<MapSelectorItem> LevelSelected;

    public void Dispose()
    {
        foreach (var view in _views) 
            view.LevelSelected -= OnLevelSelected;
    }

    public void Create()
    {
        foreach (var item in _mapSelectorConfig.MapSelectorItems)
        {
            MapSelectorItemView itemView = Instantiate(_prefab, _parent);
            itemView.Init(item);
            _views.Add(itemView);
            
            itemView.LevelSelected += OnLevelSelected;
        } 
        
        OnLevelSelected(_mapSelectorConfig.MapSelectorItems.First());
    }

    private void OnLevelSelected(MapSelectorItem mapSelectorItem) => LevelSelected?.Invoke(mapSelectorItem);
}
