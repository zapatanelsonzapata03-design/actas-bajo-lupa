using System.Security.Cryptography;
using System.Text;
using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

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

    public static void Encadenar(IList<Acta> actas)
    {
        string anterior = HashGenesis;
        foreach (var acta in actas)
        {
            acta.HashAnterior = anterior;
            acta.Hash = CalcularHash(acta, anterior);
            anterior = acta.Hash;
        }
    }

    /// <summary>Devuelve el índice de la primera acta alterada, o null si la cadena es íntegra.</summary>
    public static int? PrimeraActaAlterada(IReadOnlyList<Acta> actas)
    {
        string anterior = HashGenesis;
        for (int i = 0; i < actas.Count; i++)
        {
            var acta = actas[i];
            if (acta.HashAnterior != anterior || acta.Hash != CalcularHash(acta, anterior))
                return i;
            anterior = acta.Hash;
        }
        return null;
    }
}
