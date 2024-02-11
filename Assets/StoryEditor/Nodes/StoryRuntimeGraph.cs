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

namespace StoryEditor.Nodes
{
    public class StoryGraphProcessor : BaseGraphProcessor
    {
        List<BaseNode> processList;
        StoryStep4Start startNode;
        Dictionary<string, StoryBaseNode> storyNodes;
        Dictionary<string, List<StoryLineNode>> step2Lines;
        Dictionary<string, List<StoryActionNode>> line2Actions;
       
        /// <summary>
        /// Manage graph scheduling and processing
        /// </summary>
        /// <param name="graph">Graph to be processed</param>
        public StoryGraphProcessor(BaseGraph graph) : base(graph)
        {
            storyNodes = new Dictionary<string, StoryBaseNode>();
            step2Lines = new Dictionary<string, List<StoryLineNode>>();
            line2Actions = new Dictionary<string, List<StoryActionNode>>();
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
            int count = processList.Count;
            for (int i = 0; i < count; i++)
                processList[i].OnProcess();
        }
        
        #region story
        public virtual void InitStoryProcessor(BaseGraph graph)
        {
            foreach (BaseNode baseNode in processList)
            {
                storyNodes.Add(baseNode.GUID, (StoryBaseNode)baseNode);
                if (baseNode is StoryStep4Start)
                {
                    startNode = (StoryStep4Start)baseNode;
                }
            }
            
            if (startNode == null)
            {
                throw new Exception("a start node must be created");
            }

            InitStoryData();

            string currentStoryStep = startNode.CurrentStoryStep;
            if (string.IsNullOrEmpty(currentStoryStep))
            {
                currentStoryStep = startNode.GUID;
                startNode.CurrentStoryStep = currentStoryStep;
            }

            List<StoryLineNode> storyLineNodes = GetLines(startNode.CurrentStoryStep);
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
            StoryStepNode current = startNode;
            while (current != null)
            {
                IEnumerable<IStoryNode> lineExecutedNodes = current.GetExecutedNodes();
                List<StoryLineNode> lines = new List<StoryLineNode>();
                step2Lines.Add(current.GUID, lines);

                foreach (StoryLineNode executedNode in lineExecutedNodes)
                {
                    lines.Add(executedNode);

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
        
        #endregion
    }

    public class StoryRuntimeGraph: MonoBehaviour
    {
        public BaseGraph	graph;
        public StoryGraphProcessor	processor;

        private void Start()
        {
            if (graph != null)
                processor = new StoryGraphProcessor(graph);
        }

        void Update()
        {
            if (graph != null)
            {
                processor.Run();
            }
        }
    }
}