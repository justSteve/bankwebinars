USE BankWebinars
GO
SET IDENTITY_INSERT dbo.Webinar ON 
INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 1 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
		
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 1, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 1, -- idWebinar - int
          45  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 2 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )

          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 2, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 2, -- idWebinar - int
          46  -- idRegTypeGroup - int
          )
		  
INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 3 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 3, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 3, -- idWebinar - int
          43  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 4 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )

          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 4, -- idWebinar - int
          15  -- idTopic - int
          )
		  
INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 4, -- idWebinar - int
          44  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 5 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 5, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 5, -- idWebinar - int
          47  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 6 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 6, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 6, -- idWebinar - int
          48 -- idRegTypeGroup - int
          )

		  
INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 7 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 7, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 7, -- idWebinar - int
          49  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 8 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )

          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 8, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 8, -- idWebinar - int
          50  -- idRegTypeGroup - int
          )

          
INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 9 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 9, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 9, -- idWebinar - int
          51  -- idRegTypeGroup - int
          )

INSERT  dbo.Webinar
        ( idWebinar ,
          Description ,
          DescriptionLong ,
          ImageUrl ,
          SmallImageUrl ,
          Status ,
          Title ,
          Date ,
          LearnCaption ,
          LearnBody ,
          WhoAttend ,
          Duration ,
          RecordingUrl ,
          idPresenter ,
          WebinarKey ,
          OrganizerKey ,
          OrganizerOAuthKey ,
          ceu ,
          ConnectionInfo ,
          DateCreated ,
          DateChanged ,
          AccessPhone ,
          CitrixRegisterUrl ,
          AccessCodeAttendee ,
          AccessCodePresenter ,
          AccessCodeOrganizer
        )
VALUES  ( 10 ,
          N'Description' , -- Description - nvarchar(max)
          N'DescriptionLong' , -- DescriptionLong - nvarchar(max)
          N'ImageUrl' , -- ImageUrl - nvarchar(50)
          N'SmallImageUrl' , -- SmallImageUrl - nvarchar(50)
          2 , -- Status - int
          N'Tester 1 hour Pre' , -- Title - nvarchar(125)
          DATEADD(DAY, 10, GETDATE()) , -- Date - datetime
          N'LearnCaption' , -- LearnCaption - nvarchar(255)
          N'LearnBody' , -- LearnBody - nvarchar(max)
          N'WhoAttend' , -- WhoAttend - nvarchar(max)
          1 , -- Duration - decimal
          N'' , -- RecordingUrl - nvarchar(300)
          24 , -- idPresenter - int
          N'' , -- WebinarKey - nvarchar(max)
          N'' , -- OrganizerKey - nvarchar(max)
          N'' , -- OrganizerOAuthKey - nvarchar(max)
          N'' , -- ceu - nvarchar(1000)
          N'' , -- ConnectionInfo - nvarchar(max)
          GETDATE() , -- DateCreated - datetime
          GETDATE() , -- DateChanged - datetime
          N'' , -- AccessPhone - nvarchar(max)
          N'' , -- CitrixRegisterURL - nvarchar(max)
          N'' , -- AccessCodeAttendee - nvarchar(max)
          N'' , -- AccessCodePresenter - nvarchar(max)
          N''  -- AccessCodeOrganizer - nvarchar(max)
        )
          
INSERT  dbo.WebinarTopicXref
        ( idWebinar, idTopic )
VALUES  ( 10, -- idWebinar - int
          15  -- idTopic - int
          )

INSERT  dbo.RegTypesGroupsXref
        ( idWebinar, idRegTypeGroup )
VALUES  ( 10, -- idWebinar - int
          52  -- idRegTypeGroup - int
          )

SET IDENTITY_INSERT dbo.Webinar OFF