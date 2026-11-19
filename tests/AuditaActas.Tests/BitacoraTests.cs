using AuditaActas.Aplicacion;
using AuditaActas.Dominio;
using AuditaActas.Infraestructura;

namespace AuditaActas.Tests;

public class BitacoraTests
{
    [Fact]
    public void BitacoraDeSesion_RegistraUsuarioRolYHora()
    {
        var bitacora = new BitacoraService();
        var registro = bitacora.Registrar("nelson", RolUsuario.Auditor, "Auditar", "2000 actas");

        Assert.Single(bitacora.Registros);
        Assert.Equal("nelson", registro.Usuario);
        Assert.Equal(RolUsuario.Auditor, registro.Rol);
        Assert.Equal("Auditar", registro.Accion);
        Assert.True(registro.FechaUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void Repositorio_GuardaYListaLaBitacora()
    {
        using var repo = new ActaRepositorio("Data Source=:memory:");
        repo.RegistrarBitacora("ana", RolUsuario.Administrador, "Guardar base", "audita.db");

        var registros = repo.ListarBitacora();

        Assert.Single(registros);
        Assert.Equal("ana", registros[0].Usuario);
        Assert.Equal(RolUsuario.Administrador, registros[0].Rol);
        Assert.Equal("Guardar base", registros[0].Accion);
        Assert.Equal("audita.db", registros[0].Detalle);
    }

    [Fact]
    public void Repositorio_CuentaYDetectaClavesExistentes()
    {
        var actas = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 10, Semilla: 6));
        CadenaHashService.Encadenar(actas);

        using var repo = new ActaRepositorio("Data Source=:memory:");
        repo.InsertarVarias(actas);

        Assert.Equal(10, repo.Contar());
        Assert.True(repo.ExisteClave(actas[0].Clave));
        Assert.False(repo.ExisteClave("NO/EXISTE/XXXX/99"));
    }

    [Fact]
    public void Repositorio_RechazaInsertarDosVecesLaMismaActa()
    {
        var acta = new GeneradorService().Generar(new ParametrosSimulacion(Mesas: 1, Semilla: 6))[0];
        CadenaHashService.Encadenar(new List<Acta> { acta });

        using var repo = new ActaRepositorio("Data Source=:memory:");
        repo.Insertar(acta);

        Assert.ThrowsAny<Exception>(() => repo.Insertar(acta));
        Assert.Equal(1, repo.Contar());
    }
}
