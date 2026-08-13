using System.Reflection;
using Xunit;

namespace GestionInterventionsDemo_UnitTests;

public sealed class AssemblySmokeTests
{
    [Fact]
    public void ProductionAssemblyCanBeLoaded()
    {
        var assembly = Assembly.Load("GestionInterventions.Domain");

        Assert.Equal("GestionInterventions.Domain", assembly.GetName().Name);
    }
}
