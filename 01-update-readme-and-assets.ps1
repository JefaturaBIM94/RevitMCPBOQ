param(
    [string]$RepoPath = "C:\Users\fabian.banuet\source\repos\RevitMCPBOQ",
    [string]$DownloadsPath = "C:\Users\fabian.banuet\Downloads"
)

$ErrorActionPreference = "Stop"

function Get-ReadmePath {
    param([string]$BasePath)

    $candidates = @(
        (Join-Path $BasePath "README.md"),
        (Join-Path $BasePath "README"),
        (Join-Path $BasePath "Readme.md"),
        (Join-Path $BasePath "readme.md")
    )

    foreach ($file in $candidates) {
        if (Test-Path $file) {
            return $file
        }
    }

    return (Join-Path $BasePath "README.md")
}

function Get-InfographicFile {
    param([string]$BaseDownloadsPath)

    $patterns = @(
        "infografia MCP*.png",
        "infografia MCP*.jpg",
        "infografia MCP*.jpeg",
        "infografia MCP*.webp",
        "infografia MCP*.bmp"
    )

    $files = @()
    foreach ($pattern in $patterns) {
        $files += Get-ChildItem -Path $BaseDownloadsPath -Filter $pattern -File -ErrorAction SilentlyContinue
    }

    if (-not $files -or $files.Count -eq 0) {
        return $null
    }

    return $files | Sort-Object LastWriteTime -Descending | Select-Object -First 1
}

function Write-Utf8NoBom {
    param(
        [string]$Path,
        [string]$Content
    )

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8NoBom)
}

if (-not (Test-Path $RepoPath)) {
    throw "No existe la ruta del repositorio: $RepoPath"
}

$readmePath = Get-ReadmePath -BasePath $RepoPath
$docsAssetsPath = Join-Path $RepoPath "docs\assets"
New-Item -ItemType Directory -Path $docsAssetsPath -Force | Out-Null

$infographic = Get-InfographicFile -BaseDownloadsPath $DownloadsPath
$infographicRelativePath = $null

if ($infographic -ne $null) {
    $targetInfographicName = "infografia-mcp" + $infographic.Extension.ToLowerInvariant()
    $targetInfographicPath = Join-Path $docsAssetsPath $targetInfographicName
    Copy-Item -Path $infographic.FullName -Destination $targetInfographicPath -Force
    $infographicRelativePath = "docs/assets/$targetInfographicName"
    Write-Host "Infografía copiada a: $targetInfographicPath"
}
else {
    Write-Warning "No se encontró un archivo con nombre 'infografia MCP' en Downloads. Se actualizará el README sin imagen."
}

if (Test-Path $readmePath) {
    $readmeContent = Get-Content -Path $readmePath -Raw -Encoding UTF8
}
else {
    $readmeContent = "# RevitMCPBOQ`r`n`r`n"
}

$sectionStart = "<!-- AUTO-GENERATED:RUNS:START -->"
$sectionEnd = "<!-- AUTO-GENERATED:RUNS:END -->"

$infographicLines = @()
if ($infographicRelativePath) {
    $infographicLines += "## Infografía operativa"
    $infographicLines += ""
    $infographicLines += "![Infografía MCP]($infographicRelativePath)"
    $infographicLines += ""
}

