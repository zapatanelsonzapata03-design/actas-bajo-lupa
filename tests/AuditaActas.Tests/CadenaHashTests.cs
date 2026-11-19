using AuditaActas.Aplicacion;

namespace AuditaActas.Tests;

public class CadenaHashTests
{
    [Fact]
    public void CadenaRecienCreada_EsIntegra()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 200, Semilla: 1));
        CadenaHashService.Encadenar(actas);
        Assert.Null(CadenaHashService.PrimeraActaAlterada(actas));
    }

    [Fact]
    public void AlterarUnVoto_RompeLaCadenaEnEsaActa()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 200, Semilla: 1));
        CadenaHashService.Encadenar(actas);

        actas[57].Votos["Candidato A"]++;

        Assert.Equal(57, CadenaHashService.PrimeraActaAlterada(actas));
    }

    [Fact]
    public void PrimeraActa_ApuntaAlHashGenesis()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 20, Semilla: 1));
        CadenaHashService.Encadenar(actas);
        Assert.Equal(CadenaHashService.HashGenesis, actas[0].HashAnterior);
    }
}
