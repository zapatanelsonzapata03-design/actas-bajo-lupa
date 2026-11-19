using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

public record ResultadoRegistro(bool Exito, string Mensaje);

/// <summary>
/// RF2: reglas de alta de un acta individual. Centraliza validación, duplicados y encadenamiento
/// para que la UI no tenga lógica de negocio.
/// </summary>
public static class RegistroService
{
    public static ResultadoRegistro Agregar(IList<Acta> actas, Acta nueva)
    {
        nueva.Departamento = nueva.Departamento.Trim();
        nueva.Municipio = nueva.Municipio.Trim();
        nueva.Recinto = nueva.Recinto.Trim();

        // Una carga manual nunca debe llevar la etiqueta del simulador.
        nueva.EsFraudeSimulado = false;
        nueva.TipoFraudeSimulado = string.Empty;

        var errores = nueva.Validar();
        if (errores.Count > 0)
            return new ResultadoRegistro(false, string.Join(" ", errores));

        if (actas.Any(a => string.Equals(a.Clave, nueva.Clave, StringComparison.OrdinalIgnoreCase)))
            return new ResultadoRegistro(false, $"La mesa {nueva.Clave} ya está registrada. Las actas no se editan ni se duplican.");

        actas.Add(nueva);
        CadenaHashService.EncadenarDesde(actas, actas.Count - 1);

        return new ResultadoRegistro(true, $"Acta {nueva.Clave} registrada y encadenada.");
    }
}
