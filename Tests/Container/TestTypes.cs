namespace UnitTests.Container;

internal interface ITestInterface { }

internal class TestClassWithInterface : ITestInterface { }

internal class TestDerivedClassWithInterfaceA : TestClassWithInterface { }

internal class TestDerivedClassWithInterfaceB : TestClassWithInterface { }

internal class TestClassWithoutInterface { }

internal class TestDerivedClassWithoutInterface : TestClassWithoutInterface { }

internal interface IGenericInterface<T> { }

internal class GenericA<T> : IGenericInterface<T> { }

internal class GenericB<T> { }