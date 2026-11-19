using System.Security.Cryptography;
using System.Text;
using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

/// <summary>Resultado detallado de verificar la cadena (RF3).</summary>
public record ResultadoVerificacion(bool Integra, int? Indice, string? Clave, string Mensaje);

/// <summary>RF3: cada acta lleva el hash SHA-256 de sus datos + el hash del acta anterior.</summary>
public static class CadenaHashService
{
    public const string HashGenesis = "GENESIS";

    public static string CalcularHash(Acta acta, string hashAnterior)
    {
        var votos = string.Join(",", acta.Votos
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => $"{kv.Key}={kv.Value}"));

        var contenido = string.Join("|",
            acta.Departamento, acta.Municipio, acta.Recinto, acta.Mesa,
            acta.Habilitados, acta.Emitidos, acta.Blancos, acta.Nulos, votos, hashAnterior);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(contenido)));
    }

    public static void Encadenar(IList<Acta> actas) => Encadenar(actas, HashGenesis);

    /// <summary>Encadena desde el inicio usando un hash inicial distinto de GENESIS (por ejemplo, al continuar un lote).</summary>
    public static void Encadenar(IList<Acta> actas, string hashInicial)
    {
        string anterior = hashInicial;
        foreach (var acta in actas)
        {
            acta.HashAnterior = anterior;
            acta.Hash = CalcularHash(acta, anterior);
            anterior = acta.Hash;
        }
    }

    /// <summary>Recalcula desde la posición indicada (0 = toda la lista). Útil al agregar un acta al final.</summary>
    public static void EncadenarDesde(IList<Acta> actas, int desde)
    {
        if (desde < 0 || desde > actas.Count) throw new ArgumentOutOfRangeException(nameof(desde));
        string anterior = desde == 0 ? HashGenesis : actas[desde - 1].Hash;
        for (int i = desde; i < actas.Count; i++)
        {
            actas[i].HashAnterior = anterior;
            actas[i].Hash = CalcularHash(actas[i], anterior);
            anterior = actas[i].Hash;
        }
    }

    /// <summary>Devuelve el índice de la primera acta alterada, o null si la cadena es íntegra.</summary>
    public static int? PrimeraActaAlterada(IReadOnlyList<Acta> actas, string hashInicial = HashGenesis)
    {
        string anterior = hashInicial;
        for (int i = 0; i < actas.Count; i++)
        {
            var acta = actas[i];
            if (acta.HashAnterior != anterior || acta.Hash != CalcularHash(acta, anterior))
                return i;
            anterior = acta.Hash;
        }
        return null;
    }

    /// <summary>Igual que <see cref="PrimeraActaAlterada"/> pero con un mensaje listo para mostrar.</summary>
    public static ResultadoVerificacion Verificar(IReadOnlyList<Acta> actas, string hashInicial = HashGenesis)
    {
        var indice = PrimeraActaAlterada(actas, hashInicial);
        if (indice is null)
            return new ResultadoVerificacion(true, null, null, $"Integridad OK: {actas.Count} actas verificadas, la cadena es válida.");

        var acta = actas[indice.Value];
        return new ResultadoVerificacion(
            false,
            indice,
            acta.Clave,
            $"Cadena rota en la posición {indice.Value + 1}: el acta {acta.Clave} no coincide con su hash.");
    }
}
