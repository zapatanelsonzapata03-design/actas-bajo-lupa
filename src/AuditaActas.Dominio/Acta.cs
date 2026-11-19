namespace AuditaActas.Dominio;

/// <summary>
/// Acta de una mesa electoral (datos simulados). Una vez registrada NO se edita:
/// las correcciones se registran como nuevos hechos, nunca modificando el acta original.
/// </summary>
public class Acta
{
    public string Departamento { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Recinto { get; set; } = string.Empty;
    public int Mesa { get; set; }
    public int Habilitados { get; set; }
    public int Emitidos { get; set; }
    public int Blancos { get; set; }
    public int Nulos { get; set; }
    public Dictionary<string, int> Votos { get; set; } = new();

    /// <summary>Etiqueta del simulador (solo para evaluar el detector). No forma parte del hash.</summary>
    public bool EsFraudeSimulado { get; set; }

    /// <summary>Tipo de fraude inyectado por el simulador (vacío si el acta es normal). No forma parte del hash.</summary>
    public string TipoFraudeSimulado { get; set; } = string.Empty;

    public string HashAnterior { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;

    public string Clave => $"{Departamento}/{Municipio}/{Recinto}/{Mesa}";
    public double Participacion => Habilitados > 0 ? (double)Emitidos / Habilitados : 0;
    public int VotosValidos => Votos.Values.Sum();

    /// <summary>Devuelve la lista de errores de consistencia (vacía si el acta es válida).</summary>
    public IReadOnlyList<string> Validar()
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(Departamento) || string.IsNullOrWhiteSpace(Municipio) || string.IsNullOrWhiteSpace(Recinto))
            errores.Add("Departamento, municipio y recinto son obligatorios.");
        if (Mesa <= 0)
            errores.Add("El número de mesa debe ser mayor que cero.");
        if (Habilitados <= 0)
            errores.Add("Los votantes habilitados deben ser mayores que cero.");
        if (Emitidos < 0 || Blancos < 0 || Nulos < 0 || Votos.Values.Any(v => v < 0))
            errores.Add("No se permiten cantidades negativas.");
        if (Emitidos > Habilitados)
            errores.Add($"Votos emitidos ({Emitidos}) superan a los habilitados ({Habilitados}).");
        if (Votos.Count == 0)
            errores.Add("El acta no tiene votos por candidato.");

        var suma = VotosValidos + Blancos + Nulos;
        if (suma != Emitidos)
            errores.Add($"La suma de votos ({suma}) no coincide con los emitidos ({Emitidos}).");

        return errores;
    }
}
