// Leixuan Libs - Leixuan's Game Lib
// Copyright 2023 leixuan@pku.org.cn  All rights reserved.
// http://www.inyourcode.com
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of www.inyourcode.com nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using NodeGraphProcessor.Examples;

namespace StoryEditor.Nodes
{
    // 对话节点状态枚举值
    public enum LineNodeState{
        INIT,
        RUNNING,
        WAITTING,
        END,
    }
    
    public class StoryLineNode : StoryBaseNode
    {
        [Input(name = "Execute")]
        public ConditionalLink	execute;
        [Output(name = "Actions", allowMultiple = true)]
        public ConditionalLink	actions;
        [ShowInInspector]
        public StoryListenType listenType;
        [ShowInInspector]
        public int ActionIndex = -1;

        public LineNodeState LineState = LineNodeState.INIT;
        private List<StoryActionNode> acttionNodes = new List<StoryActionNode>();
        public IStoryContext StoryContext;
        public StoryBaseNode Step;

        public override void InitStoryNode()
        {
            base.InitStoryNode();
            IEnumerable<IStoryNode> executedNodes = GetExecutedNodes();
            if (executedNodes == null)
            {
                return;
            }
            foreach (IStoryNode node in executedNodes)
            {
                acttionNodes.Add((StoryActionNode)node);
            }

            acttionNodes.OrderBy(n => n.computeOrder);
        }

        public override IEnumerable<IStoryNode> GetExecutedNodes()
        {
            NodePort firstOrDefault = outputPorts.FirstOrDefault(n => n.fieldName == nameof(actions));
            if (firstOrDefault == null)
            {
                return null;
            }
            return firstOrDefault.GetEdges().Select(e => e.inputNode as StoryBaseNode);
        }

        public override StoryStepNode NextStep()
        {
            return null;
        }

        public StoryLineNode Listen(StoryListenType type, StoryListenParam storyListenParam)
        {
            if (listenType != type)
            {
                return null;
            }

            LineState = LineNodeState.RUNNING;
            return this;
        }

        public override void OnStoryUpdate()
        {
            if (ActionIndex >= 0 && ActionIndex == acttionNodes.Count)
            {
                ChangeStep();
                return;
            }

            if (LineState != LineNodeState.RUNNING)
            {
                return;
            }
            
            ActionIndex += 1;
            LineState = LineNodeState.WAITTING;
            
            acttionNodes[ActionIndex].OnStoryUpdate();
        }

        private void ChangeStep()
        {
            if (Step is StoryStep4End)
            {
                StoryContext.End();
                return;
            }
            
            StoryContext.ChangeStep(Step.NextStep());
        }
    }
    
    [System.Serializable, NodeMenuItem("Story/ImmediatelyExecLine")]
    public class StoryLine4Current : StoryLineNode
    {
        public override string	name => "ImmediatelyExecLine";
    }
    
    [System.Serializable, NodeMenuItem("Story/OptionsExecLine")]
    public class StoryLine4Option : StoryLineNode
    {
        public string OptionaName = "Enter option name";
        public override string	name => "OptionsExecLine";
    }
}