using AuditaActas.Dominio;

namespace AuditaActas.Aplicacion;

public record ParametrosSimulacion(
    int Mesas = 2000,
    double ParticipacionMedia = 0.80,
    double PorcentajeFraude = 0.05,
    int Semilla = 42,
    string[]? Candidatos = null);

/// <summary>RF1: genera actas sintéticas reproducibles (misma semilla = mismos datos) con fraude etiquetado.</summary>
public class GeneradorService
{
    private const int MesasPorRecinto = 10;
    private const int RecintosPorMunicipio = 5;
    private const int MunicipiosPorDepartamento = 8;

    public List<Acta> Generar(ParametrosSimulacion p)
    {
        var rng = new Random(p.Semilla);
        var candidatos = p.Candidatos is { Length: >= 2 }
            ? p.Candidatos
            : new[] { "Candidato A", "Candidato B", "Candidato C", "Candidato D" };

        var pesosPorMunicipio = new Dictionary<int, double[]>();
        var actas = new List<Acta>(p.Mesas);

        for (int i = 0; i < p.Mesas; i++)
        {
            int recinto = i / MesasPorRecinto;
            int municipio = recinto / RecintosPorMunicipio + 1;
            int departamento = (municipio - 1) / MunicipiosPorDepartamento + 1;

            if (!pesosPorMunicipio.TryGetValue(municipio, out var pesos))
            {
                pesos = GenerarPesos(rng, candidatos.Length);
                pesosPorMunicipio[municipio] = pesos;
            }

            var acta = new Acta
            {
                Departamento = $"D{departamento:00}",
                Municipio = $"M{municipio:000}",
                Recinto = $"R{recinto + 1:0000}",
                Mesa = i % MesasPorRecinto + 1,
                Habilitados = rng.Next(150, 301)
            };

            double participacion = Math.Clamp(Normal(rng, p.ParticipacionMedia, 0.06), 0.40, 0.98);
            acta.Emitidos = (int)Math.Round(acta.Habilitados * participacion);
            acta.Blancos = (int)Math.Round(acta.Emitidos * (0.02 + rng.NextDouble() * 0.02));
            acta.Nulos = (int)Math.Round(acta.Emitidos * (0.03 + rng.NextDouble() * 0.02));

            int validos = acta.Emitidos - acta.Blancos - acta.Nulos;
            foreach (var c in candidatos) acta.Votos[c] = 0;
            for (int v = 0; v < validos; v++)
                acta.Votos[candidatos[Sortear(rng, pesos)]]++;

            if (rng.NextDouble() < p.PorcentajeFraude)
                InyectarFraude(acta, rng, candidatos);

            actas.Add(acta);
        }

        return actas;
    }

    /// <summary>Dos tipos de fraude: relleno de urna (100% de participación) y transferencia de votos al favorecido.</summary>
    private static void InyectarFraude(Acta acta, Random rng, string[] candidatos)
    {
        acta.EsFraudeSimulado = true;
        var favorecido = candidatos[0];

        if (rng.Next(2) == 0)
        {
            int extra = acta.Habilitados - acta.Emitidos;
            acta.Emitidos = acta.Habilitados;
            acta.Votos[favorecido] += extra;
        }
        else
        {
            foreach (var otro in candidatos.Skip(1))
            {
                int mover = (int)Math.Round(acta.Votos[otro] * 0.30);
                acta.Votos[otro] -= mover;
                acta.Votos[favorecido] += mover;
            }
        }
    }

    private static double[] GenerarPesos(Random rng, int n)
    {
        var w = Enumerable.Range(0, n).Select(_ => rng.NextDouble() + 0.2).ToArray();
        double suma = w.Sum();
        return w.Select(x => x / suma).ToArray();
    }

    private static int Sortear(Random rng, double[] pesos)
    {
        double u = rng.NextDouble(), acumulado = 0;
        for (int i = 0; i < pesos.Length; i++)
        {
            acumulado += pesos[i];
            if (u <= acumulado) return i;
        }
        return pesos.Length - 1;
    }

    // Box-Muller
    private static double Normal(Random rng, double media, double desviacion)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = rng.NextDouble();
        return media + desviacion * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
