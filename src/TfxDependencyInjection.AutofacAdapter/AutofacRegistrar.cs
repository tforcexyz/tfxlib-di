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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;

namespace Xyz.TForce.DependencyInjection.AutofacAdapter
{

  public class AutofacRegistrar : IRegistrar
  {

    private readonly ContainerBuilder _builder;
    private IContainer _container;

    public AutofacRegistrar()
    {
      _builder = new ContainerBuilder();
    }

    public void Register<TRegister>(RegisterOptions options = null)
        where TRegister : class
    {
      Register(typeof(TRegister), options);
    }

    public void Register(Type tRegister, RegisterOptions options = null)
    {
      _ = _builder.RegisterType(tRegister);
      IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder = _builder.RegisterType(tRegister);
      if (options != null)
      {
        switch (options.LifetimeScope)
        {
          case LifetimeScope.Singleton:
            _ = registrationBuilder.SingleInstance();
            break;
          case LifetimeScope.Scoped:
            _ = registrationBuilder.InstancePerLifetimeScope();
            break;
          case LifetimeScope.Transient:
            _ = registrationBuilder.InstancePerDependency();
            break;
        }
      }
    }

    public void Register<TRegister>(TRegister instance, RegisterOptions options = null)
        where TRegister : class
    {
      IRegistrationBuilder<TRegister, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder = _builder.RegisterInstance(instance);
      if (options != null)
      {
        switch (options.LifetimeScope)
        {
          case LifetimeScope.Singleton:
            _ = registrationBuilder.SingleInstance();
            break;
          case LifetimeScope.Scoped:
            _ = registrationBuilder.InstancePerLifetimeScope();
            break;
          case LifetimeScope.Transient:
            _ = registrationBuilder.InstancePerDependency();
            break;
        }
      }
    }

    public void Register<TRegister, TResolve>(RegisterOptions options = null)
        where TResolve : TRegister
    {
      Register(typeof(TRegister), typeof(TResolve), options);
    }

    public void Register(Type tRegister, Type tResolve, RegisterOptions options = null)
    {
      IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder = _builder.RegisterType(tResolve).As(tRegister);
      if (options != null)
      {
        switch (options.LifetimeScope)
        {
          case LifetimeScope.Singleton:
            _ = registrationBuilder.SingleInstance();
            break;
          case LifetimeScope.Scoped:
            _ = registrationBuilder.InstancePerLifetimeScope();
            break;
          case LifetimeScope.Transient:
            _ = registrationBuilder.InstancePerDependency();
            break;
        }
      }
    }

    public void RegisterModule<TModule>()
        where TModule : IModule
    {
      ContainerBuilder moduleBuilder = new ContainerBuilder();
      _ = moduleBuilder.RegisterInstance<IRegistrar>(this);
      _ = moduleBuilder.RegisterType<TModule>();
      IContainer moduleContainer = moduleBuilder.Build();
      TModule module = moduleContainer.Resolve<TModule>();
      module.Initialize(this);
    }

    public void RegisterModule(IModule module)
    {
      module.Initialize(this);
    }

    public void RegisterModule(Type tModule)
    {
      Type iocModuleInterface = typeof(IModule);
      if (!iocModuleInterface.IsAssignableFrom(tModule))
      {
        throw new InvalidOperationException();
      }
      ContainerBuilder moduleBuilder = new ContainerBuilder();
      _ = moduleBuilder.RegisterInstance<IRegistrar>(this);
      _ = moduleBuilder.RegisterType(tModule).As<IModule>();
      IContainer moduleContainer = moduleBuilder.Build();
      IModule module = moduleContainer.Resolve<IModule>();
      module.Initialize(this);
    }

    public TRegister Resolve<TRegister>()
    {
      if (_container == null)
      {
        _container = _builder.Build();
      }
      return _container.Resolve<TRegister>();
    }

    public TRegister SafeResolve<TRegister>()
    {
      if (_container == null)
      {
        _container = _builder.Build();
      }

      try
      {
        TRegister result = _container.Resolve<TRegister>();
        return result;
      }
      catch (Exception)
      {
        // ignore
      }
      return default(TRegister);
    }

    public IEnumerable<TRegister> ResolveMany<TRegister>()
    {
      if (_container == null)
      {
        _container = _builder.Build();
      }
      return _container.Resolve<IEnumerable<TRegister>>();
    }

    public IEnumerable<TRegister> SafeResolveMany<TRegister>()
    {
      if (_container == null)
      {
        _container = _builder.Build();
      }
      try
      {
        IEnumerable<TRegister> results = _container.Resolve<IEnumerable<TRegister>>();
        return results;
      }
      catch (Exception)
      {
        // ignore
      }
      return Enumerable.Empty<TRegister>();
    }

    public object GetContainer()
    {
      if (_container == null)
      {
        _container = _builder.Build();
      }
      return _container;
    }
  }
}
