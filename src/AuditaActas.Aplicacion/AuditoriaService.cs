using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>
/// RF5: compara cada acta con las demás de su municipio usando z-scores robustos (mediana/MAD)
/// sobre la participación y sobre la proporción de votos de cada candidato.
/// Una alerta es un INDICIO para revisión humana, no una prueba de fraude.
/// </summary>
public class AuditoriaService
{
    public const double UmbralPorDefecto = 3.0;

    public IReadOnlyList<ResultadoAuditoria> Auditar(IReadOnlyList<Acta> actas, double umbral = UmbralPorDefecto)
    {
        var resultados = new ResultadoAuditoria[actas.Count];

        var grupos = actas
            .Select((acta, indice) => (acta, indice))
            .GroupBy(x => (x.acta.Departamento, x.acta.Municipio));

        foreach (var grupo in grupos)
        {
            var items = grupo.ToList();
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
                    motivos.Count > 0, string.Join("; ", motivos));
            }
        }

        return resultados;
    }

    private static double Proporcion(Acta acta, string candidato) =>
        acta.VotosValidos > 0 ? (double)acta.Votos.GetValueOrDefault(candidato) / acta.VotosValidos : 0;
}
