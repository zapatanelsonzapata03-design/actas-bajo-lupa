using AuditaActas.Aplicacion;
using AuditaActas.Infraestructura;

namespace AuditaActas.Tests;

public class InfraestructuraTests
{
    [Fact]
    public void Repositorio_GuardaYRecuperaLasActasEnOrden_ConCadenaIntegra()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 100, Semilla: 2));
        CadenaHashService.Encadenar(actas);

        using var repo = new ActaRepositorio("Data Source=:memory:");
        repo.InsertarVarias(actas);
        var leidas = repo.ObtenerTodas();

        Assert.Equal(actas.Count, leidas.Count);
        Assert.Equal(actas[0].Clave, leidas[0].Clave);
        Assert.Null(CadenaHashService.PrimeraActaAlterada(leidas));
    }

    [Fact]
    public void Csv_ExportarEImportar_ConservaLosDatos()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 50, Semilla: 4));
        var ruta = Path.Combine(Path.GetTempPath(), $"audita_{Guid.NewGuid():N}.csv");
        try
        {
            ActaCsv.Exportar(ruta, actas);
            var (importadas, errores) = ActaCsv.Importar(ruta);

            Assert.Empty(errores);
            Assert.Equal(actas.Count, importadas.Count);
            Assert.Equal(actas.Sum(a => a.Emitidos), importadas.Sum(a => a.Emitidos));
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Fact]
    public void Csv_ConErrores_RechazaLasFilasInvalidas()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"audita_{Guid.NewGuid():N}.csv");
        try
        {
            File.WriteAllText(ruta,
                "Departamento,Municipio,Recinto,Mesa,Habilitados,Emitidos,Blancos,Nulos,EsFraudeSimulado,Candidato A,Candidato B\n" +
                "D01,M001,R0001,1,200,160,4,6,0,100,50\n" +      // válida
                "D01,M001,R0001,2,100,120,0,0,0,70,50\n" +       // emitidos > habilitados
                "D01,M001,R0001,1,200,160,4,6,0,100,50\n");      // duplicada

            var (importadas, errores) = ActaCsv.Importar(ruta);

            Assert.Single(importadas);
            Assert.Equal(2, errores.Count);
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Fact]
    public void Csv_ConNombreDeCandidatoEntreComillas_ConservaElNombre()
    {
        var texto =
            "Departamento,Municipio,Recinto,Mesa,Habilitados,Emitidos,Blancos,Nulos,EsFraudeSimulado,\"Partido, Uno\",Candidato B\n" +
            "D01,M001,R0001,1,200,160,4,6,0,100,50\n";

        var (actas, errores) = ActaCsv.ImportarTexto(texto);

        Assert.Empty(errores);
        Assert.Single(actas);
        Assert.Equal(100, actas[0].Votos["Partido, Uno"]);
    }

    [Fact]
    public void Csv_ExportarConComasEnElNombre_SePuedeVolverAImportar()
    {
        var acta = new AuditaActas.Dominio.Acta
        {
            Departamento = "D01", Municipio = "M001", Recinto = "R0001", Mesa = 1,
            Habilitados = 200, Emitidos = 160, Blancos = 4, Nulos = 6,
            Votos = new Dictionary<string, int> { ["Partido, Uno"] = 100, ["Candidato B"] = 50 }
        };
        var ruta = Path.Combine(Path.GetTempPath(), $"audita_{Guid.NewGuid():N}.csv");
        try
        {
            ActaCsv.Exportar(ruta, new List<AuditaActas.Dominio.Acta> { acta });
            var (importadas, errores) = ActaCsv.Importar(ruta);

            Assert.Empty(errores);
            Assert.Equal(100, importadas[0].Votos["Partido, Uno"]);
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Fact]
    public void Csv_EncabezadoInvalido_SeRechaza()
    {
        var (actas, errores) = ActaCsv.ImportarTexto("A,B\n1,2\n");
        Assert.Empty(actas);
        Assert.Contains(errores, e => e.Contains("Encabezado"));
    }

    [Fact]
    public void Csv_ValorNoNumerico_IndicaLaColumna()
    {
        var texto =
            "Departamento,Municipio,Recinto,Mesa,Habilitados,Emitidos,Blancos,Nulos,EsFraudeSimulado,Candidato A,Candidato B\n" +
            "D01,M001,R0001,1,200,ABC,4,6,0,100,50\n";

        var (actas, errores) = ActaCsv.ImportarTexto(texto);

        Assert.Empty(actas);
        Assert.Contains(errores, e => e.Contains("Emitidos") && e.Contains("no es un número"));
    }

    [Fact]
    public void Csv_ArchivoVacio_SeRechaza()
    {
        var (actas, errores) = ActaCsv.ImportarTexto("   ");
        Assert.Empty(actas);
        Assert.Contains(errores, e => e.Contains("vacío"));
    }
}
