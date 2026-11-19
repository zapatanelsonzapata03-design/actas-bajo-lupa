using System.Text.Json;
using AuditaActas.Dominio;
using Microsoft.Data.Sqlite;

namespace AuditaActas.Infraestructura;

/// <summary>
/// Persistencia en SQLite. Las actas son de SOLO INSERCIÓN: este repositorio no expone
/// operaciones de edición ni borrado. Ejemplo de cadena: "Data Source=audita.db" o "Data Source=:memory:".
/// </summary>
public class ActaRepositorio : IDisposable
{
    public const string Esquema = """
        CREATE TABLE IF NOT EXISTS actas (
            orden INTEGER PRIMARY KEY AUTOINCREMENT,
            clave TEXT NOT NULL UNIQUE,
            departamento TEXT NOT NULL,
            municipio TEXT NOT NULL,
            recinto TEXT NOT NULL,
            mesa INTEGER NOT NULL,
            habilitados INTEGER NOT NULL,
            emitidos INTEGER NOT NULL,
            blancos INTEGER NOT NULL,
            nulos INTEGER NOT NULL,
            votos_json TEXT NOT NULL,
            es_fraude_simulado INTEGER NOT NULL DEFAULT 0,
            hash_anterior TEXT NOT NULL,
            hash TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS bitacora (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            fecha_utc TEXT NOT NULL,
            usuario TEXT NOT NULL,
            accion TEXT NOT NULL,
            detalle TEXT
        );
        """;

    private readonly SqliteConnection _conexion;

    public ActaRepositorio(string cadenaConexion)
    {
        _conexion = new SqliteConnection(cadenaConexion);
        _conexion.Open();
        using var cmd = _conexion.CreateCommand();
        cmd.CommandText = Esquema;
        cmd.ExecuteNonQuery();
    }

    public void InsertarVarias(IEnumerable<Acta> actas)
    {
        using var tx = _conexion.BeginTransaction();
        foreach (var acta in actas) Insertar(acta, tx);
        tx.Commit();
    }

    public void Insertar(Acta acta, SqliteTransaction? tx = null)
    {
        using var cmd = _conexion.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            INSERT INTO actas (clave, departamento, municipio, recinto, mesa, habilitados, emitidos,
                               blancos, nulos, votos_json, es_fraude_simulado, hash_anterior, hash)
            VALUES ($clave, $dep, $mun, $rec, $mesa, $hab, $emi, $bla, $nul, $votos, $fraude, $ant, $hash);
            """;
        cmd.Parameters.AddWithValue("$clave", acta.Clave);
        cmd.Parameters.AddWithValue("$dep", acta.Departamento);
        cmd.Parameters.AddWithValue("$mun", acta.Municipio);
        cmd.Parameters.AddWithValue("$rec", acta.Recinto);
        cmd.Parameters.AddWithValue("$mesa", acta.Mesa);
        cmd.Parameters.AddWithValue("$hab", acta.Habilitados);
        cmd.Parameters.AddWithValue("$emi", acta.Emitidos);
        cmd.Parameters.AddWithValue("$bla", acta.Blancos);
        cmd.Parameters.AddWithValue("$nul", acta.Nulos);
        cmd.Parameters.AddWithValue("$votos", JsonSerializer.Serialize(acta.Votos));
        cmd.Parameters.AddWithValue("$fraude", acta.EsFraudeSimulado ? 1 : 0);
        cmd.Parameters.AddWithValue("$ant", acta.HashAnterior);
        cmd.Parameters.AddWithValue("$hash", acta.Hash);
        cmd.ExecuteNonQuery();
    }

    public List<Acta> ObtenerTodas()
    {
        var actas = new List<Acta>();
        using var cmd = _conexion.CreateCommand();
        cmd.CommandText = """
            SELECT departamento, municipio, recinto, mesa, habilitados, emitidos, blancos, nulos,
                   votos_json, es_fraude_simulado, hash_anterior, hash
            FROM actas ORDER BY orden;
            """;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            actas.Add(new Acta
            {
                Departamento = r.GetString(0),
                Municipio = r.GetString(1),
                Recinto = r.GetString(2),
                Mesa = r.GetInt32(3),
                Habilitados = r.GetInt32(4),
                Emitidos = r.GetInt32(5),
                Blancos = r.GetInt32(6),
                Nulos = r.GetInt32(7),
                Votos = JsonSerializer.Deserialize<Dictionary<string, int>>(r.GetString(8)) ?? new(),
                EsFraudeSimulado = r.GetInt32(9) == 1,
                HashAnterior = r.GetString(10),
                Hash = r.GetString(11)
            });
        }
        return actas;
    }

    public void RegistrarBitacora(string usuario, string accion, string? detalle = null)
    {
        using var cmd = _conexion.CreateCommand();
        cmd.CommandText = "INSERT INTO bitacora (fecha_utc, usuario, accion, detalle) VALUES ($f, $u, $a, $d);";
        cmd.Parameters.AddWithValue("$f", DateTime.UtcNow.ToString("O"));
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.Parameters.AddWithValue("$a", accion);
        cmd.Parameters.AddWithValue("$d", (object?)detalle ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _conexion.Dispose();
}
