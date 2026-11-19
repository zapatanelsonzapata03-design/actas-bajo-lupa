using AuditaActas.Aplicacion;
using AuditaActas.Dominio;

namespace AuditaActas.Tests;

public class EvaluacionTests
{
    private static Acta Acta(bool fraude) => new()
    {
        Departamento = "D01", Municipio = "M001", Recinto = "R0001", Mesa = 1,
        Habilitados = 200, Emitidos = 160, Blancos = 4, Nulos = 6,
        Votos = new Dictionary<string, int> { ["A"] = 90, ["B"] = 60 },
        EsFraudeSimulado = fraude
    };

    private static ResultadoAuditoria Resultado(bool fraude, bool alerta) =>
        new(Acta(fraude), 0, 0, alerta ? 5 : 0, alerta, alerta ? "motivo" : "sin señal");

    [Fact]
    public void Resumen_CalculaLasMetricasDeLaMatriz()
    {
        var resultados = new[]
        {
            Resultado(fraude: true, alerta: true),
            Resultado(fraude: true, alerta: true),
            Resultado(fraude: false, alerta: true),
            Resultado(fraude: true, alerta: false),
            Resultado(fraude: false, alerta: false)
        };

        var resumen = EvaluacionService.Resumir(resultados);

        Assert.Equal(2, resumen.Matriz.VerdaderosPositivos);
        Assert.Equal(1, resumen.Matriz.FalsosPositivos);
        Assert.Equal(1, resumen.Matriz.FalsosNegativos);
        Assert.Equal(1, resumen.Matriz.VerdaderosNegativos);
        Assert.Equal(5, resumen.Total);
        Assert.Equal(3, resumen.FraudesEtiquetados);
        Assert.Equal(3, resumen.Alertas);
        Assert.Equal(2.0 / 3.0, resumen.Precision, 6);
        Assert.Equal(2.0 / 3.0, resumen.Recall, 6);
        Assert.Equal(0.5, resumen.TasaFalsosPositivos, 6);
        Assert.Equal(3.0 / 5.0, resumen.Exactitud, 6);
    }

    [Fact]
    public void Resumen_SinDatos_NoDividePorCero()
    {
        var resumen = EvaluacionService.Resumir(Array.Empty<ResultadoAuditoria>());
        Assert.Equal(0, resumen.Total);
        Assert.Equal(0, resumen.Precision);
        Assert.Equal(0, resumen.Recall);
        Assert.Equal(0, resumen.TasaFalsosPositivos);
    }

    [Fact]
    public void BarridoDeUmbral_DevuelveUmbralesOrdenadosYAmenosAlertas()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 600, PorcentajeFraude: 0.05, Semilla: 21));
        var barrido = EvaluacionService.BarrerUmbral(actas, new[] { 3.0, 1.5, 2.5 });

        Assert.Equal(new[] { 1.5, 2.5, 3.0 }, barrido.Select(r => r.Umbral));
        Assert.True(barrido[0].Alertas >= barrido[^1].Alertas);
    }
}
