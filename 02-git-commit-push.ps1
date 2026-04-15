param(
    [string]$RepoPath = "C:\Users\fabian.banuet\source\repos\RevitMCPBOQ",
    [string]$RemoteUrl = "https://github.com/JefaturaBIM94/RevitMCPBOQ.git",
    [string]$CommitMessage = ""
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $RepoPath)) {
    throw "No existe la ruta del repositorio: $RepoPath"
}

Set-Location $RepoPath

$gitExists = Get-Command git -ErrorAction SilentlyContinue
if (-not $gitExists) {
    throw "Git no está instalado o no está en el PATH."
}

$insideRepo = git rev-parse --is-inside-work-tree 2>$null
if ($LASTEXITCODE -ne 0) {
    throw "La carpeta no es un repositorio Git válido: $RepoPath"
}

$currentRemote = git remote get-url origin 2>$null
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($currentRemote)) {
    git remote add origin $RemoteUrl
}
elseif ($currentRemote -ne $RemoteUrl) {
    Write-Host "Remote origin actual: $currentRemote"
    Write-Host "Actualizando remote origin a: $RemoteUrl"
    git remote set-url origin $RemoteUrl
}

if ([string]::IsNullOrWhiteSpace($CommitMessage)) {
    $today = Get-Date -Format "yyyy-MM-dd HH:mm"
    $CommitMessage = "feat: update RevitMCPBOQ progress, README and infographic ($today)"
}

Write-Host "Estado actual del repo:"
git status --short

git add .

$hasChanges = git diff --cached --name-only
if ([string]::IsNullOrWhiteSpace($hasChanges)) {
    Write-Host "No hay cambios para commit."
    exit 0
}

git commit -m $CommitMessage

$currentBranch = git branch --show-current
if ([string]::IsNullOrWhiteSpace($currentBranch)) {
    throw "No se pudo determinar el branch actual."
}

git push origin $currentBranch

Write-Host "Push completado correctamente al branch: $currentBranch"