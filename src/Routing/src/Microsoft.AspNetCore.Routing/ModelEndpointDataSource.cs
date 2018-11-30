// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Routing
{
    internal class ModelEndpointDataSource : EndpointDataSource
    {
        private List<EndpointConventions> _endpointConventions;

        public ModelEndpointDataSource()
        {
            _endpointConventions = new List<EndpointConventions>();
        }

        public IEndpointConventions AddEndpointBuilder(EndpointBuilder endpointBuilder)
        {
            var builder = new EndpointConventions(endpointBuilder);
            _endpointConventions.Add(builder);

            return builder;
        }

        public override IChangeToken GetChangeToken()
        {
            return NullChangeToken.Singleton;
        }

        public override IReadOnlyList<Endpoint> Endpoints => _endpointConventions.Select(e => e.Build()).ToArray();

        // for testing
        internal IEnumerable<EndpointBuilder> EndpointBuilders => _endpointConventions.Select(b => b.EndpointBuilder);

        private class EndpointConventions : IEndpointConventions
        {
            internal EndpointBuilder EndpointBuilder { get; }

            private readonly List<Action<EndpointBuilder>> _conventions;

            public EndpointConventions(EndpointBuilder endpointBuilder)
            {
                EndpointBuilder = endpointBuilder;
                _conventions = new List<Action<EndpointBuilder>>();
            }

            public void Apply(Action<EndpointBuilder> convention)
            {
                _conventions.Add(convention);
            }

            public Endpoint Build()
            {
                foreach (var convention in _conventions)
                {
                    convention(EndpointBuilder);
                }

                return EndpointBuilder.Build();
            }
        }
    }
}
