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
using UnityEngine;
using UnityEngine.UI;

namespace StoryEditor.Nodes
{
    public enum StoryState
    {
        OPEN,
        CLOSE
    }

    public interface IStoryUIView
    {
        
    }
    
    public interface IStoryContext
    {
        string GetCurrentStoryStep();
        string GetCurrentStoryLine();
        StoryState GetState();
        IStoryRunner GetStoryRunner();
        void ChangeStep(StoryStepNode nextStep);
        void End();
        
        
        
        //UI
        IStoryUIView GetStoryUIView();
    }
   
    [System.Serializable]
    public class StoryStepNode : StoryBaseNode
    {
        [Output(name = "Executes", allowMultiple = true)]
        public ConditionalLink	executes;
        [Output(name = "Next Step")]
        public ConditionalLink nextStep;
        [Input(name = "Story Actor"), ShowAsDrawer]
        public StoryActor storyActor;

        public override IEnumerable<IStoryNode> GetExecutedNodes()
        {
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
                .GetEdges().Select(e => e.inputNode as StoryBaseNode);
        }

        public override StoryStepNode NextStep()
        {
            List<SerializableEdge> serializableEdges = outputPorts.FirstOrDefault(n => n.fieldName == nameof(nextStep))
                .GetEdges();
            foreach (SerializableEdge serializableEdge in serializableEdges)
            {
                if (serializableEdge.inputNode is StoryStepNode)
                {
                    return (StoryStepNode)serializableEdge.inputNode;
                }
            }
             
            return null;
        }

        public List<StoryLineNode> Listeners(StoryListenType type)
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable, NodeMenuItem("Story/StartStep")]
    public class StoryStep4Start : StoryStepNode, IStoryContext
    {
        [ShowInInspector]
        public string CurrentStoryStep;
        [ShowInInspector]
        public string CurrentStoryLine;
        [ShowInInspector]
        public StoryState State = StoryState.OPEN;
        public Sprite Background;
        
        public IStoryRunner StoryRunner;
        public StoryProcessor Processor;
        public IStoryUIView StoryUIView;
        public override string	name => "StartStep";

        public string GetCurrentStoryStep()
        {
            return this.CurrentStoryStep;
        }

        public string GetCurrentStoryLine()
        {
            return this.CurrentStoryLine;
        }

        public StoryState GetState()
        {
           return this.State;
        }

        public IStoryRunner GetStoryRunner()
        {
            return this.StoryRunner;
        }
        
        public IStoryUIView GetStoryUIView()
        {
            return this.StoryUIView;
        }

        public void ChangeStep(StoryStepNode nextStep)
        {
            CurrentStoryLine = "";
            if (nextStep != null)
            {
                CurrentStoryStep = nextStep.GUID;
            }
            StoryRunner.RemoveStory(Processor);
        }

        public void End()
        {
            State = StoryState.CLOSE;
        }

    }

    [System.Serializable, NodeMenuItem("Story/EndStep")]
    public class StoryStep4End : StoryBaseNode
    {
        [Output(name = "Executes", allowMultiple = true)]
        public ConditionalLink	executes;
        [Input(name = "Previous Step")]
        public ConditionalLink preStep;
        
        public override IEnumerable<IStoryNode> GetExecutedNodes()
        {
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
                .GetEdges().Select(e => e.inputNode as StoryBaseNode);
        }
        
        public override string	name => "EndStep";

        public override StoryStepNode NextStep()
        {
            return null;
        }
    }
    
    [System.Serializable, NodeMenuItem("Story/NormStep")]
    public class StoryStep4Norm : StoryStepNode
    {
        public string stepName;
        [Input(name = "Previous Step")]
        public ConditionalLink preStep;
        
        public override string	name => "NormStep";
    }
}