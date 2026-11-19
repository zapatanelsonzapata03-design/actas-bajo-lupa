using System.Globalization;
using System.Text;
using AuditaActas.Dominio;

namespace AuditaActas.Infraestructura;

/// <summary>
/// Importación/exportación CSV de actas. Formato: nueve columnas fijas y luego una columna por candidato.
/// Acepta campos entre comillas (nombres con comas), BOM de UTF-8 y encabezados con mayúsculas distintas.
/// </summary>
public static class ActaCsv
{
    public static readonly IReadOnlyList<string> ColumnasFijas = new[]
    {
        "Departamento", "Municipio", "Recinto", "Mesa", "Habilitados", "Emitidos", "Blancos", "Nulos", "EsFraudeSimulado"
    };

    private const int IndiceEtiquetaFraude = 8;

    public static void Exportar(string ruta, IReadOnlyList<Acta> actas, bool incluirEtiquetas = true)
    {
        var candidatos = actas.SelectMany(a => a.Votos.Keys).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList();
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", ColumnasFijas.Concat(candidatos).Select(Escapar)));

        foreach (var a in actas)
        {
            var fila = new List<string>
            {
                a.Departamento, a.Municipio, a.Recinto, a.Mesa.ToString(CultureInfo.InvariantCulture),
                a.Habilitados.ToString(CultureInfo.InvariantCulture), a.Emitidos.ToString(CultureInfo.InvariantCulture),
                a.Blancos.ToString(CultureInfo.InvariantCulture), a.Nulos.ToString(CultureInfo.InvariantCulture),
                incluirEtiquetas && a.EsFraudeSimulado ? "1" : "0"
            };
            fila.AddRange(candidatos.Select(c => a.Votos.GetValueOrDefault(c).ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine(string.Join(",", fila.Select(Escapar)));
        }

        // BOM para que Excel abra los acentos correctamente; el importador lo ignora.
        File.WriteAllText(ruta, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    /// <summary>Devuelve las actas válidas y la lista de errores por fila (RF2).</summary>
    public static (List<Acta> Actas, List<string> Errores) Importar(string ruta)
    {
        if (!File.Exists(ruta))
            return (new List<Acta>(), new List<string> { $"No existe el archivo: {ruta}" });

        return ImportarTexto(File.ReadAllText(ruta, Encoding.UTF8));
    }

    /// <summary>Misma validación que <see cref="Importar"/> pero desde texto (útil para pruebas y pegado manual).</summary>
    public static (List<Acta> Actas, List<string> Errores) ImportarTexto(string contenido)
    {
        var actas = new List<Acta>();
        var errores = new List<string>();

        var lineas = contenido.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        if (lineas.Length == 0 || lineas.All(string.IsNullOrWhiteSpace))
        {
            errores.Add("El archivo está vacío.");
            return (actas, errores);
        }

        var encabezado = Dividir(lineas[0].TrimStart('\uFEFF')).Select(x => x.Trim()).ToArray();
        bool encabezadoOk = encabezado.Length > ColumnasFijas.Count
            && ColumnasFijas.Select((c, i) => string.Equals(c, encabezado[i], StringComparison.OrdinalIgnoreCase)).All(x => x);

        if (!encabezadoOk)
        {
            errores.Add("Encabezado inválido: se esperan las nueve columnas fijas seguidas de una columna por candidato.");
            return (actas, errores);
        }

        var claves = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 1; i < lineas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue;

            var c = Dividir(lineas[i]);
            if (c.Length != encabezado.Length)
            {
                errores.Add($"Fila {i + 1}: cantidad de columnas incorrecta (se esperaban {encabezado.Length}, llegaron {c.Length}).");
                continue;
            }

            var n = new int[encabezado.Length];
            bool numerosOk = true;
            for (int k = 3; k < encabezado.Length; k++)
            {
                if (k == IndiceEtiquetaFraude) continue;
                if (!int.TryParse(c[k].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n[k]))
                {
                    numerosOk = false;
                    errores.Add($"Fila {i + 1}: el valor '{c[k].Trim()}' de la columna '{encabezado[k]}' no es un número entero.");
                    break;
                }
            }
            if (!numerosOk) continue;

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
                EsFraudeSimulado = c[IndiceEtiquetaFraude].Trim() == "1",
                TipoFraudeSimulado = c[IndiceEtiquetaFraude].Trim() == "1" ? "etiquetado_csv" : string.Empty
            };
            for (int k = ColumnasFijas.Count; k < encabezado.Length; k++)
                acta.Votos[encabezado[k]] = n[k];

            var problemas = acta.Validar();
            if (problemas.Count > 0)
            {
                errores.Add($"Fila {i + 1}: {string.Join(" ", problemas)}");
                continue;
            }
            if (!claves.Add(acta.Clave))
            {
                errores.Add($"Fila {i + 1}: la mesa {acta.Clave} está duplicada en el archivo.");
                continue;
            }

            actas.Add(acta);
        }

        return (actas, errores);
    }

    private static string Escapar(string valor)
    {
        if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n') || valor.Contains('\r'))
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        return valor;
    }

    /// <summary>Separador CSV que respeta comillas dobles y comas dentro de un campo.</summary>
    private static string[] Dividir(string linea)
    {
        var campos = new List<string>();
        var actual = new StringBuilder();
        bool enComillas = false;

        for (int i = 0; i < linea.Length; i++)
        {
            char ch = linea[i];
            if (ch == '"')
            {
                if (enComillas && i + 1 < linea.Length && linea[i + 1] == '"')
                {
                    actual.Append('"');
                    i++;
                }
                else
                {
                    enComillas = !enComillas;
                }
            }
            else if (ch == ',' && !enComillas)
            {
                campos.Add(actual.ToString());
                actual.Clear();
            }
            else
            {
                actual.Append(ch);
            }
        }

        campos.Add(actual.ToString());
        return campos.ToArray();
    }
}
