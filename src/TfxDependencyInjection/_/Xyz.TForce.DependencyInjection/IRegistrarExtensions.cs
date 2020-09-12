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

namespace Xyz.TForce.DependencyInjection
{

  public static class IRegistrarExtensions
  {

    public static void RegisterAutoInjectClasses(this IRegistrar registrar, Assembly assembly)
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
          foreach (Type serviceType in serviceTypes)
          {
            switch (autoInjectParameter.LifetimeScope)
            {
              case LifetimeScope.Scope:
                registrar.Register(serviceType, type, new RegisterOptions { LifetimeScope = LifetimeScope.Scope });
                break;
              case LifetimeScope.Singleton:
                registrar.Register(serviceType, type, new RegisterOptions { LifetimeScope = LifetimeScope.Singleton });
                break;
              case LifetimeScope.Transient:
                registrar.Register(serviceType, type, new RegisterOptions { });
                break;
            }
          }
        }
        else
        {
          switch (autoInjectParameter.LifetimeScope)
          {
            case LifetimeScope.Scope:
              registrar.Register(type, new RegisterOptions { LifetimeScope = LifetimeScope.Scope });
              break;
            case LifetimeScope.Singleton:
              registrar.Register(type, new RegisterOptions { LifetimeScope = LifetimeScope.Singleton });
              break;
            case LifetimeScope.Transient:
              registrar.Register(type, new RegisterOptions { LifetimeScope = LifetimeScope.Transient });
              break;
          }
        }
      }
    }
  }
}
