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
        private readonly IWormEventsProvider _wormEvents;

        public WormInfoFactory(WormInformationView prefab, IWormEventsProvider wormEvents)
        {
            _wormInfoPrefab = prefab;
            _wormEvents = wormEvents;
            
            _wormEvents.WormCreated += CreateInfoView;
        }
        
        public void Dispose()
        {
            _wormEvents.WormCreated -= CreateInfoView;
        }

        private void CreateInfoView(IWorm worm, Color teamColor, string wormName)
        {
            var wormTransform = worm.Transform;
            
            WormInformationView wormInfo = Instantiate(_wormInfoPrefab, wormTransform);
            wormInfo.Init(worm, teamColor, wormName);
        }
    }
}