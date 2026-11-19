using AuditaActas.Aplicacion;

namespace AuditaActas.Tests;

/// <summary>Pruebas de aceptación del objetivo general: recall >= 80 % y falsos positivos <= 10 %.</summary>
public class AuditoriaTests
{
    [Fact]
    public void DatasetEstandar_CumpleLasMetasDeDeteccion()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 2000, PorcentajeFraude: 0.05, Semilla: 42));

        var resultados = new AuditoriaService().Auditar(actas);
        var matriz = EvaluacionService.Evaluar(resultados);

        Assert.True(matriz.Recall >= 0.80, $"Recall = {matriz.Recall:P1}");
        Assert.True(matriz.TasaFalsosPositivos <= 0.10, $"Falsos positivos = {matriz.TasaFalsosPositivos:P1}");
    }

    [Fact]
    public void DatosSinFraude_GeneranPocasAlertas()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 1000, PorcentajeFraude: 0.0, Semilla: 11));

        var alertas = new AuditoriaService().Auditar(actas).Count(r => r.EsSospechosa);

        Assert.True(alertas < 100, $"Alertas = {alertas}");
    }

    [Fact]
    public void DatasetPequeno_UsaRespaldoPorDepartamentoONacional_SinErrores()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 5, Semilla: 12));

        var resultados = new AuditoriaService().Auditar(actas);

        Assert.Equal(5, resultados.Count);
        Assert.All(resultados, r => Assert.NotNull(r.Acta));
        Assert.All(resultados, r => Assert.False(string.IsNullOrWhiteSpace(r.Motivo)));
    }

    [Fact]
    public void UmbralMuyAlto_NoMarcaNingunaActa()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 200, PorcentajeFraude: 0.05, Semilla: 13));

        var resultados = new AuditoriaService().Auditar(actas, umbral: 1000);

        Assert.DoesNotContain(resultados, r => r.EsSospechosa);
    }

    [Fact]
    public void UmbralInvalido_LanzaExcepcion()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 10, Semilla: 14));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AuditoriaService().Auditar(actas, 0));
    }
}
