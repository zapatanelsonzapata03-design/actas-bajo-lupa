using AuditaActas.Aplicacion;
using AuditaActas.Dominio;

namespace AuditaActas.Tests;

public class ComputoTests
{
    [Fact]
    public void TotalNacional_CoincideConLaSumaDeTodasLasActas()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 300, Semilla: 5));

        var nacional = ComputoService.Computar(actas, NivelGeografico.Nacional).Single();

        Assert.Equal(actas.Sum(a => a.Emitidos), nacional.Emitidos);
        Assert.Equal(actas.Sum(a => a.VotosValidos), nacional.Votos.Values.Sum());
    }

    [Fact]
    public void PorDepartamento_SumaIgualAlNacional()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 500, Semilla: 9));

        var porDepartamento = ComputoService.Computar(actas, NivelGeografico.Departamento);

        Assert.Equal(actas.Sum(a => a.Emitidos), porDepartamento.Sum(r => r.Emitidos));
    }
}
