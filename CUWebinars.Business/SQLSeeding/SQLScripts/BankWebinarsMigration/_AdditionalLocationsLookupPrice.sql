SET NOCOUNT ON
CREATE TABLE BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
    (
      [idWebinar] [INT] NOT NULL ,
      [id] [INT] IDENTITY(1, 1)
                 NOT NULL ,
      [cost] [MONEY] NOT NULL ,
      CONSTRAINT [PK_AdditionalLocationsLookupPrice] PRIMARY KEY CLUSTERED
        ( [id] ASC )
        WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,
               IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
               ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
    )
ON  [PRIMARY]


PRINT 'Starts AdditionalLocationsLookupPrice'
DELETE  FROM bankwebinars.dbo.AdditionalLocationsLookupPrice WHERE idWebinar > 0

--SELECT  'INSERT bankwebinars.[dbo].[AdditionalLocationsLookupPrice] ([idWebinar], [cost]) VALUES ('
--        + CAST(idWebinar AS VARCHAR) + ',0.00)'
--FROM    dbo.Webinar
--WHERE   idWebinar NOT IN ( SELECT   idWebinar
--                           FROM     dbo.AdditionalLocationsLookupPrice )
--        --AND Status = 2

INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 2, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 3, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 4, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 5, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 6, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 7, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 8, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 9, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 10, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 842, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1585, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1606, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1632, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1647, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1671, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1708, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1739, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1602, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1621, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1636, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1656, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1706, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1707, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1608, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1623, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1630, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1635, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1640, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1728, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1658, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1659, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1674, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1675, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1676, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1677, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1711, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1712, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1605, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1618, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1703, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1619, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1643, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1648, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1743, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1486, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1571, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1572, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1573, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1595, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1596, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1601, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1604, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1610, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1612, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1622, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1624, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1639, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1646, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1672, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1681, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1717, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1718, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1719, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1721, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1722, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1723, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1724, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1726, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1742, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1683, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1592, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1613, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1633, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1587, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1611, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1626, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1586, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1617, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1628, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1644, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1599, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1607, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1615, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1631, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1641, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1664, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1736, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1737, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1568, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1569, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1579, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1614, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1620, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1662, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1663, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1730, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1731, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1732, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1733, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1734, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1744, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1745, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1649, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1650, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1720, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1582, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1701, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1702, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1594, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1627, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1637, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1661, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1678, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1679, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1680, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1738, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1603, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1609, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1645, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1666, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1667, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1668, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1669, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1670, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1581, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1651, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1665, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1705, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1709, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1710, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1655, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1657, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1660, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1682, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1704, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1625, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1638, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1740, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1741, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1727, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1729, 0.00 )
INSERT  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
        ( [idWebinar], [cost] )
VALUES  ( 1735, 0.00 )

UPDATE  BankWebinars.dbo.[AdditionalLocationsLookupPrice]
SET     cost = 50
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Duration = 1
                                AND Status = 2 )

UPDATE  BankWebinars.dbo.[AdditionalLocationsLookupPrice]
SET     cost = 75
WHERE   idWebinar IN ( SELECT   idWebinar
                       FROM     dbo.Webinar
                       WHERE    Duration = 1
                                AND Status = 2 )


UPDATE  BankWebinars.[dbo].[AdditionalLocationsLookupPrice]
SET     cost = 250
WHERE   idWebinar = 1711

GO
PRINT 'Ends AdditionalLocationsLookupPrice'