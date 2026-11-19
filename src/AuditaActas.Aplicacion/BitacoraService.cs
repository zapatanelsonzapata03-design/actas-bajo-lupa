using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>
/// RF8: bitácora de la sesión. Registra usuario, rol, acción y hora UTC.
/// Al guardar la base, la UI copia estas entradas a la tabla <c>bitacora</c>.
/// </summary>
public class BitacoraService
{
    private readonly List<RegistroBitacora> _registros = new();
    private int _secuencia;

    public IReadOnlyList<RegistroBitacora> Registros => _registros;

    public int Conteo => _registros.Count;

    public RegistroBitacora Registrar(string usuario, RolUsuario rol, string accion, string? detalle = null)
    {
        var registro = new RegistroBitacora(++_secuencia, DateTime.UtcNow, usuario, rol, accion, detalle);
        _registros.Add(registro);
        return registro;
    }
}
