--SELECT  'update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = ''CP_'
--        + CAST(idOrder AS VARCHAR) +
--        ''') where idOrder = ' + CAST(idOrder AS VARCHAR)
----
--FROM    TTSWebinars2.dbo.OrdersRows
--WHERE   TTSWebinars2.dbo.OrdersRows.idWebinar = 842
--        AND ( TTSWebinars2.dbo.OrdersRows.status > 1
--              OR TTSWebinars2.dbo.OrdersRows.status < 4
--            )
		 
		 -----------------above code generates statement below------------

		 
SET NOCOUNT ON
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_19983') where idOrder = 19983
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21253') where idOrder = 21253
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21254') where idOrder = 21254
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21268') where idOrder = 21268
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21274') where idOrder = 21274
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21278') where idOrder = 21278
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_21282') where idOrder = 21282
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22572') where idOrder = 22572
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22648') where idOrder = 22648
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_22890') where idOrder = 22890
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23162') where idOrder = 23162
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23223') where idOrder = 23223
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_23242') where idOrder = 23242
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_32174') where idOrder = 32174
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_32939') where idOrder = 32939
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33088') where idOrder = 33088
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33491') where idOrder = 33491
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33492') where idOrder = 33492
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_33817') where idOrder = 33817
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35266') where idOrder = 35266
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35460') where idOrder = 35460
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_35799') where idOrder = 35799
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_36172') where idOrder = 36172
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_43493') where idOrder = 43493
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_44807') where idOrder = 44807
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_45529') where idOrder = 45529
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_45967') where idOrder = 45967
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_46026') where idOrder = 46026
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_46196') where idOrder = 46196
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_49068') where idOrder = 49068
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50032') where idOrder = 50032
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50079') where idOrder = 50079
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50519') where idOrder = 50519
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50730') where idOrder = 50730
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50761') where idOrder = 50761
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_50919') where idOrder = 50919
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_51186') where idOrder = 51186
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_51705') where idOrder = 51705
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_52860') where idOrder = 52860
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_56893') where idOrder = 56893
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59756') where idOrder = 59756
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59797') where idOrder = 59797
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59801') where idOrder = 59801
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59820') where idOrder = 59820
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_59973') where idOrder = 59973
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_61049') where idOrder = 61049
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_62315') where idOrder = 62315
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_64834') where idOrder = 64834
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_65634') where idOrder = 65634
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_65907') where idOrder = 65907
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_66106') where idOrder = 66106
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_68033') where idOrder = 68033
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_70083') where idOrder = 70083
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_71226') where idOrder = 71226
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_71435') where idOrder = 71435
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73600') where idOrder = 73600
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73601') where idOrder = 73601
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73625') where idOrder = 73625
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_73878') where idOrder = 73878
update TTSWebinars2.dbo.OrdersRows set idDiscount =  (select TTSWebinars2.dbo.Discounts.idDiscounts from TTSWebinars2.dbo.Discounts where code = 'CP_74813') where idOrder = 74813

