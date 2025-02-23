﻿// <copyright file="ISpanBuilder.cs" company="OpenTelemetry Authors">
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

namespace OpenTelemetry.Trace
{
    using System.Collections.Generic;

    /// <summary>
    /// Span builder.
    /// </summary>
    public interface ISpanBuilder
    {
        /// <summary>
        /// Set the sampler for the span.
        /// </summary>
        /// <param name="sampler">Sampler to use to build span.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetSampler(ISampler sampler);

        /// <summary>
        /// Sets the <see cref="ISpan"/> to use as a parent for the new span.
        /// Any parent that was set previously will be discarded.
        /// </summary>
        /// <param name="parent"><see cref="ISpan"/> to set.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetParent(ISpan parent);

        /// <summary>
        /// Sets the remote <see cref="SpanContext"/> to use as a parent for the new span.
        /// Any parent that was set previously will be discarded.
        /// </summary>
        /// <param name="remoteParent"><see cref="SpanContext"/> to set.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetParent(SpanContext remoteParent);

        /// <summary>
        /// Makes the result span to become a root <see cref="ISpan"/> for a new trace.
        /// Any parent that was set previously will be discarded.
        /// </summary>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetNoParent();

        /// <summary>
        /// Set <see cref="SpanKind"/> on the span.
        /// </summary>
        /// <param name="spanKind"><see cref="SpanKind"/> to set.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetSpanKind(SpanKind spanKind);

        /// <summary>
        /// Set the <see cref="Link"/> on the span.
        /// </summary>
        /// <param name="spanContext"><see cref="Link"/> context to set on span.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder AddLink(SpanContext spanContext);

        /// <summary>
        /// Set the <see cref="Link"/> on the span.
        /// </summary>
        /// <param name="link"><see cref="Link"/> to set on span.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder AddLink(ILink link);

        /// <summary>
        /// Set the <see cref="Link"/> on the span.
        /// </summary>
        /// <param name="context"><see cref="Link"/> context.</param>
        /// <param name="attributes">The attributes of the <see cref="Link"/>.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder AddLink(SpanContext context, IDictionary<string, IAttributeValue> attributes);

        /// <summary>
        /// Set the record events value.
        /// </summary>
        /// <param name="recordEvents">Value indicating whether to record span.</param>
        /// <returns>This span builder for chaining.</returns>
        ISpanBuilder SetRecordEvents(bool recordEvents);

        /// <summary>
        /// Starts the span.
        /// </summary>
        /// <returns>Span that was just started.</returns>
        ISpan StartSpan();
    }
}
