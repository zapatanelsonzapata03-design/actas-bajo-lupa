using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>RF4: cómputo de resultados por recinto, municipio, departamento o nacional.</summary>
public static class ComputoService
{
    public static IReadOnlyList<ResultadoAgregado> Computar(IEnumerable<Acta> actas, NivelGeografico nivel)
    {
        return actas
            .GroupBy(a => ClaveDe(a, nivel))
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => new ResultadoAgregado(
                g.Key,
                g.Count(),
                g.Sum(a => a.Habilitados),
                g.Sum(a => a.Emitidos),
                g.Sum(a => a.Blancos),
                g.Sum(a => a.Nulos),
                g.SelectMany(a => a.Votos)
                 .GroupBy(kv => kv.Key)
                 .OrderBy(x => x.Key, StringComparer.Ordinal)
                 .ToDictionary(x => x.Key, x => x.Sum(kv => kv.Value))))
            .ToList();
    }

    private static string ClaveDe(Acta a, NivelGeografico nivel) => nivel switch
    {
        NivelGeografico.Recinto => $"{a.Departamento}/{a.Municipio}/{a.Recinto}",
        NivelGeografico.Municipio => $"{a.Departamento}/{a.Municipio}",
        NivelGeografico.Departamento => a.Departamento,
        _ => "NACIONAL"
    };
}
