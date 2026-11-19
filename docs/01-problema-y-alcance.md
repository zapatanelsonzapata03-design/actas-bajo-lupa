# 1. Problema y alcance

## Problema (30 segundos, sin tecnología)
El resultado de una elección se construye sumando miles de actas de mesa llenadas a mano. Si una acta se altera,
se duplica o se llena con números inventados, detectarlo hoy depende de que alguien lo note revisando papeles.
Se necesita una forma **objetiva, rápida y verificable** de decir «esta acta es sospechosa y hay que revisarla».

## Quién lo tiene y quién usa la solución
- Tienen el problema: observadores, delegados de partidos, tribunales electorales, ciudadanía.
- Usuarios del prototipo: **Auditor/Analista**, **Operador de carga**, **Administrador**.

## Evidencia
- Análisis de integridad electoral de la OEA sobre 2019: el TREP se interrumpió al 83,85 % del conteo, hubo
  problemas de cadena de custodia y más del 5 % de las imágenes de actas del TREP pasaron directo al cómputo
  oficial (ver C. de Oliveira Santos, *Elecciones bolivianas 2025: integridad, resiliencia y cambios
  estructurales*, UNAM).
- En 2025 el Órgano Electoral usó Sirepre (conteo rápido informativo) y SCORC (cómputo oficial vinculante).

### Fuentes complementarias
- **OEA, auditoría de las elecciones de 2019:** [Final Report of the Audit of the Elections in Bolivia](https://www.oas.org/fpdb/press/Audit-Report-EN-vFINAL.pdf). Es un antecedente sobre ese proceso electoral; no demuestra fraude en elecciones posteriores ni valida el detector de este prototipo. El sitio de la OEA no respondió durante esta consulta, por lo que conviene comprobar que el enlace abra antes de la entrega.
- **OEP, cómputo oficial de 2025:** [proclamación de resultados de la segunda vuelta](https://www.oep.org.bo/institucional-institucional/tse-proclama-resultados-oficiales-de-la-segunda-vuelta-electoral/). El TSE informa que SCORC procesó el 100 % de 35.253 actas. Es la fuente oficial del cómputo, no una auditoría independiente.
- **Prensa, cómputo de primera vuelta de 2025:** [El Deber, avance del cómputo en tres departamentos](https://eldeber.com.bo/pais/tres-departamentos-concluyeron-el-computo-oficial-de-votos_527004/). La nota reporta un avance de 68,73 % y el cierre del cómputo en Oruro, Pando y Tarija en el momento de publicación; es una fotografía temporal, no evidencia de irregularidades.

Estas fuentes contextualizan la trazabilidad y el cómputo electoral; no son evidencia de que las actas de 2025 hayan sido alteradas. El prototipo usa datos sintéticos y solo prioriza revisión humana.

## Consecuencias actuales
Desconfianza, conflictos postelectorales, cuestionamiento o anulación de procesos y alto costo de revisión manual.

## Valor y por qué esta solución
Reduce el universo de revisión a una lista priorizada con el motivo estadístico y garantiza que las actas
registradas no cambiaron después (cadena de hashes). Ataca las dos causas: **alteración posterior** y **datos anómalos**.

## Alcance
**Sí:** simulador, registro/importación, cadena de hashes, cómputo, auditoría estadística, reporte, evaluación, pruebas, documentación.
**No:** sistema de votación, reemplazo de Sirepre/SCORC, OCR de imágenes, datos personales, blockchain distribuida,
aplicación web o móvil, conclusiones de «fraude probado».
**Tentaciones descartadas:** mapa de calor, ley de Benford, machine learning, PDF con diseño, datos reales del OEP.

## Si hay que reducir un 40 %
Se conserva RF1, RF2, RF4, RF5 (z-score) y RF6. Se elimina primero RF8 (roles), la pantalla de RF7,
el gráfico de dispersión y chi-cuadrado.
