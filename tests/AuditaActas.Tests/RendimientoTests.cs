using AuditaActas.Aplicacion;

namespace AuditaActas.Tests;

/// <summary>Evidencia de la meta de calidad: 10.000 actas auditadas en menos de 10 segundos.</summary>
public class RendimientoTests
{
    [Fact]
    public void AuditarDiezMilActas_TardaMenosDeDiezSegundos()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 10000, PorcentajeFraude: 0.05, Semilla: 7));
        CadenaHashService.Encadenar(actas);

        var reloj = System.Diagnostics.Stopwatch.StartNew();
        var resultados = new AuditoriaService().Auditar(actas);
        reloj.Stop();

        Assert.Equal(10000, resultados.Count);
        Assert.True(reloj.ElapsedMilliseconds < 10_000, $"Tardó {reloj.ElapsedMilliseconds} ms");
    }
}
