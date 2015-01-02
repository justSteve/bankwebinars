USE BankWebinars
GO
SET NOCOUNT ON 

SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 1


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 2


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 3


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 4


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 5


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 6


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 7


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 8


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 9


SELECT w.Title, r.RegTypeLabel, r.Price, r.RegTypeExplain, r.SKU, r.SortOrder,cast(r.idRegType as varchar) + ' ' + regGroup.RegTypeGroupDesc FROM dbo.Webinar w INNER JOIN dbo.RegTypesGroupsXref rgx ON rgx.idWebinar = w.idWebinar
INNER JOIN dbo.RegTypesGroups regGroup ON regGroup.idRegTypeGroup = rgx.idRegTypeGroup
INNER JOIN dbo.RegTypesXref rx ON rx.idRegTypeGroup = regGroup.idRegTypeGroup
INNER JOIN dbo.RegType r ON r.idRegType = rx.idRegType
WHERE w.idWebinar = 10

SET NOCOUNT off