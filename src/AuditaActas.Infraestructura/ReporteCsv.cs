using System.Globalization;
using System.Text;
using AuditaActas.Dominio;

namespace AuditaActas.Infraestructura;

/// <summary>RF6: exporta la lista de actas sospechosas con su puntaje y motivo.</summary>
public static class ReporteCsv
{
    public static void Exportar(string ruta, IEnumerable<ResultadoAuditoria> sospechosas)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Acta,Puntaje,ZParticipacion,ZApoyo,Motivo");
        foreach (var r in sospechosas)
        {
            sb.AppendLine(string.Join(",",
                r.Acta.Clave,
                r.Puntaje.ToString("F2", CultureInfo.InvariantCulture),
                r.ZParticipacion.ToString("F2", CultureInfo.InvariantCulture),
                r.ZApoyo.ToString("F2", CultureInfo.InvariantCulture),
                "\"" + r.Motivo.Replace("\"", "\"\"") + "\""));
        }
        File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
    }
}
