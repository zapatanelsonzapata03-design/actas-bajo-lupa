using System.Globalization;
using System.Text;
using AuditaActas.Dominio;

namespace AuditaActas.Infraestructura;

/// <summary>
/// Importación/exportación CSV de actas. Formato: columnas fijas y luego una columna por candidato.
/// Limitación conocida: los nombres no pueden contener comas.
/// </summary>
public static class ActaCsv
{
    private static readonly string[] ColumnasFijas =
        { "Departamento", "Municipio", "Recinto", "Mesa", "Habilitados", "Emitidos", "Blancos", "Nulos", "EsFraudeSimulado" };

    public static void Exportar(string ruta, IReadOnlyList<Acta> actas)
    {
        var candidatos = actas.SelectMany(a => a.Votos.Keys).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList();
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", ColumnasFijas.Concat(candidatos)));

        foreach (var a in actas)
        {
            var fila = new List<string>
            {
                a.Departamento, a.Municipio, a.Recinto, a.Mesa.ToString(CultureInfo.InvariantCulture),
                a.Habilitados.ToString(CultureInfo.InvariantCulture), a.Emitidos.ToString(CultureInfo.InvariantCulture),
                a.Blancos.ToString(CultureInfo.InvariantCulture), a.Nulos.ToString(CultureInfo.InvariantCulture),
                a.EsFraudeSimulado ? "1" : "0"
            };
            fila.AddRange(candidatos.Select(c => a.Votos.GetValueOrDefault(c).ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine(string.Join(",", fila));
        }

        File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>Devuelve las actas válidas y la lista de errores por fila (RF2).</summary>
    public static (List<Acta> Actas, List<string> Errores) Importar(string ruta)
    {
        var actas = new List<Acta>();
        var errores = new List<string>();
        var lineas = File.ReadAllLines(ruta, Encoding.UTF8);

        if (lineas.Length == 0)
        {
            errores.Add("El archivo está vacío.");
            return (actas, errores);
        }

        var encabezado = lineas[0].Split(',');
        if (encabezado.Length <= ColumnasFijas.Length || !encabezado.Take(ColumnasFijas.Length).SequenceEqual(ColumnasFijas))
        {
            errores.Add("Encabezado inválido: se esperan las columnas fijas seguidas de una columna por candidato.");
            return (actas, errores);
        }

        var claves = new HashSet<string>();
        for (int i = 1; i < lineas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue;

            var c = lineas[i].Split(',');
            if (c.Length != encabezado.Length)
            {
                errores.Add($"Fila {i + 1}: cantidad de columnas incorrecta.");
                continue;
            }

            var n = new int[encabezado.Length];
            bool numerosOk = true;
            for (int k = 3; k < encabezado.Length; k++)
            {
                if (k == 8) continue; // EsFraudeSimulado se interpreta aparte
                if (!int.TryParse(c[k], NumberStyles.Integer, CultureInfo.InvariantCulture, out n[k]))
                    numerosOk = false;
            }
            if (!numerosOk)
            {
                errores.Add($"Fila {i + 1}: hay valores no numéricos.");
                continue;
            }

            var acta = new Acta
            {
                Departamento = c[0].Trim(),
                Municipio = c[1].Trim(),
                Recinto = c[2].Trim(),
                Mesa = n[3],
                Habilitados = n[4],
                Emitidos = n[5],
                Blancos = n[6],
                Nulos = n[7],
                EsFraudeSimulado = c[8].Trim() == "1"
            };
            for (int k = ColumnasFijas.Length; k < encabezado.Length; k++)
                acta.Votos[encabezado[k]] = n[k];

            var problemas = acta.Validar();
            if (problemas.Count > 0)
            {
                errores.Add($"Fila {i + 1}: {string.Join(" ", problemas)}");
                continue;
            }
            if (!claves.Add(acta.Clave))
            {
                errores.Add($"Fila {i + 1}: la mesa {acta.Clave} está duplicada.");
                continue;
            }

            actas.Add(acta);
        }

        return (actas, errores);
    }
}
