# 2. Requisitos

## Objetivo general
Desarrollar en 6 semanas un prototipo de escritorio en C# que registre actas simuladas, calcule resultados por
nivel geográfico y detecte actas anómalas, logrando sobre 2.000 mesas con fraude inyectado una detección ≥ 80 %
con ≤ 10 % de falsos positivos.

## Objetivos específicos
| # | Objetivo | Cómo se comprueba |
|---|---|---|
| OE1 | Datasets sintéticos reproducibles con y sin fraude | Misma semilla, mismo archivo; actas fraudulentas etiquetadas |
| OE2 | Registro con validaciones de consistencia | Actas inválidas rechazadas con mensaje claro |
| OE3 | Integridad con cadena de hashes | Alterar un dato rompe la verificación y señala el acta |
| OE4 | Cómputo por mesa/recinto/municipio/departamento/nacional | Totales iguales a un cálculo independiente en Excel |
| OE5 | Detección con z-score robusto | Informe de precisión y recall |
| OE6 | Reporte exportable | CSV con puntaje y motivo por acta |

## Requisitos funcionales y criterios de aceptación
| ID | Funcionalidad | Criterios de aceptación |
|---|---|---|
| RF1 | Generar datos sintéticos (mesas, participación, % fraude, semilla) | Misma semilla = mismos datos; emitidos ≤ habilitados en el 100 %; fraude etiquetado |
| RF2 | Registrar/importar actas (formulario y CSV) | Rechaza suma ≠ emitidos, emitidos > habilitados, mesa duplicada, campos vacíos; error por fila |
| RF3 | Cadena de hashes SHA-256 | Modificar 1 campo y «Verificar» señala la primera acta alterada |
| RF4 | Cómputo por niveles | Totales idénticos a una hoja Excel independiente |
| RF5 | Auditoría estadística (z-score robusto, luego dispersión) | Sobre 2.000 mesas genera puntaje por acta y marca las que superan el umbral |
| RF6 | Reporte de sospechosas | CSV; cada fila indica la prueba que disparó la alerta |
| RF7 | Evaluación del detector | Precisión, recall y falsos positivos con las etiquetas del simulador |
| RF8 | Roles y bitácora | Acciones registradas con usuario y hora; el Operador no accede a auditoría |

## Requisitos de calidad
Integridad (actas inmutables), rendimiento (10.000 actas auditadas en < 10 s), usabilidad (flujo completo en < 5 min),
reproducibilidad, compatibilidad (Windows 10/11, .NET 8), privacidad (sin datos personales), mantenibilidad (capas y pruebas).

## Éxito del proyecto
Criterios de aceptación del MVP al 100 %, métricas de detección alcanzadas y demo ejecutable desde cero con el README en < 15 minutos.
