SET NOCOUNT ON 
PRINT 'start inserting ttstester'

INSERT CUWebinars.dbo.WebUser
        ( idUser ,
          UserType ,
          AcctStatus ,
          DateCreated ,
          FirstName ,
          LastName ,
          Initial ,
          idUserInstitution ,
          email ,
          futureMail ,
          generalComments ,
          taxExempt ,
          idSubscriptionDiscount ,
          timeZone ,
          Title
        )
VALUES  ( 31656 , -- idUser - int
          1 , -- UserType - int
          N'A' , -- AcctStatus - nvarchar(1)
          GETDATE() , -- DateCreated - datetime
          N'Test' , -- FirstName - nvarchar(50)
          N'User' , -- LastName - nvarchar(50)
          N'' , -- Initial - nvarchar(max)
          16113 , -- idUserInstitution - int
          N'testuser1@cuwebinars.com' , -- email - nvarchar(150)
          N'n' , -- futureMail - nvarchar(1)
          N'' , -- generalComments - nvarchar(1000)
          0 , -- taxExempt - bit
          0 , -- idSubscriptionDiscount - int
          2 , -- timeZone - int
          N''  -- Title - nvarchar(200)
        )

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31660, 1, N'A', CAST(N'2014-12-02 18:54:27.493' AS DateTime), N'test1', N'200', NULL, 16116, N'1-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31661, 1, N'A', CAST(N'2014-12-02 19:33:06.137' AS DateTime), N'test1', N'201', NULL, 16116, N'1-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31662, 1, N'A', CAST(N'2014-12-02 19:33:59.710' AS DateTime), N'test1', N'203', NULL, 16116, N'1-3@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31663, 1, N'A', CAST(N'2014-12-02 19:34:25.070' AS DateTime), N'test1', N'202', NULL, 16116, N'1-4@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31664, 1, N'A', CAST(N'2014-12-02 19:34:45.853' AS DateTime), N'test2', N'203', NULL, 16116, N'2-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31665, 1, N'A', CAST(N'2014-12-02 19:35:07.293' AS DateTime), N'test2', N'201', NULL, 16116, N'2-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31666, 1, N'A', CAST(N'2014-12-02 19:35:28.720' AS DateTime), N'test3', N'206', NULL, 16116, N'3-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31667, 1, N'A', CAST(N'2014-12-02 19:36:01.920' AS DateTime), N'test3', N'208', NULL, 16116, N'3-3@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31668, 1, N'A', CAST(N'2014-12-02 19:36:23.657' AS DateTime), N'test4', N'206', NULL, 16116, N'4-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31669, 1, N'A', CAST(N'2014-12-02 19:36:44.650' AS DateTime), N'test5', N'210', NULL, 16116, N'5-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31670, 1, N'A', CAST(N'2014-12-02 19:37:11.043' AS DateTime), N'test4', N'208', NULL, 16116, N'4-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31671, 1, N'A', CAST(N'2014-12-02 19:37:36.947' AS DateTime), N'test5', N'212', NULL, 16116, N'5-4@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31672, 1, N'A', CAST(N'2014-12-02 19:37:57.473' AS DateTime), N'test5', N'214', NULL, 16116, N'5-5@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31673, 1, N'A', CAST(N'2014-12-02 19:38:17.803' AS DateTime), N'test6', N'211', NULL, 16116, N'6-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31674, 1, N'A', CAST(N'2014-12-02 19:38:40.377' AS DateTime), N'test5', N'213', NULL, 16116, N'5-3@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31675, 1, N'A', CAST(N'2014-12-02 19:39:03.473' AS DateTime), N'test7', N'216', NULL, 16116, N'7-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31676, 1, N'A', CAST(N'2014-12-02 19:39:25.710' AS DateTime), N'test6', N'213', NULL, 16116, N'6-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31677, 1, N'A', CAST(N'2014-12-02 19:39:46.613' AS DateTime), N'test5', N'211', NULL, 16116, N'5-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31678, 1, N'A', CAST(N'2014-12-02 19:40:07.460' AS DateTime), N'test7', N'219', NULL, 16116, N'7-3@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31679, 1, N'A', CAST(N'2014-12-02 19:40:31.700' AS DateTime), N'test7', N'218', NULL, 16116, N'7-4@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31680, 1, N'A', CAST(N'2014-12-02 19:40:53.120' AS DateTime), N'test8', N'219', NULL, 16116, N'8-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31681, 1, N'A', CAST(N'2014-12-02 19:41:14.170' AS DateTime), N'test8', N'217', NULL, 16116, N'8-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31682, 1, N'A', CAST(N'2014-12-02 19:41:35.990' AS DateTime), N'test9', N'222', NULL, 16116, N'9-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31683, 1, N'A', CAST(N'2014-12-02 19:41:57.407' AS DateTime), N'test9', N'224', NULL, 16116, N'9-3@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31684, 1, N'A', CAST(N'2014-12-02 19:42:18.053' AS DateTime), N'test10', N'222', NULL, 16116, N'10-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31685, 1, N'A', CAST(N'2014-12-02 19:42:39.893' AS DateTime), N'test10', N'224', NULL, 16116, N'10-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31686, 1, N'A', CAST(N'2014-12-02 19:43:00.837' AS DateTime), N'test9', N'225', NULL, 16116, N'9-5@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31687, 1, N'A', CAST(N'2014-12-02 19:43:21.903' AS DateTime), N'test9', N'223', NULL, 16116, N'9-4@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31688, 1, N'A', CAST(N'2014-12-02 19:43:43.223' AS DateTime), N'test9', N'221', NULL, 16116, N'9-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31689, 1, N'A', CAST(N'2014-12-02 19:44:04.863' AS DateTime), N'test7', N'220', NULL, 16116, N'7-5@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31690, 1, N'A', CAST(N'2014-12-02 19:44:26.647' AS DateTime), N'test7', N'217', NULL, 16116, N'7-2@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31691, 1, N'A', CAST(N'2014-12-02 19:44:47.597' AS DateTime), N'test3', N'209', NULL, 16116, N'3-4@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31692, 1, N'A', CAST(N'2014-12-02 19:45:08.837' AS DateTime), N'test3', N'205', NULL, 16116, N'3-1@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31693, 1, N'A', CAST(N'2014-12-02 19:45:31.107' AS DateTime), N'test1', N'204', NULL, 16116, N'1-5@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

