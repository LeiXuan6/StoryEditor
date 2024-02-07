using UnityEngine;

public class EventManager
{
    //public static FEvent OnGuildCreated = new FEvent);   //创建公会成功
    public static FEvent<bool> OnGuildCreated = new FEvent<bool>();   //创建公会成功
    public static FEvent<Vector2Int> ScreenResolutionEvt = new FEvent<Vector2Int>();   //分辨率变化适配

}
