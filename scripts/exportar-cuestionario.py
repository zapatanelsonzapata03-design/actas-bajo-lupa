# -*- coding: utf-8 -*-
"""Convierte docs/08-cuestionario-aprobacion.md a DOCX.

Uso:
    python scripts/exportar-cuestionario.py [origen.md] [destino.docx]

Requiere: pip install python-docx
El PDF se obtiene abriendo el DOCX en Word y exportando (o con Word COM).
"""
import re
import sys
from pathlib import Path
from docx import Document
from docx.shared import Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

ROOT = Path(__file__).resolve().parents[1]
SRC = Path(sys.argv[1]) if len(sys.argv) > 1 else ROOT / "docs" / "08-cuestionario-aprobacion.md"
DST = Path(sys.argv[2]) if len(sys.argv) > 2 else SRC.with_suffix(".docx")

doc = Document()

# --- Estilos base -----------------------------------------------------------
styles = doc.styles
normal = styles["Normal"]
normal.font.name = "Calibri"
normal.font.size = Pt(10.5)
normal.paragraph_format.space_after = Pt(6)
normal.paragraph_format.line_spacing = 1.12
normal.element.rPr.rFonts.set(qn("w:eastAsia"), "Calibri")

for name, size, color in (("Heading 1", 18, "1F3864"),
                          ("Heading 2", 15, "2E5496"),
                          ("Heading 3", 12.5, "1F3864")):
    st = styles[name]
    st.font.name = "Calibri"
    st.font.size = Pt(size)
    st.font.bold = True
    st.font.color.rgb = RGBColor.from_string(color)
    st.paragraph_format.space_before = Pt(14)
    st.paragraph_format.space_after = Pt(6)
    st.paragraph_format.keep_with_next = True

for section in doc.sections:
    section.page_width = Cm(21.0)
    section.page_height = Cm(29.7)
    section.left_margin = section.right_margin = Cm(2.2)
    section.top_margin = section.bottom_margin = Cm(2.0)

# --- Pie de página con numeración ------------------------------------------
def add_page_number(paragraph):
    paragraph.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run = paragraph.add_run("AuditaActas · Cuestionario de aprobación · Página ")
    run.font.size = Pt(8)
    run.font.color.rgb = RGBColor(0x80, 0x80, 0x80)
    fld = OxmlElement("w:fldSimple")
    fld.set(qn("w:instr"), "PAGE")
    r = OxmlElement("w:r")
    t = OxmlElement("w:t")
    t.text = "1"
    r.append(t)
    fld.append(r)
    paragraph._p.append(fld)

add_page_number(doc.sections[0].footer.paragraphs[0])

# --- Utilidades -------------------------------------------------------------
INLINE = re.compile(r"(\*\*.+?\*\*|`[^`]+`|\[[^\]]+\]\([^)]+\)|\*[^*]+\*)")

def add_runs(paragraph, text):
    for part in INLINE.split(text):
        if not part:
            continue
        if part.startswith("**") and part.endswith("**") and len(part) > 4:
            r = paragraph.add_run(part[2:-2]); r.bold = True
        elif part.startswith("`") and part.endswith("`") and len(part) > 2:
            r = paragraph.add_run(part[1:-1]); r.font.name = "Consolas"; r.font.size = Pt(9)
            r.font.color.rgb = RGBColor(0x8B, 0x1A, 0x1A)
        elif part.startswith("[") and "](" in part:
            label = part[1:part.index("]")].replace("`", "")
            r = paragraph.add_run(label)
            r.font.color.rgb = RGBColor(0x1F, 0x4E, 0x79)
            r.underline = True
        elif part.startswith("*") and part.endswith("*") and len(part) > 2:
            r = paragraph.add_run(part[1:-1]); r.italic = True
        else:
            paragraph.add_run(part)

def shade(cell, hex_color):
    tcPr = cell._tc.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:val"), "clear")
    shd.set(qn("w:color"), "auto")
    shd.set(qn("w:fill"), hex_color)
    tcPr.append(shd)

def add_table(rows):
    header, body = rows[0], rows[1:]
    table = doc.add_table(rows=1, cols=len(header))
    table.style = "Table Grid"
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = True
    hdr = table.rows[0].cells
    trPr = table.rows[0]._tr.get_or_add_trPr()
    tblHeader = OxmlElement("w:tblHeader")
    tblHeader.set(qn("w:val"), "true")
    trPr.append(tblHeader)
    for i, cell_text in enumerate(header):
        hdr[i].text = ""
        p = hdr[i].paragraphs[0]
        add_runs(p, cell_text)
        for r in p.runs:
            r.bold = True
            r.font.color.rgb = RGBColor(0xFF, 0xFF, 0xFF)
        shade(hdr[i], "2E5496")
    for row in body:
        cells = table.add_row().cells
        for i, cell_text in enumerate(row[:len(header)]):
            cells[i].text = ""
            add_runs(cells[i].paragraphs[0], cell_text)
    doc.add_paragraph()

