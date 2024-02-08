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
using Story.Core.API;

namespace Story.Core
{
    public sealed class StoryConstruct
    {
        private static readonly StoryConstruct instance = new StoryConstruct();
        private Dictionary<long, IStoryLineConfig> storyConfigs = new Dictionary<long, IStoryLineConfig>();
        private Dictionary<long, IStoryOptionConfig> storyOptionConfigs = new Dictionary<long, IStoryOptionConfig>();
        private Dictionary<long, IStoryActionConfig> storyActionConfigs = new Dictionary<long, IStoryActionConfig>();
        private Dictionary<long, IStoryConditionConfig> storyConditionConfigs = new Dictionary<long, IStoryConditionConfig>();
        private Dictionary<long, List<IStoryOptionConfig>> stageId2Options = new Dictionary<long, List<IStoryOptionConfig>>();

        public static StoryConstruct INSTANCE
        {
            get { return instance; }
        }

        public void InitConfigData()
        {
            
        }

        public void AddStoryConfig(IStoryLineConfig lineConfig)
        {
            if (storyConfigs.TryGetValue(lineConfig.ConfigId, out IStoryLineConfig old))
            {
                return;
            }
            
            storyConfigs.Add(lineConfig.ConfigId, lineConfig);
        }

        public void AddStoryStage(IStoryOptionConfig config)
        {
            if (storyOptionConfigs.TryGetValue(config.ConfigId, out IStoryOptionConfig old))
            {
                return;
            }
            /*
            storyOptionConfigs.Add(config.ConfigId, config);
            if (!stageId2Stages.TryGetValue(config.NodeId, out List<IStoryOptionConfig> list))
            {
                stageId2Stages.Add(config.NodeId, list = new List<IStoryOptionConfig>());
            }
            list.Add(config);*/
        }

        public void AddStoryAction(IStoryActionConfig config)
        {
            if (storyActionConfigs.TryGetValue(config.ConfigId, out IStoryActionConfig old))
            {
                return;
            }
            storyActionConfigs.Add(config.ConfigId, config);
        }

        public void AddStoryCondition(IStoryConditionConfig config)
        {
            if (storyConditionConfigs.TryGetValue(config.ConfigId, out IStoryConditionConfig old))
            {
                storyConditionConfigs[config.ConfigId] = old;
            }
            else
            {
                storyConditionConfigs.Add(config.ConfigId, config);
            }
        }

        public IStoryLineConfig GetStoryConfig(long id)
        {
            storyConfigs.TryGetValue(id, out IStoryLineConfig config);
            return config;
        }

        public IStoryOptionConfig GetStoryStageConfig(long id)
        {
            storyOptionConfigs.TryGetValue(id, out IStoryOptionConfig config);
            return config;
        }

        public IStoryActionConfig GetStoryActionConfig(long id)
        {
            storyActionConfigs.TryGetValue(id, out IStoryActionConfig config);
            return config;
        }

        public IStoryConditionConfig GetStoryConditionConfig(long id)
        {
            storyConditionConfigs.TryGetValue(id, out IStoryConditionConfig config);
            return config;
        }
    }
}