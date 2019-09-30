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
            _epoch = time.now(); //create a clock with epoch
            var library = aggregateLibrary.build();
            var topics = library.listTopics();
            var waveTopic = topics.First(topic => topic.DisplayName.Contains("Wave"));
            var waveBehaviors = library.listBehaviors(waveTopic);
            var sineWaveBehavior = waveBehaviors.First(behavior => behavior.DisplayName.Contains("Sine"));
            var sineWaveMachine = library.createMachine(sineWaveBehavior);
            _monitor = Monitor.createTimeWindowed(10.0);
            _graph = Graph.empty; //create linq-esque methods for chaining?
            _graph = Graph.addMachine(sineWaveMachine, _graph);
            _graph = Graph.addMachine(_monitor.Machine, _graph);
            _graph = Graph.connect(_graph.Root.Id, sineWaveMachine.Id, _graph);
            _graph = Graph.connect(sineWaveMachine.Id, _monitor.Machine.Id, _graph);
        }

        void Update()
        {
            var signal = Signal.createNow(_epoch, 0.0f);
            Graph.induce(_graph, signal);
            Debug.Log($"Recorded {_monitor.Recording.Signals.Length} signals, latest = {_monitor.LatestValue}");
            cube.transform.position = cube.transform.right * (float)_monitor.LatestValue;
        }

        private Graph _graph;
        private Monitor _monitor;
        private DateTime _epoch;
    }
}