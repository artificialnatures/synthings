using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button menuButton;
    public ScrollView scrollView;

    void Start()
    {
        titleText.text = "synthings - running";
    }

    public void SelectTopic(int topicIndex)
    {
        Debug.Log($"Selected topic {topicIndex}");
    }
}
