using System;
using System.Collections.Generic;
using EventProviders;
using UnityEngine;
using WormComponents;
using static UnityEngine.Object;

namespace Factories
{
    public class WormInfoFactory : IDisposable
    {
        private readonly WormInformationView _wormInfoPrefab;
        private readonly IWormEvents _wormEvents;
        private readonly Dictionary<Worm, WormInformationView> _informationViews = new();
        private readonly Transform _parent;

        public WormInfoFactory(WormInformationView prefab, IWormEvents wormEvents)
        {
            _wormInfoPrefab = prefab;
            _wormEvents = wormEvents;
            
            _parent = Instantiate(new GameObject()).transform;
            _parent.name = "WormInformations";
            
            _wormEvents.WormCreated += CreateInfoView;
            _wormEvents.WormDied += OnWormDied;
        }

        public void Dispose()
        {
            _wormEvents.WormCreated -= CreateInfoView;
            _wormEvents.WormDied -= OnWormDied;
        }

        private void CreateInfoView(Worm worm, Color teamColor, string wormName)
        {
            WormInformationView wormInfo = Instantiate(_wormInfoPrefab, _parent);
            wormInfo.Init(worm, teamColor, wormName);
            
            _informationViews.Add(worm, wormInfo);
        }

        private void OnWormDied(Worm worm)
        {
            Destroy(_informationViews[worm].gameObject);
            _informationViews.Remove(worm);
        }
    }
}