# 5. Riesgos y prueba de fracaso

## Tres riesgos principales
| # | Riesgo | Prob. | Impacto | Mitigación |
|---|---|---|---|---|
| R1 | El detector da demasiados falsos positivos o no llega a las metas | Media | Alto | Spikes en la semana 1; umbral configurable; si hace falta bajar la meta a recall ≥ 70 % documentando el análisis |
| R2 | Retrasos por poca disponibilidad o conflictos de integración | Alta | Alto | Tareas pequeñas, PR frecuentes, recorte del 40 % del alcance |
| R3 | Problemas de entorno o librerías (versión de .NET, paquetes) | Baja | Medio | Versiones fijas en el repositorio; probar la ejecución en otra máquina en la semana 2 |

## Si una persona clave falta
Cada módulo tiene suplente (A↔D, B↔C), el código se revisa por pares y está documentado. Con menos personas se aplica el recorte de alcance y se prioriza el MVP.

## Prueba de fracaso
| Condición de fracaso | Control |
|---|---|
| El simulador es tan simple que el detector «funciona» sin demostrar nada | Varios tipos de fraude, ruido realista y limitaciones documentadas |
| No se alcanzan las métricas y no hay análisis | Las metas son revisables; se entrega un análisis honesto |
| El MVP no está en la semana 4 | Recorte inmediato; simulador y cómputo son innegociables |
| El trabajo no se integra a tiempo | PR obligatorios, `main` estable, integración parcial desde la semana 2 |
| Alguien no puede ejecutar el proyecto | Instalación limpia en otra máquina y README verificado |
| Se interpreta como «detector de fraude real» | Aviso visible y sección de limitaciones |
