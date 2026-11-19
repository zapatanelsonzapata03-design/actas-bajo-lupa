using AuditaActas.Aplicacion;

namespace AuditaActas.Tests;

public class EstadisticaTests
{
    [Fact]
    public void Mediana_ImparYPar()
    {
        Assert.Equal(2.0, Estadistica.Mediana(new List<double> { 1, 3, 2 }));
        Assert.Equal(2.5, Estadistica.Mediana(new List<double> { 1, 2, 3, 4 }));
    }

    [Fact]
    public void ZRobusto_MarcaElOutlier()
    {
        // mediana = 3, MAD = 1 * 1.4826 -> z(100) = 97 / 1.4826 ≈ 65.4
        var z = Estadistica.ZRobusto(new List<double> { 1, 2, 3, 4, 100 });
        Assert.True(z[4] > 60);
        Assert.True(Math.Abs(z[2]) < 1e-9);
    }

    [Fact]
    public void ZRobusto_SinVariacion_DevuelveCeros()
    {
        var z = Estadistica.ZRobusto(new List<double> { 5, 5, 5, 5 });
        Assert.All(z, v => Assert.Equal(0.0, v));
    }
}
