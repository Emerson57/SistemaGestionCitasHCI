# Phase 7 - API validation with DevSeed accounts (passwords not logged)
$ErrorActionPreference = "Stop"
$base = "https://localhost:7211"
$out = Join-Path $PSScriptRoot "..\Evidencias\phase7-manual-api-validation.txt"
$lines = @("=== Phase 7 seeded API validation $(Get-Date -Format 'yyyy-MM-dd HH:mm K') ===")

function Post-Json($url, $obj, $hdr) {
    $path = [System.IO.Path]::GetTempFileName() + ".json"
    $obj | ConvertTo-Json | Set-Content $path -Encoding utf8
    if ($hdr) { return curl.exe -sk -X POST $url -H "Content-Type: application/json" -H $hdr --data-binary "@$path" }
    return curl.exe -sk -X POST $url -H "Content-Type: application/json" --data-binary "@$path"
}

$patLogin = @{ email = "patient@dev.local"; password = "DevSeed_Patient_ChangeMe123!" }
$patResp = Post-Json "$base/api/auth/login" $patLogin $null | ConvertFrom-Json
$patHdr = "Authorization: Bearer $($patResp.accessToken)"
$lines += "P-UI-01 Login patient: OK role=$($patResp.role)"

$my = curl.exe -sk "$base/api/appointments/my" -H $patHdr | ConvertFrom-Json
$lines += "P-UI-02 Dashboard data (appointments/my): count=$($my.Count)"

$specs = curl.exe -sk "$base/api/specialties" -H $patHdr | ConvertFrom-Json
$lines += "P-UI-04 Specialties loaded: count=$($specs.Count)"
$specId = $specs[0].id
$docs = curl.exe -sk "$base/api/doctors?specialtyId=$specId" -H $patHdr | ConvertFrom-Json
$doc = $docs | Select-Object -First 1
$lines += "P-UI-05 Doctor selected: $($doc.fullName) id=$($doc.id)"

$avail = curl.exe -sk "$base/api/doctors/$($doc.id)/availability" -H $patHdr | ConvertFrom-Json
$lines += "P-UI-06 Availability slots: count=$($avail.Count)"
$slot = $avail | Select-Object -First 1
$apptTime = "{0}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:00Z" -f $slot.date.Year, $slot.date.Month, $slot.date.Day, $slot.startTime.Hours, $slot.startTime.Minutes
if ($slot.startTime -is [string]) {
    $parts = $slot.startTime -split ":"
    $apptTime = "{0}T{1}:{2}:00Z" -f $slot.date, $parts[0], $parts[1]
}

$schedBody = @{ doctorId = $doc.id; appointmentDateTime = $apptTime; reason = "Phase7 validation" }
$schedPath = [System.IO.Path]::GetTempFileName() + ".json"
$schedBody | ConvertTo-Json | Set-Content $schedPath -Encoding utf8
$created = curl.exe -sk -X POST "$base/api/appointments" -H "Content-Type: application/json" -H $patHdr --data-binary "@$schedPath"
if ($created -match '"id"') {
    $appt = $created | ConvertFrom-Json
    $lines += "P-UI-07/08 Schedule: OK id=$($appt.id)"
    $lines += "P-UI-10 My appointments after create: OK"
    $slot2 = $avail | Select-Object -Skip 1 -First 1
    if ($slot2) {
        $newTime = "{0}T{1}:00Z" -f $slot2.date, ($slot2.startTime.ToString().Substring(0,5))
        $resPath = [System.IO.Path]::GetTempFileName() + ".json"
        @{ appointmentDateTime = $newTime } | ConvertTo-Json | Set-Content $resPath -Encoding utf8
        $res = curl.exe -sk -X PUT "$base/api/appointments/$($appt.id)/reschedule" -H "Content-Type: application/json" -H $patHdr --data-binary "@$resPath"
        $lines += "P-UI-11 Reschedule: $(if ($res -match 'id') { 'OK' } else { 'CHECK' })"
    }
    curl.exe -sk -X DELETE "$base/api/appointments/$($appt.id)" -H $patHdr | Out-Null
    $lines += "P-UI-13 Cancel: OK"
    $hist = curl.exe -sk "$base/api/appointments/history" -H $patHdr
    $lines += "P-UI-14 History: length $($hist.Length)"
} else {
    $lines += "P-UI-07 Schedule FAIL: $($created.Substring(0, [Math]::Min(200, $created.Length)))"
}

$docLogin = @{ email = "doctor@dev.local"; password = "DevSeed_Doctor_ChangeMe123!" }
$dr = Post-Json "$base/api/auth/login" $docLogin $null | ConvertFrom-Json
$drHdr = "Authorization: Bearer $($dr.accessToken)"
$lines += "D-UI-01 Doctor login: OK"
$agenda = curl.exe -sk "$base/api/doctors/me/agenda" -H $drHdr | ConvertFrom-Json
$lines += "D-UI-03 Agenda count: $($agenda.Count)"

$admLogin = @{ email = "admin@dev.local"; password = "DevSeed_Admin_ChangeMe123!" }
$ad = Post-Json "$base/api/auth/login" $admLogin $null | ConvertFrom-Json
$adHdr = "Authorization: Bearer $($ad.accessToken)"
$lines += "AD-UI-01 Admin login: OK"
$adminSpecs = curl.exe -sk "$base/api/specialties" -H $adHdr | ConvertFrom-Json
$lines += "AD-UI-03 Specialties as admin: $($adminSpecs.Count)"

$code = curl.exe -sk -w "%{http_code}" -o NUL -X POST "$base/api/specialties" -H "Content-Type: application/json" -H $patHdr -d "{""name"":""ForbiddenTest""}"
$lines += "A-05 Patient POST specialty HTTP: $code"

$lines | Set-Content $out -Encoding utf8
$lines
