using AuditaActas.Aplicacion;

namespace AuditaActas.Tests;

public class GeneradorTests
{
    [Fact]
    public void MismaSemilla_GeneraLosMismosDatos()
    {
        var generador = new GeneradorService();
        var p = new ParametrosSimulacion(Mesas: 500, Semilla: 7);

        var a = generador.Generar(p);
        var b = generador.Generar(p);
        CadenaHashService.Encadenar(a);
        CadenaHashService.Encadenar(b);

        Assert.Equal(a[^1].Hash, b[^1].Hash);
    }

    [Fact]
    public void TodasLasActasGeneradas_SonConsistentes()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 1000, Semilla: 3));
        Assert.All(actas, a => Assert.Empty(a.Validar()));
    }

    [Fact]
    public void ProporcionDeFraude_EsAproximadamenteLaPedida()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 2000, PorcentajeFraude: 0.05, Semilla: 42));
        int fraudes = actas.Count(a => a.EsFraudeSimulado);
        Assert.InRange(fraudes, 60, 140);
    }
}
