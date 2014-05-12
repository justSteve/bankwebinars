USE CUWebinarsMigrator
GO

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


--SELECT * FROM CUWebinars.dbo.RegType 
DELETE  CUWebinars.dbo.RegType
WHERE   RegTypeLabel = 'Additional Location(s)'
USE CUWebinarsClean
GO

UPDATE  dbo.Webinar
SET     DescriptionLong = '<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>' ,
        Description = '<p>Your member passed away yesterday and the family is grieving. Yet, in the midst of all the remembering and honoring of a life, the legal and compliance clock is ticking. In most states, checks can be paid for 10 days after the date of death.</p><p>When your account holder dies, many issues and questions arise. Who can have information on the account? Who owns the account? Who has access to the account? What happens with powers of attorney and authorized signers on account? Can the spouse of the sole proprietor continue to access the account? What about that last tax refund check? Is the account still insured? Can a check be written to a funeral home? What about the checks coming in to pay funeral expense? Learn about checks, IRAs, deposit ownership, trusts, UTMA, affidavits of heirship and other complex issues that can occur when a member dies. </p>' ,
        LearnCaption = 'Covered Topics' ,
        LearnBody = '<ul><li>Probate versus non-probate transfers</li><li>When does the will govern and when does the signature card?</li><li>What bypasses a will? PODs, IRAs, JTWROS?</li><li>Living trusts and successor trustees</li><li>When do we know that a customer is deceased?</li><li>When can we use small estate affidavits?</li><li>What happens to powers of attorney, authorized signers, etc.</li><li>How are IRAs, HSAs and UTMAs affected?</li><li>Do sole proprietorships cease at death?</li><li>NCUSIF insurance issues</li></ul>' ,
        WhoAttend = 'This informative session is designed for customer service representatives, branch administration, branch managers, tellers, training and development staff, compliance personnel, and anyone who handles customer accounts.'
WHERE   idWebinar = 404
USE CUWebinarsMigrator
GO

EXEC MigrateInstitution
PRINT 'ends: MigrateInstitution'
EXEC MigrateWebUser 
PRINT 'ends: MigrateWebUser'
EXEC MigrateAffiliate
PRINT 'ends: MigrateAffiliate'
EXEC MigrateRegTypes
PRINT 'ends: MigrateRegTypes'
EXEC [dbo].[MigrateRegTypesGroups]
PRINT 'ends: MigrateRegTypesGroups'
EXEC MigrateRegTypesXref 
PRINT 'ends: MigrateRegTypesXref'
EXEC MigratePresenter 
PRINT 'ends: MigratePresenter'
EXEC MigrateWebinar 
PRINT 'ends: MigrateWebinar'
EXEC MigrateRegTypesGroupsXref 
PRINT 'ends: MigrateOptionsGroupsXref'
EXEC MigrateTopic 
PRINT 'ends: MigrateTopic'
EXEC MigrateWebinarTopicXref 
PRINT 'ends: MigrateWebinarTopicXref'
EXEC MigrateWebinarFile 
PRINT 'ends: MigrateWebinarFile'
EXEC MigrateAddresses 
PRINT 'ends: MigrateAddresses'
PRINT '____________________________________________________________Finished Migrations'
USE CUWebinars
GO
PRINT 'inserting xref for 3558'
INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 3558, -- idWebinar - int
          34  -- idRegTypeGroup - int
          )
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 3558, -- idWebinar - int
          25  -- idTopic - int
          )
UPDATE  dbo.Webinar
SET     Date = '06/03/2014 10am'
WHERE   idWebinar = 3558
		  --SELECT * FROM dbo.Webinar WHERE idWebinar = 3558
		  --SELECT * FROM dbo.WebinarTopicXref WHERE idWebinar = 3558
	
UPDATE  CUWebinars.dbo.Webinar
SET     WebinarKey = 495203178,
Status = 3, RecordingUrl = 'VendorMgmtBestPractice042214.wmv'
WHERE   idWebinar = 701

INSERT  CUWebinars.dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 701 , -- idWebinar - int
          N'VendorMgmtCU0414.pdf' , -- fileLocation - nvarchar(255)
          N'Handouts'  -- fileDesc - nvarchar(1000)
        )
GO

USE CUWebinars
GO


UPDATE  dbo.WebinarFile
SET     fileLocation = 'VendorMgmtCU0414.pdf'
WHERE   idWebinar = 701
DBCC CHECKIDENT ([Order], RESEED, 2855)


USE MembershipReboot
GO
DELETE  MembershipRebootClean.dbo.UserClaims
WHERE   UserAccountID IN ( SELECT   id
                           FROM     MembershipRebootClean.dbo.UserAccounts
                           WHERE    Email = 'jpatterson@cfcumail.org' )
DELETE  MembershipRebootClean.dbo.UserAccounts
WHERE   Email = 'jpatterson@cfcumail.org'

DELETE  FROM dbo.UserClaims
WHERE   UserAccountID NOT IN ( SELECT   id
                               FROM     MembershipRebootClean.dbo.UserAccounts )
DELETE  FROM dbo.UserAccounts
WHERE   ID NOT IN ( SELECT  id
                    FROM    MembershipRebootClean.dbo.UserAccounts )

