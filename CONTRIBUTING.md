# Flujo de trabajo del equipo

## Reglas de oro

1. `main` **siempre compila y pasa las pruebas**. Nadie hace push directo a `main`.
2. Una rama por funcionalidad o corrección: `feature/rf3-hash`, `fix/validacion-acta`, `docs/riesgos`.
3. Commits pequeños y claros, en presente: `Agrega validación de mesas duplicadas`.
4. Todo cambio entra por **Pull Request aprobado por otro integrante** (revisión cruzada).
5. Antes de abrir el PR: `dotnet build` (0 advertencias) y `dotnet test` (47+ pruebas en verde).
6. La capa de UI **no calcula**: la lógica va en `AuditaActas.Aplicacion` y se prueba con xUnit.
7. Las actas son de **solo inserción**: no agregar operaciones de edición ni borrado en el repositorio.
8. Reunión de 15 minutos dos veces por semana; se actualiza `docs/07-backlog.md`.

## Ramas y commits

| Tipo | Patrón | Ejemplo |
|---|---|---|
| Funcionalidad | `feature/rfX-descripcion` | `feature/rf5-auditoria-zscore` |
| Corrección | `fix/descripcion` | `fix/validacion-duplicados` |
| Documentación | `docs/descripcion` | `docs/cuestionario` |
| Pruebas | `test/descripcion` | `test/csv-comillas` |

Un commit debe poder explicarse en una frase. Si toca varias cosas no relacionadas, se divide.

## Pull Request

Use la plantilla de `.github/pull_request_template.md`. El PR debe indicar:

- Qué cambia (1–2 líneas) y tarea del backlog (T1–T21).
- Requisito relacionado (RF1–RF8) y criterio de aceptación.
- Checklist: build, test, prueba nueva/actualizada, UI sin lógica, documentación.
- Evidencia: captura, salida de test o archivo generado (si aplica).

**Revisor:** otra persona del equipo (no el autor). Revisa lógica, pruebas, nombres, duplicación y que no se
haya roto la arquitectura. Comenta con `sugerencia:` o `bloqueante:`.

## Definition of Done

Un cambio está terminado cuando:

1. `dotnet build` no tiene errores ni advertencias.
2. `dotnet test` pasa (0 fallos).
3. Se agregó o actualizó una prueba si cambió la lógica.
4. La UI sigue sin lógica de negocio.
5. La documentación afectada se actualizó (requisito, plan, README, guion).
6. Otro integrante revisó y aprobó el PR.
7. El CI está en verde.

## Responsables y suplentes

| Integrante | Módulo | Suplente |
|---|---|---|
| Alex Vásquez | Coordinación, arquitectura, persistencia, hash, cómputo | Nelson |
| Nelson Zapata | Datos, simulación, estadística, evaluación | Alex |
| Cristian García | UI, reportes, pruebas, CI | Nelson |

Si alguien falta, su suplente continúa el módulo con la documentación y las pruebas como contrato. Ningún
módulo depende de conocimiento no escrito.

## Reglas de código

- C# con `Nullable` e `ImplicitUsings` activados (`Directory.Build.props`).
- Nombres de clases/servicios en español, consistentes con el dominio (`Acta`, `AuditoriaService`).
- Sin dependencias nuevas sin justificar en `docs/03-arquitectura.md` §3.5.
- Sin `Console.WriteLine` en la UI; los mensajes van a la barra de estado o a un `MessageBox`.
- Sin `Thread.Sleep`, sin rutas absolutas ni credenciales en el código.
- Los datos de prueba se generan con semilla; no se commitean bases `.db` (están en `.gitignore`).

## Pruebas

- Cada servicio nuevo tiene pruebas en `tests/AuditaActas.Tests`.
- Para integración se usa `Data Source=:memory:`.
- Los archivos temporales se borran en `finally`.
- Las pruebas no dependen del orden de ejecución ni de la UI.

## Documentación

- `README.md`: estado, comandos y índice.
- `docs/02-requisitos.md`: qué y criterios de aceptación.
- `docs/03-arquitectura.md`: dónde y por qué.
- `docs/04-plan-gantt.md` y `docs/07-backlog.md`: cuándo y quién.
- `docs/06-pruebas.md` y `docs/evidencias/`: cómo se comprueba.
- Si cambia el comportamiento, se actualiza el doc en el mismo PR.

## CI

`.github/workflows/ci.yml` ejecuta en Windows:

```
dotnet restore
dotnet build --no-restore -warnaserror
dotnet test --no-build
dotnet run --project src/AuditaActas.Evidencia --no-build
```

El reporte de evidencias se publica como artefacto. Un PR con CI rojo no se mergea.
