using UnityEngine;

public class ChooseButtonLayout : MonoBehaviour
{
    [SerializeField] private GameObject layout1;
    [SerializeField] private GameObject layout2;
    [SerializeField] private GameObject layout3;
    [SerializeField] private GameObject layout4;

    public GameObject GetLayout(int connections)
    {
        switch (connections)
        {
            case 1:
                return layout1;
            case 2:
                return layout2;
            case 3:
                return layout3;
            case 4:
                return layout4;
            default:
                Debug.LogError("Invalid number of connections: " + connections);
                return null;
        }
    }
    
}
