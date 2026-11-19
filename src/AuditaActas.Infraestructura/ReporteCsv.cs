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
        sb.AppendLine("Acta,Departamento,Municipio,Recinto,Mesa,Participacion,Puntaje,ZParticipacion,ZApoyo,Motivo");
        foreach (var r in sospechosas)
        {
            sb.AppendLine(string.Join(",",
                Escapar(r.Acta.Clave),
                Escapar(r.Acta.Departamento),
                Escapar(r.Acta.Municipio),
                Escapar(r.Acta.Recinto),
                r.Acta.Mesa.ToString(CultureInfo.InvariantCulture),
                r.Acta.Participacion.ToString("P2", CultureInfo.InvariantCulture),
                r.Puntaje.ToString("F2", CultureInfo.InvariantCulture),
                r.ZParticipacion.ToString("F2", CultureInfo.InvariantCulture),
                r.ZApoyo.ToString("F2", CultureInfo.InvariantCulture),
                Escapar(r.Motivo)));
        }

        File.WriteAllText(ruta, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    private static string Escapar(string valor) =>
        valor.Contains(',') || valor.Contains('"') || valor.Contains('\n')
            ? "\"" + valor.Replace("\"", "\"\"") + "\""
            : valor;
}
