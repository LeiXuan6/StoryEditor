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
using Story.Core.API;

namespace Story.Core
{
    public partial class StoryWorld
    {
        
        public Story CreateStoryData(int storyId)
        {
            Story story = new Story();
            story.StoryConfigId = storyId;
            story.CurrentStage = 0;
            story.CurrentActionIndex = -1;
            return story;
        }
        
        public IStoryResultCode Start(int storyId)
        {
            IStoryLineConfig storyLineConfig = StoryConstruct.INSTANCE.GetStoryConfig(storyId);
            if (storyLineConfig == null)
            {
                return IStoryResultCode.STORY_CONFIG_NOT_FOUND;
            }
            
            Story old = StoryContext.INSTANCE.GetStoryData(storyId);
            if (old != null)
            {
                return IStoryResultCode.REPEATED;
            }
            
            Story story = CreateStoryData(storyId);
            StoryContext.INSTANCE.AddStoryData(story);
            
            IStoryOptionConfig storyOptionConfig = StoryConstruct.INSTANCE.GetStoryStageConfig(storyLineConfig.RootNodeId);
            foreach (long action  in storyOptionConfig.ActionList)
            {
                
            }

            return IStoryResultCode.OK;
        }
    }
}