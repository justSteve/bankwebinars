cls

$local = "C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.OATSUS\MSSQL\Backup"
$production = "w:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\Backup\Daily\TTSWebinars2\mostRecent\mostRecent.bak"

#Copy-Item $production -Destination $local


& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\__Seeder_Runner.sql'

& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\updateRegTypesforBankWebinars.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\CreateTestUsers.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\AdditionalLocationsLookupPrice.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\BuildCompliancePerspectivesMigration.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\BuildDiscountsMigration1.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\UpdateAffiliates.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\CreateDbCreds.sql'
& sqlcmd -S "(local)\" -i 'C:\Users\Steve\OneDrive for Business\SQL\BankWebinarsMigration\__Seeder_Ender.sql'
