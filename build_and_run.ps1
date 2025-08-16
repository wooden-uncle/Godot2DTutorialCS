# 构建和运行Godot项目的PowerShell脚本

# 显示开始信息
Write-Host "开始构建和运行Godot项目..."
Write-Host "----------------------------------------"

# 构建项目
Write-Host "正在构建项目..."
dotnet build

# 检查构建是否成功
if ($LASTEXITCODE -ne 0) {
    Write-Host "构建失败，退出代码: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "构建成功!" -ForegroundColor Green
Write-Host "----------------------------------------"

# 运行Godot项目
Write-Host "正在启动Godot引擎..."

# 使用提供的Godot路径运行项目
$godotPath = "D:\Godot\Godot_v4.4.1-stable_mono_win64\Godot_v4.4.1-stable_mono_win64_console.exe"
$projectPath = "."

# 检查Godot可执行文件是否存在
if (-not (Test-Path $godotPath)) {
    Write-Host "错误: 找不到Godot可执行文件: $godotPath" -ForegroundColor Red
    exit 1
}

# 运行Godot
Write-Host "启动命令: $godotPath --path $projectPath --verbose"
& $godotPath --path $projectPath --verbose

# 检查Godot是否成功启动
if ($LASTEXITCODE -ne 0) {
    Write-Host "Godot启动失败，退出代码: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
