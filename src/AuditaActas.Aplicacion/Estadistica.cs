namespace AuditaActas.Aplicacion;

/// <summary>Funciones estadísticas robustas (mediana y MAD) para no dejar que los outliers enmascaren el fraude.</summary>
public static class Estadistica
{
    public static double Mediana(IReadOnlyList<double> valores)
    {
        int n = valores.Count;
        if (n == 0) return 0;
        var ordenados = valores.OrderBy(x => x).ToList();
        return n % 2 == 1 ? ordenados[n / 2] : (ordenados[n / 2 - 1] + ordenados[n / 2]) / 2.0;
    }

    /// <summary>z-score robusto: (x - mediana) / (1.4826 * MAD). Si MAD = 0 devuelve ceros.</summary>
    public static double[] ZRobusto(IReadOnlyList<double> valores)
    {
        var z = new double[valores.Count];
        if (valores.Count == 0) return z;

        double mediana = Mediana(valores);
        double mad = Mediana(valores.Select(x => Math.Abs(x - mediana)).ToList()) * 1.4826;
        if (mad < 1e-9) return z;

        for (int i = 0; i < valores.Count; i++)
            z[i] = (valores[i] - mediana) / mad;
        return z;
    }
}
