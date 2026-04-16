param(
    [string]$RepoPath = "C:\Users\fabian.banuet\source\repos\RevitMCPBOQ"
)

$ErrorActionPreference = "Stop"

function Write-Utf8NoBom {
    param(
        [string]$Path,
        [string]$Content
    )
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8NoBom)
}

function Get-InfographicFromRepoRoot {
    param([string]$BasePath)

    $patterns = @(
        "infografia MCP*.png",
        "infografia MCP*.jpg",
        "infografia MCP*.jpeg",
        "infografia MCP*.webp",
        "infografia MCP*.bmp"
    )

    $files = @()
    foreach ($pattern in $patterns) {
        $files += Get-ChildItem -Path $BasePath -Filter $pattern -File -ErrorAction SilentlyContinue
    }

    if (-not $files -or $files.Count -eq 0) {
        return $null
    }

    return $files | Sort-Object LastWriteTime -Descending | Select-Object -First 1
}

if (-not (Test-Path $RepoPath)) {
    throw "No existe la ruta del repositorio: $RepoPath"
}

$readmePath = Join-Path $RepoPath "README.md"
$docsAssetsPath = Join-Path $RepoPath "docs\assets"
New-Item -ItemType Directory -Path $docsAssetsPath -Force | Out-Null

$infographic = Get-InfographicFromRepoRoot -BasePath $RepoPath
$infographicRelativePath = ""

if ($infographic -ne $null) {
    $targetName = "infografia-mcp" + $infographic.Extension.ToLowerInvariant()
    $targetPath = Join-Path $docsAssetsPath $targetName
    Copy-Item -Path $infographic.FullName -Destination $targetPath -Force
    $infographicRelativePath = "docs/assets/$targetName"
    Write-Host "Infografía copiada a: $targetPath"
}
else {
    Write-Warning "No encontré la imagen 'infografia MCP' en la raíz del repo."
}

$imgBlock = ""
if (-not [string]::IsNullOrWhiteSpace($infographicRelativePath)) {
    $imgBlock = "![Infografía MCP]($infographicRelativePath)`r`n`r`n"
}

$readme = @"
# RevitMCPBOQ

$imgBlockMotor de cuantificación BIM asistido por IA para **Revit + MCP**, orientado a preconstrucción, BOQ/BOM y extracción estructurada de cantidades por disciplina.

> Proyecto hermano Navisworks: [MCP-navis-boq](https://github.com/JefaturaBIM94/MCP-navis-boq)

---

## 1. Qué es este proyecto

**RevitMCPBOQ** conecta un modelo abierto en **Autodesk Revit** con un **MCP Server** para que Claude pueda ejecutar corridas de cuantificación y devolver resultados estructurados por:

- nivel
- sistema
- categoría
- familia
- tipo
- instancia

La solución está organizada en tres capas:

- **NavisBOQ.Core** → modelos, reglas de negocio, constantes, políticas, mappers y agregación
- **NavisBOQ.Revit.Plugin** → lectura del modelo, bridge y handlers de tools
- **NavisBOQ.Revit.McpServer** → exposición de tools MCP hacia Claude

---

## 2. Flujo general de uso

1. Abre un modelo válido en Revit.
2. Selecciona manualmente los elementos que quieres cuantificar.
3. Ejecuta la corrida correspondiente.
4. Obtén resultado en modo **summary** o **detail**.
5. Usa la salida para revisión técnica, preconstrucción, BOQ o validación BIM.

> Flujo principal del producto: **selección manual en Revit**.

---

## 3. Arquitectura general

```mermaid
flowchart LR
    A[Claude Desktop] --> B[MCP Server]
    B --> C[Bridge request / response]
    C --> D[Plugin Revit]
    D --> E[Core]
    E --> F[Policies + Mappers + Aggregation]
    F --> D
    D --> C
    C --> B
    B --> A