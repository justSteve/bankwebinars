
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, 200)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, 201)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, 203)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, 202)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, 204)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 46, 201)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 46, 203)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 46, 226)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 46, 227)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, 205)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, 206)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, 207)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, 208)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, 209)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 44, 206)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 44, 208)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 44, 228)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 44, 229)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, 210)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, 211)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, 213)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, 212)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, 214)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 48, 211)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 48, 213)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 48, 230)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 48, 231)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, 216)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, 217)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, 219)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, 218)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, 220)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 50, 217)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 50, 219)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 50, 232)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 50, 233)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, 221)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, 222)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, 224)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, 223)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, 225)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 52, 222)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 52, 224)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 52, 234)
INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 52, 235)

--SELECT * FROM dbo.RegTypesGroups WHERE RegTypeGroupDesc LIKE 'BW_15%'
--SELECT * FROM dbo.RegType where sku like '%upgrad%' and WHERE TaxExempt = 0 

--SELECT 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 45, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND  (SKU LIKE '%1hr%' AND SKU NOT LIKE '%upgrade%') ORDER BY SortOrder
--SELECT 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 46, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND(( SKU LIKE '%upgrade%' AND SKU LIKE '%1hr%') OR ( RegTypeLabel LIKE 'OnDemand%' AND SKU LIKE '%1hr%') OR ( RegTypeLabel LIKE 'CD-%' AND SKU LIKE '%1hr%')  ) ORDER BY SortOrder

--SELECT RegTypeLabel, 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 43, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND (SKU LIKE '%2hr%' AND SKU NOT LIKE '%upgrade%') ORDER BY SortOrder
--SELECT 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 44, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND(( SKU LIKE '%upgrade%' AND SKU LIKE '%2hr%') OR ( RegTypeLabel LIKE 'OnDemand%' AND SKU LIKE '%2hr%') OR ( RegTypeLabel LIKE 'CD-%' AND SKU LIKE '%2hr%')  ) ORDER BY SortOrder

--SELECT RegTypeLabel, sku,'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 47, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND (SKU LIKE '%3Part%' AND SKU NOT LIKE '%upgrade%') ORDER BY SortOrder
--SELECT RegTypeLabel, sku, 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 48, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND(( SKU LIKE '%upgrade%' AND SKU LIKE '%Series3%') OR ( RegTypeLabel LIKE 'OnDemand%' AND SKU LIKE '%3Part%') OR ( RegTypeLabel LIKE 'CD-%' AND SKU LIKE '%3Part%')  ) ORDER BY SortOrder

--SELECT RegTypeLabel, sku,'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 49, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND (SKU LIKE '%4Part%' AND SKU NOT LIKE '%upgrade%') ORDER BY SortOrder
--SELECT RegTypeLabel, sku, 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 50, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND(( SKU LIKE '%upgrade%' AND SKU LIKE '%Series4%') OR ( RegTypeLabel LIKE 'OnDemand%' AND SKU LIKE '%4Part%') OR ( RegTypeLabel LIKE 'CD-%' AND SKU LIKE '%4Part%')  ) ORDER BY SortOrder

--SELECT RegTypeLabel, sku,'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 51, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND (SKU LIKE '%5Part%' AND SKU NOT LIKE '%upgrade%') ORDER BY SortOrder
--SELECT RegTypeLabel, sku, 'INSERT bankwebinars.[dbo].RegTypesXref ( idRegTypeGroup, idRegType ) VALUES  ( 52, ' + CAST(idRegType AS VARCHAR) + ')' FROM dbo.RegType WHERE TaxExempt = 0 AND(( SKU LIKE '%upgrade%' AND SKU LIKE '%Series5%') OR ( RegTypeLabel LIKE 'OnDemand%' AND SKU LIKE '%5Part%') OR ( RegTypeLabel LIKE 'CD-%' AND SKU LIKE '%5Part%')  ) ORDER BY SortOrder
