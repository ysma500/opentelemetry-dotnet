﻿// <copyright file="QuickStartExampleTest.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Stats.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTelemetry.Stats.Aggregations;
    using OpenTelemetry.Stats.Measures;
    using OpenTelemetry.Tags;
    using Xunit;
    using Xunit.Abstractions;

    public class QuickStartExampleTest
    {
        readonly ITestOutputHelper output;

        public QuickStartExampleTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Main()
        {
            var statsComponent = new StatsComponent();
            var viewManager = statsComponent.ViewManager;
            var statsRecorder = statsComponent.StatsRecorder;
            var tagsComponent = new TagsComponent();
            var tagger = tagsComponent.Tagger;

            var FRONTEND_KEY = TagKey.Create("my.org/keys/frontend");
            var FRONTEND_OS_KEY = TagKey.Create("my.org/keys/frontend/os");
            var FRONTEND_OS_VERSION_KEY = TagKey.Create("my.org/keys/frontend/os/version");

            var VIDEO_SIZE = MeasureLong.Create("my.org/measure/video_size", "size of processed videos", "MBy");

            var VIDEO_SIZE_BY_FRONTEND_VIEW_NAME = ViewName.Create("my.org/views/video_size_byfrontend");
            var VIDEO_SIZE_BY_FRONTEND_VIEW = View.Create(
                                        VIDEO_SIZE_BY_FRONTEND_VIEW_NAME,
                                        "processed video size over time",
                                        VIDEO_SIZE,
                                        Distribution.Create(BucketBoundaries.Create(new List<double>() { 0.0, 256.0, 65536.0 })),
                                        new List<TagKey>() { FRONTEND_KEY});

            var VIDEO_SIZE_ALL_VIEW_NAME = ViewName.Create("my.org/views/video_size_all");
            var VIDEO_SIZE_VIEW_ALL = View.Create(
                            VIDEO_SIZE_ALL_VIEW_NAME,
                            "processed video size over time",
                            VIDEO_SIZE,
                            Distribution.Create(BucketBoundaries.Create(new List<double>() { 0.0, 256.0, 65536.0 })),
                            new List<TagKey>() { });


            var VIDEO_SIZE_TOTAL_VIEW_NAME = ViewName.Create("my.org/views/video_size_total");
            var VIDEO_SIZE_TOTAL = View.Create(
                                  VIDEO_SIZE_TOTAL_VIEW_NAME,
                                  "total video size over time",
                                  VIDEO_SIZE,
                                  Sum.Create(),
                                  new List<TagKey>() { FRONTEND_KEY});

            var VIDEOS_PROCESSED_VIEW_NAME = ViewName.Create("my.org/views/videos_processed");
            var VIDEOS_PROCESSED = View.Create(
                                  VIDEOS_PROCESSED_VIEW_NAME,
                                  "total video processed",
                                  VIDEO_SIZE,
                                  Count.Create(),
                                  new List<TagKey>() { FRONTEND_KEY });

            viewManager.RegisterView(VIDEO_SIZE_VIEW_ALL);
            viewManager.RegisterView(VIDEO_SIZE_BY_FRONTEND_VIEW);
            viewManager.RegisterView(VIDEO_SIZE_TOTAL);
            viewManager.RegisterView(VIDEOS_PROCESSED);

            var context1 = tagger
                .EmptyBuilder
                .Put(FRONTEND_KEY, TagValue.Create("front1"))
                .Build();
            var context2 = tagger
                .EmptyBuilder
                .Put(FRONTEND_KEY, TagValue.Create("front2"))
                .Build();

            long sum = 0;
            for (var i = 0; i < 10; i++)
            {
                sum = sum + (25648 * i);
                if (i % 2 == 0)
                {
                    statsRecorder.NewMeasureMap().Put(VIDEO_SIZE, 25648 * i).Record(context1);
                } else
                {
                    statsRecorder.NewMeasureMap().Put(VIDEO_SIZE, 25648 * i).Record(context2);
                }
            }

            var viewDataByFrontend = viewManager.GetView(VIDEO_SIZE_BY_FRONTEND_VIEW_NAME);
            var viewDataAggMap = viewDataByFrontend.AggregationMap.ToList();
            output.WriteLine(viewDataByFrontend.ToString());

            var viewDataAll = viewManager.GetView(VIDEO_SIZE_ALL_VIEW_NAME);
            var viewDataAggMapAll = viewDataAll.AggregationMap.ToList();
            output.WriteLine(viewDataAll.ToString());

            var viewData1 = viewManager.GetView(VIDEO_SIZE_TOTAL_VIEW_NAME);
            var viewData1AggMap = viewData1.AggregationMap.ToList();
            output.WriteLine(viewData1.ToString());

            var viewData2 = viewManager.GetView(VIDEOS_PROCESSED_VIEW_NAME);
            var viewData2AggMap = viewData2.AggregationMap.ToList();
            output.WriteLine(viewData2.ToString());

            output.WriteLine(sum.ToString());
        }

        [Fact]
        public void Main2()
        {
            var statsComponent = new StatsComponent();
            var viewManager = statsComponent.ViewManager;
            var statsRecorder = statsComponent.StatsRecorder;
            var tagsComponent = new TagsComponent();
            var tagger = tagsComponent.Tagger;

            var FRONTEND_KEY = TagKey.Create("my.org/keys/frontend");
            var FRONTEND_OS_KEY = TagKey.Create("my.org/keys/frontend/os");
            var FRONTEND_OS_VERSION_KEY = TagKey.Create("my.org/keys/frontend/os/version");

            var VIDEO_SIZE = MeasureLong.Create("my.org/measure/video_size", "size of processed videos", "MBy");

            var VIDEO_SIZE_VIEW_NAME = ViewName.Create("my.org/views/video_size_byfrontend");
            var VIDEO_SIZE_VIEW = View.Create(
                                        VIDEO_SIZE_VIEW_NAME,
                                        "processed video size over time",
                                        VIDEO_SIZE,
                                        Distribution.Create(BucketBoundaries.Create(new List<double>() { 0.0, 256.0, 65536.0 })),
                                        new List<TagKey>() { FRONTEND_KEY, FRONTEND_OS_KEY, FRONTEND_OS_VERSION_KEY });


            viewManager.RegisterView(VIDEO_SIZE_VIEW);
    

            var context1 = tagger
                .EmptyBuilder
                .Put(FRONTEND_KEY, TagValue.Create("front1"))
                .Put(FRONTEND_OS_KEY, TagValue.Create("windows"))
                .Build();
            var context2 = tagger
                .EmptyBuilder
                .Put(FRONTEND_KEY, TagValue.Create("front2"))
                .Put(FRONTEND_OS_VERSION_KEY, TagValue.Create("1.1.1"))
                .Build();

            long sum = 0;
            for (var i = 0; i < 10; i++)
            {
                sum = sum + (25648 * i);
                if (i % 2 == 0)
                {
                    statsRecorder.NewMeasureMap().Put(VIDEO_SIZE, 25648 * i).Record(context1);
                }
                else
                {
                    statsRecorder.NewMeasureMap().Put(VIDEO_SIZE, 25648 * i).Record(context2);
                }
            }

            var videoSizeView = viewManager.GetView(VIDEO_SIZE_VIEW_NAME);
            var viewDataAggMap = videoSizeView.AggregationMap.ToList();
            var view = viewManager.AllExportedViews.ToList()[0];
            for (var i = 0; i < view.Columns.Count; i++)
            {
                output.WriteLine(view.Columns[i] + "=" + GetTagValues(i, viewDataAggMap));
            }

            var keys = new List<TagValue>() { TagValue.Create("1.1.1") };

            var results = videoSizeView.AggregationMap.Where((kvp) =>
            {
                foreach (var key in keys)
                {
                    if (!kvp.Key.Values.Contains(key))
                    {
                        return false;
                    }
                }
                return true;
            });

            output.WriteLine(videoSizeView.ToString());

            output.WriteLine(sum.ToString());
        }

        private string GetTagValues(int i, List<KeyValuePair<TagValues, IAggregationData>> viewDataAggMap)
        {
            var result = string.Empty;
            foreach (var kvp in viewDataAggMap)
            {
                var val = kvp.Key.Values[i];
                if (val != null)
                {
                    result += val.AsString;
                }
            }
            return result;
        }

    }
}
