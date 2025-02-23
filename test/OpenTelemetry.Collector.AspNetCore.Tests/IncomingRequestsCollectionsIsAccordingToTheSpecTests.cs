﻿// <copyright file="IncomingRequestsCollectionsIsAccordingToTheSpecTests.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Collector.AspNetCore.Tests
{
    using Xunit;
    using Microsoft.AspNetCore.Mvc.Testing;
    using TestApp.AspNetCore._2._0;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTelemetry.Trace;
    using OpenTelemetry.Trace.Config;
    using OpenTelemetry.Trace.Internal;
    using Moq;
    using Microsoft.AspNetCore.TestHost;
    using System;
    using Microsoft.AspNetCore.Http;

    public class IncomingRequestsCollectionsIsAccordingToTheSpecTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public IncomingRequestsCollectionsIsAccordingToTheSpecTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;

        }

        public class TestCallbackMiddlewareImpl : CallbackMiddleware.CallbackMiddlewareImpl
        {

            public override async Task<bool> ProcessAsync(HttpContext context)
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync("empty");
                return false;
            }
        }

        [Fact]
        public async Task SuccesfulTemplateControllerCallGeneratesASpan()
        {
            var startEndHandler = new Mock<IStartEndHandler>();
            var tracer = new Tracer(new RandomGenerator(), startEndHandler.Object, new TraceConfig(), null);

            // Arrange
            using (var client = this.factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices((IServiceCollection services) =>
                    {
                        services.AddSingleton<CallbackMiddleware.CallbackMiddlewareImpl>(new TestCallbackMiddlewareImpl());
                        services.AddSingleton<ITracer>(tracer);
                    }))
                .CreateClient())
            {

                try
                {
                    // Act
                    var response = await client.GetAsync("/api/values");
                }
                catch (Exception)
                {
                    // ignore errors
                }

                for (var i = 0; i < 10; i++)
                {
                    if (startEndHandler.Invocations.Count == 2)
                    {
                        break;
                    }

                    // We need to let End callback execute as it is executed AFTER response was returned.
                    // In unit tests environment there may be a lot of parallel unit tests executed, so 
                    // giving some breezing room for the End callback to complete
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            Assert.Equal(2, startEndHandler.Invocations.Count); // begin and end was called
            var spanData = ((Span)startEndHandler.Invocations[0].Arguments[0]).ToSpanData();

            Assert.Equal(SpanKind.Server, spanData.Kind);
            Assert.Equal(AttributeValue.StringAttributeValue("/api/values"), spanData.Attributes.AttributeMap["http.path"]);
            Assert.Equal(AttributeValue.LongAttributeValue(503), spanData.Attributes.AttributeMap["http.status_code"]);
        }
    }
}
