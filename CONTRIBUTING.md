# Flujo de trabajo del equipo

1. `main` **siempre compila y pasa las pruebas**. Nadie hace push directo a `main`.
2. Una rama por funcionalidad: `feature/rf3-hash`, `fix/validacion-acta`, `docs/riesgos`.
3. Commits pequeños y claros, en presente: `Agrega validación de mesas duplicadas`.
4. Todo cambio entra por **Pull Request aprobado por otro integrante** (revisión cruzada).
5. Antes de abrir el PR: `dotnet build` y `dotnet test` en verde.
6. La capa de UI no calcula: la lógica va en `AuditaActas.Aplicacion` y se prueba con xUnit.
7. Las actas son de solo inserción: no agregar operaciones de edición ni borrado en el repositorio.
8. Reunión de 15 minutos dos veces por semana; se actualiza `docs/07-backlog.md`.

## Responsables suplentes
A ↔ D, B ↔ C (si alguien falta, su suplente continúa el módulo).
