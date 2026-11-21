# Validación de la interfaz (WinForms) en Windows

**Fecha:** 21/11/2026 (capturas de pantalla 18:02–18:08)
**Equipo:** PC con **Windows 11 limpio** (última versión) y la última versión del SDK de .NET instalada para la
prueba (el proyecto apunta a `net8.0`). Falta repetirla en el equipo del docente (ver §5).
**Cómo se ejecutó:** `dotnet run --project src/AuditaActas.UI`
**Datos:** 2.000 mesas, 5 % de fraude, semilla 42 (dataset estándar).

## 1. Recorrido completo del flujo de la demo

| # | Paso | Resultado observado | Captura |
|---|---|---|---|
| 1 | Generar datos | Mensaje: "2000 actas generadas y encadenadas \| 106 con fraude etiquetado \| semilla 42" | [01](capturas/01-generar-datos.png) |
| 2 | Verificar integridad | "Integridad OK: 2000 actas verificadas, la cadena es válida." | [02](capturas/02-verificar-integridad-ok.png) |
| 3 | Alterar 1 acta (demo) y verificar | "Cadena rota en la posición 1001: el acta D03/M021/R0101/1 no coincide con su hash." | [03](capturas/03-cadena-rota-tras-alterar.png) |
| 4 | Resultados por recinto | "Cómputo por Recinto: 200 fila(s), 2000 actas sumadas." | [04](capturas/04-resultados-por-recinto.png) |
| 5 | Auditar | "154 actas sospechosas de 2000 (umbral 3.0). Recall 95.3 % \| Falsos positivos 2.8 % \| Precisión 65.6 %." | [05](capturas/05-auditoria-154-sospechosas.png) |
| 6 | Evaluación del detector | VP 101, FP 53, FN 5, VN 1841; precisión 65,6 %, recall 95,3 %, FP 2,8 %, exactitud 97,1 %, F1 77,7 %, umbral 3,0 | [06](capturas/06-evaluacion-detector.png) |
| 7 | Exportar sospechosas y abrir en Excel | CSV con acta, puntaje, z-scores y motivo; tildes correctas ("Participación atípica") | [07](capturas/07-csv-sospechosas-excel.png) |

Los valores coinciden con `reporte-verificacion.md` (106 etiquetadas, 154 alertas, 101/53/5/1841).

## 2. Importación CSV (RF2)

| Archivo | Resultado observado | Captura |
|---|---|---|
| `datos/ejemplo_minimo.csv` | "3 actas importadas y encadenadas \| 0 problema(s) detectado(s)." | [08](capturas/08-importar-csv-valido.png) |
| `datos/ejemplo_con_errores.csv` | "1 actas importadas y encadenadas \| 2 problema(s) detectado(s)." (1 válida, 2 rechazadas) | [09](capturas/09-importar-csv-con-errores.png) |
| Botón *Registrar acta* (alta manual) | Se abre el formulario "Registrar acta" con departamento, municipio, recinto, mesa, habilitados, emitidos, blancos, nulos y un campo por candidato (A–D), con botones Cancelar y Registrar | [13](capturas/13-registrar-acta-formulario.png) |

La captura 13 muestra el formulario abierto con sus valores iniciales; no muestra el resultado de pulsar
*Registrar*. Las reglas del alta (rechazo de suma ≠ emitidos y de mesa duplicada) se verifican con las pruebas
automáticas `RegistroTests` y `ActaTests`.

## 3. Persistencia SQLite (guardar y cargar base)

| Paso | Resultado observado | Captura |
|---|---|---|
| Guardar base, cargar base y verificar | "Integridad OK: 2000 actas verificadas, la cadena es válida." Los hashes de las filas son idénticos a los generados (p. ej. `EE31BF46E558` y `19FE73A49043`, ver captura 01): la cadena sobrevive al viaje a SQLite | [12](capturas/12-guardar-cargar-base-verificar.png) |

Observación: al cargar desde la base, la columna *Etiqueta* del acta fraudulenta muestra `registrado_en_bd`
en lugar del tipo original (`transferencia_votos`). Es el comportamiento del repositorio: la tabla guarda
solo la marca `es_fraude_simulado` (0/1), no el tipo. No afecta la integridad ni las métricas; el tipo de
fraude solo se conserva en el dataset recién generado.

## 4. Roles y bitácora (RF8)

| Comprobación | Resultado observado | Captura |
|---|---|---|
| Rol Operador | Los botones Auditar, Evaluación del detector y Exportar sospechosas aparecen deshabilitados; solo puede ver resultados | [10](capturas/10-operador-sin-auditoria.png) |
| Rol Administrador | Puede abrir la bitácora: id, fecha, usuario, rol, acción y detalle de cada acción (generar, verificar, alterar, auditar, evaluar, exportar, importar, ver resultados) | [11](capturas/11-bitacora-administrador.png) |

La bitácora también registra los cambios de rol: la acción "Ver resultados" del Operador aparece con rol
Operador, y las anteriores con rol Administrador.

## 5. Lo que aún NO está validado

- Ejecución en el equipo del docente.
- el selector de rol quedan parcialmente ocultos; hay barra de desplazamiento
  horizontal). Es un detalle de presentación; maximizar la ventana lo resuelve.
