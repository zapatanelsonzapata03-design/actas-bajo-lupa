namespace AuditaActas.Dominio;

/// <summary>Roles del prototipo (RF8). El rol se elige al iniciar la sesión de trabajo.</summary>
public enum RolUsuario
{
    Operador,
    Auditor,
    Administrador
}

/// <summary>Regla única de permisos. La UI consulta esta clase; no duplica condiciones.</summary>
public static class Permisos
{
    /// <summary>Operador y Auditor pueden cargar/importar actas; el Administrador también.</summary>
    public static bool PuedeRegistrar(RolUsuario rol) => true;

    /// <summary>El Operador no accede a la auditoría estadística (RF8).</summary>
    public static bool PuedeAuditar(RolUsuario rol) => rol is RolUsuario.Auditor or RolUsuario.Administrador;

    /// <summary>El reporte es parte de la auditoría; el Operador no lo genera.</summary>
    public static bool PuedeExportar(RolUsuario rol) => PuedeAuditar(rol);

    /// <summary>Solo el Administrador consulta la bitácora completa.</summary>
    public static bool PuedeVerBitacora(RolUsuario rol) => rol == RolUsuario.Administrador;

    /// <summary>El Operador no puede alterar ni borrar datos; la demo de alteración es solo para Auditor/Admin.</summary>
    public static bool PuedeEjecutarDemoDeAlteracion(RolUsuario rol) => PuedeAuditar(rol);

    /// <summary>Guardar y cargar la base local está permitido para todos los roles.</summary>
    public static bool PuedePersistir(RolUsuario rol) => true;
}

/// <summary>Entrada de bitácora: quién hizo qué y cuándo (hora UTC).</summary>
public record RegistroBitacora(int Id, DateTime FechaUtc, string Usuario, RolUsuario Rol, string Accion, string? Detalle)
{
    public string FechaLocal => FechaUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
}
