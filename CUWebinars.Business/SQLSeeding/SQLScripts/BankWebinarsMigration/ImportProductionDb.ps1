cls

$local = "C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\Backup"
$production = "w:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\Backup\Daily\TTSWebinars2\mostRecent\mostRecent.bak"

#Copy-Item $production -Destination $local


& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\__Seeder_Runner.sql'

& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_updateRegTypesforBankWebinars.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_CreateTestUsers.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_AdditionalLocationsLookupPrice.sql'

& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_BuildDiscountsMigration1.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_BuildCompliancePerspectivesMigration.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_UpdateAffiliates.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\_CreateDbCreds.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\Source\Repos\BankWebinars\CUWebinars.Business\SQLSeeding\SQLScripts\BankWebinarsMigration\__Seeder_Ender.sql'
