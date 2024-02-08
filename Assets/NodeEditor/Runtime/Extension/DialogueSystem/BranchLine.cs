using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BranchLine : CompositeNode
{
    [TextArea] public string dialogueContent;
    public Sprite speakerAvatar;
    public string speakerName;
    public List<string> optionList;
    public int nextDialogueIndex = 0;
    public bool nextDialogueStart = false;
    public override Node LogicUpdate()
    {
        // 判断进入下一节点条件成功时 需将节点状态改为非运行中 且 返回对应子节点
        if(nextDialogueStart){
            state = State.Waiting;
            if(children.Count > nextDialogueIndex){
                children[nextDialogueIndex].state = State.Running;
                return children[nextDialogueIndex];
            }
        }
        return this;
    }

    protected override void OnStart()
    {
        DialogueManager dialogueManager = GameObject.Find("UI").GetComponent<DialogueManager>();
        dialogueManager.generateOptions(optionList,this);
        dialogueManager.UpdateSpeakerInfo(dialogueContent,speakerAvatar,speakerName);
    }

    protected override void OnStop()
    {
        GameObject.Find("UI").GetComponent<DialogueManager>().SelectStop(this);
        if(children.Count == 0){
            GameObject.Find("UI").GetComponent<DialogueRunner>().tree.OnTreeEnd();
        }
    }
}
