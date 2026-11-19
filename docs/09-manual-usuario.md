# 9. Manual de usuario

## 9.1 Qué es AuditaActas

Es un prototipo académico de escritorio para **auditar actas electorales simuladas**. Hace tres cosas:

1. **Verifica integridad:** si un acta cambió después de registrarse, lo detecta y dice cuál.
2. **Calcula resultados** por recinto, municipio, departamento y país.
3. **Prioriza la revisión:** marca actas sospechosas con un puntaje y un motivo, y exporta un CSV.

> Los resultados son **indicios estadísticos**, no prueba de fraude. El sistema no decide ni acusa: ayuda a
> decidir qué revisar primero.

## 9.2 Requisitos

- Windows 10 u 11.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (o Visual Studio 2022 con la carga de trabajo
  *Desarrollo de escritorio de .NET*).
- 100 MB libres. No hace falta internet después de la primera restauración de paquetes.
- No se necesitan permisos de administrador para ejecutar la app.

## 9.3 Instalación y ejecución

```bash
dotnet restore
dotnet build
dotnet test          # opcional: 47 pruebas
dotnet run --project src/AuditaActas.UI
```

También se puede abrir `AuditaActas.sln` en Visual Studio, seleccionar `AuditaActas.UI` como proyecto de
inicio y presionar **F5**.

Si `dotnet` no se reconoce, instale el SDK y cierre/abra la terminal. Si la app no abre, revise
`docs/09-manual-usuario.md` §9.9.

## 9.4 La ventana principal

La barra superior tiene dos filas:

**Fila 1 — Datos y persistencia**

| Control | Qué hace |
|---|---|
| Mesas / % fraude / Semilla | Parámetros del simulador. Por defecto: 2.000 / 5 / 42. |
| **Generar datos** | Crea actas sintéticas, las etiqueta y las encadena. Reemplaza lo que haya en memoria. |
| **Importar CSV** | Carga un archivo con el formato de §9.6. Informa filas rechazadas. |
| **Registrar acta** | Abre un formulario para alta manual con validación. |
| **Guardar base** | Guarda las actas y la bitácora en un archivo SQLite (`.db`). |
| **Cargar base** | Abre un `.db` y verifica la cadena al cargar. |

**Fila 2 — Análisis, roles y salida**

| Control | Qué hace | Roles |
|---|---|---|
| **Verificar integridad** | Recalcula la cadena y resalta el primer acta alterada. | Todos |
| **Alterar 1 acta (demo)** | Modifica un voto a propósito para demostrar la detección. | Auditor/Admin |
| Nivel + **Resultados** | Cómputo por recinto, municipio, departamento o nacional. | Todos |
| **Auditar** | Lista de sospechosas ordenada por puntaje, con motivo. | Auditor/Admin |
| **Evaluación del detector** | Matriz de confusión y métricas (si hay etiquetas). | Auditor/Admin |
| **Exportar sospechosas** | Guarda el CSV del reporte. | Auditor/Admin |
| Usuario / Rol | Identidad de la sesión y permisos. | — |
| **Bitácora** | Historial de acciones de la sesión. | Administrador |

La barra inferior muestra el estado: cuántas actas hay, qué se hizo y, cuando corresponde, las métricas.

## 9.5 Flujo recomendado (demo de 5 minutos)

1. **Generar datos:** mesas 2000, fraude 5 %, semilla 42 → “2000 actas generadas y encadenadas… 106 con fraude”.
2. **Verificar integridad:** debe decir “Integridad OK”.
3. **Alterar 1 acta (demo):** cambia un voto; el estado avisa.
4. **Verificar integridad:** “Cadena rota en la posición … el acta … no coincide con su hash”; la fila se pinta.
5. **Resultados:** elija “Departamento” y vea los totales.
6. **Auditar:** aparecen las sospechosas; el estado muestra recall/falsos positivos.
7. **Evaluación del detector:** vea VP/FP/FN/VN y métricas.
8. **Exportar sospechosas:** guarde el CSV y ábralo en Excel.

Para reiniciar, pulse **Generar datos** otra vez (confirme). La base guardada no se modifica.

## 9.6 Formato del CSV

La primera fila debe tener **nueve columnas fijas** y luego **una columna por candidato**:

```
Departamento,Municipio,Recinto,Mesa,Habilitados,Emitidos,Blancos,Nulos,EsFraudeSimulado,Candidato A,Candidato B,Candidato C,Candidato D
D01,M001,R0001,1,200,160,4,6,0,60,45,30,15
```

