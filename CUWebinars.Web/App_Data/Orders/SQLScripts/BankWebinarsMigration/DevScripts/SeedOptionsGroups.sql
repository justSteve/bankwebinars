    SET IDENTITY_INSERT BWClean.dbo.OptionsGroups ON
    INSERT  BWClean.dbo.OptionsGroups
            ( idOptionGroup ,
              OptionGroupDesc ,
              OptionType ,
              SortOrder
            )
            SELECT  [idOptionGroup] ,
                    [OptionGroupDesc] ,
                    [OptionType] ,
                    [SortOrder]
            FROM    BWMigrator.[dbo].[OptionsGroups] WHERE idOptionGroup IN (15,23,24,25,26,27,28,29,30,32,33)

    SET IDENTITY_INSERT BWClean.dbo.OptionsGroups OFF