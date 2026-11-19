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
}