$lines = @()
$lines += $sectionStart
$lines += "# Corridas operativas del plugin"
$lines += ""
$lines += $infographicLines
$lines += "## Flujo general de uso"
$lines += ""
$lines += "1. Abre el modelo en Revit."
$lines += "2. Selecciona manualmente los elementos que quieres cuantificar."
$lines += "3. Ejecuta la corrida correspondiente."
$lines += "4. Obtén salida en modo \`summary\` o \`detail\`."
$lines += "5. Interpreta el resultado agrupado por nivel, categoría, familia y tipo, según la corrida."
$lines += ""
$lines += "> Modo principal de trabajo: **selección manual en Revit**."
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Corrida 1 — Arquitectura"
$lines += ""
$lines += "**Tool:** \`run_preconstruccion_1\`"
$lines += ""
$lines += "### Qué cuantifica"
$lines += "Elementos arquitectónicos y generales del modelo."
$lines += ""
$lines += "### Categorías objetivo"
$lines += "- Walls"
$lines += "- Floors"
$lines += "- Roofs"
$lines += "- Ceilings"
$lines += "- Doors"
$lines += "- Windows"
$lines += "- Curtain Wall Panels"
$lines += "- Railings"
$lines += "- Plumbing Fixtures"
$lines += "- Specialty Equipment"
$lines += "- Generic Models"
$lines += ""
$lines += "### Qué devuelve"
$lines += "- Resumen por **Nivel → Categoría → Familia → Tipo**"
$lines += "- Área total"
$lines += "- Volumen total"
$lines += "- Longitud total"
$lines += "- Cantidad total"
$lines += "- Detalle por instancia cuando se solicita"
$lines += ""
$lines += "### Unidades"
$lines += "- \`m2\`"
$lines += "- \`m3\`"
$lines += "- \`ml\`"
$lines += "- \`pza\`"
$lines += ""
$lines += "### Uso típico"
$lines += "```json"
$lines += "{"
$lines += '  "scope_mode": "selection",'
$lines += '  "output_mode": "summary"'
$lines += "}"
$lines += "```"
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Corrida 2 — Estructura / Concreto"
$lines += ""
$lines += "**Tool:** \`run_preconstruccion_2\`"
$lines += ""
$lines += "### Qué cuantifica"
$lines += "Elementos estructurales y de concreto seleccionados en el modelo."
$lines += ""
$lines += "### Categorías objetivo"
$lines += "- Walls"
$lines += "- Floors"
$lines += "- Roofs"
$lines += "- Structural Columns"
$lines += "- Structural Framing"
$lines += "- Structural Foundations"
$lines += "- Pads"
$lines += ""
$lines += "### Qué devuelve"
$lines += "- Resumen por **Nivel → Categoría → Familia → Tipo**"
$lines += "- Longitud total para columnas y vigas"
$lines += "- Volumen total para cimentaciones y pads"
$lines += "- Área y/o volumen para muros, losas y cubiertas"
$lines += "- Material estructural cuando exista y esté disponible en el modelo"
$lines += ""
$lines += "### Unidades"
$lines += "- \`ml\`"
$lines += "- \`m2\`"
$lines += "- \`m3\`"
$lines += ""
$lines += "### Uso típico"
$lines += "```json"
$lines += "{"
$lines += '  "scope_mode": "selection",'
$lines += '  "output_mode": "summary"'
$lines += "}"
$lines += "```"
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Corrida 4 — Eléctrica"
$lines += ""
$lines += "**Tool:** \`run_preconstruccion_4\`"
$lines += ""
$lines += "### Qué cuantifica"
$lines += "Elementos eléctricos seleccionados en Revit."
$lines += ""
$lines += "### Categorías típicas"
$lines += "- Electrical Equipment"
$lines += "- Lighting Fixtures"
$lines += "- Conduits"
$lines += "- Cable Trays"
$lines += "- Electrical Fixtures"
$lines += "- Otras categorías eléctricas soportadas por la corrida"
$lines += ""
$lines += "### Qué devuelve"
$lines += "- Resumen por **Nivel → Sistema → Categoría → Familia → Tipo**"
$lines += "- Conteos por tipo"
$lines += "- Longitudes por elementos lineales"
$lines += "- Posibilidad de expandir detalle eléctrico"
$lines += ""
$lines += "### Unidades"
$lines += "- \`pza\`"
$lines += "- \`ml\`"
$lines += ""
$lines += "### Uso típico"
$lines += "```json"
$lines += "{"
$lines += '  "scope_mode": "selection",'
$lines += '  "output_mode": "summary"'
$lines += "}"
$lines += "```"
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Corrida 5 — HVAC / MEP"
$lines += ""
$lines += "**Tool:** \`run_preconstruccion_5\`"
$lines += ""
$lines += "### Qué cuantifica"
$lines += "Elementos HVAC / MEP seleccionados en Revit."
$lines += ""
$lines += "### Categorías típicas"
$lines += "- Ducts"
$lines += "- Duct Fittings"
$lines += "- Mechanical Equipment"
$lines += "- Air Terminals"
$lines += "- Pipes"
$lines += "- Pipe Fittings"
$lines += ""
$lines += "### Qué devuelve"
$lines += "- Resumen por **Nivel → Sistema → Categoría → Familia → Tipo**"
$lines += "- Longitudes lineales"
$lines += "- Conteos"
$lines += "- Información de sistema MEP"
$lines += "- Detalle técnico cuando se solicita"
$lines += ""
$lines += "### Unidades"
$lines += "- \`ml\`"
$lines += "- \`pza\`"
$lines += ""
$lines += "### Uso típico"
$lines += "```json"
$lines += "{"
$lines += '  "scope_mode": "selection",'
$lines += '  "output_mode": "summary"'
$lines += "}"
$lines += "```"
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Corrida 6 — Acero de refuerzo / Rebar"
$lines += ""
$lines += "**Tool:** \`run_preconstruccion_6\`"
$lines += ""
$lines += "### Qué cuantifica"
$lines += "Kilogramos totales del acero de refuerzo seleccionado."
$lines += ""
$lines += "### Elementos objetivo"
$lines += "- Rebar"
$lines += "- Structural Rebar"
$lines += ""
$lines += "### Parámetros clave"
$lines += "- **Tipo:** \`Bar Diameter\`"
$lines += "- **Instancia:** \`Bar Length\`"
$lines += "- **Instancia:** \`Shape\` (se reporta principalmente en detalle)"
$lines += ""
$lines += "### Lógica de cálculo"
$lines += "1. Se lee \`Bar Diameter\` del tipo."
$lines += "2. Se convierte el diámetro a mm."
$lines += "3. Se asigna:"
$lines += "   - Número de varilla"
$lines += "   - Peso lineal \`kg/m\`"
$lines += "4. Se lee \`Bar Length\` de la instancia."
$lines += "5. Se lee la cantidad de barras del set."
$lines += "6. Se calcula:"
$lines += "   - \`Longitud total = Bar Length × Cantidad\`"
$lines += "   - \`Peso total kg = Longitud total × kg/m\`"
$lines += ""
$lines += "### Tabla de equivalencias"
$lines += "- \`#2.5\` → \`7.9 mm\` → \`0.380 kg/m\`"
$lines += "- \`#3\` → \`9.5 mm\` → \`0.560 kg/m\`"
$lines += "- \`#4\` → \`12.7 mm\` → \`0.994 kg/m\`"
$lines += "- \`#5\` → \`15.9 mm\` → \`1.552 kg/m\`"
$lines += "- \`#6\` → \`19.1 mm\` → \`2.235 kg/m\`"
$lines += "- \`#8\` → \`25.4 mm\` → \`3.975 kg/m\`"
$lines += ""
$lines += "### Qué devuelve"
$lines += "- Peso total kg por instancia"
$lines += "- Peso total kg por tipo"
$lines += "- Resumen por:"
$lines += "  - Nivel"
$lines += "  - Categoría"
$lines += "  - Tipo"
$lines += "  - Número de varilla"
$lines += "  - Diámetro mm"
$lines += "- \`Shape\` en detalle cuando el usuario lo solicita"
$lines += ""
$lines += "### Unidades"
$lines += "- \`kg\`"
$lines += "- \`m\`"
$lines += "- \`mm\`"
$lines += "- \`kg/m\`"
$lines += ""
$lines += "### Uso típico"
$lines += "```json"
$lines += "{"
$lines += '  "scope_mode": "selection",'
$lines += '  "output_mode": "summary"'
$lines += "}"
$lines += "```"
$lines += ""
$lines += "---"
$lines += ""
$lines += "## Resumen de corridas"
$lines += ""
$lines += "| Corrida | Tool | Enfoque | Unidades principales |"
$lines += "|---|---|---|---|"
$lines += "| 1 | \`run_preconstruccion_1\` | Arquitectura | \`m2\`, \`m3\`, \`ml\`, \`pza\` |"
$lines += "| 2 | \`run_preconstruccion_2\` | Estructura / Concreto | \`ml\`, \`m2\`, \`m3\` |"
$lines += "| 4 | \`run_preconstruccion_4\` | Eléctrica | \`pza\`, \`ml\` |"
$lines += "| 5 | \`run_preconstruccion_5\` | HVAC / MEP | \`ml\`, \`pza\` |"
$lines += "| 6 | \`run_preconstruccion_6\` | Rebar / acero de refuerzo | \`kg\`, \`m\`, \`mm\`, \`kg/m\` |"
$lines += ""
$lines += "## Nota operativa"
$lines += ""
$lines += "El flujo recomendado es siempre:"
$lines += ""
$lines += "**selección manual en Revit → ejecución de corrida → revisión de resumen → detalle opcional**"
$lines += $sectionEnd

$newSection = ($lines -join "`r`n") + "`r`n"

$pattern = [regex]::Escape($sectionStart) + ".*?" + [regex]::Escape($sectionEnd)

if ($readmeContent -match $pattern) {
    $updatedReadme = [regex]::Replace(
        $readmeContent,
        $pattern,
        [System.Text.RegularExpressions.MatchEvaluator]{ param($m) $newSection },
        [System.Text.RegularExpressions.RegexOptions]::Singleline
    )
}
else {
    $separator = ""
    if (-not $readmeContent.EndsWith("`n")) {
        $separator = "`r`n"
    }
    $updatedReadme = $readmeContent + $separator + "`r`n" + $newSection
}

Write-Utf8NoBom -Path $readmePath -Content $updatedReadme

Write-Host "README actualizado en: $readmePath"
Write-Host "Proceso terminado correctamente."