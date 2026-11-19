using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>RF7: compara las alertas del detector con las etiquetas del simulador.</summary>
public static class EvaluacionService
{
    public static MatrizConfusion Evaluar(IEnumerable<ResultadoAuditoria> resultados)
    {
        int vp = 0, fp = 0, fn = 0, vn = 0;
        foreach (var r in resultados)
        {
            if (r.Acta.EsFraudeSimulado && r.EsSospechosa) vp++;
            else if (!r.Acta.EsFraudeSimulado && r.EsSospechosa) fp++;
            else if (r.Acta.EsFraudeSimulado && !r.EsSospechosa) fn++;
            else vn++;
        }
        return new MatrizConfusion(vp, fp, fn, vn);
    }

    /// <summary>Resumen completo para la pantalla de evaluación y el reporte de evidencias.</summary>
    public static ResumenEvaluacion Resumir(
        IEnumerable<ResultadoAuditoria> resultados,
        double umbral = AuditoriaService.UmbralPorDefecto)
    {
        var lista = resultados.ToList();
        var matriz = Evaluar(lista);
        return new ResumenEvaluacion(
            matriz,
            Total: lista.Count,
            FraudesEtiquetados: lista.Count(r => r.Acta.EsFraudeSimulado),
            Alertas: lista.Count(r => r.EsSospechosa),
            Umbral: umbral);
    }

    /// <summary>Prueba varios umbrales sobre el mismo dataset (calibración reproducible).</summary>
    public static IReadOnlyList<ResumenEvaluacion> BarrerUmbral(
        IReadOnlyList<Acta> actas,
        IEnumerable<double> umbrales)
    {
        var servicio = new AuditoriaService();
        return umbrales
            .OrderBy(u => u)
            .Select(u => Resumir(servicio.Auditar(actas, u), u))
            .ToList();
    }
}
