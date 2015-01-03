
SET NOCOUNT ON
UPDATE  BankWebinars.dbo.RegType
SET     RegTypeLabel = REPLACE(RegTypeLabel, 'On-Demand', 'OnDemand')
UPDATE  BankWebinars.dbo.RegType
SET     ShowLiveNotifications = 'Yes' ,
        ShowRecordingNotifications = 'No' ,
        ShowShippedNotifications = 'No'
WHERE   idRegType = 1
SET IDENTITY_INSERT BankWebinars.dbo.RegType ON 
DECLARE @localPath NVARCHAR(300)

:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\Create2015OptionsGroups.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\1hourRegTypes.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\2hourRegTypes.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\3PartRegTypes.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\4PartRegTypes.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\5PartRegTypes.SQL"

SET IDENTITY_INSERT BankWebinars.dbo.RegType Off
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\GeneratesStatementsToInsertRegTypesXRef.SQL"
:r "C:\Users\Steve\Source\Repos\BankWebinars2\CUWebinars.Web\App_Data\Orders\SQLScripts\BankWebinarsMigration\InsertTestWebinars.sql"

SET NOCOUNT OFF


--SELECT  * FROM    BankWebinars.dbo.RegTypesGroups
USE BankWebinars
GO


UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = ( SELECT   idRegTypeGroup
                           FROM     dbo.RegTypesGroups
                           WHERE    RegTypeGroupDesc = 'BW_15_PreEvent_2hr'
                         )
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 2
                                AND Duration = 2 )


UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = ( SELECT   idRegTypeGroup
                           FROM     dbo.RegTypesGroups
                           WHERE    RegTypeGroupDesc = 'BW_15_PreEvent_1hr'
                         )
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 2
                                AND Duration = 1 )

UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = ( SELECT   idRegTypeGroup
                           FROM     dbo.RegTypesGroups
                           WHERE    RegTypeGroupDesc = 'BW_15_PostEvent_1hr'
                         )
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 3
                                AND Duration = 1 )

 
UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = ( SELECT   idRegTypeGroup
                           FROM     dbo.RegTypesGroups
                           WHERE    RegTypeGroupDesc = 'BW_15_PostEvent_2hr'
                         )
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Status = 3
                                AND Duration = 2 )


--5 parter
UPDATE  dbo.RegType
SET     RegTypeLabel = 'Live Plus Five' ,
        RegTypeExplain = 'Register for all five events and get five days access to the OnDemand Playback. You''ll have opportunity to ask questions during the presentation <i>and</i> be free to review the content for the next 5 (business) days.  Registration also includes links to presenter materials, handouts, and pdfs.'
WHERE   idRegType = 221--79

UPDATE  dbo.RegType
SET     RegTypeLabel = 'OnDemand Recording Only' ,
        RegTypeExplain = 'Interested in the topic but unable to attend the regularly scheduled event? Purchase the recorded version and receive OnDemand playback for 6 months (includes presenter materials).'
WHERE   idRegType = 222--80


UPDATE  dbo.RegType
SET     RegTypeLabel = 'Live Plus Six' ,
        RegTypeExplain = 'Attend live; includes six <i>months</i> access to OnDemand playback. Combine the advantages of live attendance with an unlimited number of replays for the next six months.' ,
        SortOrder = 4
WHERE   idRegType = 223--82

UPDATE  dbo.RegType
SET     RegTypeLabel = 'CD-ROM and Hardcopy Handouts' ,
        RegTypeExplain = 'CD-ROM plus Hardcopy Handouts. This option includes 6-months OnDemand playback but does <i>not</i> include live session.' ,
        SortOrder = 3
WHERE   idRegType = 224--81

UPDATE  dbo.RegType
SET     RegTypeLabel = 'Premier Package' ,
        RegTypeExplain = 'Includes all three options above.  Live, OnDemand playback, <i>and</i> CD-ROM plus Hardcopy Handouts.' ,
        SortOrder = 5
WHERE   idRegType = 225--83




--adjust 2 hour events
UPDATE  dbo.RegType
SET     RegTypeLabel = 'Live Plus Five' ,
        RegTypeExplain = 'Register for the live event and get five days access to the OnDemand Playback. You''ll have opportunity to ask questions during the presentation <i>and</i> be free to review the content for the next 5 (business) days.  Registration also includes links to presenter materials, handouts, and pdfs.'
WHERE   idRegType = 205--

UPDATE  dbo.RegType
SET     RegTypeLabel = 'OnDemand Recording Only' ,
        RegTypeExplain = 'Interested in the topic but unable to attend the regularly scheduled event? Purchase the recorded version and receive OnDemand playback for 6 months (includes presenter materials).'
