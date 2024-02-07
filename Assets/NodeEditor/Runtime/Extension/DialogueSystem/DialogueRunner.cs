using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    public DialogueTree tree;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            tree.OnTreeStart();
        }
        if(tree != null){
            tree.Update();
        }
        if(Input.GetKeyDown(KeyCode.D)){
            tree.OnTreeEnd();
        }
    }
}
