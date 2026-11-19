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
}
