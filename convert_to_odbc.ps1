# Convert QuanLyCongViec.cs and Form1.cs to ODBC

$file1 = "d:\VSII\Learn_VSII\path_2\path_2\QuanLyCongViec.cs"
$file2 = "d:\VSII\Learn_VSII\path_2\path_2\Form1.cs"

# QuanLyCongViec.cs
$content = Get-Content $file1 -Raw
$content = $content -replace 'using MySql\.Data\.MySqlClient;', 'using System.Data.Odbc;'
$content = $content -replace 'MySqlConnection', 'OdbcConnection'
$content = $content -replace 'MySqlCommand', 'OdbcCommand'
$content = $content -replace 'MySqlDataReader', 'OdbcDataReader'
$content = $content -replace 'MySqlDataAdapter', 'OdbcDataAdapter'
Set-Content $file1 $content

# Form1.cs
$content2 = Get-Content $file2 -Raw
$content2 = $content2 -replace 'MySqlConnection', 'OdbcConnection'
$content2 = $content2 -replace 'MySqlCommand', 'OdbcCommand'
Set-Content $file2 $content2

Write-Host "Done converting to ODBC!" -ForegroundColor Green
