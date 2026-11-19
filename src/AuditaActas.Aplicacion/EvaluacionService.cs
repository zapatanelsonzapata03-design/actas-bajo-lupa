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
}
