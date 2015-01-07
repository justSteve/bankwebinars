SET NOCOUNT ON


PRINT 'Starts AdditionalLocationsLookupPrice'
DELETE  FROM bankwebinars.dbo.AdditionalLocationsLookupPrice WHERE idWebinar > 0
--SELECT  'INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES ('
--        + CAST(idWebinar AS VARCHAR) + ',50.00)'
--FROM    dbo.Webinar
--WHERE   idWebinar IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar WHERE status = 2 AND duration = 1)
--SELECT  'INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES ('
--        + CAST(idWebinar AS VARCHAR) + ',75.00)'
--FROM    dbo.Webinar
--WHERE   idWebinar IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar WHERE status = 2 AND duration = 2)

INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1711,325.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (883,50.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1236,50.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1727,50.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1740,50.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1741,50.00)
        INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1710,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1711,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1712,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1717,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1718,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1719,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1721,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1722,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1723,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1724,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1726,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1728,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1729,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1730,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1731,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1732,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1733,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1734,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1735,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1736,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1737,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1738,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1739,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1742,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1743,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1744,75.00)
INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1745,75.00)


GO
PRINT '____________Ends AdditionalLocationsLookupPrice'