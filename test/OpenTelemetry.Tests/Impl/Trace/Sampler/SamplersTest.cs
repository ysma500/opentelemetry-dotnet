﻿// <copyright file="SamplersTest.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Trace.Sampler.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using OpenTelemetry.Trace.Internal;
    using OpenTelemetry.Trace.Test;
    using Xunit;

    public class SamplersTest
    {
        private static readonly String SPAN_NAME = "MySpanName";
        private static readonly int NUM_SAMPLE_TRIES = 1000;
        private readonly IRandomGenerator random = new RandomGenerator(1234);
        private readonly TraceId traceId;
        private readonly SpanId parentSpanId;
        private readonly SpanId spanId;
        private readonly SpanContext sampledSpanContext;
        private readonly SpanContext notSampledSpanContext;
        private readonly ILink sampledLink;

        public SamplersTest()
        {
            traceId = TraceId.GenerateRandomId(random);
            parentSpanId = SpanId.GenerateRandomId(random);
            spanId = SpanId.GenerateRandomId(random);
            sampledSpanContext = SpanContext.Create(traceId, parentSpanId, TraceOptions.Builder().SetIsSampled(true).Build(), Tracestate.Empty);
            notSampledSpanContext = SpanContext.Create(traceId, parentSpanId, TraceOptions.Default, Tracestate.Empty);
            sampledLink = Link.FromSpanContext(sampledSpanContext);
        }

        [Fact]
        public void AlwaysSampleSampler_AlwaysReturnTrue()
        {
            // Sampled parent.
            Assert.True(
                    Samplers.AlwaysSample
                        .ShouldSample(
                            sampledSpanContext,
                            traceId,
                            spanId,
                            "Another name",
                            null));

            // Not sampled parent.
            Assert.True(
                    Samplers.AlwaysSample
                        .ShouldSample(
                            notSampledSpanContext,
                            traceId,
                            spanId,
                            "Yet another name",
                            null));

        }

        [Fact]
        public void AlwaysSampleSampler_ToString()
        {
            Assert.Equal("AlwaysSampleSampler", Samplers.AlwaysSample.ToString());
        }

        [Fact]
        public void NeverSampleSampler_AlwaysReturnFalse()
        {
            // Sampled parent.
            Assert.False(
                    Samplers.NeverSample
                        .ShouldSample(
                            sampledSpanContext,
                            traceId,
                            spanId,
                            "bar",
                            null));
            // Not sampled parent.
            Assert.False(
                    Samplers.NeverSample
                        .ShouldSample(
                            notSampledSpanContext,
                            traceId,
                            spanId,
                            "quux",
                            null));
        }

        [Fact]
        public void NeverSampleSampler_ToString()
        {
            Assert.Equal("NeverSampleSampler", Samplers.NeverSample.ToString());
        }

        [Fact]
        public void ProbabilitySampler_OutOfRangeHighProbability()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ProbabilitySampler.Create(1.01));
        }

        [Fact]
        public void ProbabilitySampler_OutOfRangeLowProbability()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ProbabilitySampler.Create(-0.00001));
        }


        [Fact]
        public void ProbabilitySampler_DifferentProbabilities_NotSampledParent()
        {
            ISampler neverSample = ProbabilitySampler.Create(0.0);
            AssertSamplerSamplesWithProbability(
                neverSample, notSampledSpanContext, null, 0.0);
            ISampler alwaysSample = ProbabilitySampler.Create(1.0);
            AssertSamplerSamplesWithProbability(
                alwaysSample, notSampledSpanContext, null, 1.0);
            ISampler fiftyPercentSample = ProbabilitySampler.Create(0.5);
            AssertSamplerSamplesWithProbability(
                fiftyPercentSample, notSampledSpanContext, null, 0.5);
            ISampler twentyPercentSample = ProbabilitySampler.Create(0.2);
            AssertSamplerSamplesWithProbability(
                twentyPercentSample, notSampledSpanContext, null, 0.2);
            ISampler twoThirdsSample = ProbabilitySampler.Create(2.0 / 3.0);
            AssertSamplerSamplesWithProbability(
                twoThirdsSample, notSampledSpanContext, null, 2.0 / 3.0);
        }

        [Fact]
        public void ProbabilitySampler_DifferentProbabilities_SampledParent()
        {
            ISampler neverSample = ProbabilitySampler.Create(0.0);
            AssertSamplerSamplesWithProbability(
                neverSample, sampledSpanContext, null, 1.0);
            ISampler alwaysSample = ProbabilitySampler.Create(1.0);
            AssertSamplerSamplesWithProbability(
                alwaysSample, sampledSpanContext, null, 1.0);
            ISampler fiftyPercentSample = ProbabilitySampler.Create(0.5);
            AssertSamplerSamplesWithProbability(
                fiftyPercentSample, sampledSpanContext, null, 1.0);
            ISampler twentyPercentSample = ProbabilitySampler.Create(0.2);
            AssertSamplerSamplesWithProbability(
                twentyPercentSample, sampledSpanContext, null, 1.0);
            ISampler twoThirdsSample = ProbabilitySampler.Create(2.0 / 3.0);
            AssertSamplerSamplesWithProbability(
                twoThirdsSample, sampledSpanContext, null, 1.0);
        }

        [Fact]
        public void ProbabilitySampler_DifferentProbabilities_SampledParentLink()
        {
            ISampler neverSample = ProbabilitySampler.Create(0.0);
            AssertSamplerSamplesWithProbability(
                neverSample, notSampledSpanContext, new List<ILink>() { sampledLink }, 1.0);
            ISampler alwaysSample = ProbabilitySampler.Create(1.0);
            AssertSamplerSamplesWithProbability(
                alwaysSample, notSampledSpanContext, new List<ILink>() { sampledLink }, 1.0);
            ISampler fiftyPercentSample = ProbabilitySampler.Create(0.5);
            AssertSamplerSamplesWithProbability(
                fiftyPercentSample, notSampledSpanContext, new List<ILink>() { sampledLink }, 1.0);
            ISampler twentyPercentSample = ProbabilitySampler.Create(0.2);
            AssertSamplerSamplesWithProbability(
                twentyPercentSample, notSampledSpanContext, new List<ILink>() { sampledLink }, 1.0);
            ISampler twoThirdsSample = ProbabilitySampler.Create(2.0 / 3.0);
            AssertSamplerSamplesWithProbability(
                twoThirdsSample, notSampledSpanContext, new List<ILink>() { sampledLink }, 1.0);
        }

        [Fact]
        public void ProbabilitySampler_SampleBasedOnTraceId()
        {
            ISampler defaultProbability = ProbabilitySampler.Create(0.0001);
            // This traceId will not be sampled by the ProbabilitySampler because the first 8 bytes as long
            // is not less than probability * Long.MAX_VALUE;
            var notSampledtraceId =
                TraceId.FromBytes(
                    new byte[] 
                    {
                      0x8F,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                    });
            Assert.False(
                    defaultProbability.ShouldSample(
                        null,
                        notSampledtraceId,
                        SpanId.GenerateRandomId(random),
                        SPAN_NAME,
                        null));
            // This traceId will be sampled by the ProbabilitySampler because the first 8 bytes as long
            // is less than probability * Long.MAX_VALUE;
            var sampledtraceId =
                TraceId.FromBytes(
                    new byte[] 
                    {
                      0x00,
                      0x00,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0xFF,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                      0,
                    });
            Assert.True(
                    defaultProbability.ShouldSample(
                        null,
                        sampledtraceId,
                        SpanId.GenerateRandomId(random),
                        SPAN_NAME,
                        null));
        }

        [Fact]
        public void ProbabilitySampler_getDescription()
        {
            Assert.Equal(String.Format("ProbabilitySampler({0:F6})", 0.5), ProbabilitySampler.Create(0.5).Description);
        }

        [Fact]
        public void ProbabilitySampler_ToString()
        {
            var result = ProbabilitySampler.Create(0.5).ToString();
            Assert.Contains($"0{CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}5", result);
        }

        // Applies the given sampler to NUM_SAMPLE_TRIES random traceId/spanId pairs.
        private static void AssertSamplerSamplesWithProbability(
            ISampler sampler, SpanContext parent, List<ILink> links, double probability)
        {
            var random = new RandomGenerator(1234);
            var count = 0; // Count of spans with sampling enabled
            for (var i = 0; i < NUM_SAMPLE_TRIES; i++)
            {
                if (sampler.ShouldSample(
                    parent,
                    TraceId.GenerateRandomId(random),
                    SpanId.GenerateRandomId(random),
                    SPAN_NAME,
                    links))
                {
                    count++;
                }
            }
            var proportionSampled = (double)count / NUM_SAMPLE_TRIES;
            // Allow for a large amount of slop (+/- 10%) in number of sampled traces, to avoid flakiness.
            Assert.True(proportionSampled < probability + 0.1 && proportionSampled > probability - 0.1);
        }
    }
}
