using System;
using System.Linq;
using DependencyInjection;
using NUnit.Framework;

namespace Tests.Runtime
{
    [TestFixture]
    public class DIContainerTests
    {
        private DIContainer _container;

        [SetUp]
        public void Setup()
        {
            _container = new DIContainer();
        }

        [TearDown]
        public void TearDown()
        {
            _container?.Dispose();
        }

        #region Registration Tests

        [Test]
        public void Register_Service_Should_RegisterSuccessfully()
        {
            // Arrange & Act
            _container.Register<ITestService, TestService>();

            // Assert
            Assert.IsTrue(_container.IsRegistered<ITestService>());
        }

        [Test]
        public void Register_WithSingleton_Should_ReturnSameInstance()
        {
            // Arrange
            _container.Register<ITestService, TestService>(Lifetime.Singleton);

            // Act
            var instance1 = _container.Resolve<ITestService>();
            var instance2 = _container.Resolve<ITestService>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void Register_WithTransient_Should_ReturnDifferentInstances()
        {
            // Arrange
            _container.Register<ITestService, TestService>(Lifetime.Transient);

            // Act
            var instance1 = _container.Resolve<ITestService>();
            var instance2 = _container.Resolve<ITestService>();

            // Assert
            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void RegisterInstance_Should_UseSameInstance()
        {
            // Arrange
            var instance = new TestService();
            _container.RegisterInstance<ITestService>(instance);

            // Act
            var resolved = _container.Resolve<ITestService>();

            // Assert
            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void RegisterFactory_Should_UseFactory()
        {
            // Arrange
            bool factoryCalled = false;
            _container.RegisterFactory<ITestService>(c =>
            {
                factoryCalled = true;
                return new TestService();
            });

            // Act
            _container.Resolve<ITestService>();

            // Assert
            Assert.IsTrue(factoryCalled);
        }

        #endregion

        #region Resolution Tests

        [Test]
        public void Resolve_UnregisteredService_Should_ThrowException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                _container.Resolve<ITestService>();
            });
        }

        [Test]
        public void Resolve_WithDependencies_Should_InjectDependencies()
        {
            // Arrange
            _container.Register<ITestService, TestService>();
            _container.Register<IServiceWithDependency, ServiceWithDependency>();

            // Act
            var service = _container.Resolve<IServiceWithDependency>();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsNotNull((service as ServiceWithDependency)?.TestService);
        }

        [Test]
        public void Resolve_CircularDependency_Should_ThrowException()
        {
            // Arrange
            _container.Register<IServiceA, ServiceA>();
            _container.Register<IServiceB, ServiceB>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
            {
                _container.Resolve<IServiceA>();
            });
        }

        #endregion

        #region Injection Tests

        [Test]
        public void InjectProperties_Should_InjectPublicProperty()
        {
            // Arrange
            _container.Register<ITestService, TestService>();
            var obj = new TestObjectWithProperty();

            // Act
            typeof(DIContainer)
                .GetMethod("InjectProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_container, new object[] { obj });

            // Assert
            Assert.IsNotNull(obj.TestService);
        }

        [Test]
        public void InjectProperties_Should_InjectPrivateField()
        {
            // Arrange
            _container.Register<ITestService, TestService>();
            var obj = new TestObjectWithField();

            // Act
            typeof(DIContainer)
                .GetMethod("InjectProperties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(_container, new object[] { obj });

            // Assert
            Assert.IsNotNull(obj.GetService());
        }

        #endregion

        #region Scopes Tests

        [Test]
        public void CreateScope_Should_ReturnNewScope()
        {
            // Act
            var scope = _container.CreateScope();

            // Assert
            Assert.IsNotNull(scope);
            Assert.AreNotSame(_container, scope);
        }

        [Test]
        public void Scoped_WithinSameScope_Should_ReturnSameInstance()
        {
            // Arrange
            _container.Register<ITestService, TestService>(Lifetime.Scoped);

            using (var scope = _container.CreateScope())
            {
                // Act
                var instance1 = scope.Resolve<ITestService>();
                var instance2 = scope.Resolve<ITestService>();

                // Assert
                Assert.AreSame(instance1, instance2);
            }
        }

        [Test]
        public void Scoped_InDifferentScopes_Should_ReturnDifferentInstances()
        {
            // Arrange
            _container.Register<ITestService, TestService>(Lifetime.Scoped);

            ITestService instance1, instance2;

            // Act
            using (var scope1 = _container.CreateScope())
            {
                instance1 = scope1.Resolve<ITestService>();
            }

            using (var scope2 = _container.CreateScope())
            {
                instance2 = scope2.Resolve<ITestService>();
            }

            // Assert
            Assert.AreNotSame(instance1, instance2);
        }

        [Test]
        public void Dispose_ScopedService_Should_CallDispose()
        {
            // Arrange
            _container.Register<IDisposableService, DisposableService>(Lifetime.Scoped);
            DisposableService instance;

            // Act
            using (var scope = _container.CreateScope())
            {
                instance = scope.Resolve<IDisposableService>() as DisposableService;
            }

            // Assert
            Assert.IsTrue(instance.IsDisposed);
        }

        #endregion

        #region Validation Tests

        [Test]
        public void IsRegistered_RegisteredService_Should_ReturnTrue()
        {
            // Arrange
            _container.Register<ITestService, TestService>();

            // Act
            bool isRegistered = _container.IsRegistered<ITestService>();

            // Assert
            Assert.IsTrue(isRegistered);
        }

        [Test]
        public void IsRegistered_UnregisteredService_Should_ReturnFalse()
        {
            // Act
            bool isRegistered = _container.IsRegistered<ITestService>();

            // Assert
            Assert.IsFalse(isRegistered);
        }

        [Test]
        public void GetRegisteredServices_Should_ReturnAllServices()
        {
            // Arrange
            _container.Register<ITestService, TestService>();
            _container.Register<IServiceWithDependency, ServiceWithDependency>();

            // Act
            var services = _container.GetRegisteredServices();

            // Assert
            Assert.Contains(typeof(ITestService), services.ToList());
            Assert.Contains(typeof(IServiceWithDependency), services.ToList());
        }

        #endregion
    }

    #region Test Classes

    public interface ITestService { }
    public class TestService : ITestService { }

    public interface IServiceWithDependency { }
    public class ServiceWithDependency : IServiceWithDependency
    {
        public ITestService TestService { get; }

        public ServiceWithDependency(ITestService testService)
        {
            TestService = testService;
        }
    }

    public interface IServiceA { }
    public interface IServiceB { }

    public class ServiceA : IServiceA
    {
        public ServiceA(IServiceB serviceB) { }
    }

    public class ServiceB : IServiceB
    {
        public ServiceB(IServiceA serviceA) { }
    }

    public class TestObjectWithProperty
    {
        [Inject]
        public ITestService TestService { get; set; }
    }

    public class TestObjectWithField
    {
        [Inject]
        private ITestService _testService;

        public ITestService GetService() => _testService;
    }

    public interface IDisposableService : IDisposable { }
    public class DisposableService : IDisposableService
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    #endregion
}
