USE CUWebinars
GO


SELECT * FROM BankWebinars.dbo.Affiliate WHERE idUserAff   = 19
--SELECT  'INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES ('
--        + CAST(idWebinar AS VARCHAR) + ',50.00)'
--FROM    dbo.Webinar
--WHERE   idWebinar IN ( SELECT   idWebinar
--                           FROM     dbo.Webinar WHERE status = 2 AND duration = 1)


INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1747,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1749,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1750,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1751,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (1752,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4591,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4593,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4596,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4597,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4598,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4601,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4602,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4606,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4622,50.00)
INSERT CUWebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES (4623,50.00)

INSERT [dbo].[Affiliate] ([idUserAff], [CommissionModel], [URL], [WebBanner], [WebFooter], [EmailBanner], [EmailFooter], [ttsDomain], [GAPass], [supportEmail], [DisplayTitle], [BillingModel], [Logo], [ContactPerson], [ContactPhone], [ContactEmail], [ContactFax], [ContactAddress], [TechEmail], [TechPhone], [TechName], [EmailPromo]) VALUES (19, 1, N'http://www.ttstrain.com/', N'
        <div class="container" id="header" style="margin-bottom: 0;">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td style="height: 77px; width: 550px; background-image: url(/Content/images/BWBannerLeft.jpg);"></td>
                    <td style="height: 77px; width: 620px; background-image: url(/Content/images/BannerGradientBack.jpg);">
                        <div class="bannerRight aligncenter">
                            <p align="center" style="font-size: larger; color: #1F5B93;">
                                <b><i>Providing premier web-based seminars featuring<br />
                                    the best speakers in the financial industry since 2002. </i></b>
                            </p>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
', N'<p>PO Box 310 . Waunakee, Wisconsin . 53597<br><strong>phone:</strong> <a href="tel:8008310678" class="tele">800.831.0678</a><br><strong>fax:</strong> 800.831.3776<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>
', N'', N'', N'Bennett', NULL, NULL, N'Total Training Solutions', N'billed', NULL, N'Mark', N'608-849-5563', N'affiliate@ttstrain.com', N'', N'PO Box 310', N'affiliate@ttstrain.com', N'608-849-5563', N'Mark', N'None')
GO
UPDATE dbo.Affiliate SET WebFooter = '<p>PO Box 310 . Waunakee, Wisconsin . 53597<br><strong>phone:</strong> <a href="tel:8008310678" class="tele">800.831.0678</a><br><strong>fax:</strong> 800.831.3776<br><strong>email:</strong> <a href="mailto:support@bankwebinars.com">support@bankwebinars.com</a></p>~<ul class="social"><li><a class="socicon small facebook" href="http://www.facebook.com/home.php#!/pages/Total-Training-Solutions/300546163412" target="_blank" data-placement="bottom" title="Follow us on Facebook"></a></li><li><a class="socicon small twitterbird" href="http://twitter.com/#!/ttstrain" target="_blank" data-placement="bottom" title="Follow us on Twitter"></a></li><li><a class="socicon small linkedin" href="https://www.linkedin.com/company/bankwebinars.com" target="_blank" data-placement="bottom" title="Follow us on LinkedIn"></a></li></ul>' WHERE idUserAff = 19
GO
--SET IDENTITY_INSERT [dbo].[RegTypesGroups] ON 

--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (32, N'BW_13_PreEvent_2hr', 1)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (33, N'BW_13_PostEvent_2hr', 2)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (34, N'BW_13_PreEvent_1hr', 3)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (35, N'BW_13_PostEvent_1hr', 4)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (36, N'BW_13_Subscription', 5)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (37, N'BW_13_PreEvent_Series3', 6)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (38, N'BW_13_PostEvent_Series3', 7)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (39, N'BW_13_PreEvent_Series4', 7)
--GO
--INSERT [dbo].[RegTypesGroups] ([idRegTypeGroup], [RegTypeGroupDesc], [SortOrder]) VALUES (40, N'BW_13_PostEvent_Series4', 7)
--GO
--SET IDENTITY_INSERT [dbo].[RegTypesGroups] OFF
--GO
--SET IDENTITY_INSERT [dbo].[RegTypesXref] ON 

--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8395, 32, 82)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8396, 32, 83)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8401, 32, 85)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8402, 32, 86)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8403, 32, 87)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8404, 32, 88)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8406, 33, 91)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8407, 33, 92)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8408, 33, 93)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8409, 33, 95)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8410, 34, 97)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8411, 34, 98)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8412, 34, 99)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8413, 34, 100)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8414, 34, 101)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8416, 35, 103)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8417, 35, 104)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8418, 35, 105)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8419, 35, 107)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8420, 36, 118)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8421, 36, 119)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8422, 36, 120)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8424, 39, 124)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8425, 39, 125)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8426, 39, 126)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8427, 39, 127)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8428, 39, 128)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8430, 40, 130)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8431, 40, 131)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8432, 40, 132)
--GO
--INSERT [dbo].[RegTypesXref] ([idRegTypesXref], [idRegTypeGroup], [idRegType]) VALUES (8433, 40, 133)
--GO
--SET IDENTITY_INSERT [dbo].[RegTypesXref] OFF
--GO
SET IDENTITY_INSERT [dbo].[RegTypesGroupsXref] ON 

GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27156, 801, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27157, 802, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27158, 803, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27171, 825, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27180, 834, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27198, 400, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27199, 401, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27200, 402, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27201, 403, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27205, 407, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27232, 435, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27234, 437, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27263, 467, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (27486, 701, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28171, 1268, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28186, 1285, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28226, 1326, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28305, 1358, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28413, 1411, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28420, 1415, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28424, 1416, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28428, 1410, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28432, 1420, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28439, 1422, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28444, 1426, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28451, 1430, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28455, 1432, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28457, 1433, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28463, 1436, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28466, 1438, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28479, 1437, 33)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28548, 1471, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28549, 1472, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (28712, 404, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (29712, 1558, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32717, 3559, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32718, 4558, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32719, 4559, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32720, 4560, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32726, 4561, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (32727, 4562, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33733, 4563, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33734, 4564, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33735, 4565, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33736, 4566, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33737, 4567, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33739, 4569, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33740, 4568, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33741, 3558, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33742, 1473, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33743, 4570, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33744, 4571, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33745, 4572, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33746, 4574, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33747, 4578, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33748, 4580, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33760, 4622, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33761, 4623, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33762, 4617, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33763, 4606, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33764, 4593, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33765, 4596, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33766, 4602, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33767, 4597, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33768, 4598, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33769, 4600, 35)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33770, 4601, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33771, 1747, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33772, 1749, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33773, 1750, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33774, 1752, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33775, 1751, 34)
GO
INSERT [dbo].[RegTypesGroupsXref] ([idWebinarRegTypeGroup], [idWebinar], [idRegTypeGroup]) VALUES (33777, 4591, 34)
GO
SET IDENTITY_INSERT [dbo].[RegTypesGroupsXref] OFF
GO

UPDATE dbo.RegTypesGroupsXref SET idRegTypeGroup = 45 WHERE idWebinar IN 
(SELECT idWebinar FROM dbo.Webinar WHERE Status = 2)
UPDATE dbo.RegTypesGroupsXref SET idRegTypeGroup = 46 WHERE idWebinar IN 
(SELECT idWebinar FROM dbo.Webinar WHERE Status > 2)
SET IDENTITY_INSERT [dbo].[Topic] ON 

GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (15, N'Compliance', NULL, N'', 1)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (16, N'IRA', NULL, N'', 2)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (17, N'Customer Service', NULL, N'', 3)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (18, N'Security', NULL, N'', 5)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (19, N'Operations', NULL, N'', 4)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (20, N'Auditing', NULL, N'', 6)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (21, N'Sales', NULL, N'', 7)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (22, N'Lending', NULL, N'', 8)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (23, N'Human Resources', NULL, N'', 9)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (25, N'Computer Skills', NULL, N'', 1)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (26, N'Risk Management', NULL, N'', 1)
GO
INSERT [dbo].[Topic] ([idTopic], [topicDesc], [idParentTopic], [topicHTML], [sortOrder]) VALUES (27, N'Teller', NULL, N'', 1)
GO
SET IDENTITY_INSERT [dbo].[Topic] OFF
GO
SET IDENTITY_INSERT [dbo].[WebinarTopicXref] ON 

GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42215, 400, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42216, 401, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42217, 402, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42218, 403, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42219, 404, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42222, 407, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42223, 407, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42264, 435, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42266, 437, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42310, 467, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42941, 801, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42942, 801, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42943, 801, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42944, 802, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42945, 802, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42946, 802, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42947, 802, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42951, 803, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42952, 803, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (42953, 803, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43004, 825, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43005, 825, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43006, 825, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43007, 825, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43058, 834, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43059, 834, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43060, 834, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43061, 834, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43692, 1358, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43693, 1358, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43694, 1358, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43827, 1410, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43828, 1410, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43841, 1326, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43842, 1326, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43843, 1326, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43845, 1415, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43846, 1415, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43847, 1411, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43848, 1411, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43849, 1420, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43850, 1420, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43851, 1420, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43854, 1416, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43858, 1422, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43871, 1430, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43872, 1430, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43873, 1426, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43874, 1426, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43875, 1426, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43880, 1432, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43881, 1432, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43882, 1433, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43883, 1433, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43884, 1433, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43894, 1436, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43897, 1437, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43898, 1437, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43902, 1438, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43975, 1268, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (43976, 1268, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44020, 1472, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44065, 1471, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44102, 1285, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44103, 1285, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44120, 467, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44121, 437, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44122, 435, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44123, 407, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44124, 404, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (44125, 701, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45125, 403, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45126, 404, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45127, 404, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45128, 437, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45129, 701, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (45130, 825, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (46124, 1558, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (48135, 3559, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (49129, 3559, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (49131, 3559, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50147, 4563, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50148, 4564, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50149, 4565, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50150, 4566, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50151, 4567, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50152, 4558, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50153, 4558, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50154, 4558, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50155, 4559, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50156, 4559, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50157, 4560, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50158, 4560, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50159, 4560, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50160, 4561, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50161, 4561, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50162, 4561, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50163, 4561, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50164, 4562, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50165, 4562, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50166, 4562, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50172, 4569, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50173, 4569, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50174, 4569, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50175, 4568, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50176, 4568, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50177, 4568, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50178, 4568, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50179, 4568, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50180, 3558, 25)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50181, 1473, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50182, 1473, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50183, 1473, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50184, 4570, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50185, 4570, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50186, 4570, 18)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50187, 4571, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50188, 4572, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50189, 4574, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50190, 4574, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50191, 4574, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50192, 4578, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50193, 4580, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50208, 4600, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50209, 4600, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50211, 4601, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50212, 4601, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50213, 4593, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50214, 4593, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50219, 4596, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50220, 4596, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50221, 4596, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50222, 4597, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50223, 4597, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50224, 4597, 21)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50225, 4597, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50226, 4598, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50227, 4591, 16)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50228, 4591, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50229, 4591, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50230, 4591, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50231, 4602, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50232, 4602, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50233, 4602, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50234, 4606, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50235, 4606, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50236, 4606, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50237, 4617, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50238, 4617, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50239, 4617, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50243, 4622, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50244, 4622, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50245, 4622, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50246, 4623, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50247, 4623, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50248, 4623, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50249, 4600, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50250, 4600, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50251, 4600, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50252, 4591, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50253, 4591, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50254, 4591, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50255, 4591, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50256, 4601, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50257, 4601, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50258, 4601, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50259, 4596, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50260, 4596, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50261, 4596, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50262, 4597, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50263, 4597, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50264, 4597, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50265, 4597, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50266, 4598, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50267, 4598, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50268, 4598, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50269, 4593, 17)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50270, 4593, 23)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50271, 4593, 27)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50297, 1747, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50298, 1747, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50299, 1747, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50300, 1747, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50301, 1749, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50302, 1749, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50303, 1750, 20)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50304, 1750, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50305, 1750, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50306, 1751, 19)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50307, 1751, 22)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50308, 1751, 26)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50309, 1752, 15)
GO
INSERT [dbo].[WebinarTopicXref] ([idWebinarTopicXref], [idWebinar], [idTopic]) VALUES (50310, 1752, 19)
GO
SET IDENTITY_INSERT [dbo].[WebinarTopicXref] OFF
GO
SET IDENTITY_INSERT [dbo].[WebinarFile] ON 

GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (33, 4564, N'CoachEmplEff0Final81214.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (34, 4568, N'SocialMedia071714.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (35, 4568, N'SocialMediaHO071714.pdf', N'Compliance Framework Supplementary Materials ')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (36, 400, N'AcctCardsCU0214.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (37, 401, N'TrustCUC0314.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (38, 401, N'TrustCUB0314.pdf', N'Handouts - Black & White')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (39, 402, N'AnnualTrainingCU0314.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (40, 403, N'BusinessCU0414.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (41, 404, N'CU10LessonsMembDies061214.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (42, 407, N'IRARolloverCUC0314.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (43, 407, N'IRARolloverCUB0314.pdf', N'Handouts - Black & White')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (44, 435, N'IRAUpdateCU0214.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (45, 435, N'IRAUpdateCUB0214.pdf', N'Handouts - Black & White')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (46, 437, N'IRADistCUC0314.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (47, 437, N'IRADistCUB0314.pdf', N'Handouts - Black & White')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (48, 467, N'IRATradRothCUC0314.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (49, 467, N'IRATradRothCUB0314.pdf', N'Handouts - Black & White')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (50, 701, N'VendorMgmtCU0414.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (51, 825, N'ecoacu0414.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (52, 1473, N'Supplementary%20Handout.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (53, 1473, N'CUPreventCompMistSlides.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (54, 1473, N'Appendix.pdf', N'Appendix')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (55, 1558, N'MotivateSales0514.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (56, 3558, N'ExcelMadeClearIntroductionToSpreadsheets.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (57, 3559, N'CUOpeningAccountIds051914.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (58, 4558, N'CUCTRLBLFinal080714.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (59, 4563, N'CUWebinarsHeadTellerBPFinal.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (64, 4565, N'CULending101082814.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (65, 4565, N'Lending101Booklet.pdf', N'Booklet')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (66, 4559, N'DosDontsChecksFinal090314.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (67, 4570, N'CURobberyFinal091714.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (69, 4567, N'BestEverChecklistSlides092414.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (70, 4567, N'BestEverCheckslists092414.pdf', N'Checklists')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (71, 4567, N'BestEverBooklet092414.pdf', N'Explanatory Booklet')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1067, 4572, N'SalesEnvironment1014.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1068, 4560, N'HandlingPOAFinal101614.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1069, 4566, N'CUFloodIns102214.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1070, 4566, N'FloodBooklet101714.pdf', N'Booklet')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1071, 4562, N'CUNRA102914.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1072, 4561, N'NewShareMemberInt110414.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1073, 4578, N'EngagementHO1214.docx', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1074, 4578, N'EngagementB1214.pdf', N'Slides (Black and White)')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1075, 4578, N'EngagementC1214.pdf', N'Slides (Color)')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1076, 4574, N'CUHMDAHO121014.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1077, 4574, N'CUHMDAslides121014.pdf', N'Slides')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1078, 4600, N'OpenBusAccts.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1079, 4600, N'OBA50states2013.pdf', N'Supplement')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1080, 4600, N'MSBInforChart.pdf', N'Supplement II')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1081, 4617, N'AppraisalsCU0115.pdf', N'Handouts')
GO
INSERT [dbo].[WebinarFile] ([idWebinarFile], [idWebinar], [fileLocation], [fileDesc]) VALUES (1082, 4617, N'CUAppraisalHO011215.pdf', N'Handouts')
GO
SET IDENTITY_INSERT [dbo].[WebinarFile] OFF
GO
SET IDENTITY_INSERT [dbo].[Address] ON 

GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5862, N'Billing', N'Steve Hue', N'66', N'66', NULL, N'Holmen', N'54636', N'WI', N'US', 26361)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5863, N'Shipping', N'Steve Hue', N'66', N'66', NULL, N'Holmen', N'54636', N'WI', N'US', 26361)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5864, N'Billing', N'LiveOnly UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26366)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5865, N'Shipping', N'LiveOnly UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26366)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5866, N'Billing', N'CDRom UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26367)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5867, N'Shipping', N'CDRom UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26367)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5868, N'Billing', N'LivePlusOnDemand UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26368)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5869, N'Shipping', N'LivePlusOnDemand UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26368)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5870, N'Billing', N'Premier UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26369)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5871, N'Shipping', N'Premier UpcomingNoConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26369)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5872, N'Billing', N'LiveOnly UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26370)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5873, N'Shipping', N'LiveOnly UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26370)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5874, N'Billing', N'RecordedOnly UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26371)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5875, N'Shipping', N'RecordedOnly UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26371)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5876, N'Billing', N'CDRom UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26372)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5877, N'Shipping', N'CDRom UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26372)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5878, N'Billing', N'Premier UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26373)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5879, N'Shipping', N'Premier UpcomingHasConnInfo', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26373)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5880, N'Billing', N'OnDemandOnly Recorded', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26374)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5881, N'Shipping', N'OnDemandOnly Recorded', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26374)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5882, N'Billing', N'CDRom Recorded', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26375)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5883, N'Shipping', N'CDRom Recorded', N'(256) 837-6110', N'220 Wynn Drive', N' ', N'Huntsville', N'35893', N'AL', N'USA', 26375)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5884, N'Billing', N'Todd  Mason', N'(260) 432-3433', N'1209 Reckeweg Rd', N' ', N'Fort Wayne ', N'46804', N'IN', N'United States', 26376)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5885, N'Shipping', N'Todd  Mason', N'(260) 432-3433', N'1209 Reckeweg Rd', N' ', N'Fort Wayne ', N'46804', N'IN', N'United States', 26376)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5886, N'Billing', N'EVITA VILORIA', N'(425) 2971051', N'2821 HEWITT AVE', N' ', N'EVERETT', N'98201', N'WA', N'United States', 26377)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5887, N'Shipping', N'EVITA VILORIA', N'(425) 2971051', N'2821 HEWITT AVE', N' ', N'EVERETT', N'98201', N'WA', N'United States', 26377)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5888, N'Billing', N'Janet Schreiber', N'(715) 536-8351', N'300 East Second Street', N' ', N'Merrill', N'54452', N'WI', N'United States', 26378)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5889, N'Shipping', N'Janet Schreiber', N'(715) 536-8351', N'300 East Second Street', N' ', N'Merrill', N'54452', N'WI', N'United States', 26378)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5890, N'Billing', N'Tom Bradely', N'(703) 7098900-ext4426', N'200 spring St', N' ', N'Herndon', N'20170', N'VA', N'United States', 26379)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5891, N'Shipping', N'Tom Bradely', N'(703) 7098900-ext4426', N'200 spring St', N' ', N'Herndon', N'20170', N'VA', N'United States', 26379)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5892, N'Billing', N'Sofia Iniguez', N'(800) 640-1228', N'P.O. Box 261370', N' ', N'San Diego', N'92196-1370', N'CA', N'United States', 26380)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5893, N'Shipping', N'Sofia Iniguez', N'(800) 640-1228', N'P.O. Box 261370', N' ', N'San Diego', N'92196-1370', N'CA', N'United States', 26380)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5894, N'Billing', N'Sandy Demers', N'(207) 7845435', N'381 Main St', N'PO Box 741', N'Lewiston', N'04243-0741', N'ME', N'United States', 26381)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5895, N'Shipping', N'Sandy Demers', N'(207) 7845435', N'381 Main St', N'PO Box 741', N'Lewiston', N'04243-0741', N'ME', N'United States', 26381)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5896, N'Billing', N'Tanya Cason', N'(214) 748-9393', N'13649 Montfort Drive', N' ', N'Dallas', N'75240', N'TX', N'United States', 26382)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5897, N'Shipping', N'Tanya Cason', N'(214) 748-9393', N'13649 Montfort Drive', N' ', N'Dallas', N'75240', N'TX', N'United States', 26382)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5898, N'Billing', N'MARY KRAMER', N'(303) 427-6466', N'9053 HARLAN ST SUITE 10', N' ', N'WESTMINSTER', N'80031', N'CO', N'United States', 26383)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5899, N'Shipping', N'MARY KRAMER', N'(303) 427-6466', N'9053 HARLAN ST SUITE 10', N' ', N'WESTMINSTER', N'80031', N'CO', N'United States', 26383)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5900, N'Billing', N'Gloria Pina', N'(254) 741-4123', N'4631 W Waco Drive', N' ', N'Waco', N'76710', N'Tx', N'United States', 26384)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5901, N'Shipping', N'Gloria Pina', N'(254) 741-4123', N'4631 W Waco Drive', N' ', N'Waco', N'76710', N'Tx', N'United States', 26384)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5902, N'Billing', N'Emily Kleiman', N'(906) 7867213', N'2600 1st ave S', N' ', N'Escanaba', N'49829', N'mi', N'United States', 26385)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5903, N'Shipping', N'Emily Kleiman', N'(906) 7867213', N'2600 1st ave S', N' ', N'Escanaba', N'49829', N'mi', N'United States', 26385)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5904, N'Billing', N'Ashley Clark', N'(402) 552-7116', N'1910 S 44th St', N' ', N'Omaha', N'68106', N'NE', N'United States', 26386)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5905, N'Shipping', N'Ashley Clark', N'(402) 552-7116', N'1910 S 44th St', N' ', N'Omaha', N'68106', N'NE', N'United States', 26386)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5906, N'Billing', N'Mary Gilman', N'(208) 908-5216', N'7615 W Riverside Drive', N' ', N'Boise', N'83714', N'Idaho', N'United States', 26387)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5907, N'Shipping', N'Mary Gilman', N'(208) 908-5216', N'7615 W Riverside Drive', N' ', N'Boise', N'83714', N'Idaho', N'United States', 26387)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5908, N'Billing', N'Marissa Kirchner', N'(413) 749-8001', N'5 Cheshire Road', N' ', N'Pittsfield', N'01201', N'MA', N'United States', 26388)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5909, N'Shipping', N'Marissa Kirchner', N'(413) 749-8001', N'5 Cheshire Road', N' ', N'Pittsfield', N'01201', N'MA', N'United States', 26388)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5910, N'Billing', N'John Hendrix', N'(714) 2456313', N'800 W Santa Ana Blvd', N' ', N'Santa Ana', N'92701', N'CA', N'United States', 26389)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5911, N'Shipping', N'John Hendrix', N'(714) 2456313', N'800 W Santa Ana Blvd', N' ', N'Santa Ana', N'92701', N'CA', N'United States', 26389)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5912, N'Billing', N'Randy Swigart', N'(734) 929-9495', N'340 E Huron', N' ', N'Ann Arbor', N'48104', N'MI', N'United States', 26390)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5913, N'Shipping', N'Randy Swigart', N'(734) 929-9495', N'340 E Huron', N' ', N'Ann Arbor', N'48104', N'MI', N'United States', 26390)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5914, N'Billing', N'Rochelle Clausnitzer', N'(360) 943-7911', N'330 Union Ave SE', N' ', N'Olympia', N'98501', N'WA', N'United States', 26391)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5915, N'Shipping', N'Rochelle Clausnitzer', N'(360) 943-7911', N'330 Union Ave SE', N' ', N'Olympia', N'98501', N'WA', N'United States', 26391)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5916, N'Billing', N'Stephanie Cartwright', N'(225) 4086118', N'3232 S Sherwood Forest Blvd', N' ', N'Baton Rouge', N'70816', N'LA', N'United States', 26392)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5917, N'Shipping', N'Stephanie Cartwright', N'(225) 4086118', N'3232 S Sherwood Forest Blvd', N' ', N'Baton Rouge', N'70816', N'LA', N'United States', 26392)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5918, N'Billing', N'Teresa Hyatt', N'(360) 352-5033', N'PO Box 207', N' ', N'Olympia', N'98507', N'WA', N'United States', 26393)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5919, N'Shipping', N'Teresa Hyatt', N'(360) 352-5033', N'PO Box 207', N' ', N'Olympia', N'98507', N'WA', N'United States', 26393)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5920, N'Billing', N'Patricia Kornmeyer', N'(580) 4827965', N'2721 North Main Street', N' ', N'Altus', N'73521', N'OK', N'United States', 26394)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5921, N'Shipping', N'Patricia Kornmeyer', N'(580) 4827965', N'2721 North Main Street', N' ', N'Altus', N'73521', N'OK', N'United States', 26394)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5922, N'Billing', N'Julie Theriault', N'(210) 308-7893', N'8960 Huebner Rd', N' ', N'San Antonio', N'78240', N'TX', N'United States', 26395)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5923, N'Shipping', N'Julie Theriault', N'(210) 308-7893', N'8960 Huebner Rd', N' ', N'San Antonio', N'78240', N'TX', N'United States', 26395)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5924, N'Billing', N'Monica Caudle', N'(512) 438-4750', N'6411 N Lamar Blvd', N' ', N'Austin', N'78752-4088', N'TX', N'United States', 26396)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5925, N'Shipping', N'Monica Caudle', N'(512) 438-4750', N'6411 N Lamar Blvd', N' ', N'Austin', N'78752-4088', N'TX', N'United States', 26396)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5926, N'Billing', N'Dave West', N'(218) 7365528', N'413 Stanton Avenue', N' ', N'Fergus Falls', N'56537', N'MN', N'United States', 26397)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5927, N'Shipping', N'Dave West', N'(218) 7365528', N'413 Stanton Avenue', N' ', N'Fergus Falls', N'56537', N'MN', N'United States', 26397)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5928, N'Billing', N'Jill Garcia', N'(858) 6363496', N'9201 Spectrum Center Blvd', N' ', N'San Diego', N'92123', N'CA', N'United States', 26398)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5929, N'Shipping', N'Jill Garcia', N'(858) 6363496', N'9201 Spectrum Center Blvd', N' ', N'San Diego', N'92123', N'CA', N'United States', 26398)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5930, N'Billing', N'Melissa O''Shea', N'(217) 7185016', N'1420 S. 8th. St.', N' ', N'Springfield', N'62703', N'IL', N'United States', 26399)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5931, N'Shipping', N'Melissa O''Shea', N'(217) 7185016', N'1420 S. 8th. St.', N' ', N'Springfield', N'62703', N'IL', N'United States', 26399)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5932, N'Billing', N'Maggie  Donaldson', N'(505) 3428883', N'PO Box 129', N' ', N'Albuquerque', N'87103', N'NM', N'United States', 26400)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5933, N'Shipping', N'Maggie  Donaldson', N'(505) 3428883', N'PO Box 129', N' ', N'Albuquerque', N'87103', N'NM', N'United States', 26400)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5934, N'Billing', N'Sheryle Lobato', N'(970) 497-5378', N'2711 Commercial Way', N' ', N'Montrose', N'81401', N'CO', N'United States', 26401)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5935, N'Shipping', N'Sheryle Lobato', N'(970) 497-5378', N'2711 Commercial Way', N' ', N'Montrose', N'81401', N'CO', N'United States', 26401)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5936, N'Billing', N'Elizabeth  Burke', N'(617) 4795558', N'100 Quincy Ave', N' ', N'Quincy', N'2169', N'ma', N'United States', 26402)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5937, N'Shipping', N'Elizabeth  Burke', N'(617) 4795558', N'100 Quincy Ave', N' ', N'Quincy', N'2169', N'ma', N'United States', 26402)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5938, N'Billing', N'Elian Caban', N'(630) 276-5818', N'1151 E WARRENVILLE RD', N'NULL', N'NAPERVILLE', N'60563', N'IL', N'US', 26403)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5939, N'Shipping', N'Elian Caban', N'(630) 276-5818', N'1151 E WARRENVILLE RD', N'NULL', N'NAPERVILLE', N'60563', N'IL', N'US', 26403)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5940, N'Billing', N'Monica Becker', N'(970) 854-3109', N'101 W DENVER ST', N'NULL', N'Holyoke', N'80734', N'CO', N'US', 26404)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5941, N'Shipping', N'Monica Becker', N'(970) 854-3109', N'101 W DENVER ST', N'NULL', N'Holyoke', N'80734', N'CO', N'US', 26404)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5942, N'Billing', N'Ray Podolla', N'(954) 538-4439', N'2020 Nw 150 th ave', N'NULL', N'Pembroke Pines', N'33028', N'FL', N'US', 26405)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5943, N'Shipping', N'Ray Podolla', N'(954) 538-4439', N'2020 Nw 150 th ave', N'NULL', N'Pembroke Pines', N'33028', N'FL', N'US', 26405)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5944, N'Billing', N'David Tognarelli', N'(312) 527-2749', N'541 N Fairbanks ct Suite 120', N'NULL', N'Chicago', N'60611', N'Il', N'US', 26406)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5945, N'Shipping', N'David Tognarelli', N'(312) 527-2749', N'541 N Fairbanks ct Suite 120', N'NULL', N'Chicago', N'60611', N'Il', N'US', 26406)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5946, N'Billing', N'Steve Menjivar', N'(703) 682-9422', N'12 Herbert St', N'NULL', N'Alexandria', N'22305', N'VA', N'US', 26407)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5947, N'Shipping', N'Steve Menjivar', N'(703) 682-9422', N'12 Herbert St', N'NULL', N'Alexandria', N'22305', N'VA', N'US', 26407)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5948, N'Billing', N'Ana Foret', N'(603) 422-8370', N'3003 Lafayette Road', N'NULL', N'Portsmouth', N'3801', N'NH', N'US', 26408)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5949, N'Shipping', N'Ana Foret', N'(603) 422-8370', N'3003 Lafayette Road', N'NULL', N'Portsmouth', N'3801', N'NH', N'US', 26408)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5950, N'Billing', N'Jason Stidham', N'(831) 479-6000', N'3333 Clares St', N'NULL', N'Capitola', N'95010', N'CA', N'US', 26409)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5951, N'Shipping', N'Jason Stidham', N'(831) 479-6000', N'3333 Clares St', N'NULL', N'Capitola', N'95010', N'CA', N'US', 26409)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5952, N'Billing', N'Ashley Morris', N'(573) 329-3151', N'5000 Illinois Ave', N'NULL', N'Leonard Wood', N'65473', N'MO', N'US', 26410)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5953, N'Shipping', N'Ashley Morris', N'(573) 329-3151', N'5000 Illinois Ave', N'NULL', N'Leonard Wood', N'65473', N'MO', N'US', 26410)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5954, N'Billing', N'Pat Hanulik', N'(206) 628-4010', N'325 Eastlake Ave E', N'NULL', N'Seattle', N'98109', N'WA', N'US', 26411)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5955, N'Shipping', N'Pat Hanulik', N'(206) 628-4010', N'325 Eastlake Ave E', N'NULL', N'Seattle', N'98109', N'WA', N'US', 26411)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5956, N'Billing', N'Michelle McElwain', N'(301) 359-3081', N'204 North Branch Ave', N'NULL', N'Bloomington', N'21523', N'MD', N'US', 26412)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5957, N'Shipping', N'Michelle McElwain', N'(301) 359-3081', N'204 North Branch Ave', N'NULL', N'Bloomington', N'21523', N'MD', N'US', 26412)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5958, N'Billing', N'Kristin Brojanac', N'(414) 2778414', N'1351 N Dr. Martin L King Jr Dr', N'NULL', N'Milwaukee', N'NULL', N'Wisconsin', N'US', 26413)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5959, N'Shipping', N'Kristin Brojanac', N'(414) 2778414', N'1351 N Dr. Martin L King Jr Dr', N'NULL', N'Milwaukee', N'NULL', N'Wisconsin', N'US', 26413)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5960, N'Billing', N'Ann Boudreau', N'(207) 7832071', N'555 Sabattus Street', N'NULL', N'Lewiston', N'NULL', N'ME', N'US', 26414)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5961, N'Shipping', N'Ann Boudreau', N'(207) 7832071', N'555 Sabattus Street', N'NULL', N'Lewiston', N'NULL', N'ME', N'US', 26414)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5962, N'Billing', N'Sean Muldoon', N'(561) 9818779', N'790 Park of Commerce Blvd.', N'NULL', N'Boca Raton', N'NULL', N'FL', N'US', 26415)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5963, N'Shipping', N'Sean Muldoon', N'(561) 9818779', N'790 Park of Commerce Blvd.', N'NULL', N'Boca Raton', N'NULL', N'FL', N'US', 26415)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5964, N'Billing', N'Belinda Mumma', N'(855) 605-5664', N'1201 Fulling Mill Road ', N'NULL', N'Middletown', N'17022', N'Pa', N'US', 26416)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5965, N'Shipping', N'Belinda Mumma', N'(855) 605-5664', N'1201 Fulling Mill Road ', N'NULL', N'Middletown', N'17022', N'Pa', N'US', 26416)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5966, N'Billing', N'Helen Flowers', N'(601) 636-7523', N'1604 Cherry Street', N'NULL', N'Vicksburg', N'39180', N'MS', N'US', 26417)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5967, N'Shipping', N'Helen Flowers', N'(601) 636-7523', N'1604 Cherry Street', N'NULL', N'Vicksburg', N'39180', N'MS', N'US', 26417)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5968, N'Billing', N'Jamie Patterson', N'(586) 4981526', N'55834 Van Dyke Ave', N'NULL', N'Shelby Twp', N'48316', N'MI', N'US', 26418)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5969, N'Shipping', N'Jamie Patterson', N'(586) 4981526', N'55834 Van Dyke Ave', N'NULL', N'Shelby Twp', N'48316', N'MI', N'US', 26418)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5970, N'Billing', N'Patty Hemmingsen', N'(931) 4312187', N'2050 Lowes Dr', N'NULL', N'Clarksville', N'37043', N'Tn', N'US', 26419)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5971, N'Shipping', N'Patty Hemmingsen', N'(931) 4312187', N'2050 Lowes Dr', N'NULL', N'Clarksville', N'37043', N'Tn', N'US', 26419)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5972, N'Billing', N'Cheryl McCullough', N'(281) 856-5404', N'15260 Fm 529', N'NULL', N'Houston', N'77095', N'TX', N'US', 26420)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5973, N'Shipping', N'Cheryl McCullough', N'(281) 856-5404', N'15260 Fm 529', N'NULL', N'Houston', N'77095', N'TX', N'US', 26420)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5974, N'Billing', N'William  Fulk', N'(360) 619-3015', N'PO Box 324', N'200 SE Park Plaza Drive', N'Vancouver', N'98666', N'WA', N'US', 26421)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5975, N'Shipping', N'William  Fulk', N'(360) 619-3015', N'PO Box 324', N'200 SE Park Plaza Drive', N'Vancouver', N'98666', N'WA', N'US', 26421)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5976, N'Billing', N'Trina Shano', N'(504) 3482424', N'7701 Airline dr', N'NULL', N'Metairie', N'70003', N'La', N'US', 26422)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5977, N'Shipping', N'Trina Shano', N'(504) 3482424', N'7701 Airline dr', N'NULL', N'Metairie', N'70003', N'La', N'US', 26422)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5978, N'Billing', N'Kaye Sanders', N'(360) 578-5313', N'1418 15th Ave', N'PO Box 3020', N'Longview', N'98632', N'Washington', N'US', 26423)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5979, N'Shipping', N'Kaye Sanders', N'(360) 578-5313', N'1418 15th Ave', N'PO Box 3020', N'Longview', N'98632', N'Washington', N'US', 26423)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5980, N'Billing', N'Melinda Russell', N'(918) 362-1400', N'4620 W Kenosha', N'NULL', N'Broken Arrow', N'74012', N'Oklahoma', N'US', 26424)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5981, N'Shipping', N'Melinda Russell', N'(918) 362-1400', N'4620 W Kenosha', N'NULL', N'Broken Arrow', N'74012', N'Oklahoma', N'US', 26424)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5982, N'Billing', N'Joseph Felice', N'(313) 568-5000 Ext. 360', N'1480 E. Jefferson Ave', N'NULL', N'Detroit', N'48207', N'MI', N'US', 26425)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5983, N'Shipping', N'Joseph Felice', N'(313) 568-5000 Ext. 360', N'1480 E. Jefferson Ave', N'NULL', N'Detroit', N'48207', N'MI', N'US', 26425)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5984, N'Billing', N'David  DeBolt', N'(812) 314-0531', N'1430 National Rd', N'PO Box 789', N'Columbus', N'47202', N'IN', N'US', 26426)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5985, N'Shipping', N'David  DeBolt', N'(812) 314-0531', N'1430 National Rd', N'PO Box 789', N'Columbus', N'47202', N'IN', N'US', 26426)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5986, N'Billing', N'Julie Linch', N'(419) 841-9838', N'5121 Whiteford', N'NULL', N'Sylvania', N'43560', N'OH', N'US', 26427)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5987, N'Shipping', N'Julie Linch', N'(419) 841-9838', N'5121 Whiteford', N'NULL', N'Sylvania', N'43560', N'OH', N'US', 26427)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5988, N'Billing', N'Jennifer Walge', N'(301) 249-1800', N'500 Prince Georges Blvd', N'NULL', N'Upper Marlboro', N'20774', N'MD', N'US', 26428)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5989, N'Shipping', N'Jennifer Walge', N'(301) 249-1800', N'500 Prince Georges Blvd', N'NULL', N'Upper Marlboro', N'20774', N'MD', N'US', 26428)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5990, N'Billing', N'Terri Rhoades', N'(303) 427-5008', N'5005 W. 60th Ave.', N'NULL', N'Arvada', N'80003', N'CO', N'US', 26429)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5991, N'Shipping', N'Terri Rhoades', N'(303) 427-5008', N'5005 W. 60th Ave.', N'NULL', N'Arvada', N'80003', N'CO', N'US', 26429)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5992, N'Billing', N'Paige Williamson', N'(800) 336-0284x255', N'12 Herbert Street ', N'NULL', N'Alexandria', N'22305', N'VA', N'US', 26430)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5993, N'Shipping', N'Paige Williamson', N'(800) 336-0284x255', N'12 Herbert Street ', N'NULL', N'Alexandria', N'22305', N'VA', N'US', 26430)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5994, N'Billing', N'Nicholas Cook', N'(254) 7613504', N'6201 Sanger Ave', N'NULL', N'Waco', N'76710', N'TX', N'US', 26431)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5995, N'Shipping', N'Nicholas Cook', N'(254) 7613504', N'6201 Sanger Ave', N'NULL', N'Waco', N'76710', N'TX', N'US', 26431)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5996, N'Billing', N'Kim  Bradley', N'(707) 467-4810', N'963 11th St', N'NULL', N'Lakeport', N'95453', N'CA', N'US', 26432)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5997, N'Shipping', N'Kim  Bradley', N'(707) 467-4810', N'963 11th St', N'NULL', N'Lakeport', N'95453', N'CA', N'US', 26432)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5998, N'Billing', N'Melissa  Hagenbuch', N'(832) 604-2140', N'18540 Northwest Freeway', N'NULL', N'Houston', N'77065', N'TX', N'US', 26433)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (5999, N'Shipping', N'Melissa  Hagenbuch', N'(832) 604-2140', N'18540 Northwest Freeway', N'NULL', N'Houston', N'77065', N'TX', N'US', 26433)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6000, N'Billing', N'Martha Pellegrin', N'(601) 984-1315', N'POBox 55889', N'NULL', N'Jackson', N'39296', N'MS', N'US', 26434)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6001, N'Shipping', N'Martha Pellegrin', N'(601) 984-1315', N'POBox 55889', N'NULL', N'Jackson', N'39296', N'MS', N'US', 26434)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6002, N'Billing', N'Lisa Krogstad', N'(480) 786-2463', N'25 S Arizona Pl', N'Suite 111', N'Chandler', N'85225', N'AZ', N'US', 26435)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6003, N'Shipping', N'Lisa Krogstad', N'(480) 786-2463', N'25 S Arizona Pl', N'Suite 111', N'Chandler', N'85225', N'AZ', N'US', 26435)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6004, N'Billing', N'Nicole Aiello', N'(973) 798-2008', N'536 Washington Ave', N'NULL', N'Nutley', N'7110', N'NJ', N'US', 26436)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6005, N'Shipping', N'Nicole Aiello', N'(973) 798-2008', N'536 Washington Ave', N'NULL', N'Nutley', N'7110', N'NJ', N'US', 26436)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6006, N'Billing', N'Tony  Diaz', N'(714) 466-8220', N'15442 Newport Ave.', N'NULL', N'Tustin', N'92780', N'CA', N'US', 26437)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6007, N'Shipping', N'Tony  Diaz', N'(714) 466-8220', N'15442 Newport Ave.', N'NULL', N'Tustin', N'92780', N'CA', N'US', 26437)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6008, N'Billing', N'Susan Warkentin', N'(559) 737-5900', N'PO Box 5011', N'NULL', N'Visalia', N'93278', N'CA', N'US', 26438)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6009, N'Shipping', N'Susan Warkentin', N'(559) 737-5900', N'PO Box 5011', N'NULL', N'Visalia', N'93278', N'CA', N'US', 26438)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6010, N'Billing', N'Reiley Schoen', N'(607) 216-3414', N'125 North Fulton Street', N'NULL', N'Ithaca', N'14850', N'New York', N'US', 26439)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6011, N'Shipping', N'Reiley Schoen', N'(607) 216-3414', N'125 North Fulton Street', N'NULL', N'Ithaca', N'14850', N'New York', N'US', 26439)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6012, N'Billing', N'Jaci Williams', N'(858) 5978690', N'6545 Sequence Dr.', N'NULL', N'San Diego', N'92121', N'CA', N'US', 26440)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6013, N'Shipping', N'Jaci Williams', N'(858) 5978690', N'6545 Sequence Dr.', N'NULL', N'San Diego', N'92121', N'CA', N'US', 26440)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6014, N'Billing', N'Hannah Forror', N'(440) 353-9650', N'39287 Center Ridge Rd ', N'NULL', N'N. Ridgeville', N'44039', N'OH', N'US', 26441)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6015, N'Shipping', N'Hannah Forror', N'(440) 353-9650', N'39287 Center Ridge Rd ', N'NULL', N'N. Ridgeville', N'44039', N'OH', N'US', 26441)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6016, N'Billing', N'Denise  Plourde', N'(210) 308-7806', N'2023 Gold Canyon Dr', N'NULL', N'San Antonio', N'78232', N'TX', N'US', 26442)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6017, N'Shipping', N'Denise  Plourde', N'(210) 308-7806', N'2023 Gold Canyon Dr', N'NULL', N'San Antonio', N'78232', N'TX', N'US', 26442)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6018, N'Billing', N'Kimberly Quinn', N'(508) 765-5454', N'205 Main Street', N'PO Box F', N'Southbridge', N'NULL', N'MA', N'US', 26443)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6019, N'Shipping', N'Kimberly Quinn', N'(508) 765-5454', N'205 Main Street', N'PO Box F', N'Southbridge', N'NULL', N'MA', N'US', 26443)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6020, N'Billing', N'Beth Bertonassi', N'(508) 490-6762', N'293 Boston Post Road', N'NULL', N'Marlborough', N'NULL', N'MA', N'US', 26444)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6021, N'Shipping', N'Beth Bertonassi', N'(508) 490-6762', N'293 Boston Post Road', N'NULL', N'Marlborough', N'NULL', N'MA', N'US', 26444)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6022, N'Billing', N'Paulette Roach', N'(254) 690-2274', N'3305 E. Elms Rd', N'NULL', N'Killeen', N'76542', N'TX', N'US', 26445)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6023, N'Shipping', N'Paulette Roach', N'(254) 690-2274', N'3305 E. Elms Rd', N'NULL', N'Killeen', N'76542', N'TX', N'US', 26445)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6024, N'Billing', N'L Horn', N'(757) 410-2022', N'755 N Battlefield Blvd', N'NULL', N'Chesapeake', N'NULL', N'VA', N'US', 26446)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6025, N'Shipping', N'L Horn', N'(757) 410-2022', N'755 N Battlefield Blvd', N'NULL', N'Chesapeake', N'NULL', N'VA', N'US', 26446)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6026, N'Billing', N'Paul Liteplo', N'(212) 4734278', N'108 2nd Avenue', N'NULL', N'New York', N'NULL', N'NY', N'US', 26447)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6027, N'Shipping', N'Paul Liteplo', N'(212) 4734278', N'108 2nd Avenue', N'NULL', N'New York', N'NULL', N'NY', N'US', 26447)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6028, N'Billing', N'peg lucci', N'(802) 776-2178', N'30 Allen St', N'NULL', N'Rutland', N'5701', N'VT', N'US', 26448)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6029, N'Shipping', N'peg lucci', N'(802) 776-2178', N'30 Allen St', N'NULL', N'Rutland', N'5701', N'VT', N'US', 26448)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6030, N'Billing', N'Lisa  Moutsoulas', N'(781) 2440518', N'1 andrew st', N'NULL', N'lynn', N'NULL', N'ma', N'US', 26449)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6031, N'Shipping', N'Lisa  Moutsoulas', N'(781) 2440518', N'1 andrew st', N'NULL', N'lynn', N'NULL', N'ma', N'US', 26449)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6032, N'Billing', N'Mike  Losneck', N'(216) 9202000', N'333 Babbitt Road', N'NULL', N'Euclid', N'44123', N'OH', N'US', 26450)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6033, N'Shipping', N'Mike  Losneck', N'(216) 9202000', N'333 Babbitt Road', N'NULL', N'Euclid', N'44123', N'OH', N'US', 26450)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6034, N'Billing', N'Tracy Davis', N'(765) 2814203', N'3620 W Bethel Ave', N'NULL', N'Muncie', N'47304', N'IN', N'US', 26451)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6035, N'Shipping', N'Tracy Davis', N'(765) 2814203', N'3620 W Bethel Ave', N'NULL', N'Muncie', N'47304', N'IN', N'US', 26451)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6036, N'Billing', N'Stephenie Newcom', N'(423) 7520232', N'1020 Riverfront Pkwy', N'NULL', N'Chattanooga', N'37402', N'tn', N'US', 26452)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6037, N'Shipping', N'Stephenie Newcom', N'(423) 7520232', N'1020 Riverfront Pkwy', N'NULL', N'Chattanooga', N'37402', N'tn', N'US', 26452)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6038, N'Billing', N'Tillie Broadstone', N'(310) 618-9111', N'2377 Crenshaw Blvd #150', N'NULL', N'Torrance', N'90501', N'CA', N'US', 26453)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6039, N'Shipping', N'Tillie Broadstone', N'(310) 618-9111', N'2377 Crenshaw Blvd 150', N'NULL', N'Torrance', N'90501', N'CA', N'US', 26453)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6040, N'Billing', N'Karen Stanton', N'(562) 9043061', N'7800 E Imperial Hwy', N'NULL', N'Downey', N'90241', N'Ca', N'US', 26454)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6041, N'Shipping', N'Karen Stanton', N'(562) 9043061', N'7800 E Imperial Hwy', N'NULL', N'Downey', N'90241', N'Ca', N'US', 26454)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6042, N'Billing', N'Sue Pasterz', N'(715) 536-8351', N'300 E Second Street', N'NULL', N'Merrill', N'54452', N'WI', N'US', 26455)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6043, N'Shipping', N'Sue Pasterz', N'(715) 536-8351', N'300 E Second Street', N'NULL', N'Merrill', N'54452', N'WI', N'US', 26455)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6044, N'Billing', N'Heather Resor', N'(856) 9860664', N'501 Walnut Meadow Rd', N'NULL', N'Berea', N'40403', N'Kentucky', N'US', 26456)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6045, N'Shipping', N'Heather Resor', N'(856) 9860664', N'501 Walnut Meadow Rd', N'NULL', N'Berea', N'40403', N'Kentucky', N'US', 26456)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6046, N'Billing', N'Jane King', N'(802) 865-2003', N'25 Winooski Falls Way, Suite 203', N'PO Box 67', N'Winooski', N'NULL', N'vt', N'US', 26457)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6047, N'Shipping', N'Jane King', N'(802) 865-2003', N'25 Winooski Falls Way, Suite 203', N'PO Box 67', N'Winooski', N'NULL', N'vt', N'US', 26457)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6048, N'Billing', N'Angie Zuniga', N'(708) 891-7814', N'1600 Huntington Dr ', N'NULL', N'Calumet City', N'60409', N'IL', N'US', 26458)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6049, N'Shipping', N'Angie Zuniga', N'(708) 891-7814', N'1600 Huntington Dr ', N'NULL', N'Calumet City', N'60409', N'IL', N'US', 26458)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6050, N'Billing', N'Kathy Lorello', N'(724)-652-8393', N'2209 West State Street', N'NULL', N'New Castle', N'16101', N'PA', N'US', 26459)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6051, N'Shipping', N'Kathy Lorello', N'(724)-652-8393', N'2209 West State Street', N'NULL', N'New Castle', N'16101', N'PA', N'US', 26459)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6052, N'Billing', N'Berta Varao', N'(508) 994-6546', N'1150 Purchase Street', N'NULL', N'New Bedford', N'NULL', N'MA', N'US', 26460)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6053, N'Shipping', N'Berta Varao', N'(508) 994-6546', N'1150 Purchase Street', N'NULL', N'New Bedford', N'NULL', N'MA', N'US', 26460)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6054, N'Billing', N'Lynn  Reiter', N'(702) 939-3017', N'2625 N Tenaya ', N'NULL', N'Las Vegas', N'89128', N'NV', N'US', 26461)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6055, N'Shipping', N'Lynn  Reiter', N'(702) 939-3017', N'2625 N Tenaya ', N'NULL', N'Las Vegas', N'89128', N'NV', N'US', 26461)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6056, N'Billing', N'Michelle  Johnson ', N'(406) 653-2880', N'PO Box 426', N'217 3rd Ave South ', N'Wolf Point ', N'59201', N'MT ', N'US', 26462)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6057, N'Shipping', N'Michelle  Johnson ', N'(406) 653-2880', N'PO Box 426', N'217 3rd Ave South ', N'Wolf Point ', N'59201', N'MT ', N'US', 26462)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6058, N'Billing', N'Cheryl Norbury', N'(952) 736-5231', N'1400 RIVERWOOD DR', N'NULL', N'BURNSVILLE', N'55337', N'MN', N'US', 26463)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6059, N'Shipping', N'Cheryl Norbury', N'(952) 736-5231', N'1400 RIVERWOOD DR', N'NULL', N'BURNSVILLE', N'55337', N'MN', N'US', 26463)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6060, N'Billing', N'Roberta Rodgers', N'(256) 837-6110', N'220 Wynn Drive', N'NULL', N'Huntsville', N'35893', N'AL', N'US', 26464)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6061, N'Shipping', N'Roberta Rodgers', N'(256) 837-6110', N'220 Wynn Drive', N'NULL', N'Huntsville', N'35893', N'AL', N'US', 26464)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6062, N'Billing', N'Becky Gardner', N'(206) 812-5268', N'12770 Gateway Drive M/S:1017-1', N'NULL', N'Tukwila', N'NULL', N'WA', N'US', 26465)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6063, N'Shipping', N'Becky Gardner', N'(206) 812-5268', N'12770 Gateway Drive M/S:1017-1', N'NULL', N'Tukwila', N'NULL', N'WA', N'US', 26465)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6064, N'Billing', N'heather stevens', N'(307) 8754989', N'520 Wilkes dr suite 17', N'NULL', N'green river', N'82935', N'wy', N'US', 26466)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6065, N'Shipping', N'heather stevens', N'(307) 8754989', N'520 Wilkes dr suite 17', N'NULL', N'green river', N'82935', N'wy', N'US', 26466)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6066, N'Billing', N'Stephanie Vinson', N'(208) 6561000', N'1087 Erikson Dr', N'NULL', N'Rexburg', N'83440', N'ID', N'US', 26467)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6067, N'Shipping', N'Stephanie Vinson', N'(208) 6561000', N'1087 Erikson Dr', N'NULL', N'Rexburg', N'83440', N'ID', N'US', 26467)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6068, N'Billing', N'Larry Schmitz ', N'(316) 684-4101', N'1144 S Clifton', N'NULL', N'Wichita', N'67218', N'KS', N'US', 26468)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6069, N'Shipping', N'Larry Schmitz ', N'(316) 684-4101', N'1144 S Clifton', N'NULL', N'Wichita', N'67218', N'KS', N'US', 26468)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6070, N'Billing', N'joanna salguero', N'(571) 272-0350', N'501 dulany st ', N'NULL', N'alexandria', N'22314', N'va', N'US', 26469)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6071, N'Shipping', N'joanna salguero', N'(571) 272-0350', N'501 dulany st ', N'NULL', N'alexandria', N'22314', N'va', N'US', 26469)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6072, N'Billing', N'Stephanie Finck', N'(701) 253-6168', N'310 10th St SE', N'PO Box 2180', N'Jamestown', N'58401', N'ND', N'US', 26470)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6073, N'Shipping', N'Stephanie Finck', N'(701) 253-6168', N'310 10th St SE', N'PO Box 2180', N'Jamestown', N'58401', N'ND', N'US', 26470)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6074, N'Billing', N'Sue Beaubien', N'(603) 645-8108', N'425 Hooksett Road', N'NULL', N'Manchester', N'NULL', N'NH', N'US', 26471)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6075, N'Shipping', N'Sue Beaubien', N'(603) 645-8108', N'425 Hooksett Road', N'NULL', N'Manchester', N'NULL', N'NH', N'US', 26471)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6076, N'Billing', N'Karen Wiens', N'(216) 344-3960', N'2828 Euclid Ave.', N'Suite 300', N'Cleveland', N'44115', N'OH', N'US', 26472)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6077, N'Shipping', N'Karen Wiens', N'(216) 344-3960', N'2828 Euclid Ave.', N'Suite 300', N'Cleveland', N'44115', N'OH', N'US', 26472)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6908, N'Billing', N'Stacy Klingensmith', N'(561) 6252384', N'2801 PGA Blvd.', N'Ste 120', N'Palm Beach Gardens ', N'33410', N'Florida', N'US', 26473)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6909, N'Shipping', N'Stacy Klingensmith', N'(561) 6252384', N'2801 PGA Blvd.', N'Ste 120', N'Palm Beach Gardens ', N'33410', N'Florida', N'US', 26473)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6910, N'Billing', N'Julie Rachubinski', N'(920) 830-7200', N'PO Box 1487', N'NULL', N'Appleton', N'NULL', N'WI', N'US', 26474)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6911, N'Shipping', N'Julie Rachubinski', N'(920) 830-7200', N'PO Box 1487', N'NULL', N'Appleton', N'NULL', N'WI', N'US', 26474)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6912, N'Billing', N'JESSICA CAPWELL', N'(866) 862-9328', N'315 FRANKLIN AVE', N'NULL', N'SCRANTON', N'18503', N'PA', N'US', 26475)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6913, N'Shipping', N'JESSICA CAPWELL', N'(866) 862-9328', N'315 FRANKLIN AVE', N'NULL', N'SCRANTON', N'18503', N'PA', N'US', 26475)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6914, N'Billing', N'Jenna Nelson', N'(866) 8629328', N'315 Franklin Ave', N'NULL', N'Scranton', N'18503', N'Pennsylvania', N'US', 26476)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6915, N'Shipping', N'Jenna Nelson', N'(866) 8629328', N'315 Franklin Ave', N'NULL', N'Scranton', N'18503', N'Pennsylvania', N'US', 26476)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6916, N'Billing', N'Carol Hoem', N'(847) 214-2000', N'2075 Big Timber Road', N'NULL', N'Elgin', N'60123', N'IL', N'US', 26477)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6917, N'Shipping', N'Carol Hoem', N'(847) 214-2000', N'2075 Big Timber Road', N'NULL', N'Elgin', N'60123', N'IL', N'US', 26477)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6918, N'Billing', N'Sandy Labreck', N'(207) 377-2124', N'94 Highland Ave. ', N'P.O. Box 55', N'Winthrop ', N'NULL', N'ME', N'US', 26478)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6919, N'Shipping', N'Sandy Labreck', N'(207) 377-2124', N'94 Highland Ave. ', N'P.O. Box 55', N'Winthrop ', N'NULL', N'ME', N'US', 26478)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6920, N'Billing', N'Cindy Kallenberger', N'(831) 7898031', N'20 West Market Street', N'NULL', N'Salinas', N'93901', N'CA', N'US', 26479)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6921, N'Shipping', N'Cindy Kallenberger', N'(831) 7898031', N'20 West Market Street', N'NULL', N'Salinas', N'93901', N'CA', N'US', 26479)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6922, N'Billing', N'Karen Dunbeck', N'(760) 631-8700', N'1278 Rocky Point Drive', N'NULL', N'Oceanside', N'NULL', N'CA', N'US', 26480)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6923, N'Shipping', N'Karen Dunbeck', N'(760) 631-8700', N'1278 Rocky Point Drive', N'NULL', N'Oceanside', N'NULL', N'CA', N'US', 26480)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6924, N'Billing', N'Phyllis Delvin', N'(509) 942-6131', N'PO Box 500', N'NULL', N'Richland', N'99352', N'WA', N'US', 26481)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6925, N'Shipping', N'Phyllis Delvin', N'(509) 942-6131', N'PO Box 500', N'NULL', N'Richland', N'99352', N'WA', N'US', 26481)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6926, N'Billing', N'Charlie  Hissong', N'(206) 628-5805', N'PO Box 576', N'NULL', N'Seattle', N'NULL', N'WA', N'US', 26482)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6927, N'Shipping', N'Charlie  Hissong', N'(206) 628-5805', N'PO Box 576', N'NULL', N'Seattle', N'NULL', N'WA', N'US', 26482)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6928, N'Billing', N'Cheryl Doss', N'(434) 793-1278', N'539 Arnett Blvd', N'NULL', N'Danville', N'24540', N'VA', N'US', 26483)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6929, N'Shipping', N'Cheryl Doss', N'(434) 793-1278', N'539 Arnett Blvd', N'NULL', N'Danville', N'24540', N'VA', N'US', 26483)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6930, N'Billing', N'Lidiya Seifu', N'(571) 272-0350', N'501 Dulany St. 1st Floor', N'NULL', N'Alexandria', N'22314', N'VA', N'US', 26484)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6931, N'Shipping', N'Lidiya Seifu', N'(571) 272-0350', N'501 Dulany St. 1st Floor', N'NULL', N'Alexandria', N'22314', N'VA', N'US', 26484)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6932, N'Billing', N'Jonathan Treesh', N'(850) 434-2211', N'64 S. Reus Street', N'NULL', N'Pensacola', N'32502', N'FL', N'US', 26485)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6933, N'Shipping', N'Jonathan Treesh', N'(850) 434-2211', N'64 S. Reus Street', N'NULL', N'Pensacola', N'32502', N'FL', N'US', 26485)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6934, N'Billing', N'Lisa Kraut', N'(209) 5723600', N'3600 Coffee Rd', N'NULL', N'Modesto', N'95355', N'CA', N'US', 26486)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6935, N'Shipping', N'Lisa Kraut', N'(209) 5723600', N'3600 Coffee Rd', N'NULL', N'Modesto', N'95355', N'CA', N'US', 26486)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6936, N'Billing', N'Cheryl  Simmons', N'(623) 776-4715', N'18301 N 79th Ave Bldg A100', N'NULL', N'Glendale', N'85308', N'AZ', N'US', 26487)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6937, N'Shipping', N'Cheryl  Simmons', N'(623) 776-4715', N'18301 N 79th Ave Bldg A100', N'NULL', N'Glendale', N'85308', N'AZ', N'US', 26487)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6938, N'Billing', N'Brent Richins', N'(206) 643-3961', N'PO Box 780', N'NULL', N'Seattle ', N'98111', N'WA', N'US', 26488)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6939, N'Shipping', N'Brent Richins', N'(206) 643-3961', N'PO Box 780', N'NULL', N'Seattle ', N'98111', N'WA', N'US', 26488)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6940, N'Billing', N'Donna Richardson', N'(978) 4525001', N'1 Tremont Pl.', N'NULL', N'Lowell', N'NULL', N'MA', N'US', 26489)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6941, N'Shipping', N'Donna Richardson', N'(978) 4525001', N'1 Tremont Pl.', N'NULL', N'Lowell', N'NULL', N'MA', N'US', 26489)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6942, N'Billing', N'Laura  Morris', N'(712) 546-7676', N'P.O. Box 1030', N'1411 Industrial Road S.W.', N'LeMars', N'51031', N'IA', N'US', 26490)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6943, N'Shipping', N'Laura  Morris', N'(712) 546-7676', N'P.O. Box 1030', N'1411 Industrial Road S.W.', N'LeMars', N'51031', N'IA', N'US', 26490)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6944, N'Billing', N'Myra Cortes', N'(310) 643-2239', N'2100 Park Place', N'NULL', N'El Segundo', N'90245', N'Ca', N'US', 26491)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6945, N'Shipping', N'Myra Cortes', N'(310) 643-2239', N'2100 Park Place', N'NULL', N'El Segundo', N'90245', N'Ca', N'US', 26491)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6946, N'Billing', N'BORIS HUANCA', N'(310) 432-2358', N'1901 AVENUE OF THE STARS SUITE 120', N'NULL', N'LOS ANGELES', N'90067', N'CA', N'US', 26492)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6947, N'Shipping', N'BORIS HUANCA', N'(310) 432-2358', N'1901 AVENUE OF THE STARS SUITE 120', N'NULL', N'LOS ANGELES', N'90067', N'CA', N'US', 26492)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6948, N'Billing', N'Bethany Moore', N'(703) 809-8900', N'200 Spring St', N'NULL', N'Herndon', N'20170', N'VA', N'US', 26493)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6949, N'Shipping', N'Bethany Moore', N'(703) 809-8900', N'200 Spring St', N'NULL', N'Herndon', N'20170', N'VA', N'US', 26493)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6950, N'Billing', N'Mary  Forslund', N'(806) 353-9999', N'6401 S Bell', N'NULL', N'Amarillo', N'79109', N'TX', N'US', 26494)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6951, N'Shipping', N'Mary  Forslund', N'(806) 353-9999', N'6401 S Bell', N'NULL', N'Amarillo', N'79109', N'TX', N'US', 26494)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6952, N'Billing', N'Nancy  Burbank', N'(508) 363-9339', N'123 Summer Street Suite 510', N'NULL', N'Worcester', N'1608', N'MA', N'US', 26495)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6953, N'Shipping', N'Nancy  Burbank', N'(508) 363-9339', N'123 Summer Street Suite 510', N'NULL', N'Worcester', N'1608', N'MA', N'US', 26495)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6954, N'Billing', N'Catherine  Ofalla-Hernando', N'(408) 441-4763', N'2010 N. First St.', N'NULL', N'San Jose', N'95148', N'CA', N'US', 26496)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6955, N'Shipping', N'Catherine  Ofalla-Hernando', N'(408) 441-4763', N'2010 N. First St.', N'NULL', N'San Jose', N'95148', N'CA', N'US', 26496)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6956, N'Billing', N'Jennifer Pincince', N'(401) 769-2245', N'10 Monument Square', N'NULL', N'Woonsocket', N'NULL', N'RI', N'US', 26497)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6957, N'Shipping', N'Jennifer Pincince', N'(401) 769-2245', N'10 Monument Square', N'NULL', N'Woonsocket', N'NULL', N'RI', N'US', 26497)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6958, N'Billing', N'Joanne Lowe', N'(781) 878-0232', N'241 Union St.', N'NULL', N'Rockland', N'2370', N'Ma', N'US', 26498)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6959, N'Shipping', N'Joanne Lowe', N'(781) 878-0232', N'241 Union St.', N'NULL', N'Rockland', N'2370', N'Ma', N'US', 26498)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6960, N'Billing', N'Karen  Macedo', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26499)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6961, N'Shipping', N'Karen  Macedo', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26499)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6962, N'Billing', N'Nick  Estrella', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26500)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6963, N'Shipping', N'Nick  Estrella', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26500)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6964, N'Billing', N'Jim  Walseman', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26501)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6965, N'Shipping', N'Jim  Walseman', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26501)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6966, N'Billing', N'Tracy Rud', N'(800) 634-6632', N'19775 State Route 739', N'NULL', N'Marysvilloe', N'43040', N'OH', N'US', 26502)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6967, N'Shipping', N'Tracy Rud', N'(800) 634-6632', N'19775 State Route 739', N'NULL', N'Marysvilloe', N'43040', N'OH', N'US', 26502)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6968, N'Billing', N'Leanna Carney', N'(228) 272-1125', N'6006 Hwy 63', N'NULL', N'Moss Point', N'39563', N'MS', N'US', 26503)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6969, N'Shipping', N'Leanna Carney', N'(228) 272-1125', N'6006 Hwy 63', N'NULL', N'Moss Point', N'39563', N'MS', N'US', 26503)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6970, N'Billing', N'Cindy Wanamaker', N'(610) 325-5070', N'1974 Sproul Road, Ste 300', N'NULL', N'Broomall', N'19008', N'PA', N'US', 26504)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6971, N'Shipping', N'Cindy Wanamaker', N'(610) 325-5070', N'1974 Sproul Road, Ste 300', N'NULL', N'Broomall', N'19008', N'PA', N'US', 26504)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6972, N'Billing', N'Stacy Jurado', N'(800) 835-3400', N'520 N Taylor Ave', N'NULL', N'Montebello', N'90640', N'CA', N'US', 26505)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6973, N'Shipping', N'Stacy Jurado', N'(800) 835-3400', N'520 N Taylor Ave', N'NULL', N'Montebello', N'90640', N'CA', N'US', 26505)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6974, N'Billing', N'Frank Arevalo', N'(800) 835-3400', N'11417 South Street Unit C2', N'NULL', N'Cerritos', N'90703', N'CA', N'US', 26506)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6975, N'Shipping', N'Frank Arevalo', N'(800) 835-3400', N'11417 South Street Unit C2', N'NULL', N'Cerritos', N'90703', N'CA', N'US', 26506)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6976, N'Billing', N'Marsha Needham', N'(713) 244-1170', N'10100 Richmond Ave ', N'NULL', N'Houston', N'77042', N'TX', N'US', 26507)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6977, N'Shipping', N'Marsha Needham', N'(713) 244-1170', N'10100 Richmond Ave ', N'NULL', N'Houston', N'77042', N'TX', N'US', 26507)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6978, N'Billing', N'Wendy Gillies', N'(509) 483-3221', N'6103 N Astor St', N'NULL', N'Spokane', N'99208', N'WA', N'US', 26508)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6979, N'Shipping', N'Wendy Gillies', N'(509) 483-3221', N'6103 N Astor St', N'NULL', N'Spokane', N'99208', N'WA', N'US', 26508)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6980, N'Billing', N'Amy Corbin', N'(515) 282-1611', N'800 9th St', N'NULL', N'Des Moines', N'50309', N'IA', N'US', 26509)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6981, N'Shipping', N'Amy Corbin', N'(515) 282-1611', N'800 9th St', N'NULL', N'Des Moines', N'50309', N'IA', N'US', 26509)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6982, N'Billing', N'Melissa Gattenby', N'(316) 265-3272', N'711 West Douglas', N'NULL', N'Wichita', N'67213', N'KS', N'US', 26510)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6983, N'Shipping', N'Melissa Gattenby', N'(316) 265-3272', N'711 West Douglas', N'NULL', N'Wichita', N'67213', N'KS', N'US', 26510)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6984, N'Billing', N'Romal  Holder', N'(706) 563-5002', N'6333 Whitesville Rd', N'NULL', N'Columbus', N'31904', N'GA', N'US', 26511)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6985, N'Shipping', N'Romal  Holder', N'(706) 563-5002', N'6333 Whitesville Rd', N'NULL', N'Columbus', N'31904', N'GA', N'US', 26511)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6986, N'Billing', N'Angela Fulgieri', N'(516) 804-4040', N'824 HICKSVILLE RD', N'NULL', N'MASSAPEQUA', N'117588', N'NY', N'US', 26512)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6987, N'Shipping', N'Angela Fulgieri', N'(516) 804-4040', N'824 HICKSVILLE RD', N'NULL', N'MASSAPEQUA', N'117588', N'NY', N'US', 26512)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6988, N'Billing', N'David Kissling', N'(818) 242-8640', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26513)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6989, N'Shipping', N'David Kissling', N'(818) 242-8640', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26513)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6990, N'Billing', N'Anita Duvall', N'(918) 2451301', N'202 E. Morrow Rd.', N'NULL', N'Sand Springs', N'74063', N'OK', N'US', 26514)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6991, N'Shipping', N'Anita Duvall', N'(918) 2451301', N'202 E. Morrow Rd.', N'NULL', N'Sand Springs', N'74063', N'OK', N'US', 26514)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6992, N'Billing', N'Chrissy Lemke', N'(845) 452-7323', N'1 Commerce St', N'NULL', N'Poughkeepsie', N'12603', N'NY', N'US', 26515)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6993, N'Shipping', N'Chrissy Lemke', N'(845) 452-7323', N'1 Commerce St', N'NULL', N'Poughkeepsie', N'12603', N'NY', N'US', 26515)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6994, N'Billing', N'Torie Sylvia', N'(508) 8246466', N'14 Church Green', N'NULL', N'Taunton', N'NULL', N'MA', N'US', 26516)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6995, N'Shipping', N'Torie Sylvia', N'(508) 8246466', N'14 Church Green', N'NULL', N'Taunton', N'NULL', N'MA', N'US', 26516)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6996, N'Billing', N'Randa Parsons', N'(216) 344-3960', N'2828 Euclid Ave.', N'NULL', N'Cleveland', N'44115', N'OH', N'US', 26517)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6997, N'Shipping', N'Randa Parsons', N'(216) 344-3960', N'2828 Euclid Ave.', N'NULL', N'Cleveland', N'44115', N'OH', N'US', 26517)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6998, N'Billing', N'Kay Albritton', N'(850) 638-8376', N'1044 Hwy 90 East', N'NULL', N'Chipley', N'32428', N'FL', N'US', 26518)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (6999, N'Shipping', N'Kay Albritton', N'(850) 638-8376', N'1044 Hwy 90 East', N'NULL', N'Chipley', N'32428', N'FL', N'US', 26518)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7000, N'Billing', N'Carla Williams', N'(731) 664-1784', N'214 Oil Well Rd', N'NULL', N'Jackson', N'38305', N'TN', N'US', 26519)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7001, N'Shipping', N'Carla Williams', N'(731) 664-1784', N'214 Oil Well Rd', N'NULL', N'Jackson', N'38305', N'TN', N'US', 26519)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7002, N'Billing', N'Melissa Denzler', N'(508) 824-6466', N'14 Church Green', N'NULL', N'Taunton', N'2780', N'MA', N'US', 26520)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7003, N'Shipping', N'Melissa Denzler', N'(508) 824-6466', N'14 Church Green', N'NULL', N'Taunton', N'2780', N'MA', N'US', 26520)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7004, N'Billing', N'Kelli Poulos', N'(781) 878-0232', N'241 Union Street', N'NULL', N'Rockland ', N'2370', N'MA', N'US', 26521)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7005, N'Shipping', N'Kelli Poulos', N'(781) 878-0232', N'241 Union Street', N'NULL', N'Rockland ', N'2370', N'MA', N'US', 26521)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7006, N'Billing', N'Birgit Roby', N'(402) 436-4363', N'1391 S 33rd St', N'NULL', N'Lincoln', N'68510', N'NE', N'US', 26522)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7007, N'Shipping', N'Birgit Roby', N'(402) 436-4363', N'1391 S 33rd St', N'NULL', N'Lincoln', N'68510', N'NE', N'US', 26522)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7008, N'Billing', N'Jamie Costello', N'(859) 264-4243', N'2557 Sir Barton Way', N'NULL', N'Lexington', N'40509', N'KY', N'US', 26523)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7009, N'Shipping', N'Jamie Costello', N'(859) 264-4243', N'2557 Sir Barton Way', N'NULL', N'Lexington', N'40509', N'KY', N'US', 26523)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7010, N'Billing', N'Jennifer Stone', N'(919) 477-0696', N'214 Pacific Ave', N'NULL', N'Durham', N'27572', N'NC', N'US', 26524)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7011, N'Shipping', N'Jennifer Stone', N'(919) 477-0696', N'214 Pacific Ave', N'NULL', N'Durham', N'27572', N'NC', N'US', 26524)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7012, N'Billing', N'Joanna Wilbur', N'(703) 709-8900 ext 4489', N'12709 Saylers Creek lane', N'NULL', N'Herndon', N'20170', N'VA', N'US', 26525)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7013, N'Shipping', N'Joanna Wilbur', N'(703) 709-8900 ext 4489', N'12709 Saylers Creek lane', N'NULL', N'Herndon', N'20170', N'VA', N'US', 26525)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7014, N'Billing', N'Doreen Abbott', N'(615) 341-4458', N'1818 Albion Street', N'NULL', N'Nashville', N'37208', N'TN', N'US', 26526)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7015, N'Shipping', N'Doreen Abbott', N'(615) 341-4458', N'1818 Albion Street', N'NULL', N'Nashville', N'37208', N'TN', N'US', 26526)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7016, N'Billing', N'Carla Campbell', N'(301) 497-7000', N'7901 Sandy Spring Road', N'NULL', N'Laurel', N'20707', N'MD', N'US', 26527)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7017, N'Shipping', N'Carla Campbell', N'(301) 497-7000', N'7901 Sandy Spring Road', N'NULL', N'Laurel', N'20707', N'MD', N'US', 26527)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7018, N'Billing', N'Charlene Morris', N'(321) 752-2222', N'PO Box 419002', N'NULL', N'Melbouarne', N'32941', N'FL', N'US', 26528)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7019, N'Shipping', N'Charlene Morris', N'(321) 752-2222', N'PO Box 419002', N'NULL', N'Melbouarne', N'32941', N'FL', N'US', 26528)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7020, N'Billing', N'Suzan Frieze', N'951) 656-4411 ext 8906', N'23540 Cactus Ave', N'NULL', N'Moreno Valley', N'92553', N'CA', N'US', 26529)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7021, N'Shipping', N'Suzan Frieze', N'951) 656-4411 ext 8906', N'23540 Cactus Ave', N'NULL', N'Moreno Valley', N'92553', N'CA', N'US', 26529)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7022, N'Billing', N'Jennifer  Casler', N'(509) 378-3100', N'PO Box 500', N'NULL', N'Richland', N'99352', N'WA', N'US', 26530)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7023, N'Shipping', N'Jennifer  Casler', N'(509) 378-3100', N'PO Box 500', N'NULL', N'Richland', N'99352', N'WA', N'US', 26530)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7024, N'Billing', N'Michelle Hoefner', N'(510) 690-6100', N'22777 Main St', N'NULL', N'Hayward', N'94541', N'CA', N'US', 26531)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7025, N'Shipping', N'Michelle Hoefner', N'(510) 690-6100', N'22777 Main St', N'NULL', N'Hayward', N'94541', N'CA', N'US', 26531)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7026, N'Billing', N'Ben Sawyer', N'(513) 559-1234', N'7948 MASON MONTGOMERY RD', N'NULL', N'MASON', N'45040', N'OH', N'US', 26532)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7027, N'Shipping', N'Ben Sawyer', N'(513) 559-1234', N'7948 MASON MONTGOMERY RD', N'NULL', N'MASON', N'45040', N'OH', N'US', 26532)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7028, N'Billing', N'Jenny Russell', N'(800)-759-8500', N'PO Box 51770', N'NULL', N'Bowling Green', N'42102', N'KY', N'US', 26533)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7029, N'Shipping', N'Jenny Russell', N'(800)-759-8500', N'PO Box 51770', N'NULL', N'Bowling Green', N'42102', N'KY', N'US', 26533)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7030, N'Billing', N'Stacy Wallom', N'(608)-243-5000', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26534)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7031, N'Shipping', N'Stacy Wallom', N'(608)-243-5000', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26534)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7032, N'Billing', N'Denise Bernard', N'(509) 344-1319', N'613 S Washington', N'NULL', N'Spokane', N'NULL', N'WA', N'US', 26535)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7033, N'Shipping', N'Denise Bernard', N'(509) 344-1319', N'613 S Washington', N'NULL', N'Spokane', N'NULL', N'WA', N'US', 26535)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7034, N'Billing', N'Aaron  Michael', N'(740) 289-5060', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26536)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7035, N'Shipping', N'Aaron  Michael', N'(740) 289-5060', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26536)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7036, N'Billing', N'Susan Gray', N'(814) 296-2455', N'419 N 7th St', N'NULL', N'Altoona', N'16601', N'PA', N'US', 26537)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7037, N'Shipping', N'Susan Gray', N'(814) 296-2455', N'419 N 7th St', N'NULL', N'Altoona', N'16601', N'PA', N'US', 26537)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7038, N'Billing', N'Cynthia  Gaskin', N'(850 ) 942-9000 ext 1030', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26538)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7039, N'Shipping', N'Cynthia  Gaskin', N'(850 ) 942-9000 ext 1030', N'NULL', N'NULL', N'NULL', N'NULL', N'NULL', N'US', 26538)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7040, N'Billing', N'Gay Hasbrook', N'(918) 337-7730', N'P O Box 1358', N'NULL', N'Bartlesville', N'NULL', N'OK', N'US', 26539)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7041, N'Shipping', N'Gay Hasbrook', N'(918) 337-7730', N'P O Box 1358', N'NULL', N'Bartlesville', N'NULL', N'OK', N'US', 26539)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7042, N'Billing', N'Mariana  Zuniga', N'(830) 778-7502', N'P O Box 420728', N'NULL', N'Del Rio', N'78842', N'TX', N'US', 26540)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7043, N'Shipping', N'Mariana  Zuniga', N'(830) 778-7502', N'P O Box 420728', N'NULL', N'Del Rio', N'78842', N'TX', N'US', 26540)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7044, N'Billing', N'Nikki Richardson', N'(850) 942-9000', N'440 NORTH MONROE STREET', N'NULL', N'TALLAHASSEE', N'32301', N'FL', N'US', 26541)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7045, N'Shipping', N'Nikki Richardson', N'(850) 942-9000', N'440 NORTH MONROE STREET', N'NULL', N'TALLAHASSEE', N'32301', N'FL', N'US', 26541)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7046, N'Billing', N'Dee Westney', N'(386) 328-5555', N'306 S Palm Ave', N'NULL', N'Palatka', N'32177', N'FL', N'US', 26542)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7047, N'Shipping', N'Dee Westney', N'(386) 328-5555', N'306 S Palm Ave', N'NULL', N'Palatka', N'32177', N'FL', N'US', 26542)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7048, N'Billing', N'Jo Thomas', N'(414) 354-5610', N'7750 N.60th Street', N'NULL', N'Milwaukee', N'53223', N'WI', N'US', 26543)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7049, N'Shipping', N'Jo Thomas', N'(414) 354-5610', N'7750 N.60th Street', N'NULL', N'Milwaukee', N'53223', N'WI', N'US', 26543)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7050, N'Billing', N'Jane Tyo', N'(315) 764-0240', N'23 Phillips St', N'NULL', N'Massena', N'13662', N'NY', N'US', 26544)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7051, N'Shipping', N'Jane Tyo', N'(315) 764-0240', N'23 Phillips St', N'NULL', N'Massena', N'13662', N'NY', N'US', 26544)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7052, N'Billing', N'Rosanna  Rever', N'(973) 305-8889', N'One Corporate Drive', N'NULL', N'Wayne', N'7470', N'NJ', N'US', 26545)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7053, N'Shipping', N'Rosanna  Rever', N'(973) 305-8889', N'One Corporate Drive', N'NULL', N'Wayne', N'7470', N'NJ', N'US', 26545)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7054, N'Billing', N'Lyn Farley', N'(434) 793-1278', N'539 Arnett Blvd', N'NULL', N'Danville', N'24540', N'VA', N'US', 26546)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7055, N'Shipping', N'Lyn Farley', N'(434) 793-1278', N'539 Arnett Blvd', N'NULL', N'Danville', N'24540', N'VA', N'US', 26546)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7056, N'Billing', N'Jim Payne', N'(570) 826-8244', N'40 E Market Street', N'NULL', N'Wilkes-Barre', N'18711', N'PA', N'US', 26547)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7057, N'Shipping', N'Jim Payne', N'(570) 826-8244', N'40 E Market Street', N'NULL', N'Wilkes-Barre', N'18711', N'PA', N'US', 26547)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7058, N'Billing', N'Debra Nelson-Kelii', N'(808) 946-1904', N'740 Kohou St Ste A', N'NULL', N'Honolulu', N'96817', N'HI', N'US', 26548)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7059, N'Shipping', N'Debra Nelson-Kelii', N'(808) 946-1904', N'740 Kohou St Ste A', N'NULL', N'Honolulu', N'96817', N'HI', N'US', 26548)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7060, N'Billing', N'Danette Hager', N'(307) 635-7878', N'2223 Warren Ave ', N'NULL', N'Cheyenne', N'82001', N'WY', N'US', 26549)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7061, N'Shipping', N'Danette Hager', N'(307) 635-7878', N'2223 Warren Ave ', N'NULL', N'Cheyenne', N'82001', N'WY', N'US', 26549)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7062, N'Billing', N'Jill Matson', N'(513) 243-4328', N'10485 READING RD', N'NULL', N'CINCINNATI', N'45241', N'OH', N'US', 26550)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7063, N'Shipping', N'Jill Matson', N'(513) 243-4328', N'10485 READING RD', N'NULL', N'CINCINNATI', N'45241', N'OH', N'US', 26550)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7064, N'Billing', N'Daniel Morrisey', N'(703) 521-8615', N'P O Box 4509', N'NULL', N'Arlington', N'22204', N'VA', N'US', 26551)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7065, N'Shipping', N'Daniel Morrisey', N'(703) 521-8615', N'P O Box 4509', N'NULL', N'Arlington', N'22204', N'VA', N'US', 26551)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7066, N'Billing', N'Kaye Cable', N'(423) 547-1200', N'980 JASON WITTEN WAY', N'NULL', N'ELIZABETHTON', N'37643', N'TN', N'US', 26552)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7067, N'Shipping', N'Kaye Cable', N'(423) 547-1200', N'980 JASON WITTEN WAY', N'NULL', N'ELIZABETHTON', N'37643', N'TN', N'US', 26552)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7068, N'Billing', N'Jan Holt', N'(952) 736-5240', N'1400 Riverwood Drive', N'NULL', N'Burnsville', N'55337', N'MN', N'US', 26553)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7069, N'Shipping', N'Jan Holt', N'(952) 736-5240', N'1400 Riverwood Drive', N'NULL', N'Burnsville', N'55337', N'MN', N'US', 26553)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7070, N'Billing', N'Wendy Rodriguez', N'(760) 631-8700', N'1278 Rocky Point Dr', N'NULL', N'Oceanside', N'92056', N'CA', N'US', 26554)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7071, N'Shipping', N'Wendy Rodriguez', N'(760) 631-8700', N'1278 Rocky Point Dr', N'NULL', N'Oceanside', N'92056', N'CA', N'US', 26554)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7072, N'Billing', N'Priscilla  Tolemy', N'(773) 462-2054', N'11545 W. Touhy Ave', N'NULL', N'Chicago', N'60666', N'IL', N'US', 26555)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7073, N'Shipping', N'Priscilla  Tolemy', N'(773) 462-2054', N'11545 W. Touhy Ave', N'NULL', N'Chicago', N'60666', N'IL', N'US', 26555)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7074, N'Billing', N'Stacy Grube', N'(989) 463-1247', N'305 W Downie St', N'NULL', N'Alma', N'48801', N'MI', N'US', 26556)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7075, N'Shipping', N'Stacy Grube', N'(989) 463-1247', N'305 W Downie St', N'NULL', N'Alma', N'48801', N'MI', N'US', 26556)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7076, N'Billing', N'Kathy Frias', N'(763) 253-2750', N'3505 Northdale Blvd NW', N'NULL', N'Coon Rapids', N'55448', N'MN', N'US', 26557)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7077, N'Shipping', N'Kathy Frias', N'(763) 253-2750', N'3505 Northdale Blvd NW', N'NULL', N'Coon Rapids', N'55448', N'MN', N'US', 26557)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7078, N'Billing', N'Dwayne Dolly II', N'(800) 237-5626', N'785 Central Ave', N'NULL', N'New Providence ', N'7974', N'NJ', N'US', 26558)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7079, N'Shipping', N'Dwayne Dolly II', N'(800) 237-5626', N'785 Central Ave', N'NULL', N'New Providence ', N'7974', N'NJ', N'US', 26558)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7080, N'Billing', N'Nancy Barrett', N'(207) 854-6020', N'4 Davis Farm Road', N'NULL', N'Portland', N'4103', N'ME', N'US', 26559)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7081, N'Shipping', N'Nancy Barrett', N'(207) 854-6020', N'4 Davis Farm Road', N'NULL', N'Portland', N'4103', N'ME', N'US', 26559)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7082, N'Billing', N'Heather  Jones', N'(765) 482-4003', N'450 S Lebanon Street', N'NULL', N'Lebanon', N'46052', N'IN', N'US', 26560)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7083, N'Shipping', N'Heather  Jones', N'(765) 482-4003', N'450 S Lebanon Street', N'NULL', N'Lebanon', N'46052', N'IN', N'US', 26560)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7084, N'Billing', N'Cynthia Coleman', N'(817) 952-4072', N'4151 Amon Carter BLVD', N'NULL', N'Fort Worth', N'76155', N'TX', N'US', 26561)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7085, N'Shipping', N'Cynthia Coleman', N'(817) 952-4072', N'4151 Amon Carter BLVD', N'NULL', N'Fort Worth', N'76155', N'TX', N'US', 26561)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7086, N'Billing', N'Robin Horlback', N'(843) 832-2600', N'200 Marymeade Drive', N'NULL', N'Summerville', N'29483', N'SC', N'US', 26562)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7087, N'Shipping', N'Robin Horlback', N'(843) 832-2600', N'200 Marymeade Drive', N'NULL', N'Summerville', N'29483', N'SC', N'US', 26562)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7088, N'Billing', N'Tasha  Erickson', N'(620) 669-0177', N'129 W Avenue A', N'NULL', N'Hutchinson', N'67502', N'KS', N'US', 26563)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7089, N'Shipping', N'Tasha  Erickson', N'(620) 669-0177', N'129 W Avenue A', N'NULL', N'Hutchinson', N'67502', N'KS', N'US', 26563)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7090, N'Billing', N'Donna Habert', N'(920) 993-3737', N' 3401 E Calumet Street', N'NULL', N'Appletpon', N'54130', N'WI', N'US', 26564)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7091, N'Shipping', N'Donna Habert', N'(920) 993-3737', N' 3401 E Calumet Street', N'NULL', N'Appletpon', N'54130', N'WI', N'US', 26564)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7092, N'Billing', N'Aileen Jennings', N'(520) 459-1860', N'3090 E Fry Blvd', N'NULL', N'Sierra Vista', N'85635', N'AZ', N'US', 26565)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7093, N'Shipping', N'Aileen Jennings', N'(520) 459-1860', N'3090 E Fry Blvd', N'NULL', N'Sierra Vista', N'85635', N'AZ', N'US', 26565)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7094, N'Billing', N'Kristie Emfinger', N'(318) 412-3146', N'6217 Hwy 15S/P O Box 110', N'NULL', N'Chase', N'71324', N'LA', N'US', 26566)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7095, N'Shipping', N'Kristie Emfinger', N'(318) 412-3146', N'6217 Hwy 15S/P O Box 110', N'NULL', N'Chase', N'71324', N'LA', N'US', 26566)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7096, N'Billing', N'Janice Henderson', N'(434) 528-9016', N'1638 Mt Athos Rd/PO Box 1660', N'NULL', N'Lynchburg', N'24505', N'VA', N'US', 26567)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7097, N'Shipping', N'Janice Henderson', N'(434) 528-9016', N'1638 Mt Athos Rd/PO Box 1660', N'NULL', N'Lynchburg', N'24505', N'VA', N'US', 26567)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7098, N'Billing', N'Christy Moore', N'(337) 477-9190', N'4056 Ryan St', N'NULL', N'Lake Charles', N'70605', N'LA', N'US', 26568)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7099, N'Shipping', N'Christy Moore', N'(337) 477-9190', N'4056 Ryan St', N'NULL', N'Lake Charles', N'70605', N'LA', N'US', 26568)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7100, N'Billing', N'Daniel Budzko', N'(541) 338-3859', N'1050 High Street Suite 100', N'NULL', N'Eugene', N'97401', N'OR', N'US', 26569)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7101, N'Shipping', N'Daniel Budzko', N'(541) 338-3859', N'1050 High Street Suite 100', N'NULL', N'Eugene', N'97401', N'OR', N'US', 26569)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7102, N'Billing', N'Bradley Mader', N'(937) 242-6754', N'7740 Paragon Road', N'NULL', N'Dayton', N'45459', N'OH', N'US', 26570)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7103, N'Shipping', N'Bradley Mader', N'(937) 242-6754', N'7740 Paragon Road', N'NULL', N'Dayton', N'45459', N'OH', N'US', 26570)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7104, N'Billing', N'Jackie Rudolph', N'(218) 878-3629', N'101 14th St', N'NULL', N'Cloquet', N'55720', N'MN', N'US', 26571)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7105, N'Shipping', N'Jackie Rudolph', N'(218) 878-3629', N'101 14th St', N'NULL', N'Cloquet', N'55720', N'MN', N'US', 26571)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7106, N'Billing', N'Jason Altshuler', N'(303) 228-4621', N'3700 E. Alameda Ave.', N'NULL', N'Denver', N'80209', N'CO', N'US', 26572)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7107, N'Shipping', N'Jason Altshuler', N'(303) 228-4621', N'3700 E. Alameda Ave.', N'NULL', N'Denver', N'80209', N'CO', N'US', 26572)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7108, N'Billing', N'Teri Reed', N'(970) 249-5319', N'1102 S CASCADE AVE', N'NULL', N'MONTROSE', N'81401', N'CO', N'US', 26573)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7109, N'Shipping', N'Teri Reed', N'(970) 249-5319', N'1102 S CASCADE AVE', N'NULL', N'MONTROSE', N'81401', N'CO', N'US', 26573)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7110, N'Billing', N'Irene Thurston', N'(406) 656-9100', N'PO Box 20417', N'NULL', N'Billings', N'59104', N'MT', N'US', 26574)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7111, N'Shipping', N'Irene Thurston', N'(406) 656-9100', N'PO Box 20417', N'NULL', N'Billings', N'59104', N'MT', N'US', 26574)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7112, N'Billing', N'Penny Wilderman', N'(541)928-4536', N'PO Box D', N'NULL', N'Albany', N'97321', N'OR', N'US', 26575)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7113, N'Shipping', N'Penny Wilderman', N'(541)928-4536', N'PO Box D', N'NULL', N'Albany', N'97321', N'OR', N'US', 26575)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7114, N'Billing', N'Brigitte Gavas', N'(302) 678-8943', N'1075 Silver Lake Blvd.', N'NULL', N'Dover', N'19904', N'DE', N'US', 26576)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7115, N'Shipping', N'Brigitte Gavas', N'(302) 678-8943', N'1075 Silver Lake Blvd.', N'NULL', N'Dover', N'19904', N'DE', N'US', 26576)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7116, N'Billing', N'Sherry Peck', N'(304) 263-3454', N'2971 Charlestown RD', N'NULL', N'Kearneysville', N'25430', N'WV', N'US', 26577)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7117, N'Shipping', N'Sherry Peck', N'(304) 263-3454', N'2971 Charlestown RD', N'NULL', N'Kearneysville', N'25430', N'WV', N'US', 26577)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7118, N'Billing', N'Judy Riddle', N'(615) 385-6866', N'1701 21st Avenue, South', N'NULL', N'Nashville', N'37212', N'TN', N'US', 26578)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7119, N'Shipping', N'Judy Riddle', N'(615) 385-6866', N'1701 21st Avenue, South', N'NULL', N'Nashville', N'37212', N'TN', N'US', 26578)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7120, N'Billing', N'Melody Johnson', N'(850) 479-9601', N'220 E Nine Mile Road', N'NULL', N'Pensacola', N'32534', N'FL', N'US', 26579)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7121, N'Shipping', N'Melody Johnson', N'(850) 479-9601', N'220 E Nine Mile Road', N'NULL', N'Pensacola', N'32534', N'FL', N'US', 26579)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7122, N'Billing', N'Krystal  Smith', N'(937) 859-6260', N'505 Earl Blvd', N'NULL', N'Miamisburg', N'45342', N'OH', N'US', 26580)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7123, N'Shipping', N'Krystal  Smith', N'(937) 859-6260', N'505 Earl Blvd', N'NULL', N'Miamisburg', N'45342', N'OH', N'US', 26580)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7124, N'Billing', N'Peggy Bartlett', N'(513) 475-6235', N'49 William Howard Taft', N'NULL', N'Cincinnati', N'45219', N'OH', N'US', 26581)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7125, N'Shipping', N'Peggy Bartlett', N'(513) 475-6235', N'49 William Howard Taft', N'NULL', N'Cincinnati', N'45219', N'OH', N'US', 26581)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7126, N'Billing', N'Lisa St. Francis', N'(720) 855-4126', N'700 W 39TH AVE', N'NULL', N'Denver', N'80216', N'CO', N'US', 26582)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7127, N'Shipping', N'Lisa St. Francis', N'(720) 855-4126', N'700 W 39TH AVE', N'NULL', N'Denver', N'80216', N'CO', N'US', 26582)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7128, N'Billing', N'Reed Block', N'(402) 552-7122', N'14509 F. Street', N'NULL', N'Omaha', N'68137', N'NE', N'US', 26583)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7129, N'Shipping', N'Reed Block', N'(402) 552-7122', N'14509 F. Street', N'NULL', N'Omaha', N'68137', N'NE', N'US', 26583)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7130, N'Billing', N'Angela Aragon', N'(307) 633-2962', N'3207 Sparks Road', N'NULL', N'Cheyenne', N'82001', N'WY', N'US', 26584)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7131, N'Shipping', N'Angela Aragon', N'(307) 633-2962', N'3207 Sparks Road', N'NULL', N'Cheyenne', N'82001', N'WY', N'US', 26584)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7132, N'Billing', N'Linda Marshall', N'(603) 422-8329', N'3003 Lafayette Road', N'NULL', N'Portsmouth', N'3801', N'NH', N'US', 26585)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7133, N'Shipping', N'Linda Marshall', N'(603) 422-8329', N'3003 Lafayette Road', N'NULL', N'Portsmouth', N'3801', N'NH', N'US', 26585)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7134, N'Billing', N'Joseph Lavalle', N'(952) 736-5367', N'1400 Riverwood Drive', N'NULL', N'Burnsville', N'55337', N'MN', N'US', 26586)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7135, N'Shipping', N'Joseph Lavalle', N'(952) 736-5367', N'1400 Riverwood Drive', N'NULL', N'Burnsville', N'55337', N'MN', N'US', 26586)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7136, N'Billing', N'Dale Phelps', N'(801) 491-3691', N'730 East 300 South', N'NULL', N'Springville', N'84663', N'UT', N'US', 26587)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7137, N'Shipping', N'Dale Phelps', N'(801) 491-3691', N'730 East 300 South', N'NULL', N'Springville', N'84663', N'UT', N'US', 26587)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7138, N'Billing', N'Charlene Jeffers', N'(570) 558-5740', N'441 N 7th Avenue', N'NULL', N'Scranton', N'18503', N'PA', N'US', 26588)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7139, N'Shipping', N'Charlene Jeffers', N'(570) 558-5740', N'441 N 7th Avenue', N'NULL', N'Scranton', N'18503', N'PA', N'US', 26588)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7140, N'Billing', N'Melissa Goins', N'574) 400-4632', N'1828 Moreau Drive', N'NULL', N'Notre Dame', N'46556', N'IN', N'US', 26589)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7141, N'Shipping', N'Melissa Goins', N'574) 400-4632', N'1828 Moreau Drive', N'NULL', N'Notre Dame', N'46556', N'IN', N'US', 26589)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7142, N'Billing', N'Emily White', N'(919) 839-8072', N'3101 Wake Forest Road', N'NULL', N'Raleigh', N'27609', N'NC', N'US', 26590)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7143, N'Shipping', N'Emily White', N'(919) 839-8072', N'3101 Wake Forest Road', N'NULL', N'Raleigh', N'27609', N'NC', N'US', 26590)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7144, N'Billing', N'Lowell  Stevens', N'(208) 882-0232', N'912 S Washington Street', N'NULL', N'Moscow', N'83843', N'ID', N'US', 26591)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (7145, N'Shipping', N'Lowell  Stevens', N'(208) 882-0232', N'912 S Washington Street', N'NULL', N'Moscow', N'83843', N'ID', N'US', 26591)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8086, N'Billing', N'Steve Hueners', N'(301) 497-7000', N'steve''s', N'NULL', N'Holmen', N'54636', N'WI', N'US', 26592)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8087, N'Shipping', N'Steve Hueners', N'(301) 497-7000', N'steve''s', N'NULL', N'Holmen', N'54636', N'WI', N'US', 26592)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8088, N'Billing', N'Edgar Corona', N'(562) 803-6401', N'12620 Erickson Ave', N'Suite H', N'Downey', N'90242', N'CA', N'United States', 26593)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8089, N'Shipping', N'Edgar Corona', N'(562) 803-6401', N'12620 Erickson Ave', N'Suite H', N'Downey', N'90242', N'CA', N'United States', 26593)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8090, N'Billing', N'Beverly Gross', N'(517) 622-6672', N'106 N Marketplace Blvd', N' ', N'Lansing', N'48917', N'MI', N'United States', 26594)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8091, N'Shipping', N'Beverly Gross', N'(517) 622-6672', N'106 N Marketplace Blvd', N' ', N'Lansing', N'48917', N'MI', N'United States', 26594)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8092, N'Billing', N'Carla  Campbell ', N'(301) 497-7025', N'7901 Sandy Spring Road ', N' ', N'Laurel ', N'20707', N'MD', N'United States', 26595)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8093, N'Shipping', N'Carla  Campbell ', N'(301) 497-7025', N'7901 Sandy Spring Road ', N' ', N'Laurel ', N'20707', N'MD', N'United States', 26595)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8094, N'Billing', N'Carol Thompson', N'(713) 674-5778', N'P.O. Box 606', N' ', N'Galena Park', N'77547', N'Tx', N'United States', 26596)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8095, N'Shipping', N'Carol Thompson', N'(713) 674-5778', N'P.O. Box 606', N' ', N'Galena Park', N'77547', N'Tx', N'United States', 26596)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8096, N'Billing', N'Edward Munoz', N'(210) 3419602', N'2023 Gold Canyon Drive', N' ', N'San Antonio ', N'78232', N'Tx', N'United States', 26597)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8097, N'Shipping', N'Edward Munoz', N'(210) 3419602', N'2023 Gold Canyon Drive', N' ', N'San Antonio ', N'78232', N'Tx', N'United States', 26597)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8098, N'Billing', N'Elaine Karins', N'(941) 953-6744', N'1558 - 1st Street', N' ', N'Sarasota', N'34236', N'Fl', N'United States', 26598)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8099, N'Shipping', N'Elaine Karins', N'(941) 953-6744', N'1558 - 1st Street', N' ', N'Sarasota', N'34236', N'Fl', N'United States', 26598)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8100, N'Billing', N'ginger haley', N'(716) 945-5340', N'417 Broad Street', N' ', N'salamanca', N'14779', N'ny', N'United States', 26599)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8101, N'Shipping', N'ginger haley', N'(716) 945-5340', N'417 Broad Street', N' ', N'salamanca', N'14779', N'ny', N'United States', 26599)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8102, N'Billing', N'Monique Malone', N'(562) 803-6401', N'12620 Erickson Ave', N'Suite H', N'Downey', N'90242', N'CA', N'United States', 26600)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8103, N'Shipping', N'Monique Malone', N'(562) 803-6401', N'12620 Erickson Ave', N'Suite H', N'Downey', N'90242', N'CA', N'United States', 26600)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8104, N'Billing', N'Minola Price', N'(734) 2423222', N'14 Winchester ', N' ', N'Monroe', N'48161', N'MI', N'United States', 26601)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8105, N'Shipping', N'Minola Price', N'(734) 2423222', N'14 Winchester ', N' ', N'Monroe', N'48161', N'MI', N'United States', 26601)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8106, N'Billing', N'Renee Schuyler', N'(763) 235-6017', N'3575 Sioux Drive', N' ', N'Medina', N'55340', N'MN', N'United States', 26602)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8107, N'Shipping', N'Renee Schuyler', N'(763) 235-6017', N'3575 Sioux Drive', N' ', N'Medina', N'55340', N'MN', N'United States', 26602)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8108, N'Billing', N'Cheryl Kirby', N'(225) 273-1529', N'1850 S Sherwood Forest Blvd', N' ', N'Baton Rouge', N'70816', N'LA', N'United States', 26603)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8109, N'Shipping', N'Cheryl Kirby', N'(225) 273-1529', N'1850 S Sherwood Forest Blvd', N' ', N'Baton Rouge', N'70816', N'LA', N'United States', 26603)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8110, N'Billing', N'Alice Hernandez', N'(915) 592-0223', N'8870 Gazelle Dr.', N' ', N'El Paso', N'79925', N'Tx', N'United States', 26604)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8111, N'Shipping', N'Alice Hernandez', N'(915) 592-0223', N'8870 Gazelle Dr.', N' ', N'El Paso', N'79925', N'Tx', N'United States', 26604)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8112, N'Billing', N'Kenya Moore', N'(720) 855-4116', N'700 W 39th Ave', N' ', N'Denver', N'80216', N'Colorado', N'United States', 26605)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8113, N'Shipping', N'Kenya Moore', N'(720) 855-4116', N'700 W 39th Ave', N' ', N'Denver', N'80216', N'Colorado', N'United States', 26605)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8114, N'Billing', N'Tamar Smallwood', N'(301) 249-1800 ', N'500 Prince Georges Blvd', N' ', N'Upper Marlboro', N'20774', N'MD', N'United States', 26606)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8115, N'Shipping', N'Tamar Smallwood', N'(301) 249-1800 ', N'500 Prince Georges Blvd', N' ', N'Upper Marlboro', N'20774', N'MD', N'United States', 26606)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8116, N'Billing', N'Janice Phillips', N'(859) 259-3466', N'440 Park Place', N' ', N'Lexington', N'40511', N'Kentucky', N'United States', 26607)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8117, N'Shipping', N'Janice Phillips', N'(859) 259-3466', N'440 Park Place', N' ', N'Lexington', N'40511', N'Kentucky', N'United States', 26607)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8118, N'Billing', N'BRENDA DEWITT', N'(715) 6358273', N'PO BOX 100 ', N'104 EAST MAPLE STREET', N'SPOONER', N'54801', N'WI', N'United States', 26608)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8119, N'Shipping', N'BRENDA DEWITT', N'(715) 6358273', N'PO BOX 100 ', N'104 EAST MAPLE STREET', N'SPOONER', N'54801', N'WI', N'United States', 26608)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8120, N'Billing', N'Shelley DeYoung', N'(231) 547-3917', N'PO Box 185', N' ', N'Charlevoix', N'49720', N'MI', N'United States', 26609)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8121, N'Shipping', N'Shelley DeYoung', N'(231) 547-3917', N'PO Box 185', N' ', N'Charlevoix', N'49720', N'MI', N'United States', 26609)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8122, N'Billing', N'Alesha Willis', N'(512) 458-2558', N'6411 North Lamar Blvd', N' ', N'Austin', N'78752', N'TX', N'United States', 26610)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8123, N'Shipping', N'Alesha Willis', N'(512) 458-2558', N'6411 North Lamar Blvd', N' ', N'Austin', N'78752', N'TX', N'United States', 26610)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8124, N'Billing', N'Darcy Hauschulz', N'(269) 3829845', N'550 S Riverview Dr', N' ', N'Parchment', N'49004', N'MI', N'United States', 26611)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (8125, N'Shipping', N'Darcy Hauschulz', N'(269) 3829845', N'550 S Riverview Dr', N' ', N'Parchment', N'49004', N'MI', N'United States', 26611)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (9108, N'Billing', N'John Hancock', N'555-555-5555', N'968 Wildcat Dr', N'', N'Del Rio', N'5000', N'Tx', N'USA', 26612)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (9109, N'Shipping', N'John Hancock', N'555-555-5555', N'968 Wildcat Dr', N'', N'Del Rio', N'5000', N'Tx', N'USA', 26612)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10108, N'Billing', N'Deb DeVous', N'(937) 4566541', N'205 North Barron St', N' ', N'Eaton', N'45320', N'OH', N'USA', 26613)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10109, N'Shipping', N'Deb DeVous', N'(937) 4566541', N'205 North Barron St', N' ', N'Eaton', N'45320', N'OH', N'USA', 26613)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10110, N'Billing', N'Julie  Carpenter', N'(504) 348-2424', N'7701 Airline Dr', N' ', N'Metairie', N'70003', N'La', N'USA', 26614)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10111, N'Shipping', N'Julie  Carpenter', N'(504) 348-2424', N'7701 Airline Dr', N' ', N'Metairie', N'70003', N'La', N'USA', 26614)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10112, N'Billing', N'Jeremy  Fusare', N'(607) 9623144', N'One Credit Union Plaza', N' ', N'Corning', N'14830', N'NY', N'USA', 26615)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10113, N'Shipping', N'Jeremy  Fusare', N'(607) 9623144', N'One Credit Union Plaza', N' ', N'Corning', N'14830', N'NY', N'USA', 26615)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10114, N'Billing', N'Susan gribben', N'(614) 396-4594', N'365 S. 4th St', N' ', N'COLUMBUS', N'43215', N'OH', N'USA', 26616)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10115, N'Shipping', N'Susan gribben', N'(614) 396-4594', N'365 S. 4th St', N' ', N'COLUMBUS', N'43215', N'OH', N'USA', 26616)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10118, N'Billing', N'Denise Burke', N'(781) 878-0232', N'241 Union St.', N' ', N'Rockland', N'02370', N'Ma', N'United States', 26618)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10119, N'Shipping', N'Denise Burke', N'(781) 878-0232', N'241 Union St.', N' ', N'Rockland', N'02370', N'Ma', N'United States', 26618)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10120, N'Billing', N'Richard Tapia', N'(505) 342-8560', N'3939 Osuna Rd NE', N' ', N'Albuquerque', N'87109', N'NM', N'United States', 26619)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10121, N'Shipping', N'Richard Tapia', N'(505) 342-8560', N'3939 Osuna Rd NE', N' ', N'Albuquerque', N'87109', N'NM', N'United States', 26619)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10122, N'Billing', N'Shannon Kitchen', N'(931) 431-2142', N'2050 Lowe''s Drive', N' ', N'Clarksville', N'37040', N'TN', N'United States', 26620)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10123, N'Shipping', N'Shannon Kitchen', N'(931) 431-2142', N'2050 Lowe''s Drive', N' ', N'Clarksville', N'37040', N'TN', N'United States', 26620)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10124, N'Billing', N'Shelly DeYoung', N'(231) 547-3917', N'PO Box 185', N'Charlevoix', N'Charlevoix', N'49720', N'MI', N'USA', 26621)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10125, N'Shipping', N'Shelly DeYoung', N'(231) 547-3917', N'PO Box 185', N'Charlevoix', N'Charlevoix', N'49720', N'MI', N'USA', 26621)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10126, N'Billing', N'D''Carla  Clark', N'(225) 819-5769', N'12529 Perkins Road', N' ', N'Baton Rouge', N'70810-1907', N'LA', N'United States', 26622)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10127, N'Shipping', N'D''Carla  Clark', N'(225) 819-5769', N'12529 Perkins Road', N' ', N'Baton Rouge', N'70810-1907', N'LA', N'United States', 26622)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10128, N'Billing', N'Corena Thompson', N'(972) 201-1733', N'1845 Woodall Rodgers Fwy', N'Suite 1300', N'Dallas ', N'75201', N'TX', N'United States', 26623)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10129, N'Shipping', N'Corena Thompson', N'(972) 201-1733', N'1845 Woodall Rodgers Fwy', N'Suite 1300', N'Dallas ', N'75201', N'TX', N'United States', 26623)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10130, N'Billing', N'Melissa Reynolds', N'(903) 223-0000', N'P.O. Box 6085', N' ', N'Texarkana', N'75505', N'TX', N'United States', 26624)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10131, N'Shipping', N'Melissa Reynolds', N'(903) 223-0000', N'P.O. Box 6085', N' ', N'Texarkana', N'75505', N'TX', N'United States', 26624)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10132, N'Billing', N'Shannon  Huot', N'(262) 902-3883', N'1400 N Newman Rd', N' ', N'Racine ', N'53215', N'WI ', N'United States', 26625)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10133, N'Shipping', N'Shannon  Huot', N'(262) 902-3883', N'1400 N Newman Rd', N' ', N'Racine ', N'53215', N'WI ', N'United States', 26625)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10134, N'Billing', N'Kelly Vogt', N'(513) 475-6205', N'49 William Howard Taft Rd', N' ', N'Cincinnati', N'45219', N'OH', N'United States', 26626)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10135, N'Shipping', N'Kelly Vogt', N'(513) 475-6205', N'49 William Howard Taft Rd', N' ', N'Cincinnati', N'45219', N'OH', N'United States', 26626)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10136, N'Billing', N'Rebecca Smith', N'(205) 320-4000', N'1200 4th Ave N', N' ', N'Birmingham', N'35203', N'Alabama', N'United States', 26627)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10137, N'Shipping', N'Rebecca Smith', N'(205) 320-4000', N'1200 4th Ave N', N' ', N'Birmingham', N'35203', N'Alabama', N'United States', 26627)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10138, N'Billing', N'Lyndee Bennett', N'(512) 467-8080', N'8303 N MoPac Expy', N' ', N'Austin', N'78759', N'Texas', N'United States', 26628)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10139, N'Shipping', N'Lyndee Bennett', N'(512) 467-8080', N'8303 N MoPac Expy', N' ', N'Austin', N'78759', N'Texas', N'United States', 26628)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10140, N'Billing', N'Cathy Alm', N'(402) 758-6503', N'11718 M Cir', N' ', N'Omaha', N'68137', N'NE', N'United States', 26629)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10141, N'Shipping', N'Cathy Alm', N'(402) 758-6503', N'11718 M Cir', N' ', N'Omaha', N'68137', N'NE', N'United States', 26629)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10142, N'Billing', N'Melody Swindall', N'(931) 4312279', N'2050 Lowe''s Dr', N' ', N'Clarksville', N'37040', N'TN', N'United States', 26630)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10143, N'Shipping', N'Melody Swindall', N'(931) 4312279', N'2050 Lowe''s Dr', N' ', N'Clarksville', N'37040', N'TN', N'United States', 26630)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10144, N'Billing', N'Ellen Hargis', N'(434) 7931278', N'539 ARNETT BLVD', N' ', N'DANVILLE', N'24540', N'VIRGINIA', N'United States', 26631)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (10145, N'Shipping', N'Ellen Hargis', N'(434) 7931278', N'539 ARNETT BLVD', N' ', N'DANVILLE', N'24540', N'VIRGINIA', N'United States', 26631)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11130, N'Billing', N'LaVada Humphrey', N'(815) 267-7747', N'1350 W Renwick Rd', N' ', N'Romeoville', N'60446', N'IL', N'United States', 26632)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11131, N'Shipping', N'LaVada Humphrey', N'(815) 267-7747', N'1350 W Renwick Rd', N' ', N'Romeoville', N'60446', N'IL', N'United States', 26632)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11132, N'Billing', N'Loretta Phillips', N'(404) 486-4309', N'1237 Clairmont Road', N' ', N'Decatur', N'30030', N'GA', N'United States', 26633)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11133, N'Shipping', N'Loretta Phillips', N'(404) 486-4309', N'1237 Clairmont Road', N' ', N'Decatur', N'30030', N'GA', N'United States', 26633)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11134, N'Billing', N'Brittney Ryba', N'(402) 5527186', N'14509 F St.', N' ', N'Omaha', N'68137', N'NE', N'United States', 26634)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11135, N'Shipping', N'Brittney Ryba', N'(402) 5527186', N'14509 F St.', N' ', N'Omaha', N'68137', N'NE', N'United States', 26634)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11136, N'Billing', N'Kimberly Reese', N'(361) 364-3683', N'501 W Merriman', N' ', N'Sinton', N'78387', N'TX', N'United States', 26635)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11137, N'Shipping', N'Kimberly Reese', N'(361) 364-3683', N'501 W Merriman', N' ', N'Sinton', N'78387', N'TX', N'United States', 26635)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11138, N'Billing', N'Amy Zelenka', N'(313) 568-1888', N'1480 East Jefferson', N' ', N'Detroit', N'48207', N'Michigan', N'United States', 26636)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11139, N'Shipping', N'Amy Zelenka', N'(313) 568-1888', N'1480 East Jefferson', N' ', N'Detroit', N'48207', N'Michigan', N'United States', 26636)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11140, N'Billing', N'Vicki ONeill', N'(315) 488-4433', N'1753 Milton Avenue', N' ', N'Solvay', N'13209', N'NY', N'United States', 26637)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11141, N'Shipping', N'Vicki ONeill', N'(315) 488-4433', N'1753 Milton Avenue', N' ', N'Solvay', N'13209', N'NY', N'United States', 26637)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11142, N'Billing', N'JENEIL TORRES', N'(405) 8135504', N'101 N WALKER', N' ', N'OKC', N'73102', N'Oklahoma', N'United States', 26638)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11143, N'Shipping', N'JENEIL TORRES', N'(405) 8135504', N'101 N WALKER', N' ', N'OKC', N'73102', N'Oklahoma', N'United States', 26638)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11144, N'Billing', N'Karen Hopp', N'(360) 4355400', N'3710 168th St NE Ste A108', N' ', N'Arlington', N'98223', N'WA', N'United States', 26639)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11145, N'Shipping', N'Karen Hopp', N'(360) 4355400', N'3710 168th St NE Ste A108', N' ', N'Arlington', N'98223', N'WA', N'United States', 26639)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11146, N'Billing', N'Vicki Loso', N'(325) 6531465', N'3505 Wildewood Dr.', N' ', N'San Angelo', N'76904', N'Texas', N'United States', 26640)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11147, N'Shipping', N'Vicki Loso', N'(325) 6531465', N'3505 Wildewood Dr.', N' ', N'San Angelo', N'76904', N'Texas', N'United States', 26640)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11148, N'Billing', N'Amanda Skinner', N'(409)963-1191', N'5140 W Parkway', N'', N'Groves', N'77619', N'Tx', N'United States', 26641)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11149, N'Shipping', N'Amanda Skinner', N'(409)963-1191', N'5140 W Parkway', N'', N'Groves', N'77619', N'Tx', N'United States', 26641)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11150, N'Billing', N'Nathan  Holaway', N'(520)322-7406', N'1160 N. Winstel Blvd. ', N'', N'Tucson ', N'85716', N'Arizona', N'United States', 26642)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11151, N'Shipping', N'Nathan  Holaway', N'(520)322-7406', N'1160 N. Winstel Blvd. ', N'', N'Tucson ', N'85716', N'Arizona', N'United States', 26642)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11152, N'Billing', N'Peggy Deel', N'(434)793-1278', N'539 Arnett Blvd', N'', N'Danville', N'24540', N'VA', N'United States', 26643)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11153, N'Shipping', N'Peggy Deel', N'(434)793-1278', N'539 Arnett Blvd', N'', N'Danville', N'24540', N'VA', N'United States', 26643)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11154, N'Billing', N'Christine  Williams', N'(443)263-4391', N'7 E. Redwood Street ', N'', N'Baltimore', N'21202', N'Maryland ', N'United States', 26644)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11155, N'Shipping', N'Christine  Williams', N'(443)263-4391', N'7 E. Redwood Street ', N'', N'Baltimore', N'21202', N'Maryland ', N'United States', 26644)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11156, N'Billing', N'Pat Gallagher', N'(631)924-8000', N'3681 Horseblock Road', N'', N'Medford', N'11763', N'NY', N'United States', 26645)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11157, N'Shipping', N'Pat Gallagher', N'(631)924-8000', N'3681 Horseblock Road', N'', N'Medford', N'11763', N'NY', N'United States', 26645)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11158, N'Billing', N'Deborah Maull', N'(302)672-1423', N'270 Beiser Blvd', N'', N'Dover', N'19904', N'DE', N'United States', 26646)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11159, N'Shipping', N'Deborah Maull', N'(302)672-1423', N'270 Beiser Blvd', N'', N'Dover', N'19904', N'DE', N'United States', 26646)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11160, N'Billing', N'Rebecca Thomas', N'(603)889-2470', N'33 Franklin Street', N'', N'Nashua', N'03064', N'NH', N'United States', 26647)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11161, N'Shipping', N'Rebecca Thomas', N'(603)889-2470', N'33 Franklin Street', N'', N'Nashua', N'03064', N'NH', N'United States', 26647)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11162, N'Billing', N'Christy Hickingbottom', N'(318)387-4592', N'1420 Natchitoches St', N'', N'West Monroe', N'71292', N'Louisiana', N'United States', 26648)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11163, N'Shipping', N'Christy Hickingbottom', N'(318)387-4592', N'1420 Natchitoches St', N'', N'West Monroe', N'71292', N'Louisiana', N'United States', 26648)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11164, N'Billing', N'Carolyn Jordan', N'(214)7489393', N'13649 Montfort Drive', N'', N'Dallas', N'75240', N'TX', N'United States', 26649)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11165, N'Shipping', N'Carolyn Jordan', N'(214)7489393', N'13649 Montfort Drive', N'', N'Dallas', N'75240', N'TX', N'United States', 26649)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11166, N'Billing', N'Leah Tate', N'(225)819-4379', N'12529 Perkins Road', N'', N'Baton Rouge', N'70810', N'LA', N'United States', 26650)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11167, N'Shipping', N'Leah Tate', N'(225)819-4379', N'12529 Perkins Road', N'', N'Baton Rouge', N'70810', N'LA', N'United States', 26650)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11168, N'Billing', N'Melody Oseguera', N'(409)749-6338', N'776 Magnolia Ave', N'', N'Port Neches ', N'77651', N'TX', N'United States', 26651)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11169, N'Shipping', N'Melody Oseguera', N'(409)749-6338', N'776 Magnolia Ave', N'', N'Port Neches ', N'77651', N'TX', N'United States', 26651)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11170, N'Billing', N'Kyle Bennett', N'224-735-2230', N'130 S We Go Trl', NULL, N'Mount Prospect', N'60056', N'IL', N'US', 26652)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11171, N'Shipping', N'Kyle Bennett', N'224-735-2230', N'130 S We Go Trl', NULL, N'Mount Prospect', N'60056', N'IL', N'US', 26652)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11172, N'Billing', N'Mark Test', N'6088495572', N'PO Box 310', NULL, N'Waunakee', N'53597', N'WI', N'US', 26653)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11173, N'Shipping', N'Mark Test', N'6088495572', N'PO Box 310', NULL, N'Waunakee', N'53597', N'WI', N'US', 26653)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11174, N'Billing', N'Kelly Jones ', N'(410)534-4500', N'2027 E. Monument St. ', N'', N'Baltimore ', N'21205', N'MD', N'United States', 26654)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11175, N'Shipping', N'Kelly Jones ', N'(410)534-4500', N'2027 E. Monument St. ', N'', N'Baltimore ', N'21205', N'MD', N'United States', 26654)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11176, N'Billing', N'Rebecca Mollenkamp', N'(313)581-2002', N'P O Box 885', N'', N'Dearborn', N'48121-0885', N'MI', N'United States', 26655)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11177, N'Shipping', N'Rebecca Mollenkamp', N'(313)581-2002', N'P O Box 885', N'', N'Dearborn', N'48121-0885', N'MI', N'United States', 26655)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11178, N'Billing', N'David Williams', N'(707) 543-2623', N'1105 No. Dutton Ave.', N' ', N'Santa Rosa', N'95401', N'CA', N'United States', 26656)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11179, N'Shipping', N'David Williams', N'(707) 543-2623', N'1105 No. Dutton Ave.', N' ', N'Santa Rosa', N'95401', N'CA', N'United States', 26656)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11180, N'Billing', N'Judi Goosk', N'(516)620-8004', N'45 Atlantic Avenue', N'', N'Oceanside', N'11572', N'NY', N'United States', 26657)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11181, N'Shipping', N'Judi Goosk', N'(516)620-8004', N'45 Atlantic Avenue', N'', N'Oceanside', N'11572', N'NY', N'United States', 26657)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11182, N'Billing', N'Susan Lezotte', N'(317)524-5083', N'225 S. East Street, Suite 300', N'', N'Indianapolis', N'46234', N'IN', N'United States', 26658)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11183, N'Shipping', N'Susan Lezotte', N'(317)524-5083', N'225 S. East Street, Suite 300', N'', N'Indianapolis', N'46234', N'IN', N'United States', 26658)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11184, N'Billing', N'Rhonda Pfeifer', N'(402)721-5368', N'249 N Park Ave', N'', N'Fremont', N'68025', N'NE', N'United States', 26659)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11185, N'Shipping', N'Rhonda Pfeifer', N'(402)721-5368', N'249 N Park Ave', N'', N'Fremont', N'68025', N'NE', N'United States', 26659)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11186, N'Billing', N'Nykole Reed', N'(404)677-4573', N'3250 Riverwood Pkwy', N'', N'Atlanta', N'30339', N'GA', N'United States', 26660)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11187, N'Shipping', N'Nykole Reed', N'(404)677-4573', N'3250 Riverwood Pkwy', N'', N'Atlanta', N'30339', N'GA', N'United States', 26660)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11188, N'Billing', N'Lin Winter', N'(920)898-4232', N'2017 Main Street', N'P.O. Box 158', N'New Holstein', N'53061', N'WI', N'United States', 26661)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11189, N'Shipping', N'Lin Winter', N'(920)898-4232', N'2017 Main Street', N'P.O. Box 158', N'New Holstein', N'53061', N'WI', N'United States', 26661)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11190, N'Billing', N'Sean Jones', N'(913)9058065', N'9777 Ridge Drive', N'', N'Lenexa', N'66219', N'KS', N'United States', 26662)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11191, N'Shipping', N'Sean Jones', N'(913)9058065', N'9777 Ridge Drive', N'', N'Lenexa', N'66219', N'KS', N'United States', 26662)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11192, N'Billing', N'Kay Soffriti', N'434-793-1278', N'539 Arnett Blvd.', NULL, N'Danville', N'24540', N'VA', N'US', 26663)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11193, N'Shipping', N'Kay Soffriti', N'434-793-1278', N'539 Arnett Blvd.', NULL, N'Danville', N'24540', N'VA', N'US', 26663)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11194, N'Billing', N'Brenda Raker', N'(570)742-3903', N'603 Center St.', N'PO Box 455', N'Milton', N'17847', N'PA', N'United States', 26664)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11195, N'Shipping', N'Brenda Raker', N'(570)742-3903', N'603 Center St.', N'PO Box 455', N'Milton', N'17847', N'PA', N'United States', 26664)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11196, N'Billing', N'Kathy Ready', N'(323)722-3400', N'520 N. Taylor Ave.', N'', N'Montebello', N'90640', N'CA', N'United States', 26665)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11197, N'Shipping', N'Kathy Ready', N'(323)722-3400', N'520 N. Taylor Ave.', N'', N'Montebello', N'90640', N'CA', N'United States', 26665)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11198, N'Billing', N'Laura  Jack', N'(480)786-2465', N'25 South Arizona Place Ste. 111', N'', N'Chandler', N'85225', N'AZ', N'United States', 26666)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11199, N'Shipping', N'Laura  Jack', N'(480)786-2465', N'25 South Arizona Place Ste. 111', N'', N'Chandler', N'85225', N'AZ', N'United States', 26666)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11200, N'Billing', N'Janet Frederick', N'(920)830-7200', N'PO Box 1487', N'', N'Appleton', N'54912-1487', N'WI', N'United States', 26667)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11201, N'Shipping', N'Janet Frederick', N'(920)830-7200', N'PO Box 1487', N'', N'Appleton', N'54912-1487', N'WI', N'United States', 26667)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11202, N'Billing', N'Sandy Griffin', N'231-726-4871', N'1051 Peck Street', NULL, N'Muskegon', N'49440', N'MI', N'US', 26668)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11203, N'Shipping', N'Sandy Griffin', N'231-726-4871', N'1051 Peck Street', NULL, N'Muskegon', N'49440', N'MI', N'US', 26668)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11204, N'Billing', N'PAULA ABSHIER', N'(979)244-3995', N'2320 6TH STREET', N'', N'BAY CITY', N'77414', N'TEXAS', N'United States', 26669)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11205, N'Shipping', N'PAULA ABSHIER', N'(979)244-3995', N'2320 6TH STREET', N'', N'BAY CITY', N'77414', N'TEXAS', N'United States', 26669)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11206, N'Billing', N'Robin Horlback', N'(843)8322642', N'200 Marymeade Drive', N'', N'Summerville', N'29483', N'SC', N'United States', 26670)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11207, N'Shipping', N'Robin Horlback', N'(843)8322642', N'200 Marymeade Drive', N'', N'Summerville', N'29483', N'SC', N'United States', 26670)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11208, N'Billing', N'Carrie Payne', N'(901)302-1523', N'3337 Summer Ave', N'', N'Memphis', N'38122', N'TN', N'United States', 26671)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11209, N'Shipping', N'Carrie Payne', N'(901)302-1523', N'3337 Summer Ave', N'', N'Memphis', N'38122', N'TN', N'United States', 26671)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11210, N'Billing', N'Zalena  Giles ', N'(301)497-7000 x 7250', N'7901 Sandy Spring Road ', N'', N'Laurel ', N'20707', N'Maryland ', N'United States', 26672)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11211, N'Shipping', N'Zalena  Giles ', N'(301)497-7000 x 7250', N'7901 Sandy Spring Road ', N'', N'Laurel ', N'20707', N'Maryland ', N'United States', 26672)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11212, N'Billing', N'Lori McClain', N'(804)323-6000', N'7500 Boulders View Dr', N'', N'Richmond', N'23225', N'VA', N'United States', 26673)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11213, N'Shipping', N'Lori McClain', N'(804)323-6000', N'7500 Boulders View Dr', N'', N'Richmond', N'23225', N'VA', N'United States', 26673)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11214, N'Billing', N'MELINDA CROSS', N'(940) 565-5423', N'3351 UNICORN LAKE BLVD', N' ', N'DENTON', N'76210', N'TX', N'United States', 26674)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11215, N'Shipping', N'MELINDA CROSS', N'(940) 565-5423', N'3351 UNICORN LAKE BLVD', N' ', N'DENTON', N'76210', N'TX', N'United States', 26674)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11216, N'Billing', N'Karen  Woodul', N'(307-)635-7878', N'4349 E. Lincolnway ', N'', N'Cheyenne', N'82001', N'WY', N'United States', 26675)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11217, N'Shipping', N'Karen  Woodul', N'(307-)635-7878', N'4349 E. Lincolnway ', N'', N'Cheyenne', N'82001', N'WY', N'United States', 26675)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11218, N'Billing', N'MICHELLE AVANT', N'(229)5611029', N'707 North Davis Street', N'', N'Nashville', N'31639', N'Georgia', N'United States', 26676)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11219, N'Shipping', N'MICHELLE AVANT', N'(229)5611029', N'707 North Davis Street', N'', N'Nashville', N'31639', N'Georgia', N'United States', 26676)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11220, N'Billing', N'Dan Hixon', N'(970)313-4756', N'1503 9th Avenue', N'', N'Greeley', N'80620', N'Colorado', N'United States', 26677)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11221, N'Shipping', N'Dan Hixon', N'(970)313-4756', N'1503 9th Avenue', N'', N'Greeley', N'80620', N'Colorado', N'United States', 26677)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11222, N'Billing', N'Carla  Craig', N'(520) 205-5780', N'971 W  Wetmore Rd', N' ', N'Tucson', N'85705-1551', N'AZ', N'United States', 26678)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11223, N'Shipping', N'Carla  Craig', N'(520) 205-5780', N'971 W  Wetmore Rd', N' ', N'Tucson', N'85705-1551', N'AZ', N'United States', 26678)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11224, N'Billing', N'Amanda Smith', N'(731) 584-7238', N'209 Hwy 641 N', N' ', N'Camden', N'38320', N'TN', N'United States', 26679)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11225, N'Shipping', N'Amanda Smith', N'(731) 584-7238', N'209 Hwy 641 N', N' ', N'Camden', N'38320', N'TN', N'United States', 26679)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11226, N'Billing', N'Carol Nobles', N'(985)7324599', N'431 Richmond Street', N'', N'Bogalusa', N'70427', N'La', N'United States', 26680)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11227, N'Shipping', N'Carol Nobles', N'(985)7324599', N'431 Richmond Street', N'', N'Bogalusa', N'70427', N'La', N'United States', 26680)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11228, N'Billing', N'Rick O''Neal', N'(256)4393131', N'110 South 26th Street', N'', N'Gadsden', N'35904', N'AL', N'United States', 26681)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11229, N'Shipping', N'Rick O''Neal', N'(256)4393131', N'110 South 26th Street', N'', N'Gadsden', N'35904', N'AL', N'United States', 26681)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11230, N'Billing', N'Linda Krakora', N'(815)7447021', N'272 Caterpillar Dr', N'', N'Joliet', N'60436', N'Il', N'United States', 26682)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11231, N'Shipping', N'Linda Krakora', N'(815)7447021', N'272 Caterpillar Dr', N'', N'Joliet', N'60436', N'Il', N'United States', 26682)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11232, N'Billing', N'DeAnn Barton', N'(573)556-7420', N'111 East Broadway ', N'P.O.Box 1795', N'Columbia', N'65205', N'MO', N'United States', 26683)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11233, N'Shipping', N'DeAnn Barton', N'(573)556-7420', N'111 East Broadway ', N'P.O.Box 1795', N'Columbia', N'65205', N'MO', N'United States', 26683)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11234, N'Billing', N'Lori Tieman', N'(937)228-1614', N'338 S. Patterson Blvd', N'', N'Dayton', N'45402', N'Ohio', N'United States', 26684)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11235, N'Shipping', N'Lori Tieman', N'(937)228-1614', N'338 S. Patterson Blvd', N'', N'Dayton', N'45402', N'Ohio', N'United States', 26684)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11236, N'Billing', N'Jodi Maus', N'(320)2563669', N'PO BOX 10', N'', N'Melrose', N'56352', N'MN', N'United States', 26685)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11237, N'Shipping', N'Jodi Maus', N'(320)2563669', N'PO BOX 10', N'', N'Melrose', N'56352', N'MN', N'United States', 26685)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11238, N'Billing', N'Brandi Judd', N'(806)359-8571', N'7200 Hillside', N'', N'Amarillo', N'79109', N'TX', N'United States', 26686)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11239, N'Shipping', N'Brandi Judd', N'(806)359-8571', N'7200 Hillside', N'', N'Amarillo', N'79109', N'TX', N'United States', 26686)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11240, N'Billing', N'Karen Mathias', N'(210)684-1054', N'7215 Culebra Rd', N'', N'San Antonio', N'78251', N'Texas', N'United States', 26687)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11241, N'Shipping', N'Karen Mathias', N'(210)684-1054', N'7215 Culebra Rd', N'', N'San Antonio', N'78251', N'Texas', N'United States', 26687)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11242, N'Billing', N'Vicky Matteson-Burdick', N'(716)4831650', N'915  E. Second St', N'', N'Jamestown', N'14701', N'NY', N'United States', 26688)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11243, N'Shipping', N'Vicky Matteson-Burdick', N'(716)4831650', N'915  E. Second St', N'', N'Jamestown', N'14701', N'NY', N'United States', 26688)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11244, N'Billing', N'Kathy Beelmann', N'(907)257-9434', N'440 E. 36th Avenue', N'', N'Anchorage', N'99503', N'Alaska', N'United States', 26689)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11245, N'Shipping', N'Kathy Beelmann', N'(907)257-9434', N'440 E. 36th Avenue', N'', N'Anchorage', N'99503', N'Alaska', N'United States', 26689)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11246, N'Billing', N'Kaj Johansen', N'(207)347-8874', N'PO Box 10659', N'', N'Portland', N'04104', N'Maine', N'United States', 26690)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11247, N'Shipping', N'Kaj Johansen', N'(207)347-8874', N'PO Box 10659', N'', N'Portland', N'04104', N'Maine', N'United States', 26690)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11248, N'Billing', N'Karen Cavett', N'(952)736-5159', N'1400 Riverwood Drive', N'', N'Burnsville', N'55337', N'MN', N'United States', 26691)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11249, N'Shipping', N'Karen Cavett', N'(952)736-5159', N'1400 Riverwood Drive', N'', N'Burnsville', N'55337', N'MN', N'United States', 26691)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11250, N'Billing', N'Joanne DiChiara', N'(732)242-6055', N'135 Raritan Center Parkway', N'', N'Edison', N'08837', N'NJ', N'United States', 26692)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11251, N'Shipping', N'Joanne DiChiara', N'(732)242-6055', N'135 Raritan Center Parkway', N'', N'Edison', N'08837', N'NJ', N'United States', 26692)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11252, N'Billing', N'Judy Hopper', N'(513)762-1682', N'8763 Union Centre Blvd', N'', N'West Chester', N'45069', N'Oh', N'United States', 26693)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11253, N'Shipping', N'Judy Hopper', N'(513)762-1682', N'8763 Union Centre Blvd', N'', N'West Chester', N'45069', N'Oh', N'United States', 26693)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11254, N'Billing', N'Elizabeth Buonocore', N'(315)488-4433', N'1753 Milton Avenue', N'', N'Solvay', N'13209', N'New York', N'United States', 26694)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11255, N'Shipping', N'Elizabeth Buonocore', N'(315)488-4433', N'1753 Milton Avenue', N'', N'Solvay', N'13209', N'New York', N'United States', 26694)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11256, N'Billing', N'Mary Lou Ryfun', N'(315) 488-4433', N'1753 Milton Ave', N'P.O. Box 188', N'Solvay', N'13209', N'NY', N'United States', 26695)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11257, N'Shipping', N'Mary Lou Ryfun', N'(315) 488-4433', N'1753 Milton Ave', N'P.O. Box 188', N'Solvay', N'13209', N'NY', N'United States', 26695)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11258, N'Billing', N'Pat Plunkett', N'(432)586-3364', N'200 E Austin', N'', N'Kermit', N'79745', N'TX', N'United States', 26696)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11259, N'Shipping', N'Pat Plunkett', N'(432)586-3364', N'200 E Austin', N'', N'Kermit', N'79745', N'TX', N'United States', 26696)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11260, N'Billing', N'Mendi Lutze', N'(970)854-3109', N'101 W Denver St.', N'', N'Holyoke', N'80734', N'Colorado', N'United States', 26697)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11261, N'Shipping', N'Mendi Lutze', N'(970)854-3109', N'101 W Denver St.', N'', N'Holyoke', N'80734', N'Colorado', N'United States', 26697)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11262, N'Billing', N'Lori Tieman', N'937-228-1614', N'338 S Patterson Blvd', NULL, N'Dayton', N'45402', N'OH', N'US', 26698)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11263, N'Shipping', N'Lori Tieman', N'937-228-1614', N'338 S Patterson Blvd', NULL, N'Dayton', N'45402', N'OH', N'US', 26698)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11264, N'Billing', N'Andy Happ', N'(515)245-3524', N'800 9th St', N'', N'Des Moines', N'50309', N'IA', N'United States', 26699)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11265, N'Shipping', N'Andy Happ', N'(515)245-3524', N'800 9th St', N'', N'Des Moines', N'50309', N'IA', N'United States', 26699)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11266, N'Billing', N'Mark Merriman', N'(253)2722161', N'3602 Alexander Avenue', N'', N'Tacoma', N'98424', N'WA', N'United States', 26700)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11267, N'Shipping', N'Mark Merriman', N'(253)2722161', N'3602 Alexander Avenue', N'', N'Tacoma', N'98424', N'WA', N'United States', 26700)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11268, N'Billing', N'raymond ross', N'(859)4557355', N'440 Park Place', N'', N'Lexington', N'40505', N'Ky', N'United States', 26701)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11269, N'Shipping', N'raymond ross', N'(859)4557355', N'440 Park Place', N'', N'Lexington', N'40505', N'Ky', N'United States', 26701)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11270, N'Billing', N'Carey Highley', N'(417)862-0471', N'815 W Tampa', N'', N'Springfield', N'65801', N'MO', N'United States', 26702)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11271, N'Shipping', N'Carey Highley', N'(417)862-0471', N'815 W Tampa', N'', N'Springfield', N'65801', N'MO', N'United States', 26702)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11272, N'Billing', N'Elaine Keller-Petryk', N'(585)3601758', N'824 E Ridge Rd', N'', N'Rochester', N'14621', N'NY', N'United States', 26703)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11273, N'Shipping', N'Elaine Keller-Petryk', N'(585)3601758', N'824 E Ridge Rd', N'', N'Rochester', N'14621', N'NY', N'United States', 26703)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11274, N'Billing', N'Mike Hurt', N'(812)295-3484', N'PO Box 183', N'', N'Loogootee', N'47553', N'Indiana', N'United States', 26704)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11275, N'Shipping', N'Mike Hurt', N'(812)295-3484', N'PO Box 183', N'', N'Loogootee', N'47553', N'Indiana', N'United States', 26704)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11276, N'Billing', N'Jeff Havener', N'(360)891-4034', N'PO Box 324', N'', N'Cancouver', N'98666', N'WA', N'United States', 26705)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11277, N'Shipping', N'Jeff Havener', N'(360)891-4034', N'PO Box 324', N'', N'Cancouver', N'98666', N'WA', N'United States', 26705)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11278, N'Billing', N'mark hamid', N'(915)5643303', N'9983 Kenworthy St.', N'', N'El Paso', N'79924', N'TX', N'United States', 26706)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11279, N'Shipping', N'mark hamid', N'(915)5643303', N'9983 Kenworthy St.', N'', N'El Paso', N'79924', N'TX', N'United States', 26706)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11280, N'Billing', N'James King', N'(901)452-7900', N'3337 Summer Ave', N'', N'Memphis', N'38122', N'TN', N'United States', 26707)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11281, N'Shipping', N'James King', N'(901)452-7900', N'3337 Summer Ave', N'', N'Memphis', N'38122', N'TN', N'United States', 26707)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11282, N'Billing', N'Catherine Harrington', N'(617)979-1362', N'100 Quincy Ave', N'', N'Quincy', N'02169', N'MA', N'United States', 26708)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11283, N'Shipping', N'Catherine Harrington', N'(617)979-1362', N'100 Quincy Ave', N'', N'Quincy', N'02169', N'MA', N'United States', 26708)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11284, N'Billing', N'DeAnne Harris', N'(254)739-2594', N'613 Main Street', N'', N'Teague', N'75860', N'TX', N'United States', 26709)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11285, N'Shipping', N'DeAnne Harris', N'(254)739-2594', N'613 Main Street', N'', N'Teague', N'75860', N'TX', N'United States', 26709)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11286, N'Billing', N'Heather Noel', N'(503)546-5043', N'718 NE 12th Ave', N'', N'Portland', N'97232', N'OR', N'United States', 26710)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11287, N'Shipping', N'Heather Noel', N'(503)546-5043', N'718 NE 12th Ave', N'', N'Portland', N'97232', N'OR', N'United States', 26710)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11288, N'Billing', N'Tina Gerhart', N'(717)272-2210', N'300 Schneider Drive', N'', N'Lebanon', N'17046', N'PA', N'United States', 26711)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11289, N'Shipping', N'Tina Gerhart', N'(717)272-2210', N'300 Schneider Drive', N'', N'Lebanon', N'17046', N'PA', N'United States', 26711)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11290, N'Billing', N'BOBBIE GONZALEZ', N'210-684-1054', N'7215 CULEBRA RD', NULL, N'San Antonio', N'78251', N'Texas', N'US', 26712)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11291, N'Shipping', N'BOBBIE GONZALEZ', N'210-684-1054', N'7215 CULEBRA RD', NULL, N'San Antonio', N'78251', N'Texas', N'US', 26712)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11292, N'Billing', N'Yaroslav Zaviysky', N'(212)533-2980', N'215 Second Avenue', N'', N'New York', N'10003', N'NY', N'United States', 26713)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11293, N'Shipping', N'Yaroslav Zaviysky', N'(212)533-2980', N'215 Second Avenue', N'', N'New York', N'10003', N'NY', N'United States', 26713)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11294, N'Billing', N'Erica Tomlinson', N'(518)7253191', N'355 Hales Mills Road', N'P.O. Box 232', N'Gloversville', N'12078', N'NY', N'United States', 26714)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11295, N'Shipping', N'Erica Tomlinson', N'(518)7253191', N'355 Hales Mills Road', N'P.O. Box', N'Gloversville', N'12078', N'NY', N'United States', 26714)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11296, N'Billing', N'Lisa Davison', N'(678)322-2626', N'4075 Charles Hardy Parkway', N'Suite 148', N'Dallas', N'30134', N'GA', N'United States', 26715)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11297, N'Shipping', N'Lisa Davison', N'(678)322-2626', N'4075 Charles Hardy Parkway', N'Suite 148', N'Dallas', N'30134', N'GA', N'United States', 26715)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11298, N'Billing', N'Jamie  Baker ', N'(901) 321-1236', N'2608 Avery Ave ', N' ', N'Memphis ', N'38112', N'Tn. ', N'United States', 26716)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11299, N'Shipping', N'Jamie  Baker ', N'(901) 321-1236', N'2608 Avery Ave ', N' ', N'Memphis ', N'38112', N'Tn. ', N'United States', 26716)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11300, N'Billing', N'Gary MAsters', N'(972)201-1772', N'1845 Woodall Rodgers Fwy', N'Ste 1300', N'Dallas', N'75201', N'TX', N'United States', 26717)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11301, N'Shipping', N'Gary MAsters', N'(972)201-1772', N'1845 Woodall Rodgers Fwy', N'Ste 1300', N'Dallas', N'75201', N'TX', N'United States', 26717)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11302, N'Billing', N'WANDA HAMMONDS', N'(520)721-5099', N'PO BOX 12100', N'7740 E SPEEDWAY', N'TUCSON ', N'85732-2100', N'AZ', N'United States', 26718)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11303, N'Shipping', N'WANDA HAMMONDS', N'(520)721-5099', N'PO BOX 12100', N'7740 E SPEEDWAY', N'TUCSON ', N'85732-2100', N'AZ', N'United States', 26718)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11304, N'Billing', N'Steve Huenr', N'55', N'dd', NULL, N'Holmen', N'54636', N'WI', N'US', 26719)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11305, N'Shipping', N'Steve Huenr', N'55', N'dd', NULL, N'Holmen', N'54636', N'WI', N'US', 26719)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11306, N'Billing', N'ljkj llk', N'lkl', N'lk', NULL, N'Holmen', N'54636', N'WI', N'US', 26720)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11307, N'Shipping', N'ljkj llk', N'lkl', N'lk', NULL, N'Holmen', N'54636', N'WI', N'US', 26720)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11308, N'Billing', N'Scott Avants', N'(406)6512330', N'3212 Central Avenue', N'', N'Billings', N'59102', N'MT', N'United States', 26721)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11309, N'Shipping', N'Scott Avants', N'(406)6512330', N'3212 Central Avenue', N'', N'Billings', N'59102', N'MT', N'United States', 26721)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11310, N'Billing', N'Angie Vajdak', N'(281)443-8431', N'16600 JFK Blvd', N'', N'Houston ', N'77032', N'Texas', N'United States', 26722)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (11311, N'Shipping', N'Angie Vajdak', N'(281)443-8431', N'16600 JFK Blvd', N'', N'Houston ', N'77032', N'Texas', N'United States', 26722)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12268, N'Billing', N'Jessica McLaurin', N'(910)793-2223', N'2465 S. 17th Street', N'', N'Wilmington', N'28401', N'NC', N'United States', 26723)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12269, N'Shipping', N'Jessica McLaurin', N'(910)793-2223', N'2465 S. 17th Street', N'', N'Wilmington', N'28401', N'NC', N'United States', 26723)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12270, N'Billing', N'Christina Cardoza', N'(508)9735088', N'PO Box 40429', N'', N'New Bedford', N'02744', N'MA', N'United States', 26724)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12271, N'Shipping', N'Christina Cardoza', N'(508)9735088', N'PO Box 40429', N'', N'New Bedford', N'02744', N'MA', N'United States', 26724)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12272, N'Billing', N'Peggy  Wright', N'(337)626-3322', N'4321 Nelson Road', N'', N'Lake Charles', N'70605', N'LA', N'United States', 26725)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12273, N'Shipping', N'Peggy  Wright', N'(337)626-3322', N'4321 Nelson Road', N'', N'Lake Charles', N'70605', N'LA', N'United States', 26725)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12274, N'Billing', N'Nancy  Stoermann', N'(320)256-4269', N'320 Main Street East ', N'', N'Melrose', N'56352', N'MN', N'United States', 26726)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (12275, N'Shipping', N'Nancy  Stoermann', N'(320)256-4269', N'320 Main Street East ', N'', N'Melrose', N'56352', N'MN', N'United States', 26726)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13274, N'Billing', N'Andrew Barriger', N'(210)244-2525', N'610 Augusta St', N'', N'San Antonio', N'78215', N'TX', N'United States', 26727)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13275, N'Shipping', N'Andrew Barriger', N'(210)244-2525', N'610 Augusta St', N'', N'San Antonio', N'78215', N'TX', N'United States', 26727)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13276, N'Billing', N'LANICKA ALBERT', N'(225)2925490', N'10190 CELTIC DRIVE', NULL, N'BATON ROUGE', N'70809', N'LOUISIANA', N'United States', 26728)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13277, N'Shipping', N'LANICKA ALBERT', N'(225)2925490', N'10190 CELTIC DRIVE', NULL, N'BATON ROUGE', N'70809', N'LOUISIANA', N'United States', 26728)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13278, N'Billing', N'Angie  Noel', N'(205)2573466', N'600 North 18th Street', N'', N'Birmingham', N'35203', N'AL', N'United States', 26729)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13279, N'Shipping', N'Angie  Noel', N'(205)2573466', N'600 North 18th Street', N'', N'Birmingham', N'35203', N'AL', N'United States', 26729)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13280, N'Billing', N'Alicia Quevedo', N'(954)538-6225', N'2020 NW 150th Avenue', N'', N'Pembroke Pines', N'33028', N'FL', N'United States', 26730)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13281, N'Shipping', N'Alicia Quevedo', N'(954)538-6225', N'2020 NW 150th Avenue', N'', N'Pembroke Pines', N'33028', N'FL', N'United States', 26730)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13282, N'Billing', N'Diana Navarrete-Hull', N'(913)9058429', N'9777 Ridge Dr', N'', N'Lenexa', N'66219', N'Kansas', N'United States', 26731)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13283, N'Shipping', N'Diana Navarrete-Hull', N'(913)9058429', N'9777 Ridge Dr', N'', N'Lenexa', N'66219', N'Kansas', N'United States', 26731)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13284, N'Billing', N'Rani  Craft', N'(214)748-9393', N'13649 Montfort Dr. ', N'', N'Dallas', N'75240', N'TX', N'United States', 26732)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13285, N'Shipping', N'Rani  Craft', N'(214)748-9393', N'13649 Montfort Dr. ', N'', N'Dallas', N'75240', N'TX', N'United States', 26732)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13286, N'Billing', N'Renee Powell', N'(225)408-4920', N'6230 Perkins Rd', N'', N'Baton Rouge', N'70816', N'LA', N'United States', 26733)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13287, N'Shipping', N'Renee Powell', N'(225)408-4920', N'6230 Perkins Rd', N'', N'Baton Rouge', N'70816', N'LA', N'United States', 26733)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13288, N'Billing', N'joe murray', N'(318)387-4592', N'1420 Natchitoches street ', N'', N'West Monroe ', N'71292', N'la', N'United States', 26734)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13289, N'Shipping', N'joe murray', N'(318)387-4592', N'1420 Natchitoches street ', N'', N'West Monroe ', N'71292', N'la', N'United States', 26734)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13290, N'Billing', N'Ruthellen  Caldwell', N'(850)5092269', N'440 North Monroe St ', N'', N'Tallahassee ', N'32301', N'FL', N'United States', 26735)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13291, N'Shipping', N'Ruthellen  Caldwell', N'(850)5092269', N'440 North Monroe St ', N'', N'Tallahassee ', N'32301', N'FL', N'United States', 26735)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13292, N'Billing', N'Victoria Hanson', N'(651)747-1563', N'700 Apollo Drive', N'', N'Lino Lakes ', N'55014', N'MN', N'United States', 26736)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13293, N'Shipping', N'Victoria Hanson', N'(651)747-1563', N'700 Apollo Drive', N'', N'Lino Lakes ', N'55014', N'MN', N'United States', 26736)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13294, N'Billing', N'Yvonne Fossen', N'(701)438-2222', N'PO Box 10', N'', N'Maddock', N'58348', N'ND', N'United States', 26737)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (13295, N'Shipping', N'Yvonne Fossen', N'(701)438-2222', N'PO Box 10', N'', N'Maddock', N'58348', N'ND', N'United States', 26737)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14280, N'Billing', N'Lezlie Veach', N'(432)640-0129', N'600 W Louisiana Ave', N'', N'Midland', N'79701', N'tx', N'United States', 26738)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14281, N'Shipping', N'Lezlie Veach', N'(432)640-0129', N'600 W Louisiana Ave', N'', N'Midland', N'79701', N'tx', N'United States', 26738)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14282, N'Billing', N'Sarah Blalock', N'(770)531-2746', N'325 Washington Street', N'', N'Gainesville', N'30501', N'GA', N'United States', 26739)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14283, N'Shipping', N'Sarah Blalock', N'(770)531-2746', N'325 Washington Street', N'', N'Gainesville', N'30501', N'GA', N'United States', 26739)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14284, N'Billing', N'Jennifer Ortiz', N'(800)356-0067', N'500 Merrimack Street', N'', N'Lawrence', N'01843', N'MA', N'United States', 26740)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14285, N'Shipping', N'Jennifer Ortiz', N'(800)356-0067', N'500 Merrimack Street', N'', N'Lawrence', N'01843', N'MA', N'United States', 26740)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14286, N'Billing', N'Carol Wright', N'(719)551-5144', N'2132 W. Colorado Ave.', N'Colorado Springs ', N'', N'80904', N'Co', N'United States', 26741)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14287, N'Shipping', N'Carol Wright', N'(719)551-5144', N'2132 W. Colorado Ave.', N'Colorado Springs ', N'', N'80904', N'Co', N'United States', 26741)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14288, N'Billing', N'Tiffany Lipscomb', N'(925)5984822', N'5901 Gibraltar Drive North', N'', N'Pleasanton ', N'94588', N'California ', N'United States', 26742)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14289, N'Shipping', N'Tiffany Lipscomb', N'(925)5984822', N'5901 Gibraltar Drive North', N'', N'Pleasanton ', N'94588', N'California ', N'United States', 26742)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14290, N'Billing', N'Adrienne Schwindt', N'(303)430-5534', N'5005 W 60th Ave', N'', N'Arvada', N'80003', N'co', N'United States', 26743)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14291, N'Shipping', N'Adrienne Schwindt', N'(303)430-5534', N'5005 W 60th Ave', N'', N'Arvada', N'80003', N'co', N'United States', 26743)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14292, N'Billing', N'Denise  Hall', N'(720)921-3004', N'3700 E Alameda Ave', N'', N'Denver', N'80209', N'CO', N'United States', 26744)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14293, N'Shipping', N'Denise  Hall', N'(720)921-3004', N'3700 E Alameda Ave', N'', N'Denver', N'80209', N'CO', N'United States', 26744)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14294, N'Billing', N'Lisa Holacka', N'(469)429-3238', N'2154 Forest Lane', N'', N'Garland', N'75042', N'TX', N'United States', 26745)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14295, N'Shipping', N'Lisa Holacka', N'(469)429-3238', N'2154 Forest Lane', N'', N'Garland', N'75042', N'TX', N'United States', 26745)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14296, N'Billing', N'Taryn Himes', N'(970)242-0123', N'2440 Patterson Road', N'', N'Grand Junction', N'81505', N'CO', N'United States', 26746)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14297, N'Shipping', N'Taryn Himes', N'(970)242-0123', N'2440 Patterson Road', N'', N'Grand Junction', N'81505', N'CO', N'United States', 26746)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14298, N'Billing', N'Miguel Roman', N'(626)351-9651', N'670 N Rosemead Blvd.', N'', N'Pasadena', N'91107', N'California', N'United States', 26747)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14299, N'Shipping', N'Miguel Roman', N'(626)351-9651', N'670 N Rosemead Blvd.', N'', N'Pasadena', N'91107', N'California', N'United States', 26747)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14300, N'Billing', N'Kathy Moncek', N'(815)2677711', N'1350 W Renwick Road', N'', N'Romeoville', N'60446', N'IL', N'United States', 26748)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14301, N'Shipping', N'Kathy Moncek', N'(815)2677711', N'1350 W Renwick Road', N'', N'Romeoville', N'60446', N'IL', N'United States', 26748)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14302, N'Billing', N'Karen Stone', N'(231) 439-1551', N'2140 M-119', N' ', N'Petoskey', N'49770', N'MI', N'United States', 26749)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14303, N'Shipping', N'Karen Stone', N'(231) 439-1551', N'2140 M-119', N' ', N'Petoskey', N'49770', N'MI', N'United States', 26749)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14304, N'Billing', N'Brenda Lee', N'(808)930-1458', N'632 Kinoole Street', N'', N'Hilo', N'96720', N'Hawaii', N'United States', 26750)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14305, N'Shipping', N'Brenda Lee', N'(808)930-1458', N'632 Kinoole Street', N'', N'Hilo', N'96720', N'Hawaii', N'United States', 26750)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14306, N'Billing', N'Michelle Shelor', N'(925)6095015', N'3000 Clayton Rd', N'', N'Concord', N'94519', N'CA', N'United States', 26751)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14307, N'Shipping', N'Michelle Shelor', N'(925)6095015', N'3000 Clayton Rd', N'', N'Concord', N'94519', N'CA', N'United States', 26751)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14308, N'Billing', N'Geannina Hiraldo', N'(407)8804300', N'825 South Park Ave', N'', N'Apopka', N'32703', N'Florida', N'United States', 26752)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14309, N'Shipping', N'Geannina Hiraldo', N'(407)8804300', N'825 South Park Ave', N'', N'Apopka', N'32703', N'Florida', N'United States', 26752)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14310, N'Billing', N'Craig Sheffield', N'(972)201-1867', N'1845 Woodall Rodgers Fwy', N'Suite 1200', N'Dallas', N'75201', N'TX', N'United States', 26753)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14311, N'Shipping', N'Craig Sheffield', N'(972)201-1867', N'1845 Woodall Rodgers Fwy', N'Suite 1200', N'Dallas', N'75201', N'TX', N'United States', 26753)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14312, N'Billing', N'Paul Kramer', N'(408)9953169', N'140 Asbury St', N'', N'San Jose', N'95110', N'CA', N'United States', 26754)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14313, N'Shipping', N'Paul Kramer', N'(408)9953169', N'140 Asbury St', N'', N'San Jose', N'95110', N'CA', N'United States', 26754)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14314, N'Billing', N'Sherry Sullivan', N'(319)395-6406', N'1150 42nd St NE', N'', N'Cedar Rapids,', N'52402', N'IA', N'United States', 26755)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14315, N'Shipping', N'Sherry Sullivan', N'(319)395-6406', N'1150 42nd St NE', N'', N'Cedar Rapids,', N'52402', N'IA', N'United States', 26755)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14316, N'Billing', N'Stephanie Medeiros', N'(508)6789028', N'333 Milliken Blvd', N'', N'Fall River', N'02721', N'MA', N'United States', 26756)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14317, N'Shipping', N'Stephanie Medeiros', N'(508)6789028', N'333 Milliken Blvd', N'', N'Fall River', N'02721', N'MA', N'United States', 26756)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14318, N'Billing', N'David Tolle', N'(952)736-5321', N'1400 Riverwood Drive', N'', N'Burnsville', N'55337', N'MN', N'United States', 26757)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14319, N'Shipping', N'David Tolle', N'(952)736-5321', N'1400 Riverwood Drive', N'', N'Burnsville', N'55337', N'MN', N'United States', 26757)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14320, N'Billing', N'Jerry Sims', N'(509) 942-6108', N'PO Box 500', N' ', N'Richland', N'99352', N'WA', N'United States', 26758)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14321, N'Shipping', N'Jerry Sims', N'(509) 942-6108', N'PO Box 500', N' ', N'Richland', N'99352', N'WA', N'United States', 26758)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14322, N'Billing', N'W K', N'(555) 5555555', N'Test', N'test', N'test', N'test', N'test', N'United States', 26759)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14323, N'Shipping', N'W K', N'(555) 5555555', N'Test', N'test', N'test', N'test', N'test', N'United States', 26759)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14324, N'Billing', N'Susan Brokaw', N'(701)5724000', N'1300 Bison Drive', N'', N'Williston', N'58801', N'ND', N'United States', 26760)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14325, N'Shipping', N'Susan Brokaw', N'(701)5724000', N'1300 Bison Drive', N'', N'Williston', N'58801', N'ND', N'United States', 26760)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14326, N'Billing', N'Jennifer Moschetti', N'(804)2660290', N'9401 West Broad Street', N'', N'Henrico', N'23294', N'VA', N'United States', 26761)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14327, N'Shipping', N'Jennifer Moschetti', N'(804)2660290', N'9401 West Broad Street', N'', N'Henrico', N'23294', N'VA', N'United States', 26761)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14328, N'Billing', N'Donna Hawkins', N'(860)6882323', N'1901 Day Hill Road', N'', N'Windsor', N'06095', N'CT', N'United States', 26762)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14329, N'Shipping', N'Donna Hawkins', N'(860)6882323', N'1901 Day Hill Road', N'', N'Windsor', N'06095', N'CT', N'United States', 26762)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14330, N'Billing', N'Linda Gentry', N'(801)325-6219', N'7181 S Campus View dr', N'', N'West Jordan', N'84084', N'Ut', N'United States', 26763)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14331, N'Shipping', N'Linda Gentry', N'(801)325-6219', N'7181 S Campus View dr', N'', N'West Jordan', N'84084', N'Ut', N'United States', 26763)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14332, N'Billing', N'Lorraine Lachapelle', N'(714) 8838350', N'15455 Dallas Parkway Suite 900', N' ', N'Addison', N'75201', N'TX', N'United States', 26764)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14333, N'Shipping', N'Lorraine Lachapelle', N'(714) 8838350', N'15455 Dallas Parkway Suite 900', N' ', N'Addison', N'75201', N'TX', N'United States', 26764)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14334, N'Billing', N'Rosita  Bagheri', N'(877)7322848', N'6545 Sequence Dr', N'', N'San Diego', N'92121', N'CA', N'United States', 26765)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14335, N'Shipping', N'Rosita  Bagheri', N'(877)7322848', N'6545 Sequence Dr', N'', N'San Diego', N'92121', N'CA', N'United States', 26765)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14336, N'Billing', N'Genene Welborn', N'(432)687-8450', N'600 W. Louisiana', N'', N'Midland', N'79701', N'Texas', N'United States', 26766)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14337, N'Shipping', N'Genene Welborn', N'(432)687-8450', N'600 W. Louisiana', N'', N'Midland', N'79701', N'Texas', N'United States', 26766)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14338, N'Billing', N'Teressa Rich', N'(801)325-6379', N'7181 S Campus View Dr', N'', N'West Jordan', N'84084', N'UT', N'United States', 31271)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14339, N'Shipping', N'Teressa Rich', N'(801)325-6379', N'7181 S Campus View Dr', N'', N'West Jordan', N'84084', N'UT', N'United States', 31271)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14340, N'Billing', N'Linda  Battaglia', N'(614)416-7588', N'5665 N Hamilton Road', N'', N'Columbus', N'43230', N'OH', N'United States', 31590)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (14341, N'Shipping', N'Linda  Battaglia', N'(614)416-7588', N'5665 N Hamilton Road', N'', N'Columbus', N'43230', N'OH', N'United States', 31590)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15340, N'Billing', N'Kristen McCrary', N'(828)8847283', N'P.O. Box 910', N'', N'Pisgah Forest', N'28768', N'NC', N'United States', 31591)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15341, N'Shipping', N'Kristen McCrary', N'(828)8847283', N'P.O. Box 910', N'', N'Pisgah Forest', N'28768', N'NC', N'United States', 31591)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15342, N'Billing', N'Christopher Berube', N'(800)6392802', N'75 Boulder Way', N'', N'Biddeford', N'04005', N'Maine', N'United States', 31592)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15343, N'Shipping', N'Christopher Berube', N'(800)6392802', N'75 Boulder Way', N'', N'Biddeford', N'04005', N'Maine', N'United States', 31592)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15344, N'Billing', N'Karen Bell', N'(740)397-1136', N'1215 Yauger Rd', N'P.O. Box 631', N'Mount Vernon', N'43050', N'OH', N'United States', 31593)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15345, N'Shipping', N'Karen Bell', N'(740)397-1136', N'1215 Yauger Rd', N'P.O. Box 631', N'Mount Vernon', N'43050', N'OH', N'United States', 31593)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15346, N'Billing', N'Amanda Ashotn', N'(207)375-6538', N'P.O. Box 250', N'2 Middle Rd', N'Sabattus', N'04280', N'Maine', N'United States', 31594)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15347, N'Shipping', N'Amanda Ashotn', N'(207)375-6538', N'P.O. Box 250', N'2 Middle Rd', N'Sabattus', N'04280', N'Maine', N'United States', 31594)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15348, N'Billing', N'Debra Smith', N'(508)671-5056', N'271 Greenwood Street', N'', N'Worcester', N'01609', N'MA', N'United States', 31595)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15349, N'Shipping', N'Debra Smith', N'(508)671-5056', N'271 Greenwood Street', N'', N'Worcester', N'01609', N'MA', N'United States', 31595)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15350, N'Billing', N'Tiricka Tripplet', N'(301)839-8430', N'5440 Cherokee Avenue', N'Suite 200', N'Alexandria', N'22312', N'VA', N'United States', 31596)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15351, N'Shipping', N'Tiricka Tripplet', N'(301)839-8430', N'5440 Cherokee Avenue', N'Suite 200', N'Alexandria', N'22312', N'VA', N'United States', 31596)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15352, N'Billing', N'Steve jjj', N'55', N'tt', N'tt', N'Holmen', N'54636', N'WI', N'US', 31598)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15353, N'Shipping', N'Steve jjj', N'55', N'tt', N'tt', N'Holmen', N'54636', N'WI', N'US', 31598)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15354, N'Billing', N'Deborah Foley', N'(858)268-7218', N'9420 Farnham St.', N'', N'San Diego', N'92124', N'CA', N'United States', 31600)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15355, N'Shipping', N'Deborah Foley', N'(858)268-7218', N'9420 Farnham St.', N'', N'San Diego', N'92124', N'CA', N'United States', 31600)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15356, N'Billing', N'Audrey Fink', N'(605)384-5332', N'218 White Swan Dr', N'PO Box 110', N'Pickstown', N'57367', N'SD', N'United States', 31601)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (15357, N'Shipping', N'Audrey Fink', N'(605)384-5332', N'218 White Swan Dr', N'PO Box 110', N'Pickstown', N'57367', N'SD', N'United States', 31601)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16356, N'Billing', N'Pam Pettis', N'(334) 793-7714', N'411 N Foster', N' ', N'Dothan', N'36301', N'AL', N'United States', 31602)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16357, N'Shipping', N'Pam Pettis', N'(334) 793-7714', N'411 N Foster', N' ', N'Dothan', N'36301', N'AL', N'United States', 31602)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16358, N'Billing', N'Tammy DeSha', N'(845)336-4444', N'1099 Morton Boulevard', N'', N'Kingston', N'12401', N'NY', N'United States', 31603)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16359, N'Shipping', N'Tammy DeSha', N'(845)336-4444', N'1099 Morton Boulevard', N'', N'Kingston', N'12401', N'NY', N'United States', 31603)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16360, N'Billing', N'Leslie Creech', N'(804)560-5637', N'PO Box 90010', N'', N'Richmond', N'23225', N'VA', N'United States', 31604)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16361, N'Shipping', N'Leslie Creech', N'(804)560-5637', N'PO Box 90010', N'', N'Richmond', N'23225', N'VA', N'United States', 31604)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16362, N'Billing', N'Annette Sanders', N'(765)9834757', N'PO Box 817', N'', N'Richmond', N'47374', N'IN', N'United States', 31605)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16363, N'Shipping', N'Annette Sanders', N'(765)9834757', N'PO Box 817', N'', N'Richmond', N'47374', N'IN', N'United States', 31605)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16364, N'Billing', N'Nicole Chance', N'(907)4595974', N'1417 Gillam Way', N'', N'Fairbanks', N'99701', N'AK', N'United States', 31606)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16365, N'Shipping', N'Nicole Chance', N'(907)4595974', N'1417 Gillam Way', N'', N'Fairbanks', N'99701', N'AK', N'United States', 31606)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16366, N'Billing', N'Amy Cimorelli', N'(408)531-3357', N'5890 Silver Creek Valley Road', N'', N'San Jose', N'95138', N'CA', N'United States', 31607)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16367, N'Shipping', N'Amy Cimorelli', N'(408)531-3357', N'5890 Silver Creek Valley Road', N'', N'San Jose', N'95138', N'CA', N'United States', 31607)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16368, N'Billing', N'Duane Hanley', N'(651)229-2226', N'825 Rice St', N'', N'St Paul', N'55014', N'MN', N'United States', 31608)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16369, N'Shipping', N'Duane Hanley', N'(651)229-2226', N'825 Rice St', N'', N'St Paul', N'55014', N'MN', N'United States', 31608)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16370, N'Billing', N'Roxane Kain', N'(717)2491661', N'P.O. Box 1181', N'', N'Carlisle', N'17013', N'PA', N'United States', 31609)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16371, N'Shipping', N'Roxane Kain', N'(717)2491661', N'P.O. Box 1181', N'', N'Carlisle', N'17013', N'PA', N'United States', 31609)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16372, N'Billing', N'Faiza Ali', N'(281)5668000', N'1521 Lake Pointe Pkwy', N'', N'Sugar Land', N'77479', N'Texas', N'United States', 31610)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16373, N'Shipping', N'Faiza Ali', N'(281)5668000', N'1521 Lake Pointe Pkwy', N'', N'Sugar Land', N'77479', N'Texas', N'United States', 31610)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16374, N'Billing', N'Cathy Thomas', N'(702)939-3017', N'2625 N Tenaya', N'', N'Las Vegas', N'89128', N'NV', N'United States', 31611)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16375, N'Shipping', N'Cathy Thomas', N'(702)939-3017', N'2625 N Tenaya', N'', N'Las Vegas', N'89128', N'NV', N'United States', 31611)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16376, N'Billing', N'Walter Patrun', N'(215) 725-4430', N'1729 Cottman Avenue', N' ', N'Philadelphia', N'19111', N'PA', N'United States', 31612)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16377, N'Shipping', N'Walter Patrun', N'(215) 725-4430', N'1729 Cottman Avenue', N' ', N'Philadelphia', N'19111', N'PA', N'United States', 31612)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16378, N'Billing', N'Debbie Ferreira', N'(575)526-4401', N'300 E. Foster Rd', N'', N'Las Cruces', N'88005', N'NM', N'United States', 31613)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16379, N'Shipping', N'Debbie Ferreira', N'(575)526-4401', N'300 E. Foster Rd', N'', N'Las Cruces', N'88005', N'NM', N'United States', 31613)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16380, N'Billing', N'Anna Bednarek', N'(410)828-4730', N'23 West Susquehanna Avenue', N'', N'Towson', N'21204', N'MD', N'United States', 31614)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16381, N'Shipping', N'Anna Bednarek', N'(410)828-4730', N'23 West Susquehanna Avenue', N'', N'Towson', N'21204', N'MD', N'United States', 31614)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16382, N'Billing', N'Paul  Waild', N'(225) 408-6290', N'3232 S Sherwood Forest Blvd', N' ', N'Baton ROuge', N'70817', N'LA', N'United States', 31615)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16383, N'Shipping', N'Paul  Waild', N'(225) 408-6290', N'3232 S Sherwood Forest Blvd', N' ', N'Baton ROuge', N'70817', N'LA', N'United States', 31615)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16384, N'Billing', N'Kristi Hobbs', N'(318) 387-4592', N'1420 Natchitoches St', N' ', N'West Monroe', N'71292', N'Louisiana', N'United States', 31616)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16385, N'Shipping', N'Kristi Hobbs', N'(318) 387-4592', N'1420 Natchitoches St', N' ', N'West Monroe', N'71292', N'Louisiana', N'United States', 31616)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16386, N'Billing', N'Amy Shea', N'(302) 629-0100', N'1941 Bridgeville Hwy', N'Po Box 1800', N'Seaford', N'19973', N'De', N'United States', 31617)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16387, N'Shipping', N'Amy Shea', N'(302) 629-0100', N'1941 Bridgeville Hwy', N'Po Box 1800', N'Seaford', N'19973', N'De', N'United States', 31617)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16388, N'Billing', N'Chad Cash', N'(704) 435-0186', N'1200 East Church Street', N' ', N'Cherryville', N'20021', N'NC', N'United States', 31618)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16389, N'Shipping', N'Chad Cash', N'(704) 435-0186', N'1200 East Church Street', N' ', N'Cherryville', N'20021', N'NC', N'United States', 31618)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16390, N'Billing', N'Cathy Chempanos', N'(718)6589800', N'139-30 Queens Boulevard', N'', N'Briarwood', N'11435', N'NY', N'United States', 31619)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16391, N'Shipping', N'Cathy Chempanos', N'(718)6589800', N'139-30 Queens Boulevard', N'', N'Briarwood', N'11435', N'NY', N'United States', 31619)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16392, N'Billing', N'Brian  Quinn', N'(610)3255160', N'1974 Sproul Rd ', N'', N'Broomall', N'19468', N'PA', N'United States', 31620)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16393, N'Shipping', N'Brian  Quinn', N'(610)3255160', N'1974 Sproul Rd ', N'', N'Broomall', N'19468', N'PA', N'United States', 31620)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16394, N'Billing', N'Jennie Belanger', N'(906)7867213', N'2600 1st Ave S', N'', N'Escanaba', N'49829', N'MI', N'United States', 31621)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16395, N'Shipping', N'Jennie Belanger', N'(906)7867213', N'2600 1st Ave S', N'', N'Escanaba', N'49829', N'MI', N'United States', 31621)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16396, N'Billing', N'Donna McDonald', N'(517)3722400', N'525 W Willow St.', N'', N'Lansing', N'48906', N'MI', N'United States', 31622)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16397, N'Shipping', N'Donna McDonald', N'(517)3722400', N'525 W Willow St.', N'', N'Lansing', N'48906', N'MI', N'United States', 31622)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16398, N'Billing', N'Saul Schick', N'(605)3430891', N'PO BOX 1420', N'', N'RAPID CITY', N'57709', N'SD', N'United States', 31623)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16399, N'Shipping', N'Saul Schick', N'(605)3430891', N'PO BOX 1420', N'', N'RAPID CITY', N'57709', N'SD', N'United States', 31623)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16400, N'Billing', N'TTS Staff', N'800-831-0678', N'55', N' ', N'Wanakee', N'53597', N'WI', N'United States', 31624)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16401, N'Shipping', N'TTS Staff', N'800-831-0678', N'55', N' ', N'Wanakee', N'53597', N'WI', N'United States', 31624)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16402, N'Billing', N'Rachel Johnston', N'(804)266-0290', N'9401 West Broad St', N'', N'Henrico', N'23294', N'VA', N'United States', 31625)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16403, N'Shipping', N'Rachel Johnston', N'(804)266-0290', N'9401 West Broad St', N'', N'Henrico', N'23294', N'VA', N'United States', 31625)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16404, N'Billing', N'Cindy Schlichting', N'(920)830-7200', N'PO Box 1487', N'', N'Appleton', N'54912-1487', N'Wisconsin', N'United States', 31626)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16405, N'Shipping', N'Cindy Schlichting', N'(920)830-7200', N'PO Box 1487', N'', N'Appleton', N'54912-1487', N'Wisconsin', N'United States', 31626)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16406, N'Billing', N'Terry Stirewalt', N'(612)798-7126', N'345 E. 77th Street', N'', N'Richfield', N'55423', N'MN', N'United States', 31627)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16407, N'Shipping', N'Terry Stirewalt', N'(612)798-7126', N'345 E. 77th Street', N'', N'Richfield', N'55423', N'MN', N'United States', 31627)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16408, N'Billing', N'Betsy Foucha', N'(713)3782355', N'9998 Almeda Genoa', N'', N'Houston', N'77075', N'TX', N'United States', 31628)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16409, N'Shipping', N'Betsy Foucha', N'(713)3782355', N'9998 Almeda Genoa', N'', N'Houston', N'77075', N'TX', N'United States', 31628)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16410, N'Billing', N'Michael Zachau', N'(206)628-5042', N'325 Eastlake Ave E', N'', N'Seattle', N'98109', N'Wa', N'United States', 31629)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16411, N'Shipping', N'Michael Zachau', N'(206)628-5042', N'325 Eastlake Ave E', N'', N'Seattle', N'98109', N'Wa', N'United States', 31629)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16412, N'Billing', N'Veronica Rodriguez', N'(210)684-1054', N'7215 Culebra Rd', N'', N'San Antonio', N'78251', N'Texas', N'United States', 31630)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16413, N'Shipping', N'Veronica Rodriguez', N'(210)684-1054', N'7215 Culebra Rd', N'', N'San Antonio', N'78251', N'Texas', N'United States', 31630)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16414, N'Billing', N'Janet Hohf', N'(248)8144000', N'350 N. Park Blvd Po box 99', N'', N'Lake Orion', N'48361', N'MI', N'United States', 31631)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16415, N'Shipping', N'Janet Hohf', N'(248)8144000', N'350 N. Park Blvd Po box 99', N'', N'Lake Orion', N'48361', N'MI', N'United States', 31631)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16416, N'Billing', N'Cathy Carpenter', N'(518)654-9028', N'1 3rd St', N'', N'Corinth', N'12822', N'NY', N'United States', 31632)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16417, N'Shipping', N'Cathy Carpenter', N'(518)654-9028', N'1 3rd St', N'', N'Corinth', N'12822', N'NY', N'United States', 31632)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16418, N'Billing', N'Lisha Schultz', N'(509)344-2594', N'9 South Washington', N'Suite 700', N'Spokane', N'99201', N'WA', N'United States', 31633)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16419, N'Shipping', N'Lisha Schultz', N'(509)344-2594', N'9 South Washington', N'Suite 700', N'Spokane', N'99201', N'WA', N'United States', 31633)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16420, N'Billing', N'Joy Bobby', N'(734)525.0700', N'36525 Plymouth Rd', N'', N'Livonia', N'48150', N'MI', N'United States', 31634)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16421, N'Shipping', N'Joy Bobby', N'(734)525.0700', N'36525 Plymouth Rd', N'', N'Livonia', N'48150', N'MI', N'United States', 31634)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16422, N'Billing', N'Amanda Gross', N'(517)267-7423', N'8661 W Grand River Ave', N'', N'Brighton', N'48116', N'MI', N'United States', 31635)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16423, N'Shipping', N'Amanda Gross', N'(517)267-7423', N'8661 W Grand River Ave', N'', N'Brighton', N'48116', N'MI', N'United States', 31635)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16424, N'Billing', N'Esther Herrera', N'(713)844-1145', N'11526 Spencer Hwy', N'', N'La Porte', N'77536', N'TX', N'United States', 31636)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16425, N'Shipping', N'Esther Herrera', N'(713)844-1145', N'11526 Spencer Hwy', N'', N'La Porte', N'77536', N'TX', N'United States', 31636)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16426, N'Billing', N'Lori  Geis', N'(940)4236776', N'PO Box 127', N'', N'Windthorst', N'76389', N'Texas', N'United States', 31637)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16427, N'Shipping', N'Lori  Geis', N'(940)4236776', N'PO Box 127', N'', N'Windthorst', N'76389', N'Texas', N'United States', 31637)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16428, N'Billing', N'Melanie Lewis', N'(602)252-6831', N'PO Box 20525', N'', N'Phoenix', N'85036', N'AZ', N'United States', 31638)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16429, N'Shipping', N'Melanie Lewis', N'(602)252-6831', N'PO Box 20525', N'', N'Phoenix', N'85036', N'AZ', N'United States', 31638)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16430, N'Billing', N'Martin  Misset', N'(203)758-9500', N'773 Straits Turnpike', N'', N'Middlebury', N'06762', N'CT', N'United States', 31639)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16431, N'Shipping', N'Martin  Misset', N'(203)758-9500', N'773 Straits Turnpike', N'', N'Middlebury', N'06762', N'CT', N'United States', 31639)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16432, N'Billing', N'Pam  Reis', N'(701)6682224', N'115 Morton Ave', N'PO Box 33', N'Page', N'58064', N'ND', N'United States', 31640)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16433, N'Shipping', N'Pam  Reis', N'(701)6682224', N'115 Morton Ave', N'PO Box 33', N'Page', N'58064', N'ND', N'United States', 31640)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16434, N'Billing', N'Yvonne Fernandez', N'(210)476-4449', N'16211 La Cantera Parkway', N'', N'San Antonio', N'78256', N'TX', N'United States', 31641)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16435, N'Shipping', N'Yvonne Fernandez', N'(210)476-4449', N'16211 La Cantera Parkway', N'', N'San Antonio', N'78256', N'TX', N'United States', 31641)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16436, N'Billing', N'Brandy Hope', N'(801)567-3315', N'9260 S 300 E', N'', N'Sandy', N'84070', N'UT', N'United States', 31642)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16437, N'Shipping', N'Brandy Hope', N'(801)567-3315', N'9260 S 300 E', N'', N'Sandy', N'84070', N'UT', N'United States', 31642)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16438, N'Billing', N'MICHELLE COOPER', N'(504)348-2424', N'7701 AIRLINE DR', N'', N'METAIRIE', N'70003', N'LA', N'United States', 31643)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16439, N'Shipping', N'MICHELLE COOPER', N'(504)348-2424', N'7701 AIRLINE DR', N'', N'METAIRIE', N'70003', N'LA', N'United States', 31643)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16440, N'Billing', N'Karena Richard', N'(508)263-6419', N'220 Donald Lynch Blvd', N'', N'Marlborough', N'01752', N'MA ', N'United States', 31644)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16441, N'Shipping', N'Karena Richard', N'(508)263-6419', N'220 Donald Lynch Blvd', N'', N'Marlborough', N'01752', N'MA ', N'United States', 31644)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16442, N'Billing', N'David Reed', N'55', N'55', NULL, N'Holmen', N'54636', N'WI', N'US', 31645)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16443, N'Shipping', N'David Reed', N'55', N'55', NULL, N'Holmen', N'54636', N'WI', N'US', 31645)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16444, N'Billing', N'Carla Beinz', N'(260)471-8336', N'1330 Directors Row', N'', N'Fort Wayne', N'46808', N'IN', N'United States', 31646)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16445, N'Shipping', N'Carla Beinz', N'(260)471-8336', N'1330 Directors Row', N'', N'Fort Wayne', N'46808', N'IN', N'United States', 31646)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16446, N'Billing', N'Shane Racette', N'(518)324-0616', N'274 Rugar Street', N'', N'Plattsburgh', N'12901', N'NY', N'United States', 31647)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16447, N'Shipping', N'Shane Racette', N'(518)324-0616', N'274 Rugar Street', N'', N'Plattsburgh', N'12901', N'NY', N'United States', 31647)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16448, N'Billing', N'Joan  Loxtercamp', N'(320)256-3669', N'20 4th Avenue SE ', N'', N'Melrose', N'56352', N'Minnesota ', N'United States', 31648)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (16449, N'Shipping', N'Joan  Loxtercamp', N'(320)256-3669', N'20 4th Avenue SE ', N'', N'Melrose', N'56352', N'Minnesota ', N'United States', 31648)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17448, N'Billing', N'BECKY TAVIANO', N'4192235886', N'1511 N. Main St.', N'4192235886', N'Lima', N'45801', N'OH', N'US', 31649)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17449, N'Shipping', N'BECKY TAVIANO', N'4192235886', N'1511 N. Main St.', N'4192235886', N'Lima', N'45801', N'OH', N'US', 31649)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17450, N'Billing', N'Swati Momin', N'(281)5668000', N'1521 Lake Pointe Pkwy', N'', N'Sugar Land', N'77478', N'TX', N'United States', 31650)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17451, N'Shipping', N'Swati Momin', N'(281)5668000', N'1521 Lake Pointe Pkwy', N'', N'Sugar Land', N'77478', N'TX', N'United States', 31650)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17452, N'Billing', N'Ann Babich', N'(818)333-3962', N'175 E Olive Ave #100', N'', N'Burbank', N'91502', N'CA', N'United States', 31651)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17453, N'Shipping', N'Ann Babich', N'(818)333-3962', N'175 E Olive Ave #100', N'', N'Burbank', N'91502', N'CA', N'United States', 31651)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17454, N'Billing', N'Jody  Clark', N'(503)3647999', N'2096 Mission St SE', N'', N'Salem', N'97302', N'Oregon', N'United States', 31652)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17455, N'Shipping', N'Jody  Clark', N'(503)3647999', N'2096 Mission St SE', N'', N'Salem', N'97302', N'Oregon', N'United States', 31652)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17456, N'Billing', N'kristina meixner', N'(570)8236836', N'PO BOX 597', N'', N'WILKES BARRE', N'18702', N'PA', N'United States', 31653)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17457, N'Shipping', N'kristina meixner', N'(570)8236836', N'PO BOX 597', N'', N'WILKES BARRE', N'18702', N'PA', N'United States', 31653)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17458, N'Billing', N'Rhonda Cubbedge', N'(504)576-5732', N'639 Loyola Ave', N'Ste 220', N'New Orleans', N'70113', N'LA', N'United States', 31654)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17459, N'Shipping', N'Rhonda Cubbedge', N'(504)576-5732', N'639 Loyola Ave', N'Ste 220', N'New Orleans', N'70113', N'LA', N'United States', 31654)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17460, N'Billing', N'Connie Baker', N'(515)223-3182', N'2051 Westown Parkway', N'', N'West Des Moines', N'50265', N'IA', N'United States', 31655)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17461, N'Shipping', N'Connie Baker', N'(515)223-3182', N'2051 Westown Parkway', N'', N'West Des Moines', N'50265', N'IA', N'United States', 31655)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17462, N'Billing', N'Kelly Davis', N'(409)8425233', N'PO Box 20396', N'', N'Beaumont', N'77720', N'TX', N'United States', 31656)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17463, N'Shipping', N'Kelly Davis', N'(409)8425233', N'PO Box 20396', N'', N'Beaumont', N'77720', N'TX', N'United States', 31656)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17464, N'Billing', N'Julie Johnson', N'(979) 2388421', N'1001 FM 2004', N' ', N'Lake Jackson', N'77566', N'Texas', N'United States', 31657)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17465, N'Shipping', N'Julie Johnson', N'(979) 2388421', N'1001 FM 2004', N' ', N'Lake Jackson', N'77566', N'Texas', N'United States', 31657)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17466, N'Billing', N'Christopher Turner', N'(214)7243944', N'5350 Springmeadow Drive', N'', N'Dallas', N'75229', N'Texas', N'United States', 31658)

GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17467, N'Shipping', N'Christopher Turner', N'(214)7243944', N'5350 Springmeadow Drive', N'', N'Dallas', N'75229', N'Texas', N'United States', 31658)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17468, N'Billing', N'Peg Klein', N'(231)946-8796', N'PO Box 1049', N'', N'Traverse City', N'49685', N'MI', N'United States', 31659)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17469, N'Shipping', N'Peg Klein', N'(231)946-8796', N'PO Box 1049', N'', N'Traverse City', N'49685', N'MI', N'United States', 31659)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17470, N'Billing', N'Kimberly Williams', N'(763)5954025', N'14601 27th Ave N Ste 104', N'', N'Plymouth', N'55447', N'MN', N'United States', 31660)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17471, N'Shipping', N'Kimberly Williams', N'(763)5954025', N'14601 27th Ave N Ste 104', N'', N'Plymouth', N'55447', N'MN', N'United States', 31660)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17472, N'Billing', N'Caroline Salzberg', N'(703)369-8184', N'8640 Sudley Rd.', N'Suite 115', N'Falls Church', N'20110', N'VA', N'United States', 31661)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17473, N'Shipping', N'Caroline Salzberg', N'(703)369-8184', N'8640 Sudley Rd.', N'Suite 115', N'Falls Church', N'20110', N'VA', N'United States', 31661)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17474, N'Billing', N'Bettie Hatten', N'(606)329-5433', N'1300 Central Avenue', N'', N'Ashland', N'41101', N'KY', N'United States', 31662)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17475, N'Shipping', N'Bettie Hatten', N'(606)329-5433', N'1300 Central Avenue', N'', N'Ashland', N'41101', N'KY', N'United States', 31662)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17476, N'Billing', N'Christina Mayne', N'(719)482-7641', N'2735 Dublin Blvd', N'', N'Colorado Springs', N'80918', N'CO', N'United States', 31663)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17477, N'Shipping', N'Christina Mayne', N'(719)482-7641', N'2735 Dublin Blvd', N'', N'Colorado Springs', N'80918', N'CO', N'United States', 31663)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17478, N'Billing', N'Vicki  Carviou', N'(715)7320065', N'1514 Cleveland Ave.', N'P.O. Box 655', N'Marinette', N'54143', N'WI', N'United States', 31664)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17479, N'Shipping', N'Vicki  Carviou', N'(715)7320065', N'1514 Cleveland Ave.', N'P.O. Box 655', N'Marinette', N'54143', N'WI', N'United States', 31664)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17480, N'Billing', N'Becky Owens', N'(406)6512335', N'3212 Central Avenue', N'', N'Billings', N'59102', N'MT', N'United States', 31665)
GO
INSERT [dbo].[Address] ([Id], [AddressType], [Name], [Phone], [StreetAddress], [StreetAddress2], [City], [Zip], [State], [Country], [idUser]) VALUES (17481, N'Shipping', N'Becky Owens', N'(406)6512335', N'3212 Central Avenue', N'', N'Billings', N'59102', N'MT', N'United States', 31665)
GO

SET IDENTITY_INSERT [dbo].[Address] OFF
GO

