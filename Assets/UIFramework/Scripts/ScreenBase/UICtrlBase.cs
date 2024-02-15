using UnityEngine;
public enum ESceenPriority
{
    Default = 0,   //预留 目前没有使用到
    PriorityLobbyForSystem = 20,   //各种外围系统层级
    PriorityStoryForSystem = 90,   //剧情

    //PriorityCount = 100
};

public class UICtrlBase : UIFEventAutoRelease
{
    [HideInInspector]
    public Canvas ctrlCanvas;

    //基准分辨率
    public Vector2 m_referenceResolution = new Vector2(1920f, 1080f);
    [Tooltip("SceenBase 层级")]
    public ESceenPriority sceenPriority = ESceenPriority.PriorityLobbyForSystem; // 层级
    // 是否使用遮罩功能(点击遮罩关闭当前页面)
    [Tooltip("勾选此选项后,打开本界面时会自动激活并更新遮罩面板,\n用户点击到遮罩面板会自动关闭此页面")]
    public bool m_UseMask = false;
    [Tooltip("勾选此选项后,不会被 mHideOtherScreenWhenThisOnTop 控制")]
    public bool mAlwaysShow = false;
    [Tooltip("勾选此选项后,当该界面打开，会隐藏他下面的其他非AlwaysShow界面")]
    public bool mHideOtherScreenWhenThisOnTop = false;

    void Awake()
    {
        ctrlCanvas = GetComponent<Canvas>();
    }
}