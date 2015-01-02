cls

$local = "C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\Backup"
$production = "w:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\Backup\Daily\TTSWebinars2\mostRecent\mostRecent.bak"
$localScript= "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\"
#Copy-Item $production -Destination $local


& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\__Seeder_Runner.sql'

& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_updateRegTypesforBankWebinars.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_CreateTestUsers.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_AdditionalLocationsLookupPrice.sql'

& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_BuildDiscountsMigration1.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_BuildCompliancePerspectivesMigration.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_UpdateAffiliates.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\_CreateDbCreds.sql'
& sqlcmd -S "(local)\" -i $localScript'BankWebinarsMigration\__Seeder_Ender.sql'
