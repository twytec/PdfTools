
$files = [IO.Directory]::GetFiles([IO.Path]::Combine($PSScriptRoot, 'wwwroot\i18n'))
foreach ($file in $files) {
    $i18n = Get-Content -Raw $file | ConvertFrom-Json
    if ($i18n.LanguageCode -ne 'en')
    {
        $t = Get-Content -Path $PSScriptRoot\wwwroot\seo.html
        $t = $t.Replace('LanguageCode', $i18n.LanguageCode)
        $t = $t.Replace('TitleSub1', $i18n.TitleSub1)
        $t = $t.Replace('TitleSub2', $i18n.TitleSub2)
        $t = $t.Replace('TitleSub3', $i18n.TitleSub3)
        

        $p = [IO.Path]::Combine($PSScriptRoot, '.\wwwroot\' + $i18n.LanguageCode)
        $fp = [IO.Path]::Combine($p, $i18n.LanguageCode + '.html')

        New-Item -ItemType Directory -Force -Path $p
        $t | Out-File -FilePath $fp
    }
}