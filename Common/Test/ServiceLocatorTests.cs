/**
The MIT License (MIT)

Copyright (c) 2013 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
using System.Linq;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;

namespace KlaudWerk.Common.Test
{
    public interface ITypeInterface
    {
        void Do();
    }

    public interface IAnotherTypeInterface
    {
        void Do();
    }

    public class TypeImplOne:ITypeInterface
    {
        public void Do()
        {
        }
    }
    public class TypeImplTwo : ITypeInterface
    {
        public void Do()
        {
        }
    }
    public class AnoterImpl:IAnotherTypeInterface
    {
        public void Do()
        {
        }
    }

    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void TestRegisterContainer()
        {
            WindsorContainer container=new WindsorContainer();
            WindsorServiceLocator locator=new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(()=>locator);
            Assert.IsNotNull(ServiceLocator.Current);
            Assert.IsTrue(ReferenceEquals(locator,ServiceLocator.Current));
        }

        [Test]
        public void TestRegisterType()
        {
            WindsorContainer container = new WindsorContainer();
            WindsorServiceLocator locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            Assert.IsNotNull(ServiceLocator.Current);
            Assert.IsTrue(ReferenceEquals(locator, ServiceLocator.Current));
            container.Register(Component.For(typeof (ITypeInterface)).ImplementedBy(typeof (TypeImplOne)));
        }

        [Test]
        public void TestRegisterTypeRetrieveType()
        {
            WindsorContainer container = new WindsorContainer();
            WindsorServiceLocator locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            Assert.IsNotNull(ServiceLocator.Current);
            Assert.IsTrue(ReferenceEquals(locator, ServiceLocator.Current));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplOne)));
            ITypeInterface instance = ServiceLocator.Current.GetInstance<ITypeInterface>();
            Assert.IsNotNull(instance);
        }

        [Test]
        public void TestRegisterMultipleTypesRetrieveTypeByName()
        {
            WindsorContainer container = new WindsorContainer();
            WindsorServiceLocator locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            Assert.IsNotNull(ServiceLocator.Current);
            Assert.IsTrue(ReferenceEquals(locator, ServiceLocator.Current));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplOne)).Named("Impl1One"));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplTwo)).Named("Impl2One"));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplTwo)).Named("Impl2Two"));
            ITypeInterface instance = ServiceLocator.Current.GetInstance<ITypeInterface>();
            Assert.IsNotNull(instance);
            instance = ServiceLocator.Current.GetInstance<ITypeInterface>("Impl1One");
            Assert.IsNotNull(instance);
            ITypeInterface otehrInstance = ServiceLocator.Current.GetInstance<ITypeInterface>("Impl1One");
            Assert.IsNotNull(otehrInstance);
            Assert.IsTrue(ReferenceEquals(instance,otehrInstance));
            instance = ServiceLocator.Current.GetInstance<ITypeInterface>("Impl2One");
            Assert.IsNotNull(instance);
            otehrInstance = ServiceLocator.Current.GetInstance<ITypeInterface>("Impl2Two");
            Assert.IsNotNull(otehrInstance);
            Assert.IsFalse(ReferenceEquals(instance,otehrInstance));
        }

        [Test]
        public void TestRegisterAndRetrieveAllByType()
        {
            WindsorContainer container = new WindsorContainer();
            WindsorServiceLocator locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            Assert.IsNotNull(ServiceLocator.Current);
            Assert.IsTrue(ReferenceEquals(locator, ServiceLocator.Current));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplOne)).Named("Impl1One"));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplTwo)).Named("Impl2One"));
            container.Register(Component.For(typeof(ITypeInterface)).ImplementedBy(typeof(TypeImplTwo)).Named("Impl2Two"));
            container.Register(Component.For(typeof(IAnotherTypeInterface)).ImplementedBy(typeof(AnoterImpl)).Named("AnotherImplOne"));
            container.Register(Component.For(typeof(IAnotherTypeInterface)).ImplementedBy(typeof(AnoterImpl)).Named("AnotherImplWho"));
            IEnumerable<ITypeInterface> instances=ServiceLocator.Current.GetAllInstances<ITypeInterface>();
            Assert.IsNotNull(instances);
            Assert.AreEqual(3,instances.Count());
            IEnumerable<IAnotherTypeInterface> otherInstances = ServiceLocator.Current.GetAllInstances<IAnotherTypeInterface>();
            Assert.IsNotNull(otherInstances);
            Assert.AreEqual(2,otherInstances.Count());
        }

    }
}
