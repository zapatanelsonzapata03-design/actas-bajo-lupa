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

    [Fact]
    public void Verificar_IndicaActaYPosicionCuandoHayAlteracion()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 50, Semilla: 1));
        CadenaHashService.Encadenar(actas);
        actas[10].Blancos++;

        var resultado = CadenaHashService.Verificar(actas);

        Assert.False(resultado.Integra);
        Assert.Equal(10, resultado.Indice);
        Assert.Equal(actas[10].Clave, resultado.Clave);
        Assert.Contains(actas[10].Clave, resultado.Mensaje);
    }

    [Fact]
    public void EncadenarDesde_AgregarAlFinal_MantieneLaIntegridad()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 30, Semilla: 1));
        CadenaHashService.Encadenar(actas);

        var nueva = new AuditaActas.Dominio.Acta
        {
            Departamento = "D99", Municipio = "M999", Recinto = "R9999", Mesa = 1,
            Habilitados = 100, Emitidos = 80, Blancos = 2, Nulos = 3,
            Votos = new Dictionary<string, int> { ["A"] = 75 }
        };
        actas.Add(nueva);
        CadenaHashService.EncadenarDesde(actas, actas.Count - 1);

        Assert.Null(CadenaHashService.PrimeraActaAlterada(actas));
        Assert.Equal(actas[^2].Hash, actas[^1].HashAnterior);
    }

    [Fact]
    public void EncadenarConHashInicial_PermiteContinuarUnLote()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 10, Semilla: 1));
        CadenaHashService.Encadenar(actas, "HASH-DEL-LOTE-ANTERIOR");

        Assert.Equal("HASH-DEL-LOTE-ANTERIOR", actas[0].HashAnterior);
        Assert.Null(CadenaHashService.PrimeraActaAlterada(actas, "HASH-DEL-LOTE-ANTERIOR"));
    }
}
