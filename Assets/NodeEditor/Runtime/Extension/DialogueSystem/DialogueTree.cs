using UnityEngine;

[CreateAssetMenu()]
public class DialogueTree : NodeTree
{

    public override void OnTreeStart(){
        base.OnTreeStart();
        GameObject.Find("UI").transform.Find("DialogueUI").gameObject.SetActive(true);
    }
    public override void OnTreeEnd(){
        base.OnTreeEnd();
        GameObject.Find("UI").transform.Find("DialogueUI").gameObject.SetActive(false);
    }
}
