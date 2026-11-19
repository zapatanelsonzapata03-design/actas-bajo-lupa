using AuditaActas.Dominio;

namespace AuditaActas.Tests;

public class ActaTests
{
    private static Acta ActaValida() => new()
    {
        Departamento = "D01", Municipio = "M001", Recinto = "R0001", Mesa = 1,
        Habilitados = 200, Emitidos = 160, Blancos = 4, Nulos = 6,
        Votos = new Dictionary<string, int> { ["Candidato A"] = 60, ["Candidato B"] = 45, ["Candidato C"] = 30, ["Candidato D"] = 15 }
    };

    [Fact]
    public void ActaConsistente_NoTieneErrores() => Assert.Empty(ActaValida().Validar());

    [Fact]
    public void EmitidosMayoresQueHabilitados_EsError()
    {
        var acta = ActaValida();
        acta.Emitidos = 250;
        Assert.Contains(acta.Validar(), e => e.Contains("superan"));
    }

    [Fact]
    public void SumaDeVotosDistintaDeEmitidos_EsError()
    {
        var acta = ActaValida();
        acta.Votos["Candidato A"] += 5;
        Assert.Contains(acta.Validar(), e => e.Contains("no coincide"));
    }

    [Fact]
    public void CantidadNegativa_EsError()
    {
        var acta = ActaValida();
        acta.Nulos = -1;
        Assert.NotEmpty(acta.Validar());
    }
}
