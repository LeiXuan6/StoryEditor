using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Text dialogueContentText;
    private Image speakerAvatarImage;
    private Text speakerNameText;
    private void Awake() {
        speakerAvatarImage = GameObject.Find("UI").transform.Find("DialogueUI/SpeakerAvatar").gameObject.GetComponent<Image>();
        speakerNameText = GameObject.Find("UI").transform.Find("DialogueUI/Dialogue/Name").GetComponent<Text>();
        dialogueContentText = GameObject.Find("UI").transform.Find("DialogueUI/Dialogue/TextContent").GetComponent<Text>();
    }
    public void UpdateSpeakerInfo(string dialogueContent,Sprite speakerAvatar,string speakerName)
    {
        if(speakerAvatar){
            speakerAvatarImage.sprite = speakerAvatar;
        }
        if(speakerName != ""){
            speakerNameText.text = speakerName;
        }
        dialogueContentText.text = dialogueContent;
    }
    public void generateOptions(List<string> optionList,BranchLine branchLine){
        for(int i = 1;i <= optionList.Count;i++){
            float offset = 800 / (optionList.Count+1);
            float rectPositionY = 400 - i * offset;
            generateButton(i,rectPositionY,optionList,branchLine);
        }
    }
    // 即时生成按钮
    private void generateButton(int index,float rectPositionY,List<string> optionList,BranchLine branchLine){
        DefaultControls.Resources uiResources = new DefaultControls.Resources();

        GameObject optionGameObject = DefaultControls.CreateButton(uiResources);
        optionGameObject.transform.SetParent(GameObject.Find("UI").transform.Find("DialogueUI/OptionList"));
        optionGameObject.name = "Option"+index;
        RectTransform optionRectTransform = optionGameObject.GetComponent<RectTransform>();
        optionRectTransform.localScale = Vector3.one;
        optionRectTransform.sizeDelta = new Vector2(800, 100);
        optionRectTransform.anchoredPosition = new Vector2(0,rectPositionY);

        Button optionButton = optionGameObject.GetComponent<Button>();
        optionButton.onClick.AddListener(() => ReturnNextDialogue(index,branchLine));
        
        Outline outline = optionGameObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2(5,5);
        outline.effectColor = new Color((140f/255f),(250f/255f),(210f/255f),(80f/255f));

        Text optionContent = optionGameObject.transform.Find("Text (Legacy)").GetComponent<Text>();
        optionContent.text = optionList[index-1];
        optionContent.fontStyle = FontStyle.Bold;
        optionContent.fontSize = 30;
    }
    private void ReturnNextDialogue(int index,BranchLine branchLine){
        branchLine.nextDialogueStart = true;
        branchLine.nextDialogueIndex = index-1;
    }
    public void SelectStop(BranchLine branchLine){
        branchLine.nextDialogueStart = false;
        Button[] optionObjectList = GameObject.Find("UI").transform.Find("DialogueUI/OptionList").GetComponentsInChildren<Button>();
        foreach(var optionObject in optionObjectList){
            Destroy(optionObject.gameObject);
        }
    }
}
