SET IDENTITY_INSERT BWClean.dbo.Topic ON
INSERT  BWClean.dbo.Topic
        ( idTopic ,
          topicDesc ,
          idParentTopic ,
          topicHTML ,
          sortOrder
		 )
        SELECT  idTopic ,
                topicDesc ,
                idParentTopic ,
                topicHTML ,
                sortOrder
        FROM    CUWebinarsClean.dbo.Topic
SET IDENTITY_INSERT BWClean.dbo.Topic OFF