@echo off
REM ============================================
REM Database Migration Script Runner
REM ============================================
echo.
echo ========================================
echo   CHAY DATABASE MIGRATION
echo ========================================
echo.
echo File: Add_Progress_Column_Migration.sql
echo Database: path_1
echo.

set MYSQL_PATH=C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe

if not exist "%MYSQL_PATH%" (
    echo ERROR: Khong tim thay MySQL!
    echo Vui long cap nhat duong dan trong file nay.
    pause
    exit /b 1
)

echo Nhap password MySQL cho user root:
"%MYSQL_PATH%" -u root -p path_1 < Add_Progress_Column_Migration.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   MIGRATION THANH CONG!
    echo ========================================
    echo.
    echo Ban co the chay lai ung dung bay gio.
) else (
    echo.
    echo ========================================
    echo   MIGRATION THAT BAI!
    echo ========================================
    echo.
    echo Vui long kiem tra lai ket noi database.
)

echo.
pause
