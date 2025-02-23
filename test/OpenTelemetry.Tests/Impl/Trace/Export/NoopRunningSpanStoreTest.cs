﻿// <copyright file="NoopRunningSpanStoreTest.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Trace.Export.Test
{
    using System;
    using Xunit;

    public class NoopRunningSpanStoreTest
    {
        private readonly IRunningSpanStore runningSpanStore = ((NoopExportComponent)ExportComponent.NewNoopExportComponent).RunningSpanStore;

        [Fact]
        public void NoopRunningSpanStore_GetSummary()
        {
            var summary = runningSpanStore.Summary;
            Assert.Empty(summary.PerSpanNameSummary);
        }

        [Fact]
        public void NoopRunningSpanStore_GetRunningSpans_DisallowsNull()
        {
            Assert.Throws<ArgumentNullException>(() => runningSpanStore.GetRunningSpans(null));
        }

        [Fact]
        public void NoopRunningSpanStore_GetRunningSpans()
        {
            var runningSpans = runningSpanStore.GetRunningSpans(RunningSpanStoreFilter.Create("TestSpan", 0));
            Assert.Empty(runningSpans);
        }
    }
}
