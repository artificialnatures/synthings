namespace synthings.unity
{
    using System;
    using System.Linq;
    using UnityEngine;

    public class Bootstrapper : MonoBehaviour
    {
        public GameObject cube;
        
        void Start()
        {
            _epoch = synthings.core.time.now(); //create a clock with epoch
            var library = synthings.core.aggregateLibrary.build();
            var topics = library.listTopics();
            var waveTopic = topics.First(topic => topic.DisplayName.Contains("Wave"));
            var waveBehaviors = library.listBehaviors(waveTopic);
            var sineWaveBehavior = waveBehaviors.First(behavior => behavior.DisplayName.Contains("Sine"));
            var sineWaveMachine = library.createMachine(sineWaveBehavior);
            _monitor = synthings.core.monitor.createTimeWindowed(10.0);
            _graph = synthings.core.graph.empty; //create linq-esque methods for chaining?
            _graph = synthings.core.graph.addMachine(sineWaveMachine, _graph);
            _graph = synthings.core.graph.addMachine(_monitor.Machine, _graph);
            _graph = synthings.core.graph.connect(_graph.Root.Id, sineWaveMachine.Id, _graph);
            _graph = synthings.core.graph.connect(sineWaveMachine.Id, _monitor.Machine.Id, _graph);
        }

        void Update()
        {
            var signal = synthings.core.Signal.createNow(_epoch, 0.0f);
            synthings.core.graph.induce(_graph, signal);
            Debug.Log($"Recorded {_monitor.Recording.Signals.Length} signals, latest = {_monitor.LatestValue}");
            cube.transform.position = cube.transform.right * (float)_monitor.LatestValue;
        }

        private synthings.core.Graph _graph;
        private synthings.core.Monitor _monitor;
        private DateTime _epoch;
    }
}