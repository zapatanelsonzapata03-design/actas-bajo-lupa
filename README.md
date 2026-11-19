# AuditaActas

Sistema de conteo y **auditoría estadística de actas electorales** (datos simulados, contexto Bolivia).
Proyecto de **Programación II (P2)**, Ing. en Sistemas, UNIFRANZ Cochabamba. Docente: Ing. Diego Cárdenas.

> **Aviso:** esto es un prototipo académico. No reemplaza a Sirepre ni a SCORC y **no prueba fraude**:
> solo marca actas que merecen revisión humana.

## El problema en 30 segundos
El resultado de una elección se arma sumando miles de actas llenadas a mano. Si una acta se altera, se duplica
o se llena con números inventados, hoy es lento y subjetivo detectarlo. AuditaActas ofrece (1) una cadena de
hashes que revela cualquier alteración posterior y (2) pruebas estadísticas que priorizan qué actas revisar.

## Meta medible
Sobre 2.000 mesas simuladas con fraude inyectado: **detección ≥ 80 %** de actas manipuladas con **≤ 10 % de falsos positivos**.

## MVP (fin de la semana 4)
| Req. | Funcionalidad | Estado |
|---|---|---|
| RF1 | Generador de datos sintéticos reproducibles con fraude etiquetado | Esqueleto listo |
| RF2 | Registro/importación de actas con validaciones | Esqueleto listo |
| RF3 | Cadena de hashes SHA-256 y verificación | Esqueleto listo |
| RF4 | Cómputo por recinto, municipio, departamento y nacional | Esqueleto listo |
| RF5 | Auditoría con z-score robusto | Esqueleto listo |
| RF6 | Reporte CSV de actas sospechosas | Esqueleto listo |

Después del MVP: RF7 (pantalla de evaluación), RF8 (roles y bitácora), gráfico de dispersión, chi-cuadrado.
Detalle y criterios de aceptación en [`docs/02-requisitos.md`](docs/02-requisitos.md).

## Equipo y planificación
| Código | Integrante | Responsabilidad |
|---|---|---|
| A | Alex Cristhian Vásquez Pérez | Coordinación, arquitectura, integración y persistencia/cadena de hashes (RF2–RF4) |
| B | Nelson Alejandro Zapata Jiménez | Datos, simulación y estadística (RF1 y RF5; RF7 queda para después del MVP) |
| C | Cristián García Bustamante | Interfaz, reportes y calidad/pruebas (RF6) |

El equipo tiene tres integrantes; las responsabilidades de A y D del esquema inicial se consolidan en Alex. El inicio declarado es el 5 de noviembre de 2026 y el equipo indica que han transcurrido dos semanas al 19 de noviembre. El plan de seis semanas es referencial; la fecha de entrega o defensa todavía debe confirmarse. Ver el [plan Gantt](docs/04-plan-gantt.md).

## Datos pendientes de confirmar
- **Docente:** falta confirmar si acepta datos simulados, si exige otro tipo de aplicación, si requiere una base de datos específica (la arquitectura actual usa SQLite) y qué entregables solicita.
- **Fechas:** no se proporcionó fecha de entrega o defensa.
- **Entorno:** esta máquina tiene instalados los SDK .NET 9.0.319, 10.0.300, 10.0.301 y 10.0.400. La solución sigue apuntando a .NET 8 (`net8.0`/`net8.0-windows`); no se confirmó la versión de Visual Studio ni la de los equipos de los demás integrantes.
- **Alcance elegido para planificar:** priorizar RF1–RF6; RF7, RF8, dispersión y chi-cuadrado quedan después del MVP.

La validación local con el SDK .NET 10.0.400 terminó correctamente: `dotnet build` sin advertencias ni errores y `dotnet test` con 20 pruebas aprobadas.

## Cómo ejecutar
Requisitos: Windows 10/11, [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) y Visual Studio 2022 (carga de trabajo *Desarrollo de escritorio de .NET*).

```bash
dotnet restore
dotnet build
dotnet test                                   # corre las pruebas (xUnit)
dotnet run --project src/AuditaActas.UI       # abre la aplicación (solo Windows)
```
También puedes abrir `AuditaActas.sln` en Visual Studio y presionar F5 con `AuditaActas.UI` como proyecto de inicio.

Flujo de demo: **Generar datos → Verificar integridad → Alterar 1 acta (demo) → Verificar → Auditar → Exportar**.

## Estructura
```
AuditaActas/
├── AuditaActas.sln
├── Directory.Build.props          # .NET 8, nullable, implicit usings
├── README.md
├── CONTRIBUTING.md                # flujo de trabajo del equipo
├── docs/                          # problema, requisitos, arquitectura, plan, riesgos, pruebas, backlog
├── src/
│   ├── AuditaActas.Dominio/       # Acta, modelos, validaciones
│   ├── AuditaActas.Aplicacion/    # Generador, Hash, Cómputo, Auditoría, Evaluación
│   ├── AuditaActas.Infraestructura/ # SQLite, CSV
│   └── AuditaActas.UI/            # Windows Forms (sin lógica de negocio)
├── tests/AuditaActas.Tests/       # xUnit
└── datos/                         # CSV de ejemplo
```

## Documentación
1. [Problema y alcance](docs/01-problema-y-alcance.md)
2. [Requisitos](docs/02-requisitos.md)
3. [Arquitectura](docs/03-arquitectura.md)
4. [Plan y Gantt](docs/04-plan-gantt.md)
5. [Riesgos](docs/05-riesgos.md)
6. [Pruebas](docs/06-pruebas.md)
7. [Backlog (tareas T1–T21)](docs/07-backlog.md)