USE CUWebinars
GO

--UPDATE dbo.Webinar SET  WHERE idWebinar = 701

UPDATE  dbo.Webinar
SET     ConnectionInfo = NULL ,
        RecordingUrl = '' ,
        OrganizerKey = '922930' ,
        OrganizerOAuthKey = '5jxY3KZL48HWknOaOEP2eIzVmOTS' ,
        WebinarKey = NULL

UPDATE  dbo.Webinar
SET     AccessPhone = '1 877 309 2071' ,
        OrganizerKey = '901873' ,
        OrganizerOAuthKey = 'JIOHRkkCvmIKDY8QO0S4msbYH48N' ,
        WebinarKey = '271308394' ,
        AccessCode = '194-661-436',
		Status = 7
WHERE   idWebinar = 1558

INSERT dbo.WebinarFile
        ( idWebinar, fileLocation, fileDesc )
VALUES  ( 1558, -- idWebinar - int
          N'h14/MotivateSales0514.pdf', -- fileLocation - nvarchar(255)
          N'Handouts'  -- fileDesc - nvarchar(1000)
          )

UPDATE  webinar
SET     RecordingUrl = 'sharememdiesSE1113.wmv'
WHERE   idWebinar = 802
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 802 ,
          'h14/bsafrontlinecu0114.pdf' ,
          'Handouts'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 400 ,
          'h14/AcctCardsCU0214.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'AcctCrds.wmv'
WHERE   idWebinar = 400
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUIRAUpdateDP0214.wmv'
WHERE   idWebinar = 435
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 435 ,
          'h14/IRAUpdateCU0214.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 435 ,
          'h14/IRAUpdateCUB0214.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUOpenTrustAcctsEFA031114.wmv'
WHERE   idWebinar = 401
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 401 ,
          'h14/TrustCUC0314.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 401 ,
          'h14/TrustCUB0314.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'ShareComplianceCUTB031214.wmv'
WHERE   idWebinar = 402
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 402 ,
          'h14/AnnualTrainingCU0314.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUTraditionalRothIRA030714.wmv'
WHERE   idWebinar = 467
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 467 ,
          'h14/IRATradRothCUC0314.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 467 ,
          'h14/IRATradRothCUB0314.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'BusinessCUVY040214.wmv'
WHERE   idWebinar = 403
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 403 ,
          'h14/BusinessCU0414.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUCFPBEnforcementECOA040814.wmv'
WHERE   idWebinar = 825
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 825 ,
          'h14/ECOACU0414.pdf' ,
          'Handouts'
        )
UPDATE  dbo.Webinar
SET     RecordingUrl = 'IRADistCUPT032714.wmv'
WHERE   idWebinar = 437
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 437 ,
          'h14/IRADistCUC0314.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 437 ,
          'h14/IRADistCUB0314.pdf' ,
          'Handouts'
        )
		
--Opening Accounts for Minor Members
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUOpenAcctsMinor121013.wmv'
WHERE   idWebinar = 803
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 803 ,
          'h13/CUMinors1213.pdf' ,
          'Handouts'
        )

--Membership, Ownership and Access:  Account Cards and Agreements
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUMemberOwnerAccessCards103013.wmv'
WHERE   idWebinar = 407
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 407 ,
          'h13/CUMembership1013.pdf' ,
          'Handouts'
        )

--IRA Transfers and Rollovers - What's the Difference?
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUIRATransfer031314.wmv'
WHERE   idWebinar = 407
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 407 ,
          'h14/IRARolloverCUC0314.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 407 ,
          'h14/IRARolloverCUB0314.pdf' ,
          'Handouts'
        )

UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUIRAUpdateDP0214.wmv'
WHERE   idWebinar = 435

INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 435 ,
          'h14/IRAUpdateCU0214.pdf' ,
          'Handouts - Color'
        )
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 435 ,
          'h14/IRAUpdateCUB0214.pdf' ,
          'Handouts'
        )


		--Annual BSA Training for Credit Unions
UPDATE  dbo.Webinar
SET     RecordingUrl = 'CUAnnualBSATraining012214.wmv'
WHERE   idWebinar = 834
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 834 ,
          'h14/bsafrontlinecu0114.pdf' ,
          'Handouts'
        )
--For Sales Leaders: Precision Coaching
UPDATE  dbo.Webinar
SET     RecordingUrl = 'SSPrecisionCoach110413.wmv'
WHERE   idWebinar = 1471
INSERT  dbo.WebinarFile
        ( idWebinar ,
          fileLocation ,
          fileDesc
        )
VALUES  ( 1471 ,
          'h13/PrecisionCoaching1113.pdf' ,
          'Handouts'
        )
----How to Build a Personal & Business Work Plan
--UPDATE  dbo.Webinar
--SET     RecordingUrl = 'CUOpenAcctsMinor121013.wmv'
--WHERE   idWebinar = 1472
--INSERT  dbo.WebinarFile
--        ( idWebinar ,
--          fileLocation ,
--          fileDesc
--        )
--VALUES  ( 1472,
--          'h13/CUMinors1213.pdf' ,
--          'Handouts'
--        )


USE TTSDatabase
GO

DELETE  FROM dbo.CULog