INSERT CUWebinars.dbo.[WebUser] ([idUser], [UserType], [AcctStatus], [DateCreated], [FirstName], [LastName], [Initial], [idUserInstitution], [email], [futureMail], [generalComments], [taxExempt], [idSubscriptionDiscount], [timeZone], [Title]) VALUES (31694, 1, N'A', CAST(N'2014-12-02 19:48:06.403' AS DateTime), N'test3', N'209', NULL, 16116, N'3-5@ttstester.com', NULL, NULL, NULL, NULL, 3, N'Loan Officer')

GO
SET IDENTITY_insert CUWebinars.dbo.[Address] ON 

GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (2, N'Billing', N'Live Session Only', N'BW_15_PreEvent_1hr', N'LiveSessionOnly', N' ', N'Live_Session_Only_1Hr_165_97', N'54636', N'state', N'United States', 31660)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (3, N'Shipping', N'Live Session Only', N'BW_15_PreEvent_1hr', N'LiveSessionOnly', N' ', N'Live_Session_Only_1Hr_165_97', N'54636', N'state', N'United States', 31660)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (4, N'Billing', N'OnDemand Recording Only', N'BW_15_PreEvent_1hr', N'OnDemandRecordingOnly', N' ', N'OnDemand_Recording_Only_1Hr_185_201', N'54636', N'state', N'United States', 31661)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5, N'Shipping', N'OnDemand Recording Only', N'BW_15_PreEvent_1hr', N'OnDemandRecordingOnly', N' ', N'OnDemand_Recording_Only_1Hr_185_201', N'54636', N'state', N'United States', 31661)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_1hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_1Hr_215_2', N'54636', N'state', N'United States', 31662)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_1hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_1Hr_215_2', N'54636', N'state', N'United States', 31662)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8, N'Billing', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_1hr', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_Weblinks_1Hr_235_20', N'54636', N'state', N'United States', 31663)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (9, N'Shipping', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_1hr', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_Weblinks_1Hr_235_20', N'54636', N'state', N'United States', 31663)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_1hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_1Hr_215_2', N'54636', N'state', N'United States', 31664)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_1hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_1Hr_215_2', N'54636', N'state', N'United States', 31664)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12, N'Billing', N'OnDemand Recording Only', N'BW_15_PostEvent_1hr', N'OnDemandRecordingOnly', N' ', N'OnDemand_Recording_Only_1Hr_185_201', N'54636', N'state', N'United States', 31665)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13, N'Shipping', N'OnDemand Recording Only', N'BW_15_PostEvent_1hr', N'OnDemandRecordingOnly', N' ', N'OnDemand_Recording_Only_1Hr_185_201', N'54636', N'state', N'United States', 31665)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14, N'Billing', N'OnDemand Recording Only', N'BW_15_PreEvent_2hr', N'OnDemandRecordingOnly', N' ', N'On-Demand_Recording_Only_2Hr_295_206', N'54636', N'state', N'United States', 31666)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15, N'Shipping', N'OnDemand Recording Only', N'BW_15_PreEvent_2hr', N'OnDemandRecordingOnly', N' ', N'On-Demand_Recording_Only_2Hr_295_206', N'54636', N'state', N'United States', 31666)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_2hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_2Hr_325_2', N'54636', N'state', N'United States', 31667)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_2hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_2Hr_325_2', N'54636', N'state', N'United States', 31667)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (18, N'Billing', N'OnDemand Recording Only', N'BW_15_PostEvent_2hr', N'OnDemandRecordingOnly', N' ', N'On-Demand_Recording_Only_2Hr_295_206', N'54636', N'state', N'United States', 31668)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (19, N'Shipping', N'OnDemand Recording Only', N'BW_15_PostEvent_2hr', N'OnDemandRecordingOnly', N' ', N'On-Demand_Recording_Only_2Hr_295_206', N'54636', N'state', N'United States', 31668)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (20, N'Billing', N'Live Session Only', N'BW_15_PreEvent_Series3', N'LiveSessionOnly', N' ', N'Live_Session_Only_3Part_699_210', N'54636', N'state', N'United States', 31669)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (21, N'Shipping', N'Live Session Only', N'BW_15_PreEvent_Series3', N'LiveSessionOnly', N' ', N'Live_Session_Only_3Part_699_210', N'54636', N'state', N'United States', 31669)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (22, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_2hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_2Hr_325_2', N'54636', N'state', N'United States', 31670)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (23, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_2hr', N'CD-ROMandHardcopyHandouts', N' ', N'CD-ROM_and_Hardcopy_Handouts_2Hr_325_2', N'54636', N'state', N'United States', 31670)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (24, N'Billing', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_Series3', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_3Part_895_212', N'54636', N'state', N'United States', 31671)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (25, N'Shipping', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_Series3', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_3Part_895_212', N'54636', N'state', N'United States', 31671)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (26, N'Billing', N'Premier Package', N'BW_15_PreEvent_Series3', N'PremierPackage', N' ', N'Premier_3Part_995_214', N'54636', N'state', N'United States', 31672)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (27, N'Shipping', N'Premier Package', N'BW_15_PreEvent_Series3', N'PremierPackage', N' ', N'Premier_3Part_995_214', N'54636', N'state', N'United States', 31672)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (28, N'Billing', N'OnDemand Recording Only', N'BW_15_PostEvent_Series3', N'OnDemandRecordingOnly', N' ', N'OnDemand_Only_3Part_699_211', N'54636', N'state', N'United States', 31673)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (29, N'Shipping', N'OnDemand Recording Only', N'BW_15_PostEvent_Series3', N'OnDemandRecordingOnly', N' ', N'OnDemand_Only_3Part_699_211', N'54636', N'state', N'United States', 31673)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (30, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series3', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_3Part_795_213', N'54636', N'state', N'United States', 31674)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (31, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series3', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_3Part_795_213', N'54636', N'state', N'United States', 31674)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (32, N'Billing', N'Live Sessions Only', N'BW_15_PreEvent_Series4', N'LiveSessionsOnly', N' ', N'Live_Session_Only_4Part_899_216', N'54636', N'state', N'United States', 31675)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (33, N'Shipping', N'Live Sessions Only', N'BW_15_PreEvent_Series4', N'LiveSessionsOnly', N' ', N'Live_Session_Only_4Part_899_216', N'54636', N'state', N'United States', 31675)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (34, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series3', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_3Part_795_213', N'54636', N'state', N'United States', 31676)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (35, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series3', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_3Part_795_213', N'54636', N'state', N'United States', 31676)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (36, N'Billing', N'OnDemand Recording Only', N'BW_15_PreEvent_Series3', N'OnDemandRecordingOnly', N' ', N'OnDemand_Only_3Part_699_211', N'54636', N'state', N'United States', 31677)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (37, N'Shipping', N'OnDemand Recording Only', N'BW_15_PreEvent_Series3', N'OnDemandRecordingOnly', N' ', N'OnDemand_Only_3Part_699_211', N'54636', N'state', N'United States', 31677)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (38, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series4', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_4Part_979_219', N'54636', N'state', N'United States', 31678)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (39, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series4', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_4Part_979_219', N'54636', N'state', N'United States', 31678)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (40, N'Billing', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_Series4', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_4Part_1249_218', N'54636', N'state', N'United States', 31679)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (41, N'Shipping', N'Live Plus OnDemand Weblinks', N'BW_15_PreEvent_Series4', N'LivePlusOnDemandWeblinks', N' ', N'Live_Plus_OnDemand_4Part_1249_218', N'54636', N'state', N'United States', 31679)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (42, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series4', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_4Part_979_219', N'54636', N'state', N'United States', 31680)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (43, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series4', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_Only_4Part_979_219', N'54636', N'state', N'United States', 31680)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (44, N'Billing', N'OnDemand Recordings Only', N'BW_15_PostEvent_Series4', N'OnDemandRecordingsOnly', N' ', N'OnDemand_Only_4Part_899_217', N'54636', N'state', N'United States', 31681)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (45, N'Shipping', N'OnDemand Recordings Only', N'BW_15_PostEvent_Series4', N'OnDemandRecordingsOnly', N' ', N'OnDemand_Only_4Part_899_217', N'54636', N'state', N'United States', 31681)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (46, N'Billing', N'Ondemand Recordings Only', N'BW_15_PreEvent_Series5', N'OndemandRecordingsOnly', N' ', N'LIVE_SESSION_ONLY_5PART_1095_222', N'54636', N'state', N'United States', 31682)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (47, N'Shipping', N'Ondemand Recordings Only', N'BW_15_PreEvent_Series5', N'OndemandRecordingsOnly', N' ', N'LIVE_SESSION_ONLY_5PART_1095_222', N'54636', N'state', N'United States', 31682)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (48, N'Billing', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series5', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_ONLY_5PART_1245_224', N'54636', N'state', N'United States', 31683)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (49, N'Shipping', N'CD-ROM and Hardcopy Handouts', N'BW_15_PreEvent_Series5', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_ONLY_5PART_1245_224', N'54636', N'state', N'United States', 31683)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (50, N'Billing', N'10. Ondemand Recordings Only', N'BW_15_PostEvent_Series5', N'OndemandRecordingsOnly', N' ', N'LIVE_SESSION_ONLY_5PART_1095_222', N'54636', N'state', N'United States', 31684)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (51, N'Shipping', N'10. Ondemand Recordings Only', N'BW_15_PostEvent_Series5', N'OndemandRecordingsOnly', N' ', N'LIVE_SESSION_ONLY_5PART_1095_222', N'54636', N'state', N'United States', 31684)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (52, N'Billing', N'10. CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series5', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_ONLY_5PART_1245_224', N'54636', N'state', N'United States', 31685)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (53, N'Shipping', N'10. CD-ROM and Hardcopy Handouts', N'BW_15_PostEvent_Series5', N'CD-ROMandHardcopyHandouts', N' ', N'CDROM_ONLY_5PART_1245_224', N'54636', N'state', N'United States', 31685)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (54, N'Billing', N'Premier Package', N'BW_15_PreEvent_Series5', N'PremierPackage', N' ', N'PREMIER_5PART_1745_225', N'54636', N'state', N'United States', 31686)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (55, N'Shipping', N'Premier Package', N'BW_15_PreEvent_Series5', N'PremierPackage', N' ', N'PREMIER_5PART_1745_225', N'54636', N'state', N'United States', 31686)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (56, N'Billing', N'Live Plus Ondemand Weblinks', N'BW_15_PreEvent_Series5', N'LivePlusOndemandWeblinks', N' ', N'LIVE_PLUS_ONDEMAND_5PART_1495_223', N'54636', N'state', N'United States', 31687)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (57, N'Shipping', N'Live Plus Ondemand Weblinks', N'BW_15_PreEvent_Series5', N'LivePlusOndemandWeblinks', N' ', N'LIVE_PLUS_ONDEMAND_5PART_1495_223', N'54636', N'state', N'United States', 31687)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (58, N'Billing', N'Live Sessions Only', N'BW_15_PreEvent_Series5', N'LiveSessionsOnly', N' ', N'Live_session_only_5part_1095_221', N'54636', N'state', N'United States', 31688)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (59, N'Shipping', N'Live Sessions Only', N'BW_15_PreEvent_Series5', N'LiveSessionsOnly', N' ', N'Live_session_only_5part_1095_221', N'54636', N'state', N'United States', 31688)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (60, N'Billing', N'Premier Package', N'BW_15_PreEvent_Series4', N'PremierPackage', N' ', N'Premier_4Part_1399_220', N'54636', N'state', N'United States', 31689)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (61, N'Shipping', N'Premier Package', N'BW_15_PreEvent_Series4', N'PremierPackage', N' ', N'Premier_4Part_1399_220', N'54636', N'state', N'United States', 31689)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (62, N'Billing', N'OnDemand Recordings Only', N'BW_15_PreEvent_Series4', N'OnDemandRecordingsOnly', N' ', N'OnDemand_Only_4Part_899_217', N'54636', N'state', N'United States', 31690)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (63, N'Shipping', N'OnDemand Recordings Only', N'BW_15_PreEvent_Series4', N'OnDemandRecordingsOnly', N' ', N'OnDemand_Only_4Part_899_217', N'54636', N'state', N'United States', 31690)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (64, N'Billing', N'Premier Package', N'BW_15_PreEvent_2hr', N'PremierPackage', N' ', N'Premier_Package_2Hr_395_209', N'54636', N'state', N'United States', 31691)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (65, N'Shipping', N'Premier Package', N'BW_15_PreEvent_2hr', N'PremierPackage', N' ', N'Premier_Package_2Hr_395_209', N'54636', N'state', N'United States', 31691)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (66, N'Billing', N'Live Session Only', N'BW_15_PreEvent_2hr', N'LiveSessionOnly', N' ', N'Live_Session_Only_2Hr_265_205', N'54636', N'state', N'United States', 31692)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (67, N'Shipping', N'Live Session Only', N'BW_15_PreEvent_2hr', N'LiveSessionOnly', N' ', N'Live_Session_Only_2Hr_265_205', N'54636', N'state', N'United States', 31692)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (68, N'Billing', N'Premier Package', N'BW_15_PreEvent_1hr', N'PremierPackage', N' ', N'Premier_Package_1Hr_265_204', N'54636', N'state', N'United States', 31693)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (69, N'Shipping', N'Premier Package', N'BW_15_PreEvent_1hr', N'PremierPackage', N' ', N'Premier_Package_1Hr_265_204', N'54636', N'state', N'United States', 31693)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (70, N'Billing', N'Premier Package', N'BW_15_PreEvent_2hr', N'PremierPackage', N' ', N'Premier_Package_2Hr_395_209', N'54636', N'state', N'United States', 31694)
GO
insert CUWebinars.dbo.[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (71, N'Shipping', N'Premier Package', N'BW_15_PreEvent_2hr', N'PremierPackage', N' ', N'Premier_Package_2Hr_395_209', N'54636', N'state', N'United States', 31694)
GO
SET IDENTITY_insert CUWebinars.dbo.[Address] OFF
GO

PRINT '--------------------------finished inserting TestUsers'