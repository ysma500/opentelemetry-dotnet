﻿// <copyright file="NoopTagsTest.cs" company="OpenTelemetry Authors">
// Copyright 2018, OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using OpenTelemetry.Trace;

namespace OpenTelemetry.Tags.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTelemetry.Internal;
    using Xunit;

    public class NoopTagsTest
    {
        private static readonly TagKey KEY = TagKey.Create("key");
        private static readonly TagValue VALUE = TagValue.Create("value");

        private static readonly ITagContext TAG_CONTEXT = new TestTagContext();
   


        [Fact]
        public void NoopTagsComponent()
        {
            Assert.Same(NoopTags.NoopTagger, NoopTags.NewNoopTagsComponent().Tagger);
            Assert.Equal(NoopTags.NoopTagPropagationComponent, NoopTags.NewNoopTagsComponent().TagPropagationComponent);
        }

        [Fact]
        public void NoopTagger()
        {
            var noopTagger = NoopTags.NoopTagger;
            Assert.Same(NoopTags.NoopTagContext, noopTagger.Empty);
            Assert.Same(NoopTags.NoopTagContext, noopTagger.CurrentTagContext);
            Assert.Same(NoopTags.NoopTagContextBuilder, noopTagger.EmptyBuilder);
            Assert.Same(NoopTags.NoopTagContextBuilder, noopTagger.ToBuilder(TAG_CONTEXT));
            Assert.Same(NoopTags.NoopTagContextBuilder, noopTagger.CurrentBuilder);
            Assert.Same(NoopScope.Instance, noopTagger.WithTagContext(TAG_CONTEXT));
        }

        [Fact]
        public void NoopTagger_ToBuilder_DisallowsNull()
        {
            var noopTagger = NoopTags.NoopTagger;
            Assert.Throws<ArgumentNullException>(() => noopTagger.ToBuilder(null));
        }

        [Fact]
        public void NoopTagger_WithTagContext_DisallowsNull()
        {
            var noopTagger = NoopTags.NoopTagger;
            Assert.Throws<ArgumentNullException>(() => noopTagger.WithTagContext(null));
        }

        [Fact]
        public void NoopTagContextBuilder()
        {
            Assert.Same(NoopTags.NoopTagContext, NoopTags.NoopTagContextBuilder.Build());
            Assert.Same(NoopTags.NoopTagContext, NoopTags.NoopTagContextBuilder.Put(KEY, VALUE).Build());
            Assert.Same(NoopScope.Instance, NoopTags.NoopTagContextBuilder.BuildScoped());
            Assert.Same(NoopScope.Instance, NoopTags.NoopTagContextBuilder.Put(KEY, VALUE).BuildScoped());
        }

        [Fact]
        public void NoopTagContextBuilder_Put_DisallowsNullKey()
        {
            var noopBuilder = NoopTags.NoopTagContextBuilder;
            Assert.Throws<ArgumentNullException>(() => noopBuilder.Put(null, VALUE));
        }

        [Fact]
        public void NoopTagContextBuilder_Put_DisallowsNullValue()
        {
            var noopBuilder = NoopTags.NoopTagContextBuilder;
            Assert.Throws<ArgumentNullException>(() => noopBuilder.Put(KEY, null));
        }

        [Fact]
        public void NoopTagContextBuilder_Remove_DisallowsNullKey()
        {
            var noopBuilder = NoopTags.NoopTagContextBuilder;
            Assert.Throws<ArgumentNullException>(() => noopBuilder.Remove(null));
        }

        [Fact]
        public void NoopTagContext()
        {
            Assert.Empty(NoopTags.NoopTagContext.ToList());
        }

        [Fact]
        public void NoopTagPropagationComponent()
        {
            Assert.Same(NoopTags.NoopTagContextBinarySerializer, NoopTags.NoopTagPropagationComponent.BinarySerializer);
        }

        [Fact]
        public void NoopTagContextBinarySerializer()
        {
            Assert.Equal(new byte[0], NoopTags.NoopTagContextBinarySerializer.ToByteArray(TAG_CONTEXT));
            Assert.Equal(NoopTags.NoopTagContext, NoopTags.NoopTagContextBinarySerializer.FromByteArray(new byte[5]));
        }

        [Fact]
        public void NoopTagContextBinarySerializer_ToByteArray_DisallowsNull()
        {
            var noopSerializer = NoopTags.NoopTagContextBinarySerializer;
            Assert.Throws<ArgumentNullException>(() => noopSerializer.ToByteArray(null));
        }

        [Fact]
        public void NoopTagContextBinarySerializer_FromByteArray_DisallowsNull()
        {
            var noopSerializer = NoopTags.NoopTagContextBinarySerializer;
            Assert.Throws<ArgumentNullException>(() => noopSerializer.FromByteArray(null));
        }

        class TestTagContext : ITagContext
        {
            public IEnumerator<Tag> GetEnumerator()
            {
                var list = new List<Tag>() { Tag.Create(KEY, VALUE) };
                return list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
