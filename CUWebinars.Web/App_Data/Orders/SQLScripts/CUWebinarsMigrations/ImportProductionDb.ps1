cls

$localScript= "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\"


#Copy-Item $production -Destination $local


& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\__Seeder_Runner.sql'

& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_CreateBankWebinars.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_updateRegTypesforBankWebinars.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_CreateTestUsers.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_AdditionalLocationsLookupPrice.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_UpdateAffiliates.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\_CreateDbCreds.sql'
& sqlcmd -S "(local)\" -i $localScript'CUWebinarsMigrations\__Seeder_Ender.sql'
