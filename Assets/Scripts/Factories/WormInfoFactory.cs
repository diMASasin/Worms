using System;
using System.Collections.Generic;
using EventProviders;
using UnityEngine;
using WormComponents;

namespace Factories
{
    [CreateAssetMenu(fileName = "WormInfoFactory", menuName = "Factories/WormInfo")]
    public class WormInfoFactory : ScriptableObject, IDisposable
    {
        [SerializeField] private WormInformationView _wormInfoPrefab;

        private readonly List<WormInformationView> _wormInfoList = new();
        private IWormEventsProvider _wormEvents;

        public void Init(IWormEventsProvider wormEvents)
        {
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
            
            _wormInfoList.Add(wormInfo);
        }
    }
}