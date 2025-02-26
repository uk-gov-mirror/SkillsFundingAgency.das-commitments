// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using StructureMap;
using SFA.DAS.Commitments.Infrastructure.Configuration;
using SFA.DAS.Reservations.Api.Client.DependencyResolution;

namespace SFA.DAS.Commitments.Api.DependencyResolution
{
    public static class IoC
    {
        public const string ServiceName = "SFA.DAS.Commitments";
        public const string ServiceVersion = "1.0";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<EncodingRegistry>();
                c.AddRegistry<NServiceBusRegistry>();    
                c.AddRegistry<ReservationsApiClientRegistry>();
                c.Policies.Add<CurrentDatePolicy>();
                c.AddRegistry<EventsUpgradeRegistry>();                
            });
        }
    }
}
