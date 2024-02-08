using UnityEngine;
using UnityEngine.UI;

public class RootLine : RootNode
{
    [TextArea] public string dialogueContent;
    public Sprite speakerAvatar;
    public string speakerName;
    public override Node LogicUpdate()
    {
        // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
        if(Input.GetKeyDown(KeyCode.Space)){
            state = State.Waiting;
            if(children.Count > 0){
                children[0].state = State.Running;
                return children[0];
            }
        }
        return this;
    }

    protected override void OnStart()
    {
        DialogueManager dialogueManager = GameObject.Find("UI").GetComponent<DialogueManager>();
        dialogueManager.UpdateSpeakerInfo(dialogueContent,speakerAvatar,speakerName);
    }

    protected override void OnStop(){}

}
