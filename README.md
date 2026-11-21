# AuditaActas

Sistema de conteo y **auditoría estadística de actas electorales** con datos simulados (contexto Bolivia).
Proyecto de **Programación II (P2)**, Ingeniería en Sistemas, UNIFRANZ Cochabamba. Docente: Ing. Diego Cárdenas.

> **Aviso:** es un prototipo académico. No reemplaza a Sirepre ni a SCORC y **no prueba fraude**: solo marca
> actas que merecen revisión humana. No usa datos personales ni datos reales del OEP.

## Estado

| Requisito | Funcionalidad | Estado |
|---|---|---|
| RF1 | Generador de datos sintéticos reproducibles con fraude etiquetado | Implementado y verificado |
| RF2 | Registro manual e importación CSV con validaciones | Implementado y verificado |
| RF3 | Cadena de hashes SHA-256 y verificación | Implementado y verificado |
| RF4 | Cómputo por recinto, municipio, departamento y nacional | Implementado y verificado |
| RF5 | Auditoría con z-score robusto y umbral configurable | Implementado y verificado |
| RF6 | Reporte CSV de actas sospechosas con motivo | Implementado y verificado |
| RF7 | Evaluación del detector (precisión, recall, FP, F1) | Implementado (post-MVP adelantado) |
| RF8 | Roles (Operador/Auditor/Administrador) y bitácora | Implementado (post-MVP adelantado) |
| — | Gráfico de dispersión y chi-cuadrado | Pendiente (post-MVP, no bloquea) |

**Calidad actual:** `dotnet build` con **0 advertencias y 0 errores**; `dotnet test` con **47 pruebas
aprobadas**; solución en .NET 8 (probada localmente también con SDK 10.0.400).
### Cómo leer la precisión (65,6 %)

La precisión es más baja que el recall porque el fraude es raro: solo el 5 % de las
mesas (100 de 2.000). Aunque la tasa de falsos positivos sea 2,8 %, ese 2,8 % se aplica
a las ~1.900 mesas limpias y genera unas 53 alertas falsas, frente a ~95 verdaderas.
Es un efecto de la tasa base, no un defecto del umbral. El detector se usa para
priorizar revisión humana, no para decidir fraude; por eso se prioriza el recall.

### Robustez de la evaluación

El detector se evalúa contra el fraude que inyecta el propio simulador (evaluación
circular). Para reducir ese sesgo, el reporte se repite con varias semillas y distintos
porcentajes de fraude (ver `docs/06-pruebas.md`). Los resultados de esta tabla
corresponden a la semilla 42 y no son una garantía sobre datos reales.### Cómo leer la precisión (65,6 %)

La precisión es más baja que el recall porque el fraude es raro: solo el 5 % de las
mesas (100 de 2.000). Aunque la tasa de falsos positivos sea 2,8 %, ese 2,8 % se aplica
a las ~1.900 mesas limpias y genera unas 53 alertas falsas, frente a ~95 verdaderas.
Es un efecto de la tasa base, no un defecto del umbral. El detector se usa para
priorizar revisión humana, no para decidir fraude; por eso se prioriza el recall.

### Robustez de la evaluación

El detector se evalúa contra el fraude que inyecta el propio simulador (evaluación
circular). Para reducir ese sesgo, el reporte se repite con varias semillas y distintos
porcentajes de fraude (ver `docs/06-pruebas.md`). Los resultados de esta tabla
corresponden a la semilla 42 y no son una garantía sobre datos reales.

## Métricas del objetivo general

Dataset estándar: 2.000 mesas, participación media 0,80, 5 % de fraude, semilla 42.

| Métrica | Meta | Resultado |
|---|---:|---:|
| Recall (detección) | ≥ 80 % | **95,3 %** |
| Tasa de falsos positivos | ≤ 10 % | **2,8 %** |
| Precisión | Informativa | 65,6 % |
| F1 | Informativa | 77,7 % |
| Auditoría de 10.000 actas | < 10 s | **≈32 ms** |

Reporte reproducible: [`docs/evidencias/reporte-verificacion.md`](docs/evidencias/reporte-verificacion.md).
Regenérelo con:

```bash
dotnet run --project src/AuditaActas.Evidencia
```

## El problema en 30 segundos

El resultado de una elección se arma sumando miles de actas llenadas a mano. Si un acta se altera, se duplica
o se llena con números inventados, hoy detectarlo depende de que alguien lo note revisando papeles. AuditaActas
ofrece (1) una cadena de hashes que revela cualquier alteración posterior y (2) pruebas estadísticas que
priorizan qué actas revisar. No decide si hubo fraude: entrega indicios para revisión humana.

## Cómo ejecutar

Requisitos: Windows 10/11, [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) y, opcionalmente,
Visual Studio 2022 con la carga de trabajo *Desarrollo de escritorio de .NET*.

```bash
dotnet restore
dotnet build
dotnet test                                   # 47 pruebas (xUnit)
dotnet run --project src/AuditaActas.UI       # abre la aplicación (solo Windows)
dotnet run --project src/AuditaActas.Evidencia # regenera el reporte de evidencias
```

También puede abrir `AuditaActas.sln` en Visual Studio y presionar **F5** con `AuditaActas.UI` como proyecto
de inicio.

