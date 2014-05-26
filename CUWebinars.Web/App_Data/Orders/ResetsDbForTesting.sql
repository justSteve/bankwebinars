USE CUWebinars
GO

DECLARE @WebinarKey NVARCHAR(20)
SET @WebinarKey = '133202538'


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

					
UPDATE  CUWebinars.dbo.Webinar
SET     title = 'Upcoming_NoConnInfo_' + CAST(idWebinar AS VARCHAR) ,
        ConnectionInfo = '' ,
        RecordingUrl = '' ,
        WebinarKey = NULL
WHERE   Status = 2

  
UPDATE  CUWebinars.dbo.Webinar
SET     title = 'Upcoming_HasConnInfo_' + CAST(idWebinar AS VARCHAR) ,
        ConnectionInfo = '' ,
		AccessPhone = '1 866 901 6455',
		AccessCode = '672-158-048',
		OrganizerKey = '922930',
		OrganizerOAuthKey = '5jxY3KZL48HWknOaOEP2eIzVmOTS',
        RecordingUrl = '' ,
		Status = 7,
        WebinarKey = @WebinarKey
WHERE   idWebinar = 701


UPDATE  CUWebinars.dbo.Webinar
SET     title = 'Recorded_' + CAST(idWebinar AS VARCHAR) ,
        ConnectionInfo = '' ,
        RecordingUrl = '' ,
        WebinarKey = NULL
WHERE   Status = 3


--updates lookup table for recorded events.
UPDATE  CUWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = 35
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 3 )
  
  --updates lookup table for upcoming
UPDATE  CUWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = 34
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 2 )

					   
--displays idRegtypes that can be ordered when event is still in future (upcoming)
SELECT  *
FROM    CUWebinars.dbo.RegType r
WHERE   idRegType IN ( SELECT   idRegType
                       FROM     dbo.RegTypesXref
                       WHERE    idRegTypeGroup = 34 )


--displays idRegtypes that can be ordered when event is in past (recorded)
SELECT  *
FROM    dbo.RegType r
WHERE   idRegType IN ( SELECT   idRegType
                       FROM     dbo.RegTypesXref
                       WHERE    idRegTypeGroup = 35 )
					   AND  RegTypeLabel NOT LIKE 'up%'
