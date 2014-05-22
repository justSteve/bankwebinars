
DELETE  CUWebinarsClean.dbo.Options
WHERE   Type = 'additional_location'
DELETE  CUWebinars.dbo.AdditionalLocation
DELETE  CUWebinars.dbo.OrderRow
DELETE  CUWebinars.dbo.[ORDER]
DELETE  CUWebinars.dbo.Affiliate
DELETE  CUWebinars.dbo.Presenter

DELETE  CUWebinars.dbo.WebUser

DELETE  CUWebinars.dbo.Institution
DELETE  CUWebinars.dbo.RegTypesXref

DELETE  CUWebinars.dbo.RegTypesGroupsXref

DELETE  CUWebinars.dbo.RegTypesGroups

DELETE  CUWebinars.dbo.RegType

DELETE  CUWebinars.dbo.WebinarTopicXref

DELETE  CUWebinars.dbo.Topic

DELETE  CUWebinars.dbo.WebinarFile

DELETE  CUWebinars.dbo.Webinar
DELETE  CUWebinars.dbo.[Address]

exec CUWebinarsMigrator.dbo.MigrateInstitution
PRINT 'ends: MigrateInstitution'
exec CUWebinarsMigrator.dbo.MigrateWebUser 
PRINT 'ends: MigrateWebUser'
exec CUWebinarsMigrator.dbo.MigrateAffiliate
PRINT 'ends: MigrateAffiliate'
exec CUWebinarsMigrator.dbo.MigrateRegTypes
PRINT 'ends: MigrateRegTypes'
exec CUWebinarsMigrator.dbo.[MigrateRegTypesGroups]
PRINT 'ends: MigrateRegTypesGroups'
exec CUWebinarsMigrator.dbo.MigrateRegTypesXref 
PRINT 'ends: MigrateRegTypesXref'
exec CUWebinarsMigrator.dbo.MigratePresenter 
PRINT 'ends: MigratePresenter'
exec CUWebinarsMigrator.dbo.MigrateWebinar 
PRINT 'ends: MigrateWebinar'
exec CUWebinarsMigrator.dbo.MigrateRegTypesGroupsXref 
PRINT 'ends: MigrateOptionsGroupsXref'
exec CUWebinarsMigrator.dbo.MigrateTopic 
PRINT 'ends: MigrateTopic'
exec CUWebinarsMigrator.dbo.MigrateWebinarTopicXref 
PRINT 'ends: MigrateWebinarTopicXref'
exec CUWebinarsMigrator.dbo.MigrateWebinarFile 
PRINT 'ends: MigrateWebinarFile'
exec CUWebinarsMigrator.dbo.MigrateAddresses 
PRINT 'ends: MigrateAddresses'
PRINT '____________________________________________________________Finished Migrations'


DELETE  CUWebinars.dbo.RegType
WHERE   RegTypeLabel = 'Additional Location(s)'

UPDATE CUWebinars.dbo.[Order] SET OrderStatus = 3


DBCC CHECKIDENT ([Order], RESEED, 2855)



DELETE  MembershipRebootClean.dbo.UserClaims
WHERE   UserAccountID IN ( SELECT   id
                           FROM     MembershipRebootClean.dbo.UserAccounts
                           WHERE    Email = 'jpatterson@cfcumail.org' )

DELETE  MembershipRebootClean.dbo.UserAccounts
WHERE   Email = 'jpatterson@cfcumail.org'

DELETE  FROM MembershipReboot.dbo.UserClaims
WHERE   UserAccountID NOT IN ( SELECT   id
                               FROM     MembershipRebootClean.dbo.UserAccounts )
DELETE  FROM MembershipReboot.dbo.UserAccounts
WHERE   ID NOT IN ( SELECT  id
                    FROM    MembershipRebootClean.dbo.UserAccounts )

USE CUWebinars
GO

--UPDATE dbo.Webinar SET  WHERE idWebinar = 701
--set to Mark's
UPDATE  dbo.Webinar
SET     ConnectionInfo = NULL ,
        RecordingUrl = '' ,
        OrganizerKey = '901873' ,
        OrganizerOAuthKey = 'JIOHRkkCvmIKDY8QO0S4msbYH48N' ,
        WebinarKey = NULL