Reglas:

- `Mesa`, `Habilitados`, `Emitidos`, `Blancos`, `Nulos` y los votos son enteros ≥ 0.
- `Habilitados` > 0; `Emitidos` ≤ `Habilitados`.
- `Emitidos` = suma de votos + blancos + nulos.
- `EsFraudeSimulado`: `1` si es una etiqueta del simulador, `0` si es un acta normal. En una carga real,
  use `0`.
- La clave `Departamento/Municipio/Recinto/Mesa` no puede repetirse (sin distinguir mayúsculas).
- Los nombres con comas deben ir entre comillas: `"Partido, Uno"`.
- El archivo puede tener BOM de UTF-8; el importador lo ignora.
- Las filas inválidas se rechazan individualmente y se muestra el número de fila y el motivo.

Ejemplos: `datos/ejemplo_minimo.csv` (3 válidas) y `datos/ejemplo_con_errores.csv` (1 válida + 2 rechazos).

## 9.7 Roles y permisos

| Acción | Operador | Auditor | Administrador |
|---|:---:|:---:|:---:|
| Generar / importar / registrar | Sí | Sí | Sí |
| Verificar integridad | Sí | Sí | Sí |
| Guardar / cargar base | Sí | Sí | Sí |
| Alterar en demo | No | Sí | Sí |
| Auditar | **No** | Sí | Sí |
| Evaluación del detector | **No** | Sí | Sí |
| Exportar sospechosas | **No** | Sí | Sí |
| Ver bitácora | No | No | **Sí** |

El rol se elige en la barra superior. Es un prototipo: no hay contraseña. Si el rol es Operador, los botones de
auditoría se deshabilitan y el servicio también rechaza la operación (no es solo cosmético).

## 9.8 Persistencia y reporte

- **Guardar base:** elija un archivo `.db` nuevo. Se guardan las actas y la bitácora. Si el archivo ya existe,
  la operación puede fallar por claves duplicadas (las actas no se sobrescriben).
- **Cargar base:** lee las actas en orden, conserva los hashes y verifica la cadena.
- **Exportar sospechosas:** genera un CSV con columnas:
  `Acta, Departamento, Municipio, Recinto, Mesa, Participacion, Puntaje, ZParticipacion, ZApoyo, Motivo`.
- El reporte de evidencias del proyecto se regenera con:
  `dotnet run --project src/AuditaActas.Evidencia`.

## 9.9 Problemas frecuentes

| Síntoma | Causa probable | Solución |
|---|---|---|
| `dotnet` no se reconoce | SDK no instalado o PATH | Instalar .NET 8 SDK y reabrir la terminal. |
| La app no abre | Falta Windows Desktop Runtime / no es Windows | Instalar el runtime de escritorio; ejecutar en Windows. |
| `error NETSDK...` al compilar | SDK muy viejo | Instalar .NET 8 o superior. |
| No se importa ninguna fila | Encabezado distinto o columnas de más/menos | Copiar el encabezado de §9.6; revisar comas. |
| “Emitidos superan a habilitados” | Dato inconsistente | Corregir la fila; el acta no se importa. |
| “Mesa duplicada” | Ya existe esa clave | Usar otra mesa o cargar la base correcta. |
| “La cadena está rota” | Se alteró un dato después de encadenar | Es el comportamiento esperado; restaure el dato o regenere. |
| Guardar base falla | El archivo ya tiene esas claves | Guardar en un archivo nuevo. |
| La evaluación no muestra métricas | El CSV no tiene etiquetas de fraude | Generar datos con el simulador. |
| No aparece “Bitácora” | Rol distinto de Administrador | Cambiar el rol. |

## 9.10 Limitaciones que debe conocer

- Es un **prototipo académico** con datos sintéticos; no use datos personales ni reales.
- No prueba fraude ni reemplaza a Sirepre/SCORC; solo prioriza revisión humana.
- La cadena de hashes detecta alteraciones, pero no protege contra alguien que reescriba toda la base y
  recalcule los hashes. Para producción haría falta firma digital y anclaje externo.
- No hay autenticación con contraseña ni cifrado de la base.
- La UI es Windows; el núcleo y la consola de evidencias son multiplataforma.
- El gráfico de dispersión y chi-cuadrado no están implementados (post-MVP).
