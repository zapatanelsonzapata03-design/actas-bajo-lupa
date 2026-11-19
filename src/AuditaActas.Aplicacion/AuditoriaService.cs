using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>
/// RF5: compara cada acta con las demás de su municipio usando z-scores robustos (mediana/MAD)
/// sobre la participación y sobre la proporción de votos de cada candidato.
/// Si un municipio tiene muy pocas actas, se compara con su departamento y, si tampoco alcanza,
/// con el total nacional. Una alerta es un INDICIO para revisión humana, no una prueba de fraude.
/// </summary>
public class AuditoriaService
{
    public const double UmbralPorDefecto = 3.0;

    /// <summary>Con menos actas que este mínimo, la mediana/MAD del grupo no es representativa.</summary>
    public const int MinimoActasPorGrupo = 8;

    public IReadOnlyList<ResultadoAuditoria> Auditar(IReadOnlyList<Acta> actas, double umbral = UmbralPorDefecto)
    {
        if (umbral <= 0) throw new ArgumentOutOfRangeException(nameof(umbral), "El umbral debe ser mayor que cero.");

        var resultados = new ResultadoAuditoria[actas.Count];

        // 1) Grupo natural: municipio.
        var pendientes = new List<int>();
        var gruposMunicipio = actas
            .Select((acta, indice) => (acta, indice))
            .GroupBy(x => (x.acta.Departamento, x.acta.Municipio));

        foreach (var grupo in gruposMunicipio)
        {
            var items = grupo.ToList();
            if (items.Count >= MinimoActasPorGrupo) Procesar(items, umbral, resultados);
            else pendientes.AddRange(items.Select(x => x.indice));
        }

        // 2) Respaldo: departamento para los municipios pequeños.
        var sinGrupo = new List<int>();
        foreach (var grupo in pendientes.GroupBy(i => actas[i].Departamento))
        {
            var items = grupo.Select(i => (acta: actas[i], indice: i)).ToList();
            if (items.Count >= MinimoActasPorGrupo) Procesar(items, umbral, resultados);
            else sinGrupo.AddRange(items.Select(x => x.indice));
        }

        // 3) Respaldo final: nacional (también cubre datasets de una sola acta).
        if (sinGrupo.Count > 0)
            Procesar(sinGrupo.Select(i => (acta: actas[i], indice: i)).ToList(), umbral, resultados);

        return resultados;
    }

    private static void Procesar(
        IReadOnlyList<(Acta acta, int indice)> items,
        double umbral,
        ResultadoAuditoria[] resultados)
    {
        var candidatos = items.SelectMany(x => x.acta.Votos.Keys).Distinct().ToList();

        var zParticipacion = Estadistica.ZRobusto(items.Select(x => x.acta.Participacion).ToList());
        var zPorCandidato = candidatos.ToDictionary(
            c => c,
            c => Estadistica.ZRobusto(items.Select(x => Proporcion(x.acta, c)).ToList()));

        for (int k = 0; k < items.Count; k++)
        {
            double zApoyo = 0;
            string candidatoMax = string.Empty;
            foreach (var c in candidatos)
            {
                double z = zPorCandidato[c][k];
                if (Math.Abs(z) > Math.Abs(zApoyo))
                {
                    zApoyo = z;
                    candidatoMax = c;
                }
            }

            var motivos = new List<string>();
            if (Math.Abs(zParticipacion[k]) > umbral)
                motivos.Add($"Participación atípica (z={zParticipacion[k]:F1})");
            if (Math.Abs(zApoyo) > umbral)
                motivos.Add($"Apoyo atípico a {candidatoMax} (z={zApoyo:F1})");

            double puntaje = Math.Max(Math.Abs(zParticipacion[k]), Math.Abs(zApoyo));
            resultados[items[k].indice] = new ResultadoAuditoria(
                items[k].acta, zParticipacion[k], zApoyo, puntaje,
                motivos.Count > 0, motivos.Count == 0 ? "Sin señales por encima del umbral" : string.Join("; ", motivos));
        }
    }

    private static double Proporcion(Acta acta, string candidato) =>
        acta.VotosValidos > 0 ? (double)acta.Votos.GetValueOrDefault(candidato) / acta.VotosValidos : 0;
}