WHERE   idRegType = 206 -- 16


UPDATE  dbo.RegType
SET     RegTypeLabel = 'Live Plus Six' ,
        RegTypeExplain = 'Attend live; includes six <i>months</i> access to OnDemand playback. Combine the advantages of live attendance with an unlimited number of replays for the next six months.' ,
        SortOrder = 4
WHERE   idRegType = 207--3

UPDATE  dbo.RegType
SET     RegTypeLabel = 'CD-ROM and Hardcopy Handouts' ,
        RegTypeExplain = 'CD-ROM plus Hardcopy Handouts. This option includes 6-months OnDemand playback but does <i>not</i> include live session.' ,
        SortOrder = 3
WHERE   idRegType = 208 --17

UPDATE  dbo.RegType
SET     RegTypeLabel = 'Premier Package' ,
        RegTypeExplain = 'Includes all three options above.  Live, OnDemand playback, <i>and</i> CD-ROM plus Hardcopy Handouts.' ,
        SortOrder = 5
WHERE   idRegType = 209--18


PRINT 'update 1hour upcoming'
UPDATE  dbo.RegType
SET     RegTypeLabel = ( SELECT RegTypeLabel
                         FROM   BankWebinars.dbo.RegType
                         WHERE  idRegType = 205
                       ) ,
        RegTypeExplain = ( SELECT   RegTypeExplain
                           FROM     BankWebinars.dbo.RegType
                           WHERE    idRegType = 205
                         ) ,
        SortOrder = 1
WHERE   idRegType = 200--27


UPDATE  dbo.RegType
SET     RegTypeLabel = ( SELECT RegTypeLabel
                         FROM   BankWebinars.dbo.RegType
                         WHERE  idRegType = 206
                       ) ,
        RegTypeExplain = ( SELECT   RegTypeExplain
                           FROM     BankWebinars.dbo.RegType
                           WHERE    idRegType = 206
                         ) ,
        SortOrder = 2
WHERE   idRegType = 201--32



UPDATE  dbo.RegType
SET     RegTypeLabel = ( SELECT RegTypeLabel
                         FROM   BankWebinars.dbo.RegType
                         WHERE  idRegType = 207
                       ) ,
        RegTypeExplain = ( SELECT   RegTypeExplain
                           FROM     BankWebinars.dbo.RegType
                           WHERE    idRegType = 207
                         ) ,
        SortOrder = 4
WHERE   idRegType = 203--35

UPDATE  dbo.RegType
SET     RegTypeLabel = ( SELECT RegTypeLabel
                         FROM   BankWebinars.dbo.RegType
                         WHERE  idRegType = 208
                       ) ,
        RegTypeExplain = ( SELECT   RegTypeExplain
                           FROM     BankWebinars.dbo.RegType
                           WHERE    idRegType = 208
                         ) ,
        SortOrder = 3
WHERE   idRegType = 202--33

UPDATE  dbo.RegType
SET     RegTypeLabel = ( SELECT RegTypeLabel
                         FROM   BankWebinars.dbo.RegType
                         WHERE  idRegType = 209
                       ) ,
        RegTypeExplain = ( SELECT   RegTypeExplain
                           FROM     BankWebinars.dbo.RegType
                           WHERE    idRegType = 209
                         ) ,
        SortOrder = 5
WHERE   idRegType = 204--36

UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = ( SELECT   idRegTypeGroup
                           FROM     dbo.RegTypesGroups
                           WHERE    RegTypeGroupDesc = 'BW_15_PreEvent_Series5'
                         )
WHERE   idWebinar IN ( 1711 ) 


UPDATE  BankWebinars.dbo.RegTypesGroupsXref
SET     idRegTypeGroup = 38
WHERE   idWebinar IN ( 842 ) 

--SELECT DISTINCT
--       reg.*
--FROM    BankWebinars.dbo.RegType reg
--        INNER JOIN BankWebinars.dbo.RegTypesXref rx ON rx.idRegType = reg.idRegType
--        INNER JOIN BankWebinars.dbo.RegTypesGroups rGroup ON rGroup.idRegTypeGroup = rx.idRegTypeGroup
--        INNER JOIN BankWebinars.dbo.RegTypesGroupsXref groupXRef ON groupXRef.idRegTypeGroup = rGroup.idRegTypeGroup
--WHERE   groupXRef.idWebinar IN (SELECT idWebinar FROM dbo.Webinar WHERE Status = 3 AND Duration = 1)
----AND rGroup.RegTypeGroupDesc LIKE '%13%'
--ORDER BY SortOrder

--SELECT title, * FROM dbo.Webinar WHERE Duration = 1 AND Status = 3