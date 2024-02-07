using UnityEngine;

public class NodeTreeRunner : MonoBehaviour
{
    public NodeTree tree;

    private void Start() {
    }
    void Update()
    {
        tree.Update();
    }
}
