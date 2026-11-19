namespace AuditaActas.Dominio;

public enum NivelGeografico
{
    Recinto,
    Municipio,
    Departamento,
    Nacional
}

public record ResultadoAgregado(
    string Clave,
    int Mesas,
    int Habilitados,
    int Emitidos,
    int Blancos,
    int Nulos,
    IReadOnlyDictionary<string, int> Votos);

public record ResultadoAuditoria(
    Acta Acta,
    double ZParticipacion,
    double ZApoyo,
    double Puntaje,
    bool EsSospechosa,
    string Motivo);

public record MatrizConfusion(int VerdaderosPositivos, int FalsosPositivos, int FalsosNegativos, int VerdaderosNegativos)
{
    public double Precision => VerdaderosPositivos + FalsosPositivos == 0 ? 0 : (double)VerdaderosPositivos / (VerdaderosPositivos + FalsosPositivos);
    public double Recall => VerdaderosPositivos + FalsosNegativos == 0 ? 0 : (double)VerdaderosPositivos / (VerdaderosPositivos + FalsosNegativos);
    public double TasaFalsosPositivos => FalsosPositivos + VerdaderosNegativos == 0 ? 0 : (double)FalsosPositivos / (FalsosPositivos + VerdaderosNegativos);
    public double Exactitud => VerdaderosPositivos + FalsosPositivos + FalsosNegativos + VerdaderosNegativos == 0
        ? 0
        : (double)(VerdaderosPositivos + VerdaderosNegativos) / (VerdaderosPositivos + FalsosPositivos + FalsosNegativos + VerdaderosNegativos);
    public double F1 => Precision + Recall == 0 ? 0 : 2 * Precision * Recall / (Precision + Recall);
}

/// <summary>Resumen legible de la evaluación del detector (RF7).</summary>
public record ResumenEvaluacion(MatrizConfusion Matriz, int Total, int FraudesEtiquetados, int Alertas, double Umbral)
{
    public double Precision => Matriz.Precision;
    public double Recall => Matriz.Recall;
    public double TasaFalsosPositivos => Matriz.TasaFalsosPositivos;
    public double Exactitud => Matriz.Exactitud;
    public double F1 => Matriz.F1;
}

