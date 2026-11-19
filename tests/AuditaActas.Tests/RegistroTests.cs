using AuditaActas.Aplicacion;
using AuditaActas.Dominio;

namespace AuditaActas.Tests;

public class RegistroTests
{
    private static Acta Nueva(string mesa = "1") => new()
    {
        Departamento = "D01", Municipio = "M001", Recinto = "R0001", Mesa = int.Parse(mesa),
        Habilitados = 200, Emitidos = 100, Blancos = 2, Nulos = 3,
        Votos = new Dictionary<string, int> { ["A"] = 60, ["B"] = 35 }
    };

    [Fact]
    public void Agregar_ActaValida_LaEncadena()
    {
        var actas = new List<Acta>();
        var resultado = RegistroService.Agregar(actas, Nueva());

        Assert.True(resultado.Exito);
        Assert.Single(actas);
        Assert.Null(CadenaHashService.PrimeraActaAlterada(actas));
    }

    [Fact]
    public void Agregar_MesaDuplicada_SeRechaza()
    {
        var actas = new List<Acta>();
        Assert.True(RegistroService.Agregar(actas, Nueva()).Exito);

        var repetida = Nueva();
        repetida.Recinto = "r0001"; // la comparación de claves no distingue mayúsculas
        var resultado = RegistroService.Agregar(actas, repetida);

        Assert.False(resultado.Exito);
        Assert.Single(actas);
    }

    [Fact]
    public void Agregar_ActaInconsistente_SeRechazaYNoSeAgrega()
    {
        var actas = new List<Acta>();
        var invalida = Nueva();
        invalida.Emitidos = 999;

        var resultado = RegistroService.Agregar(actas, invalida);

        Assert.False(resultado.Exito);
        Assert.Empty(actas);
        Assert.Contains("superan", resultado.Mensaje);
    }

    [Fact]
    public void Agregar_QuitaLaEtiquetaDelSimulador()
    {
        var actas = new List<Acta>();
        var acta = Nueva();
        acta.EsFraudeSimulado = true;
        acta.TipoFraudeSimulado = "relleno_urna";

        Assert.True(RegistroService.Agregar(actas, acta).Exito);
        Assert.False(actas[0].EsFraudeSimulado);
        Assert.Equal(string.Empty, actas[0].TipoFraudeSimulado);
    }

    [Fact]
    public void Agregar_AlFinal_MantieneLaCadenaAnterior()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 30, Semilla: 8));
        CadenaHashService.Encadenar(actas);
        int anterior = actas.Count;

        var resultado = RegistroService.Agregar(actas, Nueva(mesa: "99"));

        Assert.True(resultado.Exito);
        Assert.Equal(anterior + 1, actas.Count);
        Assert.Null(CadenaHashService.PrimeraActaAlterada(actas));
    }
}
