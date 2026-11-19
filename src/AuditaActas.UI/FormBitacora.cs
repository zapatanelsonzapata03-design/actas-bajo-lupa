using AuditaActas.Dominio;

namespace AuditaActas.UI;

/// <summary>Vista de solo lectura de la bitácora (RF8).</summary>
public class FormBitacora : Form
{
    public FormBitacora(IEnumerable<RegistroBitacora> registros)
    {
        Text = "Bitácora de acciones";
        Width = 900;
        Height = 450;
        StartPosition = FormStartPosition.CenterParent;

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            DataSource = registros
                .OrderByDescending(r => r.FechaUtc)
                .Select(r => new
                {
                    r.Id,
                    Fecha = r.FechaLocal,
                    r.Usuario,
                    Rol = r.Rol.ToString(),
                    r.Accion,
                    Detalle = r.Detalle ?? string.Empty
                })
                .ToList()
        };

        Controls.Add(grid);
    }
}