def is_table_sep(line):
    return bool(re.match(r"^\|?[\s:|-]+\|[\s:|-]*$", line)) and "-" in line

def split_row(line):
    line = line.strip()
    if line.startswith("|"):
        line = line[1:]
    if line.endswith("|"):
        line = line[:-1]
    return [c.strip() for c in line.split("|")]

# --- Lectura y conversión ---------------------------------------------------
with open(SRC, "r", encoding="utf-8-sig") as fh:
    lines = fh.read().splitlines()

i = 0
while i < len(lines):
    line = lines[i].rstrip()
    stripped = line.strip()

    if not stripped:
        i += 1
        continue

    # Regla horizontal
    if re.fullmatch(r"-{3,}", stripped):
        i += 1
        continue

    # Encabezados
    m = re.match(r"^(#{1,4})\s+(.*)$", stripped)
    if m:
        level = len(m.group(1))
        doc.add_heading(m.group(2), level=min(level, 3))
        i += 1
        continue

    # Bloques de código
    if stripped.startswith("```"):
        i += 1
        code = []
        while i < len(lines) and not lines[i].strip().startswith("```"):
            code.append(lines[i])
            i += 1
        i += 1
        p = doc.add_paragraph()
        p.paragraph_format.left_indent = Cm(0.6)
        p.paragraph_format.space_before = Pt(4)
        p.paragraph_format.space_after = Pt(8)
        r = p.add_run("\n".join(code))
        r.font.name = "Consolas"
        r.font.size = Pt(9)
        pPr = p._p.get_or_add_pPr()
        shd = OxmlElement("w:shd")
        shd.set(qn("w:val"), "clear")
        shd.set(qn("w:color"), "auto")
        shd.set(qn("w:fill"), "F2F2F2")
        pPr.append(shd)
        continue

    # Tablas
    if stripped.startswith("|") and i + 1 < len(lines) and is_table_sep(lines[i + 1]):
        rows = [split_row(stripped)]
        i += 2
        while i < len(lines) and lines[i].strip().startswith("|"):
            rows.append(split_row(lines[i]))
            i += 1
        add_table(rows)
        continue

    # Citas
    if stripped.startswith(">"):
        buf = []
        while i < len(lines) and lines[i].strip().startswith(">"):
            buf.append(lines[i].strip()[1:].strip())
            i += 1
        p = doc.add_paragraph()
        p.paragraph_format.left_indent = Cm(0.7)
        p.paragraph_format.right_indent = Cm(0.5)
        add_runs(p, " ".join(buf))
        for r in p.runs:
            r.italic = True
            r.font.color.rgb = RGBColor(0x40, 0x40, 0x40)
        continue

    # Listas
    m = re.match(r"^(\s*)([-*+]|\d+[.)])\s+(.*)$", line)
    if m:
        ordered = m.group(2)[0].isdigit()
        prefix = f"{m.group(2)} " if ordered else "•  "
        buf = [m.group(3).strip()]
        i += 1
        while i < len(lines):
            nxt = lines[i]
            if not nxt.strip():
                break
            m2 = re.match(r"^(\s*)([-*+]|\d+[.)])\s+(.*)$", nxt)
            if m2 or not nxt.startswith((" ", "\t")):
                break
            buf.append(nxt.strip())
            i += 1
        p = doc.add_paragraph()
        p.paragraph_format.left_indent = Cm(0.75)
        p.paragraph_format.first_line_indent = Cm(-0.5)
        p.paragraph_format.space_after = Pt(3)
        add_runs(p, prefix + " ".join(buf))
        continue

    # Párrafo normal
    buf = [stripped]
    i += 1
    while i < len(lines):
        nxt = lines[i]
        if not nxt.strip() or nxt.lstrip().startswith(("#", "|", ">", "```")):
            break
        if re.match(r"^\s*([-*+]|\d+[.)])\s+", nxt):
            break
        buf.append(nxt.strip())
        i += 1
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    add_runs(p, " ".join(buf))

doc.save(DST)
print("DOCX generado:", DST)
