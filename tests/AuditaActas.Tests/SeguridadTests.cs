using AuditaActas.Dominio;

namespace AuditaActas.Tests;

public class SeguridadTests
{
    [Fact]
    public void Operador_NoAccedeALaAuditoria()
    {
        Assert.False(Permisos.PuedeAuditar(RolUsuario.Operador));
        Assert.False(Permisos.PuedeExportar(RolUsuario.Operador));
        Assert.True(Permisos.PuedeRegistrar(RolUsuario.Operador));
    }

    [Fact]
    public void Auditor_AuditaYExporta_PeroNoVeLaBitacora()
    {
        Assert.True(Permisos.PuedeAuditar(RolUsuario.Auditor));
        Assert.True(Permisos.PuedeExportar(RolUsuario.Auditor));
        Assert.False(Permisos.PuedeVerBitacora(RolUsuario.Auditor));
    }

    [Fact]
    public void Administrador_TieneTodosLosPermisos()
    {
        Assert.True(Permisos.PuedeRegistrar(RolUsuario.Administrador));
        Assert.True(Permisos.PuedeAuditar(RolUsuario.Administrador));
        Assert.True(Permisos.PuedeExportar(RolUsuario.Administrador));
        Assert.True(Permisos.PuedeVerBitacora(RolUsuario.Administrador));
    }
}
