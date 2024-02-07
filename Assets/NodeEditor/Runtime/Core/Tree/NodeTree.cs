using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* 可创建资产声明 */
// [CreateAssetMenu()]
/* 继承脚本数据化结构对象 ScriptableObject */
public class NodeTree : ScriptableObject
{
    // 对话树的开始 根节点
    public Node rootNode;
    // 当前正在播放的对话
    public Node runningNode;
    // 对话树当前状态 用于判断是否要开始这段对话
    public Node.State treeState = Node.State.Waiting;
    // 所有对话内容的存储列表
    public List<Node> nodes = new List<Node>();

    // 判断当前对话树和对话内容都是运行中状态则进行OnUpdate()方法更新
    public virtual void Update() {
        if(treeState == Node.State.Running && runningNode.state == Node.State.Running){
            runningNode = runningNode.OnUpdate();
        }
    }
    // 对话树开始的触发方法
    public virtual void OnTreeStart(){
        runningNode = rootNode;
        rootNode.state = Node.State.Running;
        treeState = Node.State.Running;
    }
    // 对话树结束的触发方法
    public virtual void OnTreeEnd(){
        treeState = Node.State.Waiting;
    }
    
#if UNITY_EDITOR
        public Node CreateNode(System.Type type){
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name =type.Name;
            node.guid = GUID.Generate().ToString();
         
            Undo.RecordObject(this,"Node Tree (CreateNode)");

            nodes.Add(node);
            if(!Application.isPlaying){
                AssetDatabase.AddObjectToAsset(node,this);
            }
            Undo.RegisterCreatedObjectUndo(node,"Node Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }
        public Node DeleteeNode(Node node){
            Undo.RecordObject(this,"Node Tree (DeleteNode)");
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void AddChild(Node parent, Node child){
            if(parent != null && child != null ){
                SingleNode singleNode = parent as SingleNode;
                if(singleNode){
                    Undo.RecordObject(singleNode,"Node Tree (AddChild)");
                    singleNode.child = child;
                    EditorUtility.SetDirty(singleNode);
                }
                CompositeNode compositeNode = parent as CompositeNode;
                if(compositeNode){
                    Undo.RecordObject(compositeNode,"Node Tree (AddChild)");
                    compositeNode.children.Add(child);
                    EditorUtility.SetDirty(compositeNode);
                }
            }
        }
        public void RemoveChild(Node parent, Node child){
                SingleNode singleNode = parent as SingleNode;
                if(singleNode){
                    Undo.RecordObject(singleNode,"Node Tree (AddChild)");
                    singleNode.child = null;
                    EditorUtility.SetDirty(singleNode);
                }
                CompositeNode compositeNode = parent as CompositeNode;
                if(compositeNode){
                    Undo.RecordObject(compositeNode,"Node Tree (AddChild)");
                    compositeNode.children.Remove(child);
                    EditorUtility.SetDirty(compositeNode);
                }
        }

        public List<Node> GetChildren(Node parent){
            List<Node> Children = new List<Node>();
            SingleNode singleNode = parent as SingleNode;
            if(singleNode && singleNode.child != null){
                Children.Add(singleNode.child);
            }
            CompositeNode composite = parent as CompositeNode;
            if(composite){
               return composite.children;
            }
            return Children;
        }
#endif

}
