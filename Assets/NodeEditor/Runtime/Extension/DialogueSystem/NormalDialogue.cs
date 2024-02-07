using UnityEngine;
using UnityEngine.UI;

// 普通对话节点 只会返回
public class NormalDialogue : SingleNode
{
    [TextArea] public string dialogueContent;
    public Sprite speakerAvatar;
    public string speakerName;
    public override Node LogicUpdate()
    {
        // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
        if(Input.GetKeyDown(KeyCode.Space)){
            state = State.Waiting;
            if(child != null){
                child.state = State.Running;
                return child;
            }
        }
        return this;
    }

    protected override void OnStart()
    {
        DialogueManager dialogueManager = GameObject.Find("UI").GetComponent<DialogueManager>();
        dialogueManager.UpdateSpeakerInfo(dialogueContent,speakerAvatar,speakerName);
    }

    protected override void OnStop()
    {
        if(child == null){
            GameObject.Find("UI").GetComponent<DialogueRunner>().tree.OnTreeEnd();
        }
    }

}
