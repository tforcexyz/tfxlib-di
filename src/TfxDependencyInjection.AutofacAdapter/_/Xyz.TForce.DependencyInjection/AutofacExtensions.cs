// Copyright 2017 T-Force Xyz
// Please refer to LICENSE & CONTRIB files in the project root for license information.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Xyz.TForce.DependencyInjection
{

  public static class AutofacExtensions
  {

    public static void RegisterAssembly(this ContainerBuilder builder, Assembly assembly)
    {
      Type[] autoInjectTypes = assembly.ExportedTypes
          .Where(x => { return Attribute.IsDefined(x, typeof(AutoInjectAttribute)); })
          .ToArray();

      foreach (Type type in autoInjectTypes)
      {
        AutoInjectAttribute autoInjectParameter = (AutoInjectAttribute)Attribute.GetCustomAttribute(type, typeof(AutoInjectAttribute));
        Type[] serviceTypes = autoInjectParameter.ServiceTypes;
        if (serviceTypes.IsTruthy())
        {
          switch (autoInjectParameter.LifetimeScope)
          {
            case LifetimeScope.Scoped:
              _ = builder.RegisterType(type)
                .As(serviceTypes)
                .InstancePerLifetimeScope();
              break;
            case LifetimeScope.Singleton:
              _ = builder.RegisterType(type)
                .As(serviceTypes)
                .SingleInstance();
              break;
            case LifetimeScope.Transient:
              _ = builder.RegisterType(type)
                .As(serviceTypes)
                .InstancePerDependency();
              break;
          }
        }
        else
        {
          switch (autoInjectParameter.LifetimeScope)
          {
            case LifetimeScope.Scoped:
              _ = builder.RegisterType(type)
                .InstancePerLifetimeScope();
              break;
            case LifetimeScope.Singleton:
              _ = builder.RegisterType(type)
                .SingleInstance();
              break;
            case LifetimeScope.Transient:
              _ = builder.RegisterType(type)
                .InstancePerDependency();
              break;
          }
        }
      }
    }
  }
}
