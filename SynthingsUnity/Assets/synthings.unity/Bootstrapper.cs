namespace synthings.unity
{
    using System;
    using System.Linq;
    using UnityEngine;
    using synthings.core;
    
    public class Bootstrapper : MonoBehaviour
    {
        public GameObject cube;
        
        void Start()
        {
            _epoch = Instant.now(); //create a clock with epoch
            _application = new synthings.core.Application();
            var topics = _application.Library.listTopics();
            var waveTopic = topics.First(topic => topic.DisplayName.Contains("Wave"));
            var waveBehaviors = _application.Library.listBehaviors(waveTopic);
            var sineWaveBehavior = waveBehaviors.First(behavior => behavior.DisplayName.Contains("Sine"));
            var changeSet = _application.CreateMachine(sineWaveBehavior);
            _viewId = changeSet.ViewChanges.First(change => change.Subject.DisplayName.Contains("Sine")).Subject.SubjectId;
            _application.ConnectToRoot(changeSet.MachineChanges.First().Subject.Id);
        }

        void Update()
        {
            var signal = Signal.createNow(_epoch, 0.0f);
            var changeSet = _application.Induce(signal);
            foreach (var change in changeSet.ViewChanges)
            {
                if (change.Subject.SubjectId.Guid != _viewId.Guid) continue;
                var value = change.Subject.History.Last().Value;
                cube.transform.position = cube.transform.right * (float)value;
            }
        }

        private synthings.core.Application _application;
        private Graph _graph;
        private Monitor _monitor;
        private Instant _epoch;
        private Identifier _viewId;
    }
}