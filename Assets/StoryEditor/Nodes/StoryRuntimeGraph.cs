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

using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StoryEditor.Nodes
{
    public interface IStoryRunner
    {
        void RemoveStory(StoryProcessor processor);
        void ExecuteStory(StoryProcessor processor);
    }
    
    public class StoryProcessor : BaseGraphProcessor
    {
        private static int ID_GEN = 0;
        public int GUID;
        List<BaseNode> processList;
        internal StoryStep4Start StartNode;
        private IStoryRunner storyRunner;
        Dictionary<string, StoryBaseNode> storyNodes;
        Dictionary<string, List<StoryLineNode>> step2Lines;
        Dictionary<string, List<StoryActionNode>> line2Actions;
       
        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public StoryProcessor(BaseGraph graph, IStoryRunner storyRunner) : base(graph)
        {
            GUID = ++ID_GEN;
            this.storyRunner = storyRunner;
            InitStoryProcessor(graph);
        }

        public override void UpdateComputeOrder()
        {
            processList = graph.nodes.OrderBy(n => n.computeOrder).ToList();
        }

        /// <summary>
        /// Process all the nodes following the compute order.
        /// </summary>
        public override void Run()
        {
            if(storyNodes.TryGetValue(StartNode.CurrentStoryLine, out StoryBaseNode lineNode))
            {
                ((StoryLineNode)lineNode).OnProcess();
            }
        }
        
        #region story
        public virtual void InitStoryProcessor(BaseGraph graph)
        {
            foreach (BaseNode baseNode in processList)
            {
                storyNodes.Add(baseNode.GUID, (StoryBaseNode)baseNode);
                if (baseNode is StoryStep4Start)
                {
                    StartNode = (StoryStep4Start)baseNode;
                }
            }
            
            if (StartNode == null)
            {
                throw new Exception("a start node must be created");
            }

            if (StartNode.State == StoryState.CLOSE)
            {
                return;
            }

            StartNode.StoryRunner = this.storyRunner;
            StartNode.Processor = this;
            
            storyNodes = new Dictionary<string, StoryBaseNode>();
            step2Lines = new Dictionary<string, List<StoryLineNode>>();
            line2Actions = new Dictionary<string, List<StoryActionNode>>();
            InitStoryData();

            string currentStoryStep = StartNode.CurrentStoryStep;
            if (string.IsNullOrEmpty(currentStoryStep))
            {
                currentStoryStep = StartNode.GUID;
                StartNode.CurrentStoryStep = currentStoryStep;
            }

            List<StoryLineNode> storyLineNodes = GetLines(StartNode.CurrentStoryStep);
            if (storyLineNodes == null)
            {
                throw new Exception("current step not have line node data");
            }
 
            
            Debug.Log("init story graph");
        }

        public List<StoryLineNode> GetLines(string step)
        {
            step2Lines.TryGetValue(step, out List<StoryLineNode> lines);
            return lines;
        }

        private void InitStoryData()
        {
            StoryStepNode current = StartNode;
            while (current != null)
            {
                IEnumerable<IStoryNode> lineExecutedNodes = current.GetExecutedNodes();
                List<StoryLineNode> lines = new List<StoryLineNode>();
                step2Lines.Add(current.GUID, lines);

                foreach (StoryLineNode executedNode in lineExecutedNodes)
                {
                    lines.Add(executedNode);
                    
                    executedNode.StoryContext = StartNode;
                    executedNode.Step = current;
                        
                    IEnumerable<IStoryNode> actionExecutedNodes = executedNode.GetExecutedNodes();
                    List<StoryActionNode> actionList = new List<StoryActionNode>();
                    line2Actions.Add(executedNode.GUID, actionList);
                    foreach (StoryActionNode action in actionExecutedNodes)
                    {
                        actionList.Add(action);
                    }
                }

                IStoryNode nextStep = current.NextStep();
                if (nextStep == null)
                {
                    break;
                }

                current = (StoryStepNode)nextStep;
            }
        }
        
        public bool Listen(StoryListenType type, StoryListenParam storyListenParam)
        {
            if (!string.IsNullOrEmpty(StartNode.CurrentStoryLine))
            {
                return false;
            }
            
            List<StoryLineNode> storyLineNodes = GetLines(StartNode.CurrentStoryStep);
            foreach (StoryLineNode storyLineNode in storyLineNodes)
            {
                StoryLineNode listen = storyLineNode.Listen(type, storyListenParam);
                if (listen != null)
                {
                    StartNode.CurrentStoryLine = listen.GUID;
                    break;
                }
            }

            return true;
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is StoryProcessor)
            {
                return false;
            }
            return this.GUID == ((StoryProcessor)obj).GUID;
        }

        public override int GetHashCode()
        {
            return this.GUID;
        }
    }

    public class StoryRuntimeGraph: MonoSingleton<StoryRuntimeGraph>,IStoryRunner
    {
        private Dictionary<string, StoryProcessor> storyGraphProcessors = new Dictionary<string, StoryProcessor>();
        private List<StoryProcessor> executeQueue = new List<StoryProcessor>();
        protected override void Init()
        {
            base.Init();
            Object[] scriptableObjectArray = Resources.LoadAll("StoryDatas", typeof(ScriptableObject));
            foreach (var scriptableObject in scriptableObjectArray)
            {
                if (scriptableObject is ScriptableObject)
                {
                    StoryProcessor storyProcessor = new StoryProcessor((BaseGraph)scriptableObject, this);
                    storyGraphProcessors.Add(scriptableObject.name, storyProcessor);
                    Debug.Log("Loaded Story: " + scriptableObject.name);
                }
            }
        }
 
        public void Trigger(StoryListenType type, StoryListenParam param)
        {
            foreach (KeyValuePair<string,StoryProcessor> kv in storyGraphProcessors)
            {
                bool listen = kv.Value.Listen(type, param);
                if (listen)
                {
                    executeQueue.Add(kv.Value);
                }
            }
        }

      
        void Update()
        {
            
        }

        public void RemoveStory(StoryProcessor processor)
        {
            executeQueue.Remove(processor);
        }

        public void ExecuteStory(StoryProcessor processor)
        {
            if (executeQueue.Count == 0)
            {
                return;
            }
            
            executeQueue[0].Run();
        }
    }
}