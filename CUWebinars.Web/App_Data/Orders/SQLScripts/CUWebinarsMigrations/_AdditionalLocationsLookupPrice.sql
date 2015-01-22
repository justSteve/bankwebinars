SET NOCOUNT ON


PRINT 'Starts AdditionalLocationsLookupPrice'
DELETE  FROM CUWebinars.dbo.AdditionalLocationsLookupPrice WHERE idWebinar > 0

--SELECT  'INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES ('
--        + CAST(idWebinar AS VARCHAR) + ',75.00)'
--FROM    dbo.Webinar
--WHERE   idWebinar IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar WHERE status = 2 AND duration = 2)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4591, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4593, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4596, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4597, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4598, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4601, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4602, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4606, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4617, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4622, 50)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4623, 50)

GO
PRINT '____________Ends AdditionalLocationsLookupPrice'