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
$sectionEnd   = "<!-- AUTO-GENERATED:RUNS:END -->"

$infographicMarkdown = ""
if ($infographicRelativePath) {
    $infographicMarkdown = @"
## Infografía operativa

![Infografía MCP]($infographicRelativePath)

"@
}

$newSection = @"
<!-- AUTO-GENERATED:RUNS:START -->
# Corridas operativas del plugin

$infographicMarkdown## Flujo general de uso

1. Abre el modelo en Revit.
2. Selecciona manualmente los elementos que quieres cuantificar.
3. Ejecuta la corrida correspondiente.
4. Obtén salida en modo `summary` o `detail`.
5. Interpreta el resultado agrupado por nivel, categoría, familia y tipo, según la corrida.

> Modo principal de trabajo: **selección manual en Revit**.

---

## Corrida 1 — Arquitectura

**Tool:** `run_preconstruccion_1`

### Qué cuantifica
Elementos arquitectónicos y generales del modelo.

### Categorías objetivo
- Walls
- Floors
- Roofs
- Ceilings
- Doors
- Windows
- Curtain Wall Panels
- Railings
- Plumbing Fixtures
- Specialty Equipment
- Generic Models

### Qué devuelve
- Resumen por **Nivel → Categoría → Familia → Tipo**
- Área total
- Volumen total
- Longitud total
- Cantidad total
- Detalle por instancia cuando se solicita

### Unidades
- `m2`
- `m3`
- `ml`
- `pza`

### Uso típico
```json
{
  "scope_mode": "selection",
  "output_mode": "summary"
}