**Flujo de demo:** Generar datos → Verificar integridad → Alterar 1 acta (demo) → Verificar → Resultados →
Auditar → Evaluación → Exportar.

## Roles

| Rol | Puede |
|---|---|
| Operador | Generar/importar/registrar, verificar, guardar/cargar base |
| Auditor | Todo lo anterior + auditar, evaluar y exportar (no ve la bitácora) |
| Administrador | Todo + consultar la bitácora |

El rol Operador **no** accede a la auditoría ni a la exportación (RF8). Es un prototipo: no hay contraseña.

## Estructura

```
AuditaActas/
├─ AuditaActas.sln
├─ Directory.Build.props              # .NET 8, nullable, implicit usings
├─ README.md
├─ CONTRIBUTING.md                    # flujo de trabajo y Definition of Done
├─ docs/                              # problema, requisitos, arquitectura, plan, riesgos, pruebas
│  ├─ 08-cuestionario-aprobacion.md   # respuestas a las 52 preguntas
│  ├─ 08-cuestionario-aprobacion.docx # versión para entregar (Word)
│  ├─ 08-cuestionario-aprobacion.pdf  # versión para entregar (PDF)
│  ├─ 09-manual-usuario.md
│  ├─ 10-guion-demo.md
│  ├─ 11-matriz-trazabilidad.md
│  └─ evidencias/                     # reporte generado + CSV de sospechosas
├─ datos/                             # CSV de ejemplo (válido y con errores)
├─ scripts/                           # exportación del cuestionario a DOCX
├─ src/
│  ├─ AuditaActas.Dominio/            # Acta, modelos, permisos
│  ├─ AuditaActas.Aplicacion/         # generador, hash, cómputo, auditoría, evaluación, registro, bitácora
│  ├─ AuditaActas.Infraestructura/    # CSV y SQLite
│  ├─ AuditaActas.UI/                 # WinForms (sin lógica de negocio)
│  └─ AuditaActas.Evidencia/          # consola que genera el reporte reproducible
└─ tests/AuditaActas.Tests/           # xUnit (47 pruebas)
```

## Equipo

| Integrante | Responsabilidad |
|---|---|
| **Alex Cristhian Vásquez Pérez** | Coordinación, arquitectura, integración, persistencia y cadena de hashes (RF2–RF4) |
| **Nelson Alejandro Zapata Jiménez** | Datos, simulación, estadística y evaluación (RF1, RF5, RF7) |
| **Cristián García Bustamante** | Interfaz, reportes y calidad/pruebas (RF6, RF8 en UI) |

Suplencias: Alex↔Nelson, Nelson↔Cristian, Cristian↔Alex. Plan de 6 semanas y 153 h en
[`docs/04-plan-gantt.md`](docs/04-plan-gantt.md).

## Documentación

1. [Problema y alcance](docs/01-problema-y-alcance.md)
2. [Usuarios y requisitos](docs/02-requisitos.md)
3. [Arquitectura y viabilidad](docs/03-arquitectura.md)
4. [Plan y Gantt](docs/04-plan-gantt.md)
5. [Riesgos y prueba de fracaso](docs/05-riesgos.md)
6. [Pruebas y evidencia](docs/06-pruebas.md)
7. [Backlog](docs/07-backlog.md)
8. [**Cuestionario de aprobación (52 respuestas)**](docs/08-cuestionario-aprobacion.md) —
   también en [DOCX](docs/08-cuestionario-aprobacion.docx) y [PDF](docs/08-cuestionario-aprobacion.pdf)
9. [Manual de usuario](docs/09-manual-usuario.md)
10. [Guion de demo y defensa](docs/10-guion-demo.md)
11. [Matriz de trazabilidad](docs/11-matriz-trazabilidad.md)

## Supuestos y pendientes

- **Docente:** falta confirmar si acepta datos simulados, el formato de entrega y si exige una base de datos
  específica. La arquitectura actual usa SQLite. No bloquea el desarrollo: el producto es autocontenido y
  exportable (CSV/SQLite).
- **Fechas:** no hay fecha de entrega/defensa confirmada. El plan de 6 semanas es una proyección desde el
  5/11/2026; al 19/11 se cumplieron dos semanas.
- **Entorno:** la solución apunta a .NET 8 (`net8.0`/`net8.0-windows`); en esta máquina se validó también con
  SDK 10.0.400. La UI requiere Windows; el núcleo y la evidencia son multiplataforma.
- **Alcance:** MVP RF1–RF6; RF7/RF8 se adelantaron; dispersión y chi-cuadrado quedan para después.

## Limitaciones declaradas

- Los datos son sintéticos y el detector se evalúa contra el fraude que el propio simulador inyecta; eso mide
  consistencia interna, no validez contra datos reales.
- La cadena de hashes detecta alteraciones, pero no protege contra un atacante que reescriba toda la base y
  recalcule los hashes. Para producción se necesitaría firma digital y anclaje externo.
- No hay autenticación con contraseña ni cifrado de la base. Es un prototipo local.
- El gráfico de dispersión y chi-cuadrado no están implementados.

## Flujo de trabajo

`main` siempre compila y pasa las pruebas; una rama por funcionalidad; PR con revisión cruzada; CI en Windows.
Ver [`CONTRIBUTING.md`](CONTRIBUTING.md) y `.github/workflows/ci.yml`.
