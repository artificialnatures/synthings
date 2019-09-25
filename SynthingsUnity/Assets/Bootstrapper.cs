using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    void Start()
    {
        var library = synthings.core.aggregateLibrary.build();
        var topics = library.listTopics();
        Debug.Log("Topics:");
        foreach (var topic in topics)
        {
            Debug.Log($"  - {topic.DisplayName}");
        }
    }

    void Update()
    {
        
    }
